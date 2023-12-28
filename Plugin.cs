using WECCL.Content;
using WECCL.Animation;
using WECCL.API;
using WECCL.Saves;

namespace WECCL;

[BepInPlugin(PluginGuid, PluginName, PluginVer)]
[HarmonyPatch]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "IngoH.WrestlingEmpire.WECCL";
    public const string PluginName = "WECCL";
    public const string PluginVer = "1.7.11";
    public const string PluginPatchVer = "";
    public const string PluginVerLong = "v" + PluginVer + PluginPatchVer;
    public const float PluginCharacterVersion = 1.56f;
    public const float PluginVersion = 1.61f;
    public static readonly float GameVersion = MappedGlobals.optVersion;

    public const bool PreRelease = false;
    public static string[] PreReleaseReasons = { "Testing" };


    internal static ManualLogSource Log;
    internal static readonly Harmony Harmony = new(PluginGuid);

    internal static string PluginPath;
    internal static string PersistentDataPath;

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
                    LogWarning("This is a pre-release version. It may contain bugs and/or unfinished features.");
                }
                else
                {
                    foreach (string reason in PreReleaseReasons)
                    {
                        switch (reason)
                        {
                            case "GameUpdate":
                                LogWarning("Due to a recent game update, some features may not work as intended.");
                                break;
                            case "Experimental":
                                LogWarning(
                                    "This version contains experimental features that may not work as intended.");
                                break;
                            case "Testing":
                                LogWarning(
                                    "This is an early release of a future version that may not be ready for release yet.");
                                break;
                            default:
                                LogWarning("This version may not work as intended for the following reason: " +
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
            
            if (GameVersion != PluginVersion)
            {
                if (GameVersion > PluginVersion)
                {
                    LogWarning($"Your game version ({GameVersion}) is newer than the plugin version ({PluginVersion}). Keep in mind that this plugin may not work as intended on this version. Check the Steam Workshop page for updates.");
                }
                else
                {
                    LogWarning($"Your game version ({GameVersion}) is older than the plugin version ({PluginVersion}). Keep in mind that this plugin may not work as intended on this version. Check for updates in the game's properties on Steam.");
                }
            }

            string egg = Secrets.GetEasterEgg();
            if (egg != null)
            {
                Log./* Keep Log here */LogInfo(egg);
            }

            CreateBackups();

            Locations.MoveLegacyLocations();
            Locations.CreateDirectories();

            StartCoroutine(LoadContent.Load());
            
            this.RegisterCustomButton("Reset Imported Characters", () =>
            {
                int numRemoved = CharacterMappings.CharacterMap.PreviouslyImportedCharacters.Count;
                CharacterMappings.CharacterMap.PreviouslyImportedCharacters.Clear();
                CharacterMappings.CharacterMap.PreviouslyImportedCharacterIds.Clear();
                CharacterMappings.CharacterMap.Save();
                MappedSound.Play(MappedSound.flush);
                if (numRemoved == 0)
                {
                    return "No reset needed";
                }
                return $"Reset {numRemoved} imported character{(numRemoved == 1 ? "" : "s")}";
            }, true);
        }
        catch (Exception e)
        {
            LogError(e);
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
            LogError(
                "Failed to patch with Harmony. This is likely caused by the game version being incompatible with the plugin version. The current plugin version is v" +
                PluginVersion);

            LogError(e);
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