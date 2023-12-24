using Newtonsoft.Json;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;
using WECCL.Content;
using WECCL.Animation;
using WECCL.Saves;
using PromoData = WECCL.Content.PromoData;

namespace WECCL;

[BepInPlugin(PluginGuid, PluginName, PluginVer)]
[HarmonyPatch]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "IngoH.WrestlingEmpire.WECCL";
    public const string PluginName = "Wrestling Empire Custom Content Loader";
    public const string PluginVer = "1.7.8";
    public const string PluginPatchVer = "";
    public const string PluginVerLong = "v" + PluginVer + PluginPatchVer;
    public const float PluginCharacterVersion = 1.56f;
    public const float PluginVersion = 1.61f;

    public const bool PreRelease = false;
    public static string[] PreReleaseReasons = { "Testing" };


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

    private static readonly List<string> AssetBundleExtensions = new() { ".mesh", ".assetbundle", ".bundle", "" };

    private static readonly List<string> PromoExtensions = new() { ".promo" };

    internal static Plugin Instance { get; private set; }

    internal static ConfigEntry<bool> AutoExportCharacters { get; set; }
    internal static ConfigEntry<bool> EnableOverrides { get; set; }
    internal static ConfigEntry<bool> EnableCustomContent { get; set; }
    internal static ConfigEntry<bool> UseFullQualityTextures { get; set; }
    internal static ConfigEntry<bool> AllowImportingCharacters { get; set; }
    internal static ConfigEntry<bool> DeleteImportedCharacters { get; set; }
    internal static ConfigEntry<bool> EnableWrestlerSearchScreen { get; set; }
    internal static ConfigEntry<bool> EnableGameUnityLog { get; set; }
    internal static ConfigEntry<string> GameUnityLogLevel { get; set; }
    internal static ConfigEntry<int> BaseFedLimit { get; set; }
    internal static ConfigEntry<int> MaxBackups { get; set; }
    internal static ConfigEntry<bool> CacheEnabled { get; set; }
    internal static ConfigEntry<bool> Debug { get; set; }
    internal static ConfigEntry<bool> DebugRender { get; set; }
    internal static ConfigEntry<string> DataSharingLevel { get; set; }
    internal static ConfigEntry<string> SaveFileName { get; set; }

    public static float CharactersVersion => Characters.latestVersion;

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
            
            AutoExportCharacters = this.Config.Bind("General", "AutoExportCharacters", true,
                "Automatically export characters to /Export when the game is saved.");
            EnableOverrides = this.Config.Bind("General", "EnableOverrides", true,
                "Enable custom content overrides from /Overrides.");
            EnableCustomContent = this.Config.Bind("General", "EnableCustomContent", true,
                "Enable custom content loading from /Assets.");
            UseFullQualityTextures = this.Config.Bind("General", "UseFullQualityTextures", false,
                "(EXPERIMENTAL) Allow WECCL to use the full resolution textures without scaling them down (the game will still change the aspect ratio to fit the texture).");
            AllowImportingCharacters = this.Config.Bind("General", "AllowImportingCharacters", true,
                "Allow importing characters from /Import");
            DeleteImportedCharacters = this.Config.Bind("General", "DeleteImportedCharacters", false,
                "Delete imported characters from /Import after importing them (and saving the game).");
            EnableWrestlerSearchScreen = this.Config.Bind("General", "EnableWrestlerSearchScreen", true,
                "Enable the wrestler search screen in the roster menu.");
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
            DataSharingLevel = this.Config.Bind("General", "DataSharingLevel", "Full",
                new ConfigDescription(
                    "The level of data to share with the developer of this plugin. This data will be used to improve the plugin. If you don't want to share any data, set this to None. All data is anonymous.",
                    new AcceptableValueList<string>("None", "Basic", "Full")));
            SaveFileName = this.Config.Bind("General", "SaveFileName", "ModdedSave",
                "The name of the save file to save to. Set to 'Save' to use the vanilla save file (not recommended). If no modded save file exists, the vanilla save file contents will be copied to a new modded save file. Note that changing this would require manually renaming the save file if you want to continue using it.");
            if (!Directory.Exists(Locations.Data.FullName))
            {
                throw new DirectoryNotFoundException("Data directory not found. Please make sure you copied the Data folder to the same directory as the WECCL DLL.");
            }
            Locations.LoadData();
            AnimationActions.Initialize();
            // End of keep on top
            
            if (PreRelease)
