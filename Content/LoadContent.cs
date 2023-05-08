using System.Collections;
using WECCL.Patches;
using WECCL.Saves;

namespace WECCL.Content;

public static class LoadContent
{
    internal static bool _modsLoaded = false;
    internal static float _progressGradual = 0f;
    
    internal static string _lastItemLoaded = "";
    
    internal static int _totalAssets = 0;
    internal static int _loadedAssets = 0;
    
    internal static float _progress => _totalAssets == 0 ? 1f : (float)_loadedAssets / _totalAssets;
    
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
    
    internal static IEnumerator Load()
    {
        List<DirectoryInfo> AllModsAssetsDirs = new();
        List<DirectoryInfo> AllModsOverridesDirs = new();
        List<DirectoryInfo> AllModsLibrariesDirs = new();

        foreach (string modPath in Directory.GetDirectories(Path.Combine(Paths.BepInExRootPath, "plugins")))
        {
            Plugin.FindContent(modPath, ref AllModsAssetsDirs, ref AllModsOverridesDirs, ref Plugin.AllModsImportDirs, ref AllModsLibrariesDirs);
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


        VanillaCounts.MusicCount = EHIOFPLJLKH.CJNGPFLPOBM;
        VanillaCounts.NoLocations = World.no_locations;

        foreach (var dir in AllModsLibrariesDirs)
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
                yield return Plugin.LoadMeshes(modAssetsDir);
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
        
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_material"].InternalPrefix] = GameTextures.AOBEBHODFKG[1];
        GameTextures.AOBEBHODFKG[1] += CustomCostumes["legs_material"].Count;
        ContentMappings.ContentMap.MaterialNameMap[1]
            .AddRange(CustomCostumes["legs_material"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_flesh"].InternalPrefix] = GameTextures.GEANFMLIDLN[1];
        GameTextures.GEANFMLIDLN[1] += CustomCostumes["legs_flesh"].Count;
        ContentMappings.ContentMap.FleshNameMap[1]
            .AddRange(CustomCostumes["legs_flesh"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_shape"].InternalPrefix] = GameTextures.AEBNMPCDDAL[1];
        GameTextures.AEBNMPCDDAL[1] += CustomCostumes["legs_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[1]
            .AddRange(CustomCostumes["legs_shape"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_material"].InternalPrefix] = GameTextures.AOBEBHODFKG[2];
        GameTextures.AOBEBHODFKG[2] += CustomCostumes["body_material"].Count;
        ContentMappings.ContentMap.MaterialNameMap[2]
            .AddRange(CustomCostumes["body_material"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_flesh_male"].InternalPrefix] = GameTextures.GEANFMLIDLN[2];
        GameTextures.GEANFMLIDLN[2] += CustomCostumes["body_flesh_male"].Count;
        ContentMappings.ContentMap.FleshNameMap[2]
            .AddRange(CustomCostumes["body_flesh_male"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_flesh_female"].InternalPrefix] = GameTextures.IANHIBOPGJD;
        GameTextures.IANHIBOPGJD += CustomCostumes["body_flesh_female"].Count;
        ContentMappings.ContentMap.BodyFemaleNameMap.AddRange(CustomCostumes["body_flesh_female"].CustomObjects
            .Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_shape"].InternalPrefix] = GameTextures.AEBNMPCDDAL[2];
        GameTextures.AEBNMPCDDAL[2] += CustomCostumes["body_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[2]
            .AddRange(CustomCostumes["body_shape"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_female"].InternalPrefix] = GameTextures.MBDNIAIJFDE;
        GameTextures.MBDNIAIJFDE += CustomCostumes["face_female"].Count;
        ContentMappings.ContentMap.FaceFemaleNameMap.AddRange(CustomCostumes["face_female"].CustomObjects
            .Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_male"].InternalPrefix] = GameTextures.AOBEBHODFKG[3];
        GameTextures.AOBEBHODFKG[3] += CustomCostumes["face_male"].Count;
        ContentMappings.ContentMap.MaterialNameMap[3]
            .AddRange(CustomCostumes["face_male"].CustomObjects.Select(c => c.Item1));
        GameTextures.GEANFMLIDLN[3] += 0; // face_flesh (default 0)
        ContentPatch._internalCostumeCounts[CustomCostumes["face_shape"].InternalPrefix] = GameTextures.GEANFMLIDLN[3];
        GameTextures.AEBNMPCDDAL[3] += CustomCostumes["face_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[3]
            .AddRange(CustomCostumes["face_shape"].CustomObjects.Select(c => c.Item1));
        for (GameTextures.DLMFHPGACEP = 4; GameTextures.DLMFHPGACEP <= 7; GameTextures.DLMFHPGACEP++)
        {
            GameTextures.AOBEBHODFKG[GameTextures.DLMFHPGACEP] +=
                CustomCostumes["face_male"].Count; // Unknown (default face_male)
            ContentMappings.ContentMap.MaterialNameMap[GameTextures.DLMFHPGACEP]
                .AddRange(CustomCostumes["face_male"].CustomObjects.Select(c => c.Item1));
            GameTextures.GEANFMLIDLN[GameTextures.DLMFHPGACEP] += 0; // face_flesh2 (default face_flesh)
            GameTextures.AEBNMPCDDAL[GameTextures.DLMFHPGACEP] +=
                CustomCostumes["face_shape"].Count; // Unknown (default face shapes)
            ContentMappings.ContentMap.ShapeNameMap[GameTextures.DLMFHPGACEP]
                .AddRange(CustomCostumes["face_shape"].CustomObjects.Select(c => c.Item1));
        }

        for (GameTextures.DLMFHPGACEP = 8; GameTextures.DLMFHPGACEP <= 12; GameTextures.DLMFHPGACEP++)
        {
            if (GameTextures.DLMFHPGACEP != 10)
            {
                ContentPatch._internalCostumeCounts[CustomCostumes["arms_material"].InternalPrefix] =
                    GameTextures.AOBEBHODFKG[GameTextures.DLMFHPGACEP];
                GameTextures.AOBEBHODFKG[GameTextures.DLMFHPGACEP] += CustomCostumes["arms_material"].Count;
                ContentMappings.ContentMap.MaterialNameMap[GameTextures.DLMFHPGACEP]
                    .AddRange(CustomCostumes["arms_material"].CustomObjects.Select(c => c.Item1));
                ContentPatch._internalCostumeCounts[CustomCostumes["arms_flesh"].InternalPrefix] =
                    GameTextures.GEANFMLIDLN[GameTextures.DLMFHPGACEP];
                GameTextures.GEANFMLIDLN[GameTextures.DLMFHPGACEP] += CustomCostumes["arms_flesh"].Count;
                ContentMappings.ContentMap.FleshNameMap[GameTextures.DLMFHPGACEP]
                    .AddRange(CustomCostumes["arms_flesh"].CustomObjects.Select(c => c.Item1));
                ContentPatch._internalCostumeCounts[CustomCostumes["arms_shape"].InternalPrefix] =
                    GameTextures.AEBNMPCDDAL[GameTextures.DLMFHPGACEP];
                GameTextures.AEBNMPCDDAL[GameTextures.DLMFHPGACEP] += CustomCostumes["arms_shape"].Count;
                ContentMappings.ContentMap.ShapeNameMap[GameTextures.DLMFHPGACEP]
                    .AddRange(CustomCostumes["arms_shape"].CustomObjects.Select(c => c.Item1));
            }
        }

        ContentPatch._internalCostumeCounts[CustomCostumes["arms_glove"].InternalPrefix] = GameTextures.AOBEBHODFKG[10];
        GameTextures.AOBEBHODFKG[10] += CustomCostumes["arms_glove"].Count;
        ContentMappings.ContentMap.MaterialNameMap[10]
            .AddRange(CustomCostumes["arms_glove"].CustomObjects.Select(c => c.Item1));
        GameTextures.GEANFMLIDLN[10] += 0; // arms_glove_flesh (default 1)
        GameTextures.AEBNMPCDDAL[10] += 0; // arms_glove_shape (default 1)
        GameTextures.AOBEBHODFKG[13] += CustomCostumes["arms_glove"].Count;
        ContentMappings.ContentMap.MaterialNameMap[13]
            .AddRange(CustomCostumes["arms_glove"].CustomObjects.Select(c => c.Item1));
        GameTextures.GEANFMLIDLN[13] += 0; // arms_glove_flesh2 (default arms_glove_flesh)
        GameTextures.AEBNMPCDDAL[13] += 0; // arms_glove_shape2 (default arms_glove_shape)
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_footwear_special"].InternalPrefix] = GameTextures.MIBHEJJOBME;
        GameTextures.MIBHEJJOBME += CustomCostumes["legs_footwear_special"].Count;
        ContentMappings.ContentMap.SpecialFootwearNameMap.AddRange(CustomCostumes["legs_footwear_special"]
            .CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_footwear"].InternalPrefix] = GameTextures.AOBEBHODFKG[14];
        GameTextures.AOBEBHODFKG[14] += CustomCostumes["legs_footwear"].Count;
        ContentMappings.ContentMap.MaterialNameMap[14]
            .AddRange(CustomCostumes["legs_footwear"].CustomObjects.Select(c => c.Item1));
        GameTextures.GEANFMLIDLN[14] += 0; // legs_footwear_flesh (default 0)
        GameTextures.AEBNMPCDDAL[14] += 0; // legs_footwear_shape (default 0)
        GameTextures.AOBEBHODFKG[15] += CustomCostumes["legs_footwear"].Count;
        ContentMappings.ContentMap.MaterialNameMap[15]
            .AddRange(CustomCostumes["legs_footwear"].CustomObjects.Select(c => c.Item1));
        GameTextures.GEANFMLIDLN[15] += 0; // legs_footwear_flesh2 (default legs_footwear_flesh)
        GameTextures.AEBNMPCDDAL[15] += 0; // legs_footwear_shape2 (default legs_footwear_shape)
        ContentPatch._internalCostumeCounts[CustomCostumes["body_collar"].InternalPrefix] = GameTextures.AOBEBHODFKG[16];
        GameTextures.AOBEBHODFKG[16] += CustomCostumes["body_collar"].Count;
        ContentMappings.ContentMap.MaterialNameMap[16]
            .AddRange(CustomCostumes["body_collar"].CustomObjects.Select(c => c.Item1));
        GameTextures.GEANFMLIDLN[16] += 0; // body_collar_flesh (default 1)  
        GameTextures.AEBNMPCDDAL[16] += 0; // body_collar_shape (default 0)
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_texture_transparent"].InternalPrefix] = GameTextures.GHOGKPDHOJH;
        GameTextures.GHOGKPDHOJH += CustomCostumes["hair_texture_transparent"].Count;
        ContentMappings.ContentMap.TransparentHairMaterialNameMap.AddRange(
            CustomCostumes["hair_texture_transparent"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_texture_solid"].InternalPrefix] = GameTextures.AOBEBHODFKG[17];
        GameTextures.AOBEBHODFKG[17] += CustomCostumes["hair_texture_solid"].Count;
        ContentMappings.ContentMap.MaterialNameMap[17]
            .AddRange(CustomCostumes["hair_texture_solid"].CustomObjects.Select(c => c.Item1));
        GameTextures.GEANFMLIDLN[17] += 0; // hair_texture_solid_flesh (default 100)
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_hairstyle_solid"].InternalPrefix] = GameTextures.AEBNMPCDDAL[17];
        GameTextures.AEBNMPCDDAL[17] += CustomCostumes["hair_hairstyle_solid"].Count;
        ContentMappings.ContentMap.ShapeNameMap[17]
            .AddRange(CustomCostumes["hair_hairstyle_solid"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_hairstyle_transparent"].InternalPrefix] = GameTextures.IGDBDHBFCDE;
        GameTextures.IGDBDHBFCDE += CustomCostumes["hair_hairstyle_transparent"].Count;
        ContentMappings.ContentMap.TransparentHairHairstyleNameMap.AddRange(
            CustomCostumes["hair_hairstyle_transparent"].CustomObjects.Select(c => c.Item1));
        GameTextures.AOBEBHODFKG[18] += 0; // hair_hairstyle_transparent_texture (default 2)
        GameTextures.GEANFMLIDLN[18] += 0; // hair_hairstyle_transparent_flesh (default 100)
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_extension"].InternalPrefix] = GameTextures.AEBNMPCDDAL[18];
        GameTextures.AEBNMPCDDAL[18] += CustomCostumes["hair_extension"].Count;
        ContentMappings.ContentMap.ShapeNameMap[18]
            .AddRange(CustomCostumes["hair_extension"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_shave"].InternalPrefix] = GameTextures.AOBEBHODFKG[19];
        GameTextures.AOBEBHODFKG[19] += CustomCostumes["hair_shave"].Count;
        ContentMappings.ContentMap.MaterialNameMap[19]
            .AddRange(CustomCostumes["hair_shave"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_beard"].InternalPrefix] = GameTextures.AOBEBHODFKG[20];
        GameTextures.AOBEBHODFKG[20] += CustomCostumes["face_beard"].Count;
        ContentMappings.ContentMap.MaterialNameMap[20]
            .AddRange(CustomCostumes["face_beard"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_mask"].InternalPrefix] = GameTextures.AOBEBHODFKG[21];
        GameTextures.AOBEBHODFKG[21] += CustomCostumes["face_mask"].Count;
        ContentMappings.ContentMap.MaterialNameMap[21]
            .AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
        GameTextures.AOBEBHODFKG[22] += CustomCostumes["face_mask"].Count;
        ContentMappings.ContentMap.MaterialNameMap[22]
            .AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
        GameTextures.AOBEBHODFKG[23] += CustomCostumes["body_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[23]
            .AddRange(CustomCostumes["body_pattern"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_pattern"].InternalPrefix] = GameTextures.AOBEBHODFKG[24];
        GameTextures.AOBEBHODFKG[24] += CustomCostumes["body_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[24]
            .AddRange(CustomCostumes["body_pattern"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_kneepad"].InternalPrefix] = GameTextures.JOKEKDGHCGL;
        GameTextures.JOKEKDGHCGL += CustomCostumes["legs_kneepad"].Count;
        ContentMappings.ContentMap.KneepadNameMap.AddRange(CustomCostumes["legs_kneepad"].CustomObjects
            .Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_pattern"].InternalPrefix] = GameTextures.AOBEBHODFKG[25];
        GameTextures.AOBEBHODFKG[25] += CustomCostumes["legs_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[25]
            .AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
        GameTextures.AOBEBHODFKG[26] += CustomCostumes["legs_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[26]
            .AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_laces"].InternalPrefix] = GameTextures.AOBEBHODFKG[27];
        GameTextures.AOBEBHODFKG[27] += CustomCostumes["legs_laces"].Count;
        ContentMappings.ContentMap.MaterialNameMap[27]
            .AddRange(CustomCostumes["legs_laces"].CustomObjects.Select(c => c.Item1));
        GameTextures.AOBEBHODFKG[28] += 0; // face_eyewear_texture (default 1)
        ContentPatch._internalCostumeCounts[CustomCostumes["face_headwear"].InternalPrefix] = GameTextures.AEBNMPCDDAL[28];
        GameTextures.AEBNMPCDDAL[28] += CustomCostumes["face_headwear"].Count;
        ContentMappings.ContentMap.ShapeNameMap[28]
            .AddRange(CustomCostumes["face_headwear"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["arms_elbow_pad"].InternalPrefix] = GameTextures.AOBEBHODFKG[29];
        GameTextures.AOBEBHODFKG[29] += CustomCostumes["arms_elbow_pad"].Count;
        ContentMappings.ContentMap.MaterialNameMap[29]
            .AddRange(CustomCostumes["arms_elbow_pad"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["arms_wristband"].InternalPrefix] = GameTextures.AOBEBHODFKG[30];
        GameTextures.AOBEBHODFKG[30] += CustomCostumes["arms_wristband"].Count;
        ContentMappings.ContentMap.MaterialNameMap[30]
            .AddRange(CustomCostumes["arms_wristband"].CustomObjects.Select(c => c.Item1));
        GameTextures.AOBEBHODFKG[31] += 0; // face_headwear_texture (default face_eyewear_texture)
        GameTextures.AEBNMPCDDAL[31] += CustomCostumes["face_headwear"].Count;
        ContentMappings.ContentMap.ShapeNameMap[31]
            .AddRange(CustomCostumes["face_headwear"].CustomObjects.Select(c => c.Item1));
        GameTextures.AOBEBHODFKG[32] += CustomCostumes["arms_elbow_pad"].Count;
        ContentMappings.ContentMap.MaterialNameMap[32]
            .AddRange(CustomCostumes["arms_elbow_pad"].CustomObjects.Select(c => c.Item1));
        GameTextures.AOBEBHODFKG[33] += CustomCostumes["arms_wristband"].Count;
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
                Plugin.ImportCharacters(modImportDir);
            }
        }
            
        LoadPrefixes();
        
        _modsLoaded = true;
    }
}