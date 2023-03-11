using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using WECCL.Content;
using WECCL.Saves;
using static WECCL.Content.CustomContent;

namespace WECCL
{
    [BepInPlugin(PluginGuid, PluginName, PluginVer)]
    [HarmonyPatch]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance { get; private set; }
        
        public const string PluginGuid = "IngoH.WrestlingEmpire.WECCL";
        public const string PluginName = "Wrestling Empire Custom Content Loader";
        public const string PluginVer = "1.0.2";
        
        internal ConfigEntry<bool> AutoExportCharacters { get; set; }
        internal ConfigEntry<bool> EnableOverrides { get; set; }
        internal ConfigEntry<bool> EnableCustomContent { get; set; }
        internal ConfigEntry<bool> AllowImportingCharacters { get; set; }
        internal ConfigEntry<bool> DeleteImportedCharacters { get; set; }

        internal static DirectoryInfo AssetsDir;
        internal static DirectoryInfo ExportDir;
        internal static DirectoryInfo ImportDir;
        internal static DirectoryInfo OverrideDir;
        
        internal static ManualLogSource Log;
        internal readonly static Harmony Harmony = new(PluginGuid);

        internal static string PluginPath;

        internal static string CustomContentSavePath;
        
        private static readonly List<string> ImageExtensions = new() { ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".gif" };
        private static readonly List<string> AudioExtensions = new() { ".ogg", ".wav", ".mp3", ".aif", ".aiff", ".mod", ".xm", ".it", ".s3m" };
        
