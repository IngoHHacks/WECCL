using BepInEx;
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
        public const string PluginGuid = "IngoH.WrestlingEmpire.WECCL";
        public const string PluginName = "Wrestling Empire Custom Content Loader";
        public const string PluginVer = "1.0.0";

        internal static ManualLogSource Log;
        internal readonly static Harmony Harmony = new(PluginGuid);

        internal static string PluginPath;

        internal static string CustomContentSavePath;
        
        private static bool _costumesLoaded = false;
        
        private static readonly List<string> ImageExtensions = new() { ".png", ".jpg", ".jpeg", ".bmp", ".tga", ".gif" };
        private static readonly List<string> AudioExtensions = new() { ".ogg", ".wav", ".mp3", ".aif", ".aiff", ".mod", ".xm", ".it", ".s3m" };
        
        private void Awake()
        {
            Plugin.Log = base.Logger;
            PluginPath = Path.GetDirectoryName(Info.Location) ?? string.Empty;
            CustomContentSavePath = Path.Combine(PluginPath, "CustomContentSaveFile.json");
            
            LoadAudioClips();
            LoadCostumes();
            LoadOverrides();
            ImportCharacters();
        }

        internal static void LoadAudioClips()
        {
            try
            {
                int clipsCount = 0;
                // Load custom audio clips
                var dir = new DirectoryInfo(Path.Combine(PluginPath, "Assets"));
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
                        UpdateConsoleLogLoadingBar("Loading custom audio clips", cur, count);
                    }

                }

                if (clipsCount != 0)
                {
                    Log.LogInfo($"Loaded {clipsCount} custom audio clips.");
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

        internal static void LoadCostumes()
        {
            try
            {
                if (_costumesLoaded) return;
                _costumesLoaded = true;
                int costumeCount = 0;
                // Load custom costumes
                var dir = new DirectoryInfo(Path.Combine(PluginPath, "Assets"));
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
                        if (fileName.StartsWith(pair.Key + "_") || file.Directory?.Name == pair.Key)
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
                        UpdateConsoleLogLoadingBar("Loading custom costumes", cur, count);
                    }
                }

                if (costumeCount != 0)
                {
                    Log.LogInfo($"Loaded {costumeCount} custom costumes.");
                }
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
        }

        internal static void LoadOverrides()
        {
            try
            {
                int overrideCount = 0;
                // Load resource overrides
                var dir = new DirectoryInfo(Path.Combine(PluginPath, "Overrides"));
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
                        UpdateConsoleLogLoadingBar("Loading resource overrides", cur, count);
                    }
                }

                if (overrideCount != 0)
                {
                    Log.LogInfo($"Loaded {overrideCount} resource overrides.");
                }
            }
            catch (Exception e)
            {
                Log.LogError(e);
            }
        }
        
        internal static void ImportCharacters()
        {
            try
            {
                var dir = new DirectoryInfo(Path.Combine(PluginPath, "Import"));
                if (!dir.Exists) return;
                var files = dir.GetFiles("*", SearchOption.AllDirectories).Where(f => f.Extension.ToLower() == ".json")
                    .ToArray();
                var count = files.Length;
                var lastProgressUpdate = Time.time;
                int cur = 0;
                foreach (var file in files)
                {
                    var character = ModdedCharacterManager.ImportCharacter(file.FullName);
                    if (character == null || character.name == null || character.id == null)
                    {
                        Log.LogError($"Failed to import character from {file.FullName}.");
                        continue;
                    }

                    if (character != null)
                    {
                        ImportedCharacters.Add(character);
                        FilesToDeleteOnSave.Add(file.FullName);
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