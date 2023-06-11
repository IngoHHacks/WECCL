using Newtonsoft.Json;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;
using WECCL.Content;
using WECCL.Saves;
using PromoData = WECCL.Content.PromoData;
using Random = UnityEngine.Random;

namespace WECCL;

[BepInPlugin(PluginGuid, PluginName, PluginVer)]
[HarmonyPatch]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "IngoH.WrestlingEmpire.WECCL";
    public const string PluginName = "Wrestling Empire Custom Content Loader";
    public const string PluginVer = "1.3.7";


    internal static List<DirectoryInfo> AllModsImportDirs = new();

    internal static ManualLogSource Log;
    internal static readonly Harmony Harmony = new(PluginGuid);

    internal static string PluginPath;
    internal static string PersistentDataPath;

    private static long _nextProgressUpdate = DateTime.Now.Ticks;

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

    private static readonly List<string> MeshExtensions = new() { ".mesh", "" };

    private static readonly List<string> PromoExtensions = new() { ".promo" };

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

    internal static ConfigEntry<bool> DebugRender { get; set; }

    public static float CharactersVersion => Characters.latestVersion;
    public const float PluginCharacterVersion = 1.56f;
    public const float PluginVersion = 1.58f;

    public const bool PreRelease = false;
    public static string[] PreReleaseReasons = { };

    private void Awake()
    {
        try
        {
            // Keep on top
            Log = this.Logger;
            Instance = this;
            PluginPath = Path.GetDirectoryName(this.Info.Location) ?? string.Empty;
            PersistentDataPath = Path.Combine(Application.persistentDataPath, "WECCL");
            if (!Directory.Exists(PersistentDataPath))
            {
                Directory.CreateDirectory(PersistentDataPath);
            }
            // End of keep on top

            var egg = Secrets.GetEasterEgg();
            if (egg != null)
            {
                Log.LogInfo(egg);
            }


            if (PreRelease)
            {
                Log.LogWarning("This is a pre-release version. It may contain bugs and/or unfinished features.");
                foreach (var reason in PreReleaseReasons)
                {
                    switch (reason)
                    {
                        case "GameUpdate":
                            Log.LogWarning("Due to a recent game update, some features may not work as intended.");
                            break;
                        case "Experimental":
                            Log.LogWarning(
                                "This version contains experimental features that may not work as intended.");
                            break;
                        default:
                            Log.LogWarning("This version may not work as intended for the following reason: " + reason);
                            break;
                    }
                }
            }

            if (CharactersVersion != PluginCharacterVersion)
            {
                throw new Exception($"Unsupported game version: {CharactersVersion}");
            }

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
            DebugRender = this.Config.Bind("General", "DebugRender", false,
                "Enable debug rendering. This will render debug information on the screen, such as collision boxes.");

            CreateBackups();

            Locations.MoveLegacyLocations();
            Locations.CreateDirectories();

            StartCoroutine(LoadContent.Load());
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    private void OnEnable()
    {
        if (CharactersVersion != PluginCharacterVersion)
        {
            return;
        }

        try
        {
            Harmony.PatchAll();
        }
        catch (Exception e)
        {
            Log.LogError(
                "Failed to patch with Harmony. This is likely caused by the game version being incompatible with the plugin version. The current plugin version is v" +
                PluginVersion);
            
            Log.LogError(e);
            Harmony.UnpatchSelf();
        }

        this.Logger.LogInfo($"Loaded {PluginName}!");
    }

    private void OnDisable()
    {
        if (CharactersVersion != PluginCharacterVersion)
        {
            return;
        }
        Harmony.UnpatchSelf();
        this.Logger.LogInfo($"Unloaded {PluginName}!");
    }

    internal static void FindContent(string modPath, ref List<DirectoryInfo> AllModsAssetsDirs,
        ref List<DirectoryInfo> AllModsOverridesDirs, ref List<DirectoryInfo> AllModsImportDirs,
        ref List<DirectoryInfo> AllModsLibrariesDirs)

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
            
            DirectoryInfo modLibrariesDir = new(Path.Combine(modPath, "Libraries"));
            if (modLibrariesDir.Exists)
            {
                AllModsLibrariesDirs.Add(modLibrariesDir);
                shouldCheckSubDirs = false;
            }

            if (shouldCheckSubDirs)
            {
                foreach (string subDir in Directory.GetDirectories(modPath))
                {
                    FindContent(subDir, ref AllModsAssetsDirs, ref AllModsOverridesDirs, ref AllModsImportDirs, ref AllModsLibrariesDirs);
                }
            }
        }
        catch (Exception e)
        {
            Log.LogError(e);
        }
    }

    internal static IEnumerator LoadAudioClips(DirectoryInfo dir)
    {
        int clipsCount = 0;
        // Load custom audio clips
        if (!dir.Exists)
        {
            yield break;
        }

        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => AudioExtensions.Contains(f.Extension.ToLower())).ToArray();
        int count = files.Length;
        long lastProgressUpdate = DateTime.Now.Ticks;
        int cur = 0;
        foreach (FileInfo file in files)
        {
            var fileName = file.Name;
            try
            {
                var modGuid = FindPluginName(file.DirectoryName);
                if (modGuid != null && modGuid != "plugins")
                {
                    fileName = $"{modGuid}/{fileName}";
                }

                LoadContent._lastItemLoaded = fileName;
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
            if (DateTime.Now.Ticks > _nextProgressUpdate)
            {
                _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                yield return null;
            }
            try
            {
                if (!CacheEnabled.Value || !TryLoadAudioFromCache(fileName, out AudioClip clip, out long time, out string chksum) ||
                    file.LastWriteTimeUtc.Ticks != time || Checksum.GetChecksum(File.ReadAllBytes(file.FullName)) != chksum)
                {

                    UnityWebRequest wr = new(file.FullName);
                    wr.downloadHandler = new DownloadHandlerAudioClip(file.Name, AudioType.UNKNOWN);
                    wr.SendWebRequest();
                    while (!wr.isDone) { }

                    clip = DownloadHandlerAudioClip.GetContent(wr);
                    wr.Dispose();
                    clip.name = fileName;
                    var chksum2 = Checksum.GetChecksum(File.ReadAllBytes(file.FullName));
                    CacheAudioClip(clip, file.LastWriteTimeUtc.Ticks, chksum2);
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
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }

            GC.Collect();
            LoadContent._loadedAssets++;
            if (DateTime.Now.Ticks > _nextProgressUpdate)
            {
                _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                yield return null;
            }
        }

        if (clipsCount != 0)
        {
            Log.LogInfo($"Loaded {clipsCount} custom audio clips from {dir.FullName}");
        }

        if (CustomClips.Count != 0)
        {
            // Update the number of audio clips in the game
            JKPIHABGBGP.CDAIKKJLDDD = VanillaCounts.MusicCount + CustomClips.Count;
            JKPIHABGBGP.BDMIHNKDBDF = new AudioClip[JKPIHABGBGP.CDAIKKJLDDD + 1];
        }

        ContentMappings.ContentMap.MusicNameMap.AddRange(CustomClips.Select(c => c.name));
    }

    private static void CacheAudioClip(AudioClip clip, long ticks, string chksum)
    {
        // Don't cache clips that are too big
        if (clip.samples * clip.channels * 4 > 2000000000)
        {
            return;
        }
        var floatArray = new float[clip.samples * clip.channels];
        clip.GetData(floatArray, 0);
        var byteArray = new byte[floatArray.Length * 4];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        var fileName = clip.name.Replace("/","_") + ".audioclip";
        File.WriteAllBytes(Path.Combine(Locations.Cache.FullName, fileName), byteArray);
        var meta = "channels: " + clip.channels + "\n" +
                   "frequency: " + clip.frequency + "\n" +
                   "length: " + clip.length + "\n" +
                   "samples: " + clip.samples + "\n" +
                   "time: " + ticks + "\n" +
                   "chksum: " + chksum;
        File.WriteAllText(Path.Combine(Locations.Cache.FullName, clip.name.Replace("/","_") + ".meta"), meta);
        GC.Collect();
    }
    
    private static bool TryLoadAudioFromCache(string name, out AudioClip clip, out long time, out string chksum)
    {
        name = name.Replace("/", "_");
        var fileName = name + ".audioclip";
        var path = Path.Combine(Locations.Cache.FullName, fileName);
        if (!File.Exists(path))
        {
            clip = null;
            time = 0;
            chksum = null;
            return false;
        }
        var bytes = File.ReadAllBytes(path);
        var floatArray = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, floatArray, 0, bytes.Length);
        if (!File.Exists(Path.Combine(Locations.Cache.FullName, name + ".meta")))
        {
            clip = null;
            time = 0;
            chksum = null;
            return false;
        }
        var meta = File.ReadAllText(Path.Combine(Locations.Cache.FullName, name + ".meta"));
        var lines = meta.Split('\n');
        var channels = int.Parse(lines[0].Split(' ')[1]);
        var frequency = int.Parse(lines[1].Split(' ')[1]);
        var samples = int.Parse(lines[3].Split(' ')[1]);
        time = long.Parse(lines[4].Split(' ')[1]);
        chksum = lines.Length > 5 ? lines[5].Split(' ')[1] : "";
        clip = AudioClip.Create(name, samples, channels, frequency, false);
        clip.SetData(floatArray, 0);
        return true;
    }

    internal static IEnumerator LoadCostumes(DirectoryInfo dir)
    {
        int costumeCount = 0;
        // Load custom costumes
        if (!dir.Exists)
        {
            yield break;
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
                    Texture2D tex = new(2, 2);
                    try
                    {
                        if (costumeData.Type != typeof(Texture2D) || costumeData.InternalPrefix == "custom")
                        {
                            Log.LogError($"Custom {costumeData.FilePrefix} costumes are currently not supported.");
                        }
                        else
                        {
                            byte[] bytes = File.ReadAllBytes(file.FullName);
                            tex.LoadImage(bytes);
                            tex.name = fileName;
                            var modGuid = FindPluginName(file.DirectoryName);
                            if (modGuid != null && modGuid != "plugins")
                            {
                                fileName = $"{modGuid}/{fileName}";
                            }

                            LoadContent._lastItemLoaded = fileName;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.LogError(e);
                    }

                    if (DateTime.Now.Ticks > _nextProgressUpdate)
                    {
                        _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                        yield return null;
                    }
                    try
                    {
                        var meta = Path.GetFileNameWithoutExtension(file.Name) + ".meta";
                        if (File.Exists(Path.Combine(file.DirectoryName, meta)))
                        {

                            List<string> metaLines =
                                File.ReadAllLines(Path.Combine(file.DirectoryName, meta)).ToList();
                            Dictionary<string, string> metaDict = new();
                            foreach (string line in metaLines)
                            {
                                string[] split = line.Split(new[] { ':' }, 2);
                                if (split.Length == 2)
                                {
                                    metaDict.Add(split[0].Trim(), split[1].Trim());
                                }
                                else if (split.Length == 1)
                                {
                                    metaDict.Add(split[0].Trim(), "");
                                }
                            }

                            costumeData.AddCustomObject(fileName, tex, metaDict);
                        }
                        else
                        {
                            costumeData.AddCustomObject(fileName, tex, new());
                        }

                        costumeCount++;
                    } catch (Exception e)
                    {
                        Log.LogError(e);
                    }
                }
            }

            cur++;
            if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
            {
                lastProgressUpdate = DateTime.Now.Ticks;
                UpdateConsoleLogLoadingBar($"Loading custom costumes from {dir.FullName}", cur, count);
            }
            
            GC.Collect();
            LoadContent._loadedAssets++;
            if (DateTime.Now.Ticks > _nextProgressUpdate)
            {
                _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                yield return null;
            }
        }



        if (costumeCount != 0)
        {
            Log.LogInfo($"Loaded {costumeCount} custom costumes from {dir.FullName}");
        }
    }

    internal static IEnumerator LoadLibraries(DirectoryInfo dir)
    {
        if (!dir.Exists)
        {
            yield break;
        }
        
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => f.Extension.ToLower() == ".dll").ToArray();
        
        int count = files.Length;
        long lastProgressUpdate = DateTime.Now.Ticks;
        int cur = 0;
        foreach (FileInfo file in files)
        {
            string fileName = file.Name;
            try
            {
                Assembly.LoadFrom(file.FullName);
                LoadContent._lastItemLoaded = fileName;
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
            if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
            {
                lastProgressUpdate = DateTime.Now.Ticks;
                UpdateConsoleLogLoadingBar($"Loading custom libraries from {dir.FullName}", cur, count);
            }
            cur++;
            if (DateTime.Now.Ticks > _nextProgressUpdate)
            {
                _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                yield return null;
            }
        }
    }

    internal static IEnumerator LoadPromos(DirectoryInfo dir)
    {
        int promoCount = 0;
        // Load custom promos
        if (!dir.Exists)
        {
            yield break;
        }

        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => PromoExtensions.Contains(f.Extension.ToLower())).ToArray();
        int count = files.Length;
        long lastProgressUpdate = DateTime.Now.Ticks;
        int cur = 0;
        foreach (FileInfo file in files)
        {
            string fileName = file.Name;
            try
            {
                var modGuid = FindPluginName(file.DirectoryName);
                if (modGuid != null && modGuid != "plugins")
                {
                    fileName = $"{modGuid}/{fileName}";
                }

                LoadContent._lastItemLoaded = fileName;
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
            if (DateTime.Now.Ticks > _nextProgressUpdate)
            {
                _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                yield return null;
            }
            try
            {
                var promo = PromoData.CreatePromo(file.FullName);
                promoCount++;
                cur++;
                if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
                {
                    lastProgressUpdate = DateTime.Now.Ticks;
                    UpdateConsoleLogLoadingBar($"Loading custom audio clips from {dir.FullName}", cur, count);
                }

                CustomContent.PromoData.Add(promo);
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }

            GC.Collect();
            LoadContent._loadedAssets++;
            if (DateTime.Now.Ticks > _nextProgressUpdate)
            {
                _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                yield return null;
            }
        }

        if (promoCount != 0)
        {
            Log.LogInfo($"Loaded {promoCount} custom promos from {dir.FullName}");
        }
    }
    
    internal static IEnumerator LoadMeshes(DirectoryInfo dir)
    {
        int meshCount = 0;
        // Load custom meshes
        if (!dir.Exists)
        {
            yield break;
        }
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => MeshExtensions.Contains(f.Extension.ToLower())).ToArray();
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
                    Mesh mesh = null;
                    try
                    {
                        if (costumeData.Type != typeof(Mesh))
                        {
                            Log.LogError($"{costumeData.FilePrefix} is not a mesh.");
                        }
                        else
                        {
                            mesh = AssetBundle.LoadFromFile(file.FullName).LoadAllAssets<Mesh>().First();
                            mesh.name = fileName;

                            var modGuid = FindPluginName(file.DirectoryName);
                            if (modGuid != null && modGuid != "plugins")
                            {
                                fileName = $"{modGuid}/{fileName}";
                            }

                            LoadContent._lastItemLoaded = fileName;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.LogError(e);
                    }

                    yield return null;
                    try
                    {
                        var meta = Path.GetFileNameWithoutExtension(file.Name) + ".meta";
                        if (File.Exists(Path.Combine(file.DirectoryName, meta)))
                        {

                            List<string> metaLines =
                                File.ReadAllLines(Path.Combine(file.DirectoryName, meta)).ToList();
                            Dictionary<string, string> metaDict = new();
                            foreach (string line in metaLines)
                            {
                                string[] split = line.Split(new[] { ':' }, 2);
                                if (split.Length == 2)
                                {
                                    metaDict.Add(split[0].Trim(), split[1].Trim());
                                }
                                else if (split.Length == 1)
                                {
                                    metaDict.Add(split[0].Trim(), "");
                                }
                            }

                            costumeData.AddCustomObject(fileName, mesh, metaDict);
                        }
                        else
                        {
                            costumeData.AddCustomObject(fileName, mesh, new());
                        }

                        meshCount++;
                    } catch (Exception e)
                    {
                        Log.LogError(e);
                    }
                    break;
                }
            }

            if (file.Directory?.Name == "arena")
            {
                GameObject arena;
                try
                {
                    arena = AssetBundle.LoadFromFile(file.FullName).LoadAllAssets<GameObject>().First();
                    arena.name = fileName;
                    CustomArenaPrefabs.Add(arena);
                    World.no_locations++;
                    meshCount++;
                }
                catch (Exception e)
                {
                    Log.LogError(e);
                }
            }
            cur++;
            if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
            {
                lastProgressUpdate = DateTime.Now.Ticks;
                UpdateConsoleLogLoadingBar($"Loading custom meshes from {dir.FullName}", cur, files.Length);
            }
        }

        if (meshCount != 0)
        {
            Log.LogInfo($"Loaded {meshCount} custom meshes from {dir.FullName}");
        }
    }
    
    internal static IEnumerator LoadOverrides(DirectoryInfo dir)
    {
        int overrideCount = 0;
        // Load resource overrides
        if (!dir.Exists)
        {
            yield break;
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
                try
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

                    LoadContent._lastItemLoaded = fileName;
                    AddResourceOverride(fileNameWithoutExtension.Replace(".", "/"), fileName, tex);
                }
                catch (Exception e)
                {
                    Log.LogError(e);
                }

                overrideCount++;
                GC.Collect();
                LoadContent._loadedAssets++;
                if (DateTime.Now.Ticks > _nextProgressUpdate)
                {
                    _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                    yield return null;
                }
            }
            else if (AudioExtensions.Contains(file.Extension.ToLower()))
            {
                string fileName = file.Name;
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                try
                {

                    var modGuid = FindPluginName(file.DirectoryName);
                    if (modGuid != null && modGuid != "plugins")
                    {
                        fileName = $"{modGuid}/{fileName}";
                    }

                    LoadContent._lastItemLoaded = fileName;
                } catch (Exception e)
                {
                    Log.LogError(e);
                }
                if (DateTime.Now.Ticks > _nextProgressUpdate)
                {
                    _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                    yield return null;
                }
                try
                {
                    if (!CacheEnabled.Value || !TryLoadAudioFromCache(fileName, out AudioClip clip, out long time, out string chksum) ||
                        file.LastWriteTimeUtc.Ticks != time || Checksum.GetChecksum(File.ReadAllBytes(file.FullName)) != chksum)
                    {
                        UnityWebRequest wr = new(file.FullName);
                        wr.downloadHandler = new DownloadHandlerAudioClip(file.Name, AudioType.UNKNOWN);
                        wr.SendWebRequest();
                        while (!wr.isDone) { }

                        clip = DownloadHandlerAudioClip.GetContent(wr);
                        wr.Dispose();
                        clip.name = fileName;
                        var chksum2 = Checksum.GetChecksum(File.ReadAllBytes(file.FullName));
                        CacheAudioClip(clip, file.LastWriteTimeUtc.Ticks, chksum2);
                    }

                    clip.name = fileName;

                    AddResourceOverride(fileNameWithoutExtension.Replace(".", "/"), fileName, clip);

                    overrideCount++;
                }
                catch (Exception e)
                {
                    Log.LogError(e);
                }
                cur++;
                if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
                {
                    lastProgressUpdate = DateTime.Now.Ticks;
                    UpdateConsoleLogLoadingBar($"Loading resource overrides from {dir.FullName}", cur, count);
                }
                GC.Collect();
                LoadContent._loadedAssets++;
                if (DateTime.Now.Ticks > _nextProgressUpdate)
                {
                    _nextProgressUpdate = DateTime.Now.Ticks + 666666;
                    yield return null;
                }
            }
        }

        if (overrideCount != 0)
        {
            Log.LogInfo($"Loaded {overrideCount} resource overrides from {dir.FullName}");
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
                .Where(f => f.Extension.ToLower() == ".character")
                .ToArray();
            int count = files.Length;
            long lastProgressUpdate = DateTime.Now.Ticks;
            int cur = 0;
            foreach (FileInfo file in files)
            {
                var json = File.ReadAllText(file.FullName);
                BetterCharacterDataFile character = JsonConvert.DeserializeObject<BetterCharacterDataFile>(json);
                if (character == null)
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
                
                character._guid = name;

                ImportedCharacters.Add(character);
                FilesToDeleteOnSave.Add(file.FullName);
                cur++;
                if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
                {
                    lastProgressUpdate = DateTime.Now.Ticks;
                    UpdateConsoleLogLoadingBar($"Importing characters from {dir.FullName}", cur, count);
                }
            }

            if (ImportedCharacters.Count > 0)
            {
                Log.LogInfo($"Imported {ImportedCharacters.Count} character(s) from {dir.FullName}");
            }
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

    public static int CountFiles(List<DirectoryInfo> dirs, LoadContent.ContentType type)
    {
        int count = 0;
        foreach (DirectoryInfo dir in dirs)
        {
            List<string> extensions = new();
            if ((type & LoadContent.ContentType.Costume) != 0)
            {
                extensions.AddRange(ImageExtensions);
            }
            if ((type & LoadContent.ContentType.Audio) != 0)
            {
                extensions.AddRange(AudioExtensions);
            }
            if ((type & LoadContent.ContentType.Mesh) != 0)
            {
                extensions.AddRange(MeshExtensions);
            }
            if ((type & LoadContent.ContentType.Promo) != 0)
            {
                extensions.AddRange(PromoExtensions);
            }
            count += dir
                .GetFiles("*", SearchOption.AllDirectories)
                .Count(f => extensions.Contains(f.Extension.ToLower()));
        }
        return count;
    }
}