        private void Awake()
        {
            try
            {
                Plugin.Log = base.Logger;
                PluginPath = Path.GetDirectoryName(Info.Location) ?? string.Empty;
                CustomContentSavePath = Path.Combine(PluginPath, "CustomContentSaveFile.json");
                
                AssetsDir = new(Path.Combine(PluginPath, "Assets"));
                if (!AssetsDir.Exists)
                {
                    AssetsDir.Create();
                }
                OverrideDir = new(Path.Combine(PluginPath, "Overrides"));
                if (!OverrideDir.Exists)
                {
                    OverrideDir.Create();
                }
                ImportDir = new(Path.Combine(PluginPath, "Import"));
                if (!ImportDir.Exists)
                {
                    ImportDir.Create();
                }
                ExportDir = new(Path.Combine(PluginPath, "Export"));
                if (!ExportDir.Exists)
                {
                    ExportDir.Create();
                }


                Instance = this;

                List<DirectoryInfo> AllModsAssetsDirs = new();
                List<DirectoryInfo> AllModsOverridesDirs = new();
                List<DirectoryInfo> AllModsImportDirs = new();
                foreach (var modPath in Directory.GetDirectories(Path.Combine(Paths.BepInExRootPath, "plugins")))
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
                    Plugin.Log.LogInfo($"Found {AllModsAssetsDirs.Count} mod(s) with Assets directories.");
                }

                if (AllModsOverridesDirs.Count > 0)
                {
                    Plugin.Log.LogInfo($"Found {AllModsOverridesDirs.Count} mod(s) with Overrides directories.");
                }

                if (AllModsImportDirs.Count > 0)
                {
                    Plugin.Log.LogInfo($"Found {AllModsImportDirs.Count} mod(s) with Import directories.");
                }

                AutoExportCharacters = Config.Bind("General", "AutoExportCharacters", true,
                    "Automatically export characters to /Export when the game is saved.");
                EnableOverrides = Config.Bind("General", "EnableOverrides", true,
                    "Enable custom content overrides from /Overrides.");
                EnableCustomContent = Config.Bind("General", "EnableCustomContent", true,
                    "Enable custom content loading from /Assets.");
                AllowImportingCharacters = Config.Bind("General", "AllowImportingCharacters", true,
                    "Allow importing characters from /Import");
                DeleteImportedCharacters = Config.Bind("General", "DeleteImportedCharacters", true,
                    "Delete imported characters from /Import after importing them (and saving the game).");

                if (EnableCustomContent.Value)
                {
                    foreach (var modAssetsDir in AllModsAssetsDirs)
                    {
                        LoadAudioClips(modAssetsDir);
                        LoadCostumes(modAssetsDir);
                    }
                }

                if (EnableOverrides.Value)
                {
                    foreach (var modOverridesDir in AllModsOverridesDirs)
                    {
                        LoadOverrides(modOverridesDir);
                    }
                }

                if (AllowImportingCharacters.Value)
                {
                    foreach (var modImportDir in AllModsImportDirs)
                    {
                        ImportCharacters(modImportDir);
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError(e);
            }
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
                var modAssetsDir = new DirectoryInfo(Path.Combine(modPath, "Assets"));
                if (modAssetsDir.Exists)
                {
                    AllModsAssetsDirs.Add(modAssetsDir);
                    shouldCheckSubDirs = false;
                }

                var modOverridesDir = new DirectoryInfo(Path.Combine(modPath, "Overrides"));
                if (modOverridesDir.Exists)
                {
                    AllModsOverridesDirs.Add(modOverridesDir);
                    shouldCheckSubDirs = false;
                }

                var modImportDir = new DirectoryInfo(Path.Combine(modPath, "Import"));
                if (modImportDir.Exists)
                {
                    AllModsImportDirs.Add(modImportDir);
                    shouldCheckSubDirs = false;
                }

                if (shouldCheckSubDirs)
                {
                    foreach (var subDir in Directory.GetDirectories(modPath))
                    {
                        FindContent(subDir, ref AllModsAssetsDirs, ref AllModsOverridesDirs, ref AllModsImportDirs);
                    }

                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError(e);
            }
        }

        internal static void LoadAudioClips(DirectoryInfo dir)
        {
            try
            {
                int clipsCount = 0;
                // Load custom audio clips
                if (!dir.Exists) return;
                var files = dir.GetFiles("*", SearchOption.AllDirectories)
                    .Where(f => AudioExtensions.Contains(f.Extension.ToLower())).ToArray();
                var count = files.Length;
                var lastProgressUpdate = Time.time;
                int cur = 0;
                foreach (var file in files)
                {
                    var wr = new UnityWebRequest(file.FullName);
                    wr.downloadHandler = new DownloadHandlerAudioClip(file.Name, AudioType.UNKNOWN);
                    wr.SendWebRequest();
                    while (!wr.isDone) { }

                    var clip = DownloadHandlerAudioClip.GetContent(wr);
                    clip.name = file.Name;
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
                    VanillaCounts.MusicCount = CKAMIAJJDBP.GGICEBAECGK;
                    CKAMIAJJDBP.GGICEBAECGK += CustomClips.Count;
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
                if (!dir.Exists) return;
                var files = dir.GetFiles("*", SearchOption.AllDirectories)
                    .Where(f => ImageExtensions.Contains(f.Extension.ToLower())).ToArray();
                var count = files.Length;
                var lastProgressUpdate = Time.time;
                int cur = 0;
                foreach (var file in files)
                {
                    var fileName = file.Name;
                    foreach (var pair in CustomCostumes)
                    {
                        if (fileName.StartsWith(pair.Key) || file.Directory?.Name == pair.Key)
                        {
                            var costumeData = pair.Value;
                            if (costumeData.Type != typeof(Texture2D) || costumeData.InternalPrefix == "custom")
                            {
                                Log.LogError($"Custom {costumeData.FilePrefix} costumes are currently not supported.");
                            }
                            else
                            {
                                var bytes = File.ReadAllBytes(file.FullName);
                                var tex = new Texture2D(2, 2);
                                tex.LoadImage(bytes);
                                tex.name = fileName;
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
                if (!dir.Exists) return;
                var files = dir.GetFiles("*", SearchOption.AllDirectories).Where(f =>
                        ImageExtensions.Contains(f.Extension.ToLower()) ||
                        AudioExtensions.Contains(f.Extension.ToLower()))
                    .ToArray();
                var count = files.Length;
                var lastProgressUpdate = Time.time;
                int cur = 0;
                foreach (var file in files)
                {
                    if (ImageExtensions.Contains(file.Extension.ToLower()))
                    {
                        var fileName = file.Name;
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        var bytes = File.ReadAllBytes(file.FullName);
                        var tex = new Texture2D(2, 2);
                        tex.LoadImage(bytes);
                        tex.name = fileName;
                        ResourceOverridesTextures[fileNameWithoutExtension.Replace(".", "/")] = tex;
                        overrideCount++;
                    }

                    if (AudioExtensions.Contains(file.Extension.ToLower()))
                    {
                        var fileName = file.Name;
                        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        var wr = new UnityWebRequest(file.FullName);
                        wr.downloadHandler = new DownloadHandlerAudioClip(fileName, AudioType.UNKNOWN);
                        wr.SendWebRequest();
                        while (!wr.isDone) { }

                        var clip = DownloadHandlerAudioClip.GetContent(wr);
                        clip.name = fileName;
                        ResourceOverridesAudio[fileNameWithoutExtension.Replace(".", "/")] = clip;
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
                if (!dir.Exists) return;
                var files = dir.GetFiles("*", SearchOption.AllDirectories).Where(f => f.Extension.ToLower() == ".json")
                    .ToArray();
                var count = files.Length;
                var lastProgressUpdate = Time.time;
                int cur = 0;
                foreach (var file in files)
                {
                    var character = ModdedCharacterManager.ImportCharacter(file.FullName);
                    if (character == null || character.name == null || character.id == 0)
                    {
                        Log.LogError($"Failed to import character from {file.FullName}.");
                        continue;
                    }

                    if (character != null)
                    {
                        ImportedCharacters.Add(character);
                        FilesToDeleteOnSave.Add(file.FullName);
                    }
                    cur++;
                    if (Time.time - lastProgressUpdate > 1f)
                    {
                        lastProgressUpdate = Time.time;
                        UpdateConsoleLogLoadingBar($"Importing characters from {dir.Name}", cur, count);
                    }
                }
                Plugin.Log.LogInfo($"Imported {ImportedCharacters.Count} characters from {dir.Name}");
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
        }

        private void OnEnable()
        {
            Harmony.PatchAll();
            Logger.LogInfo($"Loaded {PluginName}!");
        }

        private void OnDisable()
        {
            Harmony.UnpatchSelf();
            Logger.LogInfo($"Unloaded {PluginName}!");
        }
        
        internal static void UpdateConsoleLogLoadingBar(string message, int current, int total)
        {
            var bar = "[";
            for (int i = 0; i < 20; i++)
            {
                if (i < (current / (float)total) * 20)
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
    }
}