using System.Collections;
using WECCL.Patches;
using WECCL.Saves;

namespace WECCL.Content;

public static class LoadContent
{
    [Flags]
    public enum ContentType
    {
        None = 0,
        Costume = 1,
        Audio = 2,
        Mesh = 4,
        Promo = 8,
        All = Costume | Audio | Mesh | Promo
    }

    internal static bool _modsLoaded;
    internal static float _progressGradual = 0f;

    internal static string _lastItemLoaded = "";

    internal static int _totalAssets;
    internal static int _loadedAssets = 0;

    internal static float _progress => _totalAssets == 0 ? 1f : (float)_loadedAssets / _totalAssets;

    internal static IEnumerator Load()
    {
        Aliases.Load();

        List<DirectoryInfo> AllModsAssetsDirs = new();
        List<DirectoryInfo> AllModsOverridesDirs = new();
        List<DirectoryInfo> AllModsLibrariesDirs = new();

        foreach (string modPath in Directory.GetDirectories(Path.Combine(Paths.BepInExRootPath, "plugins")))
        {
            Plugin.FindContent(modPath, ref AllModsAssetsDirs, ref AllModsOverridesDirs, ref Plugin.AllModsImportDirs,
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
            Plugin.AllModsImportDirs.Add(new DirectoryInfo(Path.Combine(Paths.BepInExRootPath, "plugins", "Import")));
        }

        if (Directory.Exists(Path.Combine(Paths.BepInExRootPath, "plugins", "Libraries")))
        {
            AllModsLibrariesDirs.Add(new DirectoryInfo(Path.Combine(Paths.BepInExRootPath, "plugins", "Libraries")));
        }

        if (!AllModsAssetsDirs.Exists(x => x.FullName == Locations.Assets.FullName))
        {
            AllModsAssetsDirs.Add(Locations.Assets);
        }

        if (!AllModsOverridesDirs.Exists(x => x.FullName == Locations.Overrides.FullName))
        {
            AllModsOverridesDirs.Add(Locations.Overrides);
        }

        if (!Plugin.AllModsImportDirs.Exists(x => x.FullName == Locations.Import.FullName))
        {
            Plugin.AllModsImportDirs.Add(Locations.Import);
        }

        if (!AllModsLibrariesDirs.Exists(x => x.FullName == Locations.Libraries.FullName))
        {
            AllModsLibrariesDirs.Add(Locations.Libraries);
        }

        if (AllModsAssetsDirs.Count > 0)
        {
            Plugin.Log.LogInfo($"Found {AllModsAssetsDirs.Count} mod(s) with Assets directories.");
        }

        if (AllModsOverridesDirs.Count > 0)
        {
            Plugin.Log.LogInfo($"Found {AllModsOverridesDirs.Count} mod(s) with Overrides directories.");
        }

        _totalAssets += Plugin.CountFiles(AllModsAssetsDirs, ContentType.All);
        _totalAssets += Plugin.CountFiles(AllModsOverridesDirs, ContentType.All);


        VanillaCounts.MusicCount = UnmappedSound.NABPGAFNBMP;
        VanillaCounts.NoLocations = World.no_locations;
        VanillaCounts.NoFeds = Characters.no_feds;

        foreach (DirectoryInfo dir in AllModsLibrariesDirs)
        {
            yield return Plugin.LoadLibraries(dir);
        }

        if (Plugin.EnableCustomContent.Value)
        {
            foreach (DirectoryInfo modAssetsDir in AllModsAssetsDirs)
            {
                yield return Plugin.LoadPromos(modAssetsDir);
                yield return Plugin.LoadAudioClips(modAssetsDir);
                yield return Plugin.LoadCostumes(modAssetsDir);
                yield return Plugin.LoadAssetBundles(modAssetsDir);
            }
        }

        PromoPatch.PatchPromoInfo();

        if (Plugin.EnableOverrides.Value)
        {
            foreach (DirectoryInfo modOverridesDir in AllModsOverridesDirs)
            {
                yield return Plugin.LoadOverrides(modOverridesDir);
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


        if (Plugin.AllModsImportDirs.Count > 0)
        {
            Plugin.Log.LogInfo($"Found {Plugin.AllModsImportDirs.Count} mod(s) with Import directories.");
        }

        if (Plugin.AllowImportingCharacters.Value)
        {
            foreach (DirectoryInfo modImportDir in Plugin.AllModsImportDirs)
            {
                Plugin.Log.LogDebug($"Importing characters from {modImportDir.Name}...");
                Plugin.ImportCharacters(modImportDir);
            }
        }

        LoadPrefixes();

        _modsLoaded = true;
    }
}