#pragma warning disable CS0162 // Unreachable code detected
            {
                if (PreReleaseReasons.Length == 0)
                {
                    Log.LogWarning("This is a pre-release version. It may contain bugs and/or unfinished features.");
                }
                else
                {
                    foreach (string reason in PreReleaseReasons)
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
                            case "Testing":
                                Log.LogWarning(
                                    "This is an early release of a future version that may not be ready for release yet.");
                                break;
                            default:
                                Log.LogWarning("This version may not work as intended for the following reason: " +
                                               reason);
                                break;
                        }
                    }
                }
            }
#pragma warning restore CS0162 // Unreachable code detected

            if (CharactersVersion != PluginCharacterVersion)
            {
                throw new Exception($"Unsupported game version: {CharactersVersion}");
            }

            string egg = Secrets.GetEasterEgg();
            if (egg != null)
            {
                Log.LogInfo(egg);
            }

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
        string save = Locations.SaveFile.FullName;
        if (!File.Exists(save))
        {
            string vanillaSave = Locations.SaveFileVanilla.FullName;
            if (File.Exists(vanillaSave))
            {
                File.Copy(vanillaSave, save);
            }
            else
            {
                GLPGLJAJJOP.NJMFCPGCKNL(); // Restore default
                GLPGLJAJJOP.OIIAHNGBNIF(); // Save
            }
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
                    FindContent(subDir, ref AllModsAssetsDirs, ref AllModsOverridesDirs, ref AllModsImportDirs,
                        ref AllModsLibrariesDirs);
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
            string fileName = file.Name;
            try
            {
                string modGuid = FindPluginName(file.DirectoryName);
                if (modGuid != null && modGuid != "plugins")
                {
                    fileName = $"{modGuid}/{fileName}";
                }

                LoadContent.LastItemLoaded = fileName;
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
                if (!CacheEnabled.Value ||
                    !TryLoadAudioFromCache(fileName, out AudioClip clip, out long time, out string chksum) ||
                    file.LastWriteTimeUtc.Ticks != time ||
                    Checksum.GetChecksum(File.ReadAllBytes(file.FullName)) != chksum)
                {
                    UnityWebRequest wr = new(file.FullName);
                    wr.downloadHandler = new DownloadHandlerAudioClip(file.Name, AudioType.UNKNOWN);
                    wr.SendWebRequest();
                    while (!wr.isDone) { }

                    clip = DownloadHandlerAudioClip.GetContent(wr);
                    wr.Dispose();
                    clip.name = fileName;
                    string chksum2 = Checksum.GetChecksum(File.ReadAllBytes(file.FullName));
                    CacheAudioClip(clip, file.LastWriteTimeUtc.Ticks, chksum2);
                }

                clip.name = fileName;
                string shortFileName = Path.GetFileNameWithoutExtension(file.Name);
                
                var at = CustomClips.FindIndex(s => string.Compare(s.Name, shortFileName, StringComparison.Ordinal) > 0);
                if (at == -1)
                {
                    at = CustomClips.Count;
                }
                CustomClips.Insert(at, new NamedAudioClip(shortFileName, clip));
                ContentMappings.ContentMap.MusicNameMap.Insert(at, fileName);
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
            LoadContent.LoadedAssets++;
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
            UnmappedSound.NABPGAFNBMP = VanillaCounts.Data.MusicCount + CustomClips.Count;
            UnmappedSound.OOFPHCHKOBE = new AudioClip[UnmappedSound.NABPGAFNBMP + 1];
        }
    }

    private static void CacheAudioClip(AudioClip clip, long ticks, string chksum)
    {
        // Don't cache clips that are too big
        if (clip.samples * clip.channels * 4 > 2000000000)
        {
            return;
        }

        float[] floatArray = new float[clip.samples * clip.channels];
        clip.GetData(floatArray, 0);
        byte[] byteArray = new byte[floatArray.Length * 4];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        string fileName = clip.name.Replace("/", "_") + ".audioclip";
        File.WriteAllBytes(Path.Combine(Locations.Cache.FullName, fileName), byteArray);
        string meta = "channels: " + clip.channels + "\n" +
                      "frequency: " + clip.frequency + "\n" +
                      "length: " + clip.length + "\n" +
                      "samples: " + clip.samples + "\n" +
                      "time: " + ticks + "\n" +
                      "chksum: " + chksum;
        File.WriteAllText(Path.Combine(Locations.Cache.FullName, clip.name.Replace("/", "_") + ".meta"), meta);
        GC.Collect();
    }

