using JetBrains.Annotations;
using System.Security.Cryptography;
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
    public const string PluginVer = "1.1.4";

    internal static DirectoryInfo AssetsDir;
    internal static DirectoryInfo ExportDir;
    internal static DirectoryInfo ImportDir;
    internal static DirectoryInfo OverrideDir;
    internal static DirectoryInfo CacheDir;
    internal static DirectoryInfo DebugFilesDir;

    internal static List<DirectoryInfo> AllModsImportDirs = new();

    internal static ManualLogSource Log;
    internal static readonly Harmony Harmony = new(PluginGuid);

    internal static string PluginPath;

    internal static string ContentMappingsPath;
    
    internal static string MetaFilePath;

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

    internal static ConfigEntry<bool> AutoExportCharacters { get; set; }
    internal static ConfigEntry<bool> EnableOverrides { get; set; }
    internal static ConfigEntry<bool> EnableCustomContent { get; set; }
    internal static ConfigEntry<bool> AllowImportingCharacters { get; set; }
    internal static ConfigEntry<bool> DeleteImportedCharacters { get; set; }
    internal static ConfigEntry<bool> EnableGameUnityLog { get; set; }
    internal static ConfigEntry<string> GameUnityLogLevel { get; set; }
    internal static ConfigEntry<int> BaseFedLimit { get; set; }
    internal static ConfigEntry<int> MaxBackups { get; set; }
    
    internal static ConfigEntry<bool> CacheEnabled { get; set; }
    
    internal static ConfigEntry<bool> Debug { get; set; }

    public static float GameVersion => Characters.latestVersion;

    private void Awake()
    {
        try
        {
            Log = this.Logger;
            PluginPath = Path.GetDirectoryName(this.Info.Location) ?? string.Empty;

            Instance = this;
            
            AutoExportCharacters = this.Config.Bind("General", "AutoExportCharacters", true,
                "Automatically export characters to /Export when the game is saved.");
            EnableOverrides = this.Config.Bind("General", "EnableOverrides", true,
                "Enable custom content overrides from /Overrides.");
            EnableCustomContent = this.Config.Bind("General", "EnableCustomContent", true,
                "Enable custom content loading from /Assets.");
            AllowImportingCharacters = this.Config.Bind("General", "AllowImportingCharacters", true,
                "Allow importing characters from /Import");
            DeleteImportedCharacters = this.Config.Bind("General", "DeleteImportedCharacters", true,
                "Delete imported characters from /Import after importing them (and saving the game).");
            EnableGameUnityLog = this.Config.Bind("General", "EnableGameUnityLog", true,
                "Enable Unity log messages sent by the game itself. If you don't know what this is, leave it enabled.");
            GameUnityLogLevel = this.Config.Bind("General", "GameUnityLogLevel", "Warning",
                new ConfigDescription(
                    "The log level for Unity log messages sent by the game itself. If you don't know what this is, leave it at Warning.",
                    new AcceptableValueList<string>("Error", "Warning", "Info")));
            BaseFedLimit = this.Config.Bind("General", "BaseFedLimit", 48,
                "The base limit for the number of characters that can be fed's roster. This actual limit may be increased if characters are imported (Experimental).");
            MaxBackups = this.Config.Bind("General", "MaxBackups", 100,
                "The maximum number of backups to keep. Set to 0 to disable backups. Set to -1 to keep all backups.");
            CacheEnabled = this.Config.Bind("General", "CacheEnabled", true,
                "Enable caching of custom content. This will speed up loading times with the downside of more disk space usage. The cache is stored in the .cache folder, which is hidden by default. Disabling this will automatically delete the cache on startup.");
            Debug = this.Config.Bind("General", "Debug", false,
                "Enable debug mode. This will create debugging files in the /Debug folder.");
            
            CreateBackups();
            
            var oldContentMappingsPath = Path.Combine(PluginPath, "CustomContentSaveFile.json");
            var oldMetaFilePath = Path.Combine(PluginPath, "CustomConfigsSaveFile.json");

            ContentMappingsPath = Path.Combine(PluginPath, "ContentMappings.json");
            MetaFilePath = Path.Combine(PluginPath, "Meta.json");
            
            if (File.Exists(oldContentMappingsPath))
            {
                File.Move(oldContentMappingsPath, ContentMappingsPath);
            }
            if (File.Exists(oldMetaFilePath))
            {
                File.Move(oldMetaFilePath, MetaFilePath);
            }
            
            DebugFilesDir = new DirectoryInfo(Path.Combine(PluginPath, "Debug"));

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
            
            CacheDir = new DirectoryInfo(Path.Combine(PluginPath, ".cache"));
            if (CacheEnabled.Value && !CacheDir.Exists)
            {
                CacheDir.Create();
                CacheDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                CacheDir.Refresh();
            } else if (!CacheEnabled.Value && CacheDir.Exists)
            {
                CacheDir.Delete(true);
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

            if (EnableCustomContent.Value)
            {
                foreach (DirectoryInfo modAssetsDir in AllModsAssetsDirs)
                {
                    LoadAudioClips(modAssetsDir);
                    LoadCostumes(modAssetsDir);
                }
            }

            if (EnableOverrides.Value)
            {
                foreach (DirectoryInfo modOverridesDir in AllModsOverridesDirs)
                {
                    LoadOverrides(modOverridesDir);
                }
            }
            
            LoadPrefixes();
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
            long lastProgressUpdate = DateTime.Now.Ticks;
            int cur = 0;
            foreach (FileInfo file in files)
            {
                                
                var fileName = file.Name;
                var modGuid = FindPluginName(file.DirectoryName);
                if (modGuid != null && modGuid != "plugins")
                {
                    fileName = $"{modGuid}/{fileName}";
                }

                if (!CacheEnabled.Value || !TryLoadAudioFromCache(fileName, out AudioClip clip, out long time) || file.LastWriteTimeUtc.Ticks != time)
                {

                    UnityWebRequest wr = new(file.FullName);
                    wr.downloadHandler = new DownloadHandlerAudioClip(file.Name, AudioType.UNKNOWN);
                    wr.SendWebRequest();
                    while (!wr.isDone) { }

                    clip = DownloadHandlerAudioClip.GetContent(wr);
                    wr.Dispose();
                    clip.name = fileName;
                    CacheAudioClip(clip, file.LastWriteTimeUtc.Ticks);
                }

                clip.name = fileName;

                CustomClips.Add(clip);
                clipsCount++;
                cur++;
                if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
                {
                    lastProgressUpdate = DateTime.Now.Ticks;
                    UpdateConsoleLogLoadingBar($"Loading custom audio clips from {dir.FullName}", cur, count);
                }
                GC.Collect();
            }

            if (clipsCount != 0)
            {
                Log.LogInfo($"Loaded {clipsCount} custom audio clips from {dir.FullName}");
            }

            if (CustomClips.Count != 0)
            {
                // Update the number of audio clips in the game
                CKAMIAJJDBP.GGICEBAECGK = VanillaCounts.MusicCount + CustomClips.Count;
                CKAMIAJJDBP.CKEJAMLGLAL = new AudioClip[CKAMIAJJDBP.GGICEBAECGK + 1];
            }

            ContentMappings.ContentMap.MusicNameMap.AddRange(CustomClips.Select(c => c.name));
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    private static void CacheAudioClip(AudioClip clip, long ticks)
    {
        var floatArray = new float[clip.samples * clip.channels];
        clip.GetData(floatArray, 0);
        var byteArray = new byte[floatArray.Length * 4];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        var fileName = clip.name.Replace("/","_") + ".audioclip";
        File.WriteAllBytes(Path.Combine(CacheDir.FullName, fileName), byteArray);
        var meta = "channels: " + clip.channels + "\n" +
                   "frequency: " + clip.frequency + "\n" +
                   "length: " + clip.length + "\n" +
                   "samples: " + clip.samples + "\n" +
                   "time: " + ticks;
        File.WriteAllText(Path.Combine(CacheDir.FullName, clip.name.Replace("/","_") + ".meta"), meta);
    }
    
    private static bool TryLoadAudioFromCache(string name, out AudioClip clip, out long time)
    {
        name = name.Replace("/", "_");
        var fileName = name + ".audioclip";
        var path = Path.Combine(CacheDir.FullName, fileName);
        if (!File.Exists(path))
        {
            clip = null;
            time = 0;
            return false;
        }
        var bytes = File.ReadAllBytes(path);
        var floatArray = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, floatArray, 0, bytes.Length);
        if (!File.Exists(Path.Combine(CacheDir.FullName, name + ".meta")))
        {
            clip = null;
            time = 0;
            return false;
        }
        var meta = File.ReadAllText(Path.Combine(CacheDir.FullName, name + ".meta"));
        var lines = meta.Split('\n');
        var channels = int.Parse(lines[0].Split(' ')[1]);
        var frequency = int.Parse(lines[1].Split(' ')[1]);
        var length = float.Parse(lines[2].Split(' ')[1]);
        var samples = int.Parse(lines[3].Split(' ')[1]);
        time = long.Parse(lines[4].Split(' ')[1]);
        clip = AudioClip.Create(name, samples, channels, frequency, false);
        clip.SetData(floatArray, 0);
        return true;
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
            long lastProgressUpdate = DateTime.Now.Ticks;
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
                            var modGuid = FindPluginName(file.DirectoryName);
                            if (modGuid != null && modGuid != "plugins")
                            {
                                fileName = $"{modGuid}/{fileName}";
                            }
                            
                            var meta = Path.GetFileNameWithoutExtension(file.Name) + ".meta";
                            if (File.Exists(Path.Combine(file.DirectoryName, meta)))
                            {

                                List<string> metaLines =
                                    File.ReadAllLines(Path.Combine(file.DirectoryName, meta)).ToList();
                                List<Tuple<string, string>> metaTuples = new();
                                foreach (string line in metaLines)
                                {
                                    string[] split = line.Split(new[] { ':' }, 2);
                                    if (split.Length == 2)
                                    {
                                        metaTuples.Add(new(split[0].Trim(), split[1].Trim()));
                                    }
                                    else if (split.Length == 1)
                                    {
                                        metaTuples.Add(new(split[0].Trim(), ""));
                                    }
                                }
                                costumeData.AddCustomObject(fileName, tex, metaTuples);
                            }
                            else
                            {
                                costumeData.AddCustomObject(fileName, tex, new());
                            }

                            costumeCount++;
                            GC.Collect();
                        }
                    }
                }

                cur++;
                if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
                {
                    lastProgressUpdate = DateTime.Now.Ticks;
                    UpdateConsoleLogLoadingBar($"Loading custom costumes from {dir.FullName}", cur, count);
                }
            }

            if (costumeCount != 0)
            {
                Log.LogInfo($"Loaded {costumeCount} custom costumes from {dir.FullName}");
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
            long lastProgressUpdate = DateTime.Now.Ticks;
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
                    
                    var modGuid = FindPluginName(file.DirectoryName);
                    if (modGuid != null && modGuid != "plugins")
                    {
                        fileName = $"{modGuid}/{fileName}";
                    }

                    AddResourceOverride(fileNameWithoutExtension.Replace(".", "/"), fileName, tex);
                    
                    overrideCount++;
                    GC.Collect();
                }

                if (AudioExtensions.Contains(file.Extension.ToLower()))
                {
                    string fileName = file.Name;
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                                        
                    var modGuid = FindPluginName(file.DirectoryName);
                    if (modGuid != null && modGuid != "plugins")
                    {
                        fileName = $"{modGuid}/{fileName}";
                    }


                    if (!CacheEnabled.Value || !TryLoadAudioFromCache(fileName, out AudioClip clip, out long time))
                    {

                        UnityWebRequest wr = new(file.FullName);
                        wr.downloadHandler = new DownloadHandlerAudioClip(file.Name, AudioType.UNKNOWN);
                        wr.SendWebRequest();
                        while (!wr.isDone) { }
                        clip = DownloadHandlerAudioClip.GetContent(wr);
                        wr.Dispose();
                        clip.name = fileName;
                        CacheAudioClip(clip, file.LastWriteTimeUtc.Ticks);
                    }

                    clip.name = fileName;
                    
                    AddResourceOverride(fileNameWithoutExtension.Replace(".", "/"), fileName, clip);

                    overrideCount++;
                    GC.Collect();
                }

                cur++;
                if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
                {
                    lastProgressUpdate = DateTime.Now.Ticks;
                    UpdateConsoleLogLoadingBar($"Loading resource overrides from {dir.FullName}", cur, count);
                }
            }

            if (overrideCount != 0)
            {
                Log.LogInfo($"Loaded {overrideCount} resource overrides from {dir.FullName}");
            }
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    private static string FindPluginName(string fileDirectoryName)
    {
        var dir = new DirectoryInfo(fileDirectoryName);
        var child = dir;
        while (dir != null && dir.Name != "plugins")
        {
            child = dir;
            dir = dir.Parent;
        }
        if (dir == null)
        {
            throw new Exception($"Could not find 'plugins' directory for {fileDirectoryName}");
        }
        return child.Name;
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
            long lastProgressUpdate = DateTime.Now.Ticks;
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
                if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
                {
                    lastProgressUpdate = DateTime.Now.Ticks;
                    UpdateConsoleLogLoadingBar($"Importing characters from {dir.FullName}", cur, count);
                }
            }

            Log.LogInfo($"Imported {ImportedCharacters.Count} characters from {dir.FullName}");
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
        if (MaxBackups.Value == 0)
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
        if (MaxBackups.Value < 0)
        {
            return;
        }

        if (files.Length > MaxBackups.Value)
        {
            Array.Sort(files);
            for (int i = 0; i < files.Length - MaxBackups.Value; i++)
            {
                File.Delete(files[i]);
            }
        }
    }
}