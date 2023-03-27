using UnityEngine.Networking;
using WECCL.Content;
using WECCL.Saves;

namespace WECCL;

[BepInPlugin(PluginGuid, PluginName, PluginVer)]
[HarmonyPatch]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "IngoH.WrestlingEmpire.WECCL";
    public const string PluginName = "Wrestling Empire Custom Content Loader";
    public const string PluginVer = "1.0.0";

    internal static DirectoryInfo AssetsDir;
    internal static DirectoryInfo ExportDir;
    internal static DirectoryInfo ImportDir;
    internal static DirectoryInfo OverrideDir;

    internal static List<DirectoryInfo> AllModsImportDirs = new();

    internal static ManualLogSource Log;
    internal static readonly Harmony Harmony = new(PluginGuid);

    internal static string PluginPath;

    internal static string CustomContentSavePath;

    private static readonly List<string> ImageExtensions = new()
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".bmp",
        ".tga",
        ".gif"
    };

    private static readonly List<string> AudioExtensions = new()
    {
        ".ogg",
        ".wav",
        ".mp3",
        ".aif",
        ".aiff",
        ".mod",
        ".xm",
        ".it",
        ".s3m"
    };

    internal static Plugin Instance { get; private set; }

    internal ConfigEntry<bool> AutoExportCharacters { get; set; }
    internal ConfigEntry<bool> EnableOverrides { get; set; }
    internal ConfigEntry<bool> EnableCustomContent { get; set; }
    internal ConfigEntry<bool> AllowImportingCharacters { get; set; }
    internal ConfigEntry<bool> DeleteImportedCharacters { get; set; }
    internal ConfigEntry<bool> EnableGameUnityLog { get; set; }
    internal ConfigEntry<string> GameUnityLogLevel { get; set; }
    internal ConfigEntry<int> BaseFedLimit { get; set; }
    internal ConfigEntry<int> MaxBackups { get; set; }

    private void Awake()
    {
        try
        {
            Log = this.Logger;
            PluginPath = Path.GetDirectoryName(this.Info.Location) ?? string.Empty;

            Instance = this;
            this.AutoExportCharacters = this.Config.Bind("General", "AutoExportCharacters", true,
                "Automatically export characters to /Export when the game is saved.");
            this.EnableOverrides = this.Config.Bind("General", "EnableOverrides", true,
                "Enable custom content overrides from /Overrides.");
            this.EnableCustomContent = this.Config.Bind("General", "EnableCustomContent", true,
                "Enable custom content loading from /Assets.");
            this.AllowImportingCharacters = this.Config.Bind("General", "AllowImportingCharacters", true,
                "Allow importing characters from /Import");
            this.DeleteImportedCharacters = this.Config.Bind("General", "DeleteImportedCharacters", true,
                "Delete imported characters from /Import after importing them (and saving the game).");
            this.EnableGameUnityLog = this.Config.Bind("General", "EnableGameUnityLog", true,
                "Enable Unity log messages sent by the game itself. If you don't know what this is, leave it enabled.");
            this.GameUnityLogLevel = this.Config.Bind("General", "GameUnityLogLevel", "Warning",
                new ConfigDescription(
                    "The log level for Unity log messages sent by the game itself. If you don't know what this is, leave it at Warning.",
                    new AcceptableValueList<string>("Error", "Warning", "Info")));
            this.BaseFedLimit = this.Config.Bind("General", "BaseFedLimit", 48,
                "The base limit for the number of characters that can be fed's roster. This actual limit may be increased if characters are imported.");
            this.MaxBackups = this.Config.Bind("General", "MaxBackups", 100,
                "The maximum number of backups to keep. Set to 0 to disable backups. Set to -1 to keep all backups.");

            CreateBackups();

            CustomContentSavePath = Path.Combine(PluginPath, "CustomContentSaveFile.json");

            AssetsDir = new DirectoryInfo(Path.Combine(PluginPath, "Assets"));
            if (!AssetsDir.Exists)
            {
                AssetsDir.Create();
            }

            OverrideDir = new DirectoryInfo(Path.Combine(PluginPath, "Overrides"));
            if (!OverrideDir.Exists)
            {
                OverrideDir.Create();
            }

            ImportDir = new DirectoryInfo(Path.Combine(PluginPath, "Import"));
            if (!ImportDir.Exists)
            {
                ImportDir.Create();
            }

            ExportDir = new DirectoryInfo(Path.Combine(PluginPath, "Export"));
            if (!ExportDir.Exists)
            {
                ExportDir.Create();
            }

            List<DirectoryInfo> AllModsAssetsDirs = new();
            List<DirectoryInfo> AllModsOverridesDirs = new();

            foreach (string modPath in Directory.GetDirectories(Path.Combine(Paths.BepInExRootPath, "plugins")))
            {
                FindContent(modPath, ref AllModsAssetsDirs, ref AllModsOverridesDirs, ref AllModsImportDirs);
            }

            if (!AllModsAssetsDirs.Contains(AssetsDir))
            {
                AllModsAssetsDirs.Add(AssetsDir);
            }

            if (!AllModsOverridesDirs.Contains(OverrideDir))
            {
                AllModsOverridesDirs.Add(OverrideDir);
            }

            if (!AllModsImportDirs.Contains(ImportDir))
            {
                AllModsImportDirs.Add(ImportDir);
            }

            if (AllModsAssetsDirs.Count > 0)
            {
                Log.LogInfo($"Found {AllModsAssetsDirs.Count} mod(s) with Assets directories.");
            }

            if (AllModsOverridesDirs.Count > 0)
            {
                Log.LogInfo($"Found {AllModsOverridesDirs.Count} mod(s) with Overrides directories.");
            }
            
            VanillaCounts.MusicCount = CKAMIAJJDBP.GGICEBAECGK;

            if (this.EnableCustomContent.Value)
            {
                foreach (DirectoryInfo modAssetsDir in AllModsAssetsDirs)
                {
                    LoadAudioClips(modAssetsDir);
                    LoadCostumes(modAssetsDir);
                }
            }

            if (this.EnableOverrides.Value)
            {
                foreach (DirectoryInfo modOverridesDir in AllModsOverridesDirs)
                {
                    LoadOverrides(modOverridesDir);
                }
            }
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    private void OnEnable()
    {
        Harmony.PatchAll();
        this.Logger.LogInfo($"Loaded {PluginName}!");
    }

    private void OnDisable()
    {
        Harmony.UnpatchSelf();
        this.Logger.LogInfo($"Unloaded {PluginName}!");
    }

    private static void FindContent(string modPath, ref List<DirectoryInfo> AllModsAssetsDirs,
        ref List<DirectoryInfo> AllModsOverridesDirs, ref List<DirectoryInfo> AllModsImportDirs)
    {
        try
        {
            if (modPath == null)
            {
                return;
            }

            bool shouldCheckSubDirs = true;
            DirectoryInfo modAssetsDir = new(Path.Combine(modPath, "Assets"));
            if (modAssetsDir.Exists)
            {
                AllModsAssetsDirs.Add(modAssetsDir);
                shouldCheckSubDirs = false;
            }

            DirectoryInfo modOverridesDir = new(Path.Combine(modPath, "Overrides"));
            if (modOverridesDir.Exists)
            {
                AllModsOverridesDirs.Add(modOverridesDir);
                shouldCheckSubDirs = false;
            }

            DirectoryInfo modImportDir = new(Path.Combine(modPath, "Import"));
            if (modImportDir.Exists)
            {
                AllModsImportDirs.Add(modImportDir);
                shouldCheckSubDirs = false;
            }

            if (shouldCheckSubDirs)
            {
                foreach (string subDir in Directory.GetDirectories(modPath))
                {
                    FindContent(subDir, ref AllModsAssetsDirs, ref AllModsOverridesDirs, ref AllModsImportDirs);
                }
            }
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    internal static void LoadAudioClips(DirectoryInfo dir)
    {
        try
        {
            int clipsCount = 0;
            // Load custom audio clips
            if (!dir.Exists)
            {
                return;
            }

            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
                .Where(f => AudioExtensions.Contains(f.Extension.ToLower())).ToArray();
            int count = files.Length;
            float lastProgressUpdate = Time.time;
            int cur = 0;
            foreach (FileInfo file in files)
            {
                UnityWebRequest wr = new(file.FullName);
                wr.downloadHandler = new DownloadHandlerAudioClip(file.Name, AudioType.UNKNOWN);
                wr.SendWebRequest();
                while (!wr.isDone) { }

                AudioClip clip = DownloadHandlerAudioClip.GetContent(wr);
                
                var fileName = file.Name;
                var modGuid = Directory.GetParent(file.DirectoryName!)?.Name;
                if (modGuid != null && modGuid != "plugins")
                {
                    fileName = $"{modGuid}/{fileName}";
                }
                
                clip.name = fileName;
                CustomClips.Add(clip);
                clipsCount++;
                cur++;
                if (Time.time - lastProgressUpdate > 1f)
                {
                    lastProgressUpdate = Time.time;
                    UpdateConsoleLogLoadingBar($"Loading custom audio clips from {dir.Name}", cur, count);
                }
            }

            if (clipsCount != 0)
            {
                Log.LogInfo($"Loaded {clipsCount} custom audio clips from {dir.Name}");
            }

            if (CustomClips.Count != 0)
            {
                // Update the number of audio clips in the game
                CKAMIAJJDBP.GGICEBAECGK = VanillaCounts.MusicCount + CustomClips.Count;
                CKAMIAJJDBP.CKEJAMLGLAL = new AudioClip[CKAMIAJJDBP.GGICEBAECGK + 1];
            }

            CustomContentSaveFile.ContentMap.MusicNameMap.AddRange(CustomClips.Select(c => c.name));
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    internal static void LoadCostumes(DirectoryInfo dir)
    {
        try
        {
            int costumeCount = 0;
            // Load custom costumes
            if (!dir.Exists)
            {
                return;
            }

            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
                .Where(f => ImageExtensions.Contains(f.Extension.ToLower())).ToArray();
            int count = files.Length;
            float lastProgressUpdate = Time.time;
            int cur = 0;
            foreach (FileInfo file in files)
            {
                string fileName = file.Name;
                foreach (KeyValuePair<string, CostumeData> pair in CustomCostumes)
                {
                    if (fileName.StartsWith(pair.Key) || file.Directory?.Name == pair.Key)
                    {
                        CostumeData costumeData = pair.Value;
                        if (costumeData.Type != typeof(Texture2D) || costumeData.InternalPrefix == "custom")
                        {
                            Log.LogError($"Custom {costumeData.FilePrefix} costumes are currently not supported.");
                        }
                        else
                        {
                            byte[] bytes = File.ReadAllBytes(file.FullName);
                            Texture2D tex = new(2, 2);
                            tex.LoadImage(bytes);
                            tex.name = fileName;
                            var modGuid = Directory.GetParent(file.DirectoryName!)?.Name;
                            if (modGuid != null && modGuid != "plugins")
                            {
                                fileName = $"{modGuid}/{fileName}";
                            }
                            costumeData.AddCustomObject(fileName, tex);
                            costumeCount++;
                        }
                    }
                }

                cur++;
                if (Time.time - lastProgressUpdate > 1f)
                {
                    lastProgressUpdate = Time.time;
                    UpdateConsoleLogLoadingBar($"Loading custom costumes from {dir.Name}", cur, count);
                }
            }

            if (costumeCount != 0)
            {
                Log.LogInfo($"Loaded {costumeCount} custom costumes from {dir.Name}");
            }
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    internal static void LoadOverrides(DirectoryInfo dir)
    {
        try
        {
            int overrideCount = 0;
            // Load resource overrides
            if (!dir.Exists)
            {
                return;
            }

            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories).Where(f =>
                    ImageExtensions.Contains(f.Extension.ToLower()) ||
                    AudioExtensions.Contains(f.Extension.ToLower()))
                .ToArray();
            int count = files.Length;
            float lastProgressUpdate = Time.time;
            int cur = 0;

            foreach (FileInfo file in files)
            {
                if (ImageExtensions.Contains(file.Extension.ToLower()))
                {
                    string fileName = file.Name;
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    byte[] bytes = File.ReadAllBytes(file.FullName);
                    Texture2D tex = new(2, 2);
                    tex.LoadImage(bytes);
                    tex.name = fileName;
                    
                    var modGuid = Directory.GetParent(file.DirectoryName!)?.Name;
                    if (modGuid != null && modGuid != "plugins")
                    {
                        fileName = $"{modGuid}/{fileName}";
                    }

                    AddResourceOverride(fileNameWithoutExtension.Replace(".", "/"), fileName, tex);
                    
                    overrideCount++;
                }

                if (AudioExtensions.Contains(file.Extension.ToLower()))
                {
                    string fileName = file.Name;
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    UnityWebRequest wr = new(file.FullName);
                    wr.downloadHandler = new DownloadHandlerAudioClip(fileName, AudioType.UNKNOWN);
                    wr.SendWebRequest();
                    while (!wr.isDone) { }

                    AudioClip clip = DownloadHandlerAudioClip.GetContent(wr);
                    clip.name = fileName;
                    
                    var modGuid = Directory.GetParent(file.DirectoryName!)?.Name;
                    if (modGuid != null && modGuid != "plugins")
                    {
                        fileName = $"{modGuid}/{fileName}";
                    }
                    
                    AddResourceOverride(fileNameWithoutExtension.Replace(".", "/"), fileName, clip);

                    overrideCount++;
                }

                cur++;
                if (Time.time - lastProgressUpdate > 1f)
                {
                    lastProgressUpdate = Time.time;
                    UpdateConsoleLogLoadingBar($"Loading resource overrides from {dir.Name}", cur, count);
                }
            }

            if (overrideCount != 0)
            {
                Log.LogInfo($"Loaded {overrideCount} resource overrides from {dir.Name}");
            }
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    internal static void ImportCharacters(DirectoryInfo dir)
    {
        try
        {
            if (!dir.Exists)
            {
                return;
            }

            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
                .Where(f => f.Extension.ToLower() == ".json")
                .ToArray();
            int count = files.Length;
            float lastProgressUpdate = Time.time;
            int cur = 0;
            foreach (FileInfo file in files)
            {
                Character character = ModdedCharacterManager.ImportCharacter(file.FullName, out string overrideMode);
                if (character == null || character.name == null ||
                    (character.id == 0 && overrideMode.ToLower() == "override"))
                {
                    Log.LogError($"Failed to import character from {file.FullName}.");
                    continue;
                }
                
                var name = file.Name;
                var guid = Directory.GetParent(file.DirectoryName!)?.Name;
                if (guid != null && guid != "plugins")
                {
                    name = $"{guid}/{name}";
                }
                
                ImportedCharacters.Add(new Tuple<string, string, Character>(name, overrideMode, character));
                FilesToDeleteOnSave.Add(file.FullName);
                cur++;
                if (Time.time - lastProgressUpdate > 1f)
                {
                    lastProgressUpdate = Time.time;
                    UpdateConsoleLogLoadingBar($"Importing characters from {dir.Name}", cur, count);
                }
            }

            Log.LogInfo($"Imported {ImportedCharacters.Count} characters from {dir.Name}");
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    internal static void UpdateConsoleLogLoadingBar(string message, int current, int total)
    {
        string bar = "[";
        for (int i = 0; i < 20; i++)
        {
            if (i < current / (float)total * 20)
            {
                bar += "=";
            }
            else
            {
                bar += " ";
            }
        }

        bar += "]";
        Log.LogInfo($"{message}: {bar} {current}/{total}");
    }

    internal static void CreateBackups()
    {
        if (Instance.MaxBackups.Value == 0)
        {
            return;
        }

        string save = Application.persistentDataPath + "/Save.bytes";
        if (!File.Exists(save))
        {
            return;
        }

        string backup = Path.Combine(Application.persistentDataPath, "backups",
            DateTime.Now.ToString("Save-yyyy-MM-dd_HH-mm-ss") + ".bytes");
        string bd = Path.GetDirectoryName(backup);
        if (!Directory.Exists(bd))
        {
            Directory.CreateDirectory(bd!);
        }

        File.Copy(save, backup);
        string[] files = Directory.GetFiles(bd, "Save-*.bytes");
        if (Instance.MaxBackups.Value < 0)
        {
            return;
        }

        if (files.Length > Instance.MaxBackups.Value)
        {
            Array.Sort(files);
            for (int i = 0; i < files.Length - Instance.MaxBackups.Value; i++)
            {
                File.Delete(files[i]);
            }
        }
    }
}