#pragma warning disable Harmony003
    private static bool TryLoadAudioFromCache(string name, out AudioClip clip, out long time, out string chksum)
    {
        name = name.Replace("/", "_");
        string fileName = name + ".audioclip";
        string path = Path.Combine(Locations.Cache.FullName, fileName);
        if (!File.Exists(path))
        {
            clip = null;
            time = 0;
            chksum = null;
            return false;
        }

        byte[] bytes = File.ReadAllBytes(path);
        float[] floatArray = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, floatArray, 0, bytes.Length);
        if (!File.Exists(Path.Combine(Locations.Cache.FullName, name + ".meta")))
        {
            clip = null;
            time = 0;
            chksum = null;
            return false;
        }

        string meta = File.ReadAllText(Path.Combine(Locations.Cache.FullName, name + ".meta"));
        string[] lines = meta.Split('\n');
        int channels = int.Parse(lines[0].Split(' ')[1]);
        int frequency = int.Parse(lines[1].Split(' ')[1]);
        int samples = int.Parse(lines[3].Split(' ')[1]);
        time = long.Parse(lines[4].Split(' ')[1]);
        chksum = lines.Length > 5 ? lines[5].Split(' ')[1] : "";
        clip = AudioClip.Create(name, samples, channels, frequency, false);
        clip.SetData(floatArray, 0);
        return true;
    }
