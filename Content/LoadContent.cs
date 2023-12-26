using Newtonsoft.Json;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;
using WECCL.Animation;
using WECCL.Patches;
using WECCL.Saves;

namespace WECCL.Content;

internal static class LoadContent
{
    [Flags]
    public enum ContentType
    {
        None = 0,
        Costume = 1,
        Audio = 2,
        Mesh = 4,
        Promo = 8,
        CharacterImports = 16,
        AllAssets = Costume | Audio | Mesh | Promo,
        All = AllAssets | CharacterImports
    }

    internal static bool ModsLoaded;
    internal static float ProgressGradual = 0f;
    
    internal static int TotalAssets;
    internal static int LoadedAssets = 0;
    internal static string LastAsset = "";

    internal enum LoadPhase
    {
        None,
        Counting,
        Libraries,
        Promos,
        Audio,
        Costumes,
        Asset_Bundles,
        Overrides,
        Characters,
        Finalizing
    }
    
    internal static LoadPhase LoadingPhase = LoadPhase.None;
    
    internal static long LastProgressUpdate = DateTime.Now.Ticks;

    internal static float _progress => TotalAssets == 0 ? 1f : (float)LoadedAssets / TotalAssets;

    internal static IEnumerator Load()
    {
        Aliases.Load();
        
        LoadingPhase = LoadPhase.Counting;

        List<DirectoryInfo> AllModsAssetsDirs = new();
        List<DirectoryInfo> AllModsOverridesDirs = new();
        List<DirectoryInfo> AllModsLibrariesDirs = new();
        List<DirectoryInfo> AllModsImportDirs = new();

        foreach (string modPath in Directory.GetDirectories(Path.Combine(Paths.BepInExRootPath, "plugins")))
        {
            FindContent(modPath, ref AllModsAssetsDirs, ref AllModsOverridesDirs, ref AllModsImportDirs,
                ref AllModsLibrariesDirs);
        }

        if (Directory.Exists(Path.Combine(Paths.BepInExRootPath, "plugins", "Assets")))
        {
            AllModsAssetsDirs.Add(new DirectoryInfo(Path.Combine(Paths.BepInExRootPath, "plugins", "Assets")));
        }

        if (Directory.Exists(Path.Combine(Paths.BepInExRootPath, "plugins", "Overrides")))
        {
            AllModsOverridesDirs.Add(new DirectoryInfo(Path.Combine(Paths.BepInExRootPath, "plugins", "Overrides")));
        }

        if (Directory.Exists(Path.Combine(Paths.BepInExRootPath, "plugins", "Import")))
        {
            AllModsImportDirs.Add(new DirectoryInfo(Path.Combine(Paths.BepInExRootPath, "plugins", "Import")));
        }

        if (Directory.Exists(Path.Combine(Paths.BepInExRootPath, "plugins", "Libraries")))
        {
            AllModsLibrariesDirs.Add(new DirectoryInfo(Path.Combine(Paths.BepInExRootPath, "plugins", "Libraries")));
        }
       

        if (AllModsAssetsDirs.Count > 0)
        {
            LogInfo($"Found {AllModsAssetsDirs.Count} mod(s) with Assets directories.");
        }

        if (AllModsOverridesDirs.Count > 0)
        {
            LogInfo($"Found {AllModsOverridesDirs.Count} mod(s) with Overrides directories.");
        }

        TotalAssets += CountFiles(AllModsAssetsDirs, ContentType.AllAssets);
        TotalAssets += CountFiles(AllModsOverridesDirs, ContentType.AllAssets);
        TotalAssets += CountFiles(AllModsImportDirs, ContentType.CharacterImports);


        VanillaCounts.Data.MusicCount = UnmappedSound.NABPGAFNBMP;
        VanillaCounts.Data.NoLocations = World.no_locations;
        VanillaCounts.Data.NoFeds = Characters.no_feds;
        
        LoadingPhase = LoadPhase.Libraries;
        foreach (DirectoryInfo dir in AllModsLibrariesDirs)
        {
            yield return LoadLibraries(dir);
        }

        if (Plugin.EnableCustomContent.Value)
        {
            LoadingPhase = LoadPhase.Promos;
            foreach (DirectoryInfo modAssetsDir in AllModsAssetsDirs)
            {
                yield return LoadPromos(modAssetsDir);
            }
            LoadingPhase = LoadPhase.Audio;
            foreach (DirectoryInfo modAssetsDir in AllModsAssetsDirs)
            {
                yield return LoadAudioClips(modAssetsDir);
            }
            LoadingPhase = LoadPhase.Costumes;
            foreach (DirectoryInfo modAssetsDir in AllModsAssetsDirs)
            {
                yield return LoadCostumes(modAssetsDir);
            }
            LoadingPhase = LoadPhase.Asset_Bundles;
            foreach (DirectoryInfo modAssetsDir in AllModsAssetsDirs)
            {
                yield return LoadAssetBundles(modAssetsDir);
            }
        }

        PromoPatch.PatchPromoInfo();

        if (Plugin.EnableOverrides.Value)
        {
            LoadingPhase = LoadPhase.Overrides;
            foreach (DirectoryInfo modOverridesDir in AllModsOverridesDirs)
            {
                yield return LoadOverrides(modOverridesDir);
            }
        }

        yield return new WaitUntil(() => ContentPatch._contentLoaded);

        ContentPatch._internalCostumeCounts[CustomCostumes["legs_material"].InternalPrefix] =
            UnmappedTextures.BFEEOEOEFPG[1];
        UnmappedTextures.BFEEOEOEFPG[1] += CustomCostumes["legs_material"].Count;
        ContentMappings.ContentMap.MaterialNameMap[1]
            .AddRange(CustomCostumes["legs_material"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_flesh"].InternalPrefix] = UnmappedTextures.HJHFIPCDPBD[1];
        UnmappedTextures.HJHFIPCDPBD[1] += CustomCostumes["legs_flesh"].Count;
        ContentMappings.ContentMap.FleshNameMap[1]
            .AddRange(CustomCostumes["legs_flesh"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_shape"].InternalPrefix] = UnmappedTextures.ONGGFJMFJDP[1];
        UnmappedTextures.ONGGFJMFJDP[1] += CustomCostumes["legs_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[1]
            .AddRange(CustomCostumes["legs_shape"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_material"].InternalPrefix] =
            UnmappedTextures.BFEEOEOEFPG[2];
        UnmappedTextures.BFEEOEOEFPG[2] += CustomCostumes["body_material"].Count;
        ContentMappings.ContentMap.MaterialNameMap[2]
            .AddRange(CustomCostumes["body_material"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_flesh_male"].InternalPrefix] =
            UnmappedTextures.HJHFIPCDPBD[2];
        UnmappedTextures.HJHFIPCDPBD[2] += CustomCostumes["body_flesh_male"].Count;
        ContentMappings.ContentMap.FleshNameMap[2]
            .AddRange(CustomCostumes["body_flesh_male"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_flesh_female"].InternalPrefix] =
            UnmappedTextures.DAKBNPKLNGC;
        UnmappedTextures.DAKBNPKLNGC += CustomCostumes["body_flesh_female"].Count;
        ContentMappings.ContentMap.BodyFemaleNameMap.AddRange(CustomCostumes["body_flesh_female"].CustomObjects
            .Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_shape"].InternalPrefix] = UnmappedTextures.ONGGFJMFJDP[2];
        UnmappedTextures.ONGGFJMFJDP[2] += CustomCostumes["body_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[2]
            .AddRange(CustomCostumes["body_shape"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_female"].InternalPrefix] = UnmappedTextures.GFKAKKGLBOI;
        UnmappedTextures.GFKAKKGLBOI += CustomCostumes["face_female"].Count;
        ContentMappings.ContentMap.FaceFemaleNameMap.AddRange(CustomCostumes["face_female"].CustomObjects
            .Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_male"].InternalPrefix] = UnmappedTextures.BFEEOEOEFPG[3];
        UnmappedTextures.BFEEOEOEFPG[3] += CustomCostumes["face_male"].Count;
        ContentMappings.ContentMap.MaterialNameMap[3]
            .AddRange(CustomCostumes["face_male"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.HJHFIPCDPBD[3] += 0; // face_flesh (default 0)
        ContentPatch._internalCostumeCounts[CustomCostumes["face_shape"].InternalPrefix] = UnmappedTextures.HJHFIPCDPBD[3];
        UnmappedTextures.ONGGFJMFJDP[3] += CustomCostumes["face_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[3]
            .AddRange(CustomCostumes["face_shape"].CustomObjects.Select(c => c.Item1));
        for (UnmappedTextures.IKBHGAKKJMM = 4; UnmappedTextures.IKBHGAKKJMM <= 7; UnmappedTextures.IKBHGAKKJMM++)
        {
            UnmappedTextures.BFEEOEOEFPG[UnmappedTextures.IKBHGAKKJMM] +=
                CustomCostumes["face_male"].Count; // Unknown (default face_male)
            ContentMappings.ContentMap.MaterialNameMap[UnmappedTextures.IKBHGAKKJMM]
                .AddRange(CustomCostumes["face_male"].CustomObjects.Select(c => c.Item1));
            UnmappedTextures.HJHFIPCDPBD[UnmappedTextures.IKBHGAKKJMM] += 0; // face_flesh2 (default face_flesh)
            UnmappedTextures.ONGGFJMFJDP[UnmappedTextures.IKBHGAKKJMM] +=
                CustomCostumes["face_shape"].Count; // Unknown (default face shapes)
            ContentMappings.ContentMap.ShapeNameMap[UnmappedTextures.IKBHGAKKJMM]
                .AddRange(CustomCostumes["face_shape"].CustomObjects.Select(c => c.Item1));
        }

        for (UnmappedTextures.IKBHGAKKJMM = 8; UnmappedTextures.IKBHGAKKJMM <= 12; UnmappedTextures.IKBHGAKKJMM++)
        {
            if (UnmappedTextures.IKBHGAKKJMM != 10)
            {
                ContentPatch._internalCostumeCounts[CustomCostumes["arms_material"].InternalPrefix] =
                    UnmappedTextures.BFEEOEOEFPG[UnmappedTextures.IKBHGAKKJMM];
                UnmappedTextures.BFEEOEOEFPG[UnmappedTextures.IKBHGAKKJMM] += CustomCostumes["arms_material"].Count;
                ContentMappings.ContentMap.MaterialNameMap[UnmappedTextures.IKBHGAKKJMM]
                    .AddRange(CustomCostumes["arms_material"].CustomObjects.Select(c => c.Item1));
                ContentPatch._internalCostumeCounts[CustomCostumes["arms_flesh"].InternalPrefix] =
                    UnmappedTextures.HJHFIPCDPBD[UnmappedTextures.IKBHGAKKJMM];
                UnmappedTextures.HJHFIPCDPBD[UnmappedTextures.IKBHGAKKJMM] += CustomCostumes["arms_flesh"].Count;
                ContentMappings.ContentMap.FleshNameMap[UnmappedTextures.IKBHGAKKJMM]
                    .AddRange(CustomCostumes["arms_flesh"].CustomObjects.Select(c => c.Item1));
                ContentPatch._internalCostumeCounts[CustomCostumes["arms_shape"].InternalPrefix] =
                    UnmappedTextures.ONGGFJMFJDP[UnmappedTextures.IKBHGAKKJMM];
                UnmappedTextures.ONGGFJMFJDP[UnmappedTextures.IKBHGAKKJMM] += CustomCostumes["arms_shape"].Count;
                ContentMappings.ContentMap.ShapeNameMap[UnmappedTextures.IKBHGAKKJMM]
                    .AddRange(CustomCostumes["arms_shape"].CustomObjects.Select(c => c.Item1));
            }
        }

        ContentPatch._internalCostumeCounts[CustomCostumes["arms_glove"].InternalPrefix] = UnmappedTextures.BFEEOEOEFPG[10];
        UnmappedTextures.BFEEOEOEFPG[10] += CustomCostumes["arms_glove"].Count;
        ContentMappings.ContentMap.MaterialNameMap[10]
            .AddRange(CustomCostumes["arms_glove"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.HJHFIPCDPBD[10] += 0; // arms_glove_flesh (default 1)
        UnmappedTextures.ONGGFJMFJDP[10] += 0; // arms_glove_shape (default 1)
        UnmappedTextures.BFEEOEOEFPG[13] += CustomCostumes["arms_glove"].Count;
        ContentMappings.ContentMap.MaterialNameMap[13]
            .AddRange(CustomCostumes["arms_glove"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.HJHFIPCDPBD[13] += 0; // arms_glove_flesh2 (default arms_glove_flesh)
        UnmappedTextures.ONGGFJMFJDP[13] += 0; // arms_glove_shape2 (default arms_glove_shape)
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_footwear_special"].InternalPrefix] =
            UnmappedTextures.ABKAOIHDJPA;
        UnmappedTextures.ABKAOIHDJPA += CustomCostumes["legs_footwear_special"].Count;
        ContentMappings.ContentMap.SpecialFootwearNameMap.AddRange(CustomCostumes["legs_footwear_special"]
            .CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_footwear"].InternalPrefix] =
            UnmappedTextures.BFEEOEOEFPG[14];
        UnmappedTextures.BFEEOEOEFPG[14] += CustomCostumes["legs_footwear"].Count;
        ContentMappings.ContentMap.MaterialNameMap[14]
            .AddRange(CustomCostumes["legs_footwear"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.HJHFIPCDPBD[14] += 0; // legs_footwear_flesh (default 0)
        UnmappedTextures.ONGGFJMFJDP[14] += 0; // legs_footwear_shape (default 0)
        UnmappedTextures.BFEEOEOEFPG[15] += CustomCostumes["legs_footwear"].Count;
        ContentMappings.ContentMap.MaterialNameMap[15]
            .AddRange(CustomCostumes["legs_footwear"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.HJHFIPCDPBD[15] += 0; // legs_footwear_flesh2 (default legs_footwear_flesh)
        UnmappedTextures.ONGGFJMFJDP[15] += 0; // legs_footwear_shape2 (default legs_footwear_shape)
        ContentPatch._internalCostumeCounts[CustomCostumes["body_collar"].InternalPrefix] =
            UnmappedTextures.BFEEOEOEFPG[16];
        UnmappedTextures.BFEEOEOEFPG[16] += CustomCostumes["body_collar"].Count;
        ContentMappings.ContentMap.MaterialNameMap[16]
            .AddRange(CustomCostumes["body_collar"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.HJHFIPCDPBD[16] += 0; // body_collar_flesh (default 1)  
        UnmappedTextures.ONGGFJMFJDP[16] += 0; // body_collar_shape (default 0)
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_texture_transparent"].InternalPrefix] =
            UnmappedTextures.MHOGCFLJEFN;
        UnmappedTextures.MHOGCFLJEFN += CustomCostumes["hair_texture_transparent"].Count;
        ContentMappings.ContentMap.TransparentHairMaterialNameMap.AddRange(
            CustomCostumes["hair_texture_transparent"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_texture_solid"].InternalPrefix] =
            UnmappedTextures.BFEEOEOEFPG[17];
        UnmappedTextures.BFEEOEOEFPG[17] += CustomCostumes["hair_texture_solid"].Count;
        ContentMappings.ContentMap.MaterialNameMap[17]
            .AddRange(CustomCostumes["hair_texture_solid"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.HJHFIPCDPBD[17] += 0; // hair_texture_solid_flesh (default 100)
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_hairstyle_solid"].InternalPrefix] =
            UnmappedTextures.ONGGFJMFJDP[17];
        UnmappedTextures.ONGGFJMFJDP[17] += CustomCostumes["hair_hairstyle_solid"].Count;
        ContentMappings.ContentMap.ShapeNameMap[17]
            .AddRange(CustomCostumes["hair_hairstyle_solid"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_hairstyle_transparent"].InternalPrefix] =
            UnmappedTextures.NLHJDAEKLAB;
        UnmappedTextures.NLHJDAEKLAB += CustomCostumes["hair_hairstyle_transparent"].Count;
        ContentMappings.ContentMap.TransparentHairHairstyleNameMap.AddRange(
            CustomCostumes["hair_hairstyle_transparent"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.BFEEOEOEFPG[18] += 0; // hair_hairstyle_transparent_texture (default 2)
        UnmappedTextures.HJHFIPCDPBD[18] += 0; // hair_hairstyle_transparent_flesh (default 100)
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_extension"].InternalPrefix] =
            UnmappedTextures.ONGGFJMFJDP[18];
        UnmappedTextures.ONGGFJMFJDP[18] += CustomCostumes["hair_extension"].Count;
        ContentMappings.ContentMap.ShapeNameMap[18]
            .AddRange(CustomCostumes["hair_extension"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_shave"].InternalPrefix] = UnmappedTextures.BFEEOEOEFPG[19];
        UnmappedTextures.BFEEOEOEFPG[19] += CustomCostumes["hair_shave"].Count;
        ContentMappings.ContentMap.MaterialNameMap[19]
            .AddRange(CustomCostumes["hair_shave"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_beard"].InternalPrefix] = UnmappedTextures.BFEEOEOEFPG[20];
        UnmappedTextures.BFEEOEOEFPG[20] += CustomCostumes["face_beard"].Count;
        ContentMappings.ContentMap.MaterialNameMap[20]
            .AddRange(CustomCostumes["face_beard"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_mask"].InternalPrefix] = UnmappedTextures.BFEEOEOEFPG[21];
        UnmappedTextures.BFEEOEOEFPG[21] += CustomCostumes["face_mask"].Count;
        ContentMappings.ContentMap.MaterialNameMap[21]
            .AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.BFEEOEOEFPG[22] += CustomCostumes["face_mask"].Count;
        ContentMappings.ContentMap.MaterialNameMap[22]
            .AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.BFEEOEOEFPG[23] += CustomCostumes["body_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[23]
            .AddRange(CustomCostumes["body_pattern"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_pattern"].InternalPrefix] =
            UnmappedTextures.BFEEOEOEFPG[24];
        UnmappedTextures.BFEEOEOEFPG[24] += CustomCostumes["body_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[24]
            .AddRange(CustomCostumes["body_pattern"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_kneepad"].InternalPrefix] = UnmappedTextures.BKMPEAALEEM;
        UnmappedTextures.BKMPEAALEEM += CustomCostumes["legs_kneepad"].Count;
        ContentMappings.ContentMap.KneepadNameMap.AddRange(CustomCostumes["legs_kneepad"].CustomObjects
            .Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_pattern"].InternalPrefix] =
            UnmappedTextures.BFEEOEOEFPG[25];
        UnmappedTextures.BFEEOEOEFPG[25] += CustomCostumes["legs_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[25]
            .AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.BFEEOEOEFPG[26] += CustomCostumes["legs_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[26]
            .AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_laces"].InternalPrefix] = UnmappedTextures.BFEEOEOEFPG[27];
        UnmappedTextures.BFEEOEOEFPG[27] += CustomCostumes["legs_laces"].Count;
        ContentMappings.ContentMap.MaterialNameMap[27]
            .AddRange(CustomCostumes["legs_laces"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.BFEEOEOEFPG[28] += 0; // face_eyewear_texture (default 1)
        ContentPatch._internalCostumeCounts[CustomCostumes["face_headwear"].InternalPrefix] =
            UnmappedTextures.ONGGFJMFJDP[28];
        UnmappedTextures.ONGGFJMFJDP[28] += CustomCostumes["face_headwear"].Count;
        ContentMappings.ContentMap.ShapeNameMap[28]
            .AddRange(CustomCostumes["face_headwear"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["arms_elbow_pad"].InternalPrefix] =
            UnmappedTextures.BFEEOEOEFPG[29];
        UnmappedTextures.BFEEOEOEFPG[29] += CustomCostumes["arms_elbow_pad"].Count;
        ContentMappings.ContentMap.MaterialNameMap[29]
            .AddRange(CustomCostumes["arms_elbow_pad"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["arms_wristband"].InternalPrefix] =
            UnmappedTextures.BFEEOEOEFPG[30];
        UnmappedTextures.BFEEOEOEFPG[30] += CustomCostumes["arms_wristband"].Count;
        ContentMappings.ContentMap.MaterialNameMap[30]
            .AddRange(CustomCostumes["arms_wristband"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.BFEEOEOEFPG[31] += 0; // face_headwear_texture (default face_eyewear_texture)
        UnmappedTextures.ONGGFJMFJDP[31] += CustomCostumes["face_headwear"].Count;
        ContentMappings.ContentMap.ShapeNameMap[31]
            .AddRange(CustomCostumes["face_headwear"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.BFEEOEOEFPG[32] += CustomCostumes["arms_elbow_pad"].Count;
        ContentMappings.ContentMap.MaterialNameMap[32]
            .AddRange(CustomCostumes["arms_elbow_pad"].CustomObjects.Select(c => c.Item1));
        UnmappedTextures.BFEEOEOEFPG[33] += CustomCostumes["arms_wristband"].Count;
        ContentMappings.ContentMap.MaterialNameMap[33]
            .AddRange(CustomCostumes["arms_wristband"].CustomObjects.Select(c => c.Item1));


        if (AllModsImportDirs.Count > 0)
        {
            LogInfo($"Found {AllModsImportDirs.Count} mod(s) with Import directories.");
        }

        if (Plugin.AllowImportingCharacters.Value)
        {
            LoadingPhase = LoadPhase.Characters;
            foreach (DirectoryInfo modImportDir in AllModsImportDirs)
            {
                LogDebug($"Importing characters from {modImportDir.Name}...");
                yield return ImportCharacters(modImportDir);
            }
        }
        LoadingPhase = LoadPhase.Finalizing;

        LoadPrefixes();

        ModsLoaded = true;
    }

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
            LogError(e);
        }
    }

    internal static IEnumerator LoadAudioClips(DirectoryInfo dir)
    {
        // Load custom audio clips
        if (!dir.Exists)
        {
            yield break;
        }

        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => AudioExtensions.Contains(f.Extension.ToLower())).ToArray();
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
            }
            catch (Exception e)
            {
                LogError(e);
            }

            try
            {
                if (!Plugin.CacheEnabled.Value ||
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
                LoadedAssets++;
                LastAsset = fileName;
            }
            catch (Exception e)
            {
                LogError(e);
            }
            yield return Tick();

            GC.Collect();
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

    internal static IEnumerator LoadCostumes(DirectoryInfo dir)
    {
        // Load custom costumes
        if (!dir.Exists)
        {
            yield break;
        }

        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => ImageExtensions.Contains(f.Extension.ToLower())).ToArray();
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
                            LogError($"Custom {costumeData.FilePrefix} costumes are currently not supported.");
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
                        }
                    }
                    catch (Exception e)
                    {
                        LogError(e);
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
                            LoadedAssets++;
                            LastAsset = fileName;

                        }
                        else
                        {
                            costumeData.AddCustomObject(fileName, tex, new Dictionary<string, string>());
                            LoadedAssets++;
                            LastAsset = fileName;
                        }
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }
                    yield return Tick();
                }
            }

            GC.Collect();
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

        foreach (FileInfo file in files)
        {
            string fileName = file.Name;
            try
            {
                Assembly.LoadFrom(file.FullName);
                LoadedAssets++;
                LastAsset = fileName;
            }
            catch (Exception e)
            {
                LogError(e);
            }
            yield return Tick();
        }
    }

    internal static IEnumerator LoadPromos(DirectoryInfo dir)
    {
        // Load custom promos
        if (!dir.Exists)
        {
            yield break;
        }

        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => PromoExtensions.Contains(f.Extension.ToLower())).ToArray();
        foreach (FileInfo file in files)
        {
            try
            {
                PromoData promo = PromoData.CreatePromo(file.FullName);
                if (promo == null)
                {
                    continue;
                }

                promo._id = 1000000 + CustomContent.PromoData.Count;
                CustomContent.PromoData.Add(promo);
                LoadedAssets++;
                LastAsset = promo.Title ?? file.Name;
            }
            catch (Exception e)
            {
                LogError(e);
            }
            yield return Tick();

            GC.Collect();
        }
    }

    internal static IEnumerator LoadAssetBundles(DirectoryInfo dir)
    {
        // Load custom AssetBundles
        if (!dir.Exists)
        {
            yield break;
        }

        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => AssetBundleExtensions.Contains(f.Extension.ToLower())).ToArray();

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
                    LoadedAssets++;
                    LastAsset = fileName;
                }
                catch (Exception e)
                {
                    LogError(e);
                }
                yield return Tick();
            }
            else if (file.Directory?.Name == "animation")
            {
                try
                {
                    string metaPath = file.FullName.Contains(".")
                        ? Path.GetFileNameWithoutExtension(file.FullName) + ".meta"
                        : file.FullName + ".meta";
                    if (!File.Exists(metaPath))
                    {
                        LogError($"No meta file found for {file.FullName}");
                        continue;
                    }

                    var ab = AssetBundle.LoadFromFile(file.FullName);
                    var anim = ab.LoadAllAssets<AnimationClip>().FirstOrDefault() ?? ab
                        .LoadAllAssets<RuntimeAnimatorController>().FirstOrDefault().animationClips.FirstOrDefault();
                    anim.name = fileName;
                    var ad = AnimationParser.ReadFile(metaPath);
                    string receivePath = file.FullName.Contains(".")
                        ? Path.GetFileNameWithoutExtension(file.FullName) + ".receive"
                        : file.FullName + ".receive";
                    if (File.Exists(receivePath))
                    {
                        var ab2 = AssetBundle.LoadFromFile(receivePath);
                        var anim2 = ab2.LoadAllAssets<AnimationClip>().FirstOrDefault() ?? ab2
                            .LoadAllAssets<RuntimeAnimatorController>().FirstOrDefault().animationClips
                            .FirstOrDefault();
                        anim2.name = fileName;
                        ad.ReceiveAnim = anim2;
                    }

                    var modGuid = FindPluginName(file.DirectoryName);
                    if (modGuid != null && modGuid != "plugins")
                    {
                        fileName = $"{modGuid}/{fileName}";
                    }

                    ad.Anim = anim;
                    AnimationData.AddAnimation(ad);
                    ContentMappings.ContentMap.AnimationNameMap.Add(fileName);
                    LoadedAssets++;
                    LastAsset = fileName;
                }
                catch (Exception e)
                {
                    LogError(e);
                }
                yield return Tick();
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
                                LogError($"{costumeData.FilePrefix} is not a mesh.");
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
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e);
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

                                costumeData.AddCustomObject(fileName, mesh, metaDict);
                                LoadedAssets++;
                                LastAsset = fileName;
                                
                            }
                            else
                            {
                                costumeData.AddCustomObject(fileName, mesh, new Dictionary<string, string>());
                                LoadedAssets++;
                                LastAsset = fileName;
                            }
                        }
                        catch (Exception e)
                        {
                            LogError(e);
                        }
                        yield return Tick();
                    }
                }
            }
        }
    }

    internal static IEnumerator LoadOverrides(DirectoryInfo dir)
    {
        // Load resource overrides
        if (!dir.Exists)
        {
            yield break;
        }

        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories).Where(f => ImageExtensions.Contains(f.Extension.ToLower()) || AudioExtensions.Contains(f.Extension.ToLower()))
            .ToArray();

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

                    AddResourceOverride(fileNameWithoutExtension.Replace(".", "/"), fileName, tex);
                    LoadedAssets++;
                    LastAsset = fileName;
                }
                catch (Exception e)
                {
                    LogError(e);
                }
                yield return Tick();
                GC.Collect();
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
                }
                catch (Exception e)
                {
                    LogError(e);
                }

                try
                {
                    if (!Plugin.CacheEnabled.Value ||
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
                    LoadedAssets++;
                    LastAsset = fileName;
                }
                catch (Exception e)
                {
                    LogError(e);
                }
                yield return Tick();
                GC.Collect();
            }
        }
    }

    private static string FindPluginName(string fileDirectoryName)
    {
        DirectoryInfo dir = new(fileDirectoryName);
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

    internal static IEnumerator ImportCharacters(DirectoryInfo dir)
    {
        try
        {
            if (!dir.Exists)
            {
                yield break;
            }

            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories)
                .Where(f => f.Extension.ToLower() == ".character")
                .ToArray();
            foreach (FileInfo file in files)
            {
                try
                {
                    string json = File.ReadAllText(file.FullName);
                    BetterCharacterDataFile character = JsonConvert.DeserializeObject<BetterCharacterDataFile>(json);
                    if (character == null)
                    {
                        LogError($"Failed to import character from {file.FullName}.");
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
                    LoadedAssets++;
                    LastAsset = name;
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }
        }
        catch (Exception e)
        {
            LogError(e);
        }
        yield return Tick();
    }

    internal static int CountFiles(List<DirectoryInfo> dirs, LoadContent.ContentType type)
    {
        int count = 0;
        foreach (DirectoryInfo dir in dirs)
        {
            List<string> extensions = new();
            if ((type & ContentType.Costume) != 0)
            {
                extensions.AddRange(ImageExtensions);
            }

            if ((type & ContentType.Audio) != 0)
            {
                extensions.AddRange(AudioExtensions);
            }

            if ((type & ContentType.Mesh) != 0)
            {
                extensions.AddRange(AssetBundleExtensions);
            }

            if ((type & ContentType.Promo) != 0)
            {
                extensions.AddRange(PromoExtensions);
            }

            if ((type & ContentType.CharacterImports) != 0)
            {
                extensions.Add(".character");
            }

            count += dir
                .GetFiles("*", SearchOption.AllDirectories)
                .Count(f => extensions.Contains(f.Extension.ToLower()));
        }

        return count;
    }
    
    internal static IEnumerator Tick()
    {
        if (_nextProgressUpdate + 10000000 >= DateTime.Now.Ticks)
        {
            _nextProgressUpdate = DateTime.Now.Ticks - 10000000;
        }
        while (_nextProgressUpdate >= DateTime.Now.Ticks)
        {
            _nextProgressUpdate += 10000000 / 30;
            yield return null;
        }
    }
}