#pragma warning restore Harmony003
    
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
                            string modGuid = FindPluginName(file.DirectoryName);
                            if (modGuid != null && modGuid != "plugins")
                            {
                                fileName = $"{modGuid}/{fileName}";
                            }

                            LoadContent.LastItemLoaded = fileName;
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
                        string meta = Path.GetFileNameWithoutExtension(file.Name) + ".meta";
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
                            costumeData.AddCustomObject(fileName, tex, new Dictionary<string, string>());
                        }

                        costumeCount++;
                    }
                    catch (Exception e)
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
            LoadContent.LoadedAssets++;
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
                LoadContent.LastItemLoaded = fileName;
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
                string modGuid = FindPluginName(file.DirectoryName);
                if (modGuid != null && modGuid != "plugins")
                {
                    fileName = $"{modGuid}/{fileName}";
                }

                LoadContent.LastItemLoaded = fileName;
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
                PromoData promo = PromoData.CreatePromo(file.FullName);
                if (promo == null)
                {
                    continue;
                }
                promoCount++;
                cur++;
                if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
                {
                    lastProgressUpdate = DateTime.Now.Ticks;
                    UpdateConsoleLogLoadingBar($"Loading custom audio clips from {dir.FullName}", cur, count);
                }

                promo._id = 1000000 + CustomContent.PromoData.Count;
                CustomContent.PromoData.Add(promo);
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }

            GC.Collect();
            LoadContent.LoadedAssets++;
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

    internal static IEnumerator LoadAssetBundles(DirectoryInfo dir)
    {
        int assetBundleCount = 0;
        // Load custom AssetBundles
        if (!dir.Exists)
        {
            yield break;
        }

        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => AssetBundleExtensions.Contains(f.Extension.ToLower())).ToArray();
        long lastProgressUpdate = DateTime.Now.Ticks;
        int cur = 0;
        foreach (FileInfo file in files)
        {
            string fileName = file.Name;
            if (file.Directory?.Name == "arena")
            {
                GameObject arena;
                try
                {
                    arena = AssetBundle.LoadFromFile(file.FullName).LoadAllAssets<GameObject>().First();
                    arena.name = fileName;
                    CustomArenaPrefabs.Add(arena);
                    World.no_locations++;
                    assetBundleCount++;
                }
                catch (Exception e)
                {
                    Log.LogError(e);
                }
            }
            else if (file.Directory?.Name == "animation")
            {
                try {
                    string metaPath = file.FullName.Contains(".") ? Path.GetFileNameWithoutExtension(file.FullName) + ".meta" : file.FullName + ".meta";
                    if (!File.Exists(metaPath))
                    {
                        Log.LogError($"No meta file found for {file.FullName}");
                        continue;
                    }
                    var ab = AssetBundle.LoadFromFile(file.FullName);
                    var anim = ab.LoadAllAssets<AnimationClip>().FirstOrDefault() ?? ab.LoadAllAssets<RuntimeAnimatorController>().FirstOrDefault().animationClips.FirstOrDefault();
                    anim.name = fileName;
                    var ad = AnimationParser.ReadFile(metaPath);
                    string receivePath = file.FullName.Contains(".") ? Path.GetFileNameWithoutExtension(file.FullName) + ".receive" : file.FullName + ".receive";
                    if (File.Exists(receivePath))
                    {
                        var ab2 = AssetBundle.LoadFromFile(receivePath);
                        var anim2 = ab2.LoadAllAssets<AnimationClip>().FirstOrDefault() ?? ab2.LoadAllAssets<RuntimeAnimatorController>().FirstOrDefault().animationClips.FirstOrDefault();
                        anim2.name = fileName;
                        ad.ReceiveAnim = anim2;
                    }
                    ad.Anim = anim;
                    AnimationData.AddAnimation(ad);
                    
                    var modGuid = FindPluginName(file.DirectoryName);
                    if (modGuid != null && modGuid != "plugins")
                    {
                        fileName = $"{modGuid}/{fileName}";
                    }
                    ContentMappings.ContentMap.AnimationNameMap.Add(fileName);
                    
                    assetBundleCount++;
                }
                catch (Exception e)
                {
                    Log.LogError(e);
                }
            }
            else
            {
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

                                LoadContent.LastItemLoaded = fileName;
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

                            assetBundleCount++;
                        }
                        catch (Exception e)
                        {

                            Log.LogError(e);
                        }

                        break;
                    }
                }
            }

            cur++;
            if (DateTime.Now.Ticks - lastProgressUpdate > 10000000)
            {
                lastProgressUpdate = DateTime.Now.Ticks;
                UpdateConsoleLogLoadingBar($"Loading custom AssetBundles from {dir.FullName}", cur, files.Length);
            }
        }

        if (assetBundleCount != 0)
        {
            Log.LogInfo($"Loaded {assetBundleCount} custom AssetBundles from {dir.FullName}");
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

                    string modGuid = FindPluginName(file.DirectoryName);
                    if (modGuid != null && modGuid != "plugins")
                    {
                        fileName = $"{modGuid}/{fileName}";
                    }

                    LoadContent.LastItemLoaded = fileName;
                    AddResourceOverride(fileNameWithoutExtension.Replace(".", "/"), fileName, tex);
                }
                catch (Exception e)
                {
                    Log.LogError(e);
                }

                overrideCount++;
                GC.Collect();
                LoadContent.LoadedAssets++;
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
                    string modGuid = FindPluginName(file.DirectoryName);
                    if (modGuid != null && modGuid != "plugins")
                    {
                        fileName = $"{modGuid}/{fileName}";
                    }

                    LoadContent.LastItemLoaded = fileName;
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
                    if (!CacheEnabled.Value ||
                        !TryLoadAudioFromCache(fileName, out AudioClip clip, out long time, out string chksum) ||
                        file.LastWriteTimeUtc.Ticks != time ||
                        Checksum.GetChecksum(File.ReadAllBytes(file.FullName)) != chksum)
                    {
                        UnityWebRequest wr = new(file.FullName);
                        wr.downloadHandler = new DownloadHandlerAudioClip(file.Name, AudioType.UNKNOWN);
                        wr.SendWebRequest();
                        while (!wr.isDone) { }

                        clip = DownloadHandlerAudioClip.GetContent(wr);
                        wr.Dispose();
                        clip.name = fileName;
                        string chksum2 = Checksum.GetChecksum(File.ReadAllBytes(file.FullName));
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
                LoadContent.LoadedAssets++;
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
        DirectoryInfo dir = new DirectoryInfo(fileDirectoryName);
        DirectoryInfo child = dir;
        while (dir != null && dir.Name != "plugins")
        {
            child = dir;
            dir = dir.Parent;
        }

        if (dir == null)
        {
            throw new Exception($"Could not find 'plugins' directory for {fileDirectoryName}");
        }
        
        string manifestPath = Path.Combine(child.FullName, "manifest.txt");
        if (File.Exists(manifestPath))
        {
            string[] lines = File.ReadAllLines(manifestPath);
            string author = null;
            string name = null;
            foreach (string line in lines)
            {
                if (line.Trim().ToLower().StartsWith("author:"))
                {
                    author = line.Trim().Substring(7).Trim();
                }
                else if (line.Trim().ToLower().StartsWith("modname:"))
                {
                    name = line.Trim().Substring(8).Trim();
                }
            }
            if (author != null)
            {
                return $"{author}-{name}";
            }
            if (name != null)
            {
                return name;
            }
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
            Log.LogDebug($"Importing {count} character(s) from {dir.FullName}");
            foreach (FileInfo file in files)
            {
                try
                {
                    string json = File.ReadAllText(file.FullName);
                    BetterCharacterDataFile character = JsonConvert.DeserializeObject<BetterCharacterDataFile>(json);
                    if (character == null)
                    {
                        Log.LogError($"Failed to import character from {file.FullName}.");
                        continue;
                    }

                    string name = file.Name;
                    string guid = Directory.GetParent(file.DirectoryName!)?.Name;
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
                    Log.LogDebug($"Imported character {character.CharacterData?.name} from {file.FullName}");
                }
                catch (Exception e)
                {
                    Log.LogError(e);
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

        string save = Locations.SaveFile.FullName;
        if (!File.Exists(save))
        {
            return;
        }

        string backup = Path.Combine(Application.persistentDataPath, "backups",
            DateTime.Now.ToString($"Save-yyyy-MM-dd_HH-mm-ss") + ".bytes");
        string bd = Path.GetDirectoryName(backup);
        if (!Directory.Exists(bd))
        {
            Directory.CreateDirectory(bd!);
        }
        
        if (Directory.GetFiles(bd, "Save-*.bytes").Length == 0)
        {
            File.Copy(save, Path.Combine(bd, "InitialSave.bytes"));
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

    internal static int CountFiles(List<DirectoryInfo> dirs, LoadContent.ContentType type)
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
                extensions.AddRange(AssetBundleExtensions);
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

    public static string GetNonDefaultConfigValues()
    {
        string result = "";
        foreach (KeyValuePair<ConfigDefinition, ConfigEntryBase> pair in Instance.Config.Select(x => new KeyValuePair<ConfigDefinition, ConfigEntryBase>(x.Key, x.Value)))
        {
            if (pair.Value.BoxedValue.ToString() == pair.Value.DefaultValue.ToString())
            {
                continue;
            }
            result += $"{pair.Key.Key}={pair.Value.BoxedValue}\n";
        }

        return result;
    }
}