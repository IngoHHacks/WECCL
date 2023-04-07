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
    
    public enum ContentType
    {
        Costume = 1,
        Audio = 2,
        All = Costume | Audio
    }
    
    internal static IEnumerator Load()
    {
        List<DirectoryInfo> AllModsAssetsDirs = new();
        List<DirectoryInfo> AllModsOverridesDirs = new();

        foreach (string modPath in Directory.GetDirectories(Path.Combine(Paths.BepInExRootPath, "plugins")))
        {
            Plugin.FindContent(modPath, ref AllModsAssetsDirs, ref AllModsOverridesDirs, ref Plugin.AllModsImportDirs);
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

        if (!AllModsAssetsDirs.Contains(Plugin.AssetsDir))
        {
            AllModsAssetsDirs.Add(Plugin.AssetsDir);
        }

        if (!AllModsOverridesDirs.Contains(Plugin.OverrideDir))
        {
            AllModsOverridesDirs.Add(Plugin.OverrideDir);
        }

        if (!Plugin.AllModsImportDirs.Contains(Plugin.ImportDir))
        {
            Plugin.AllModsImportDirs.Add(Plugin.ImportDir);
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


        VanillaCounts.MusicCount = CKAMIAJJDBP.GGICEBAECGK;
        
        if (Plugin.EnableCustomContent.Value)
        {
            foreach (DirectoryInfo modAssetsDir in AllModsAssetsDirs)
            {
                yield return Plugin.LoadAudioClips(modAssetsDir);
                yield return Plugin.LoadCostumes(modAssetsDir);
            }
        }

        if (Plugin.EnableOverrides.Value)
        {
            foreach (DirectoryInfo modOverridesDir in AllModsOverridesDirs)
            {
                yield return Plugin.LoadOverrides(modOverridesDir);
            }
        }
        
        yield return new WaitUntil(() => ContentPatch._contentLoaded);
        
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_material"].InternalPrefix] = GameTextures.CEJEFCFAEOB[1];
        GameTextures.CEJEFCFAEOB[1] += CustomCostumes["legs_material"].Count;
        ContentMappings.ContentMap.MaterialNameMap[1]
            .AddRange(CustomCostumes["legs_material"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_flesh"].InternalPrefix] = GameTextures.BJOHIDFGJJL[1];
        GameTextures.BJOHIDFGJJL[1] += CustomCostumes["legs_flesh"].Count;
        ContentMappings.ContentMap.FleshNameMap[1]
            .AddRange(CustomCostumes["legs_flesh"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_shape"].InternalPrefix] = GameTextures.EBIAAOEBLLB[1];
        GameTextures.EBIAAOEBLLB[1] += CustomCostumes["legs_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[1]
            .AddRange(CustomCostumes["legs_shape"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_material"].InternalPrefix] = GameTextures.CEJEFCFAEOB[2];
        GameTextures.CEJEFCFAEOB[2] += CustomCostumes["body_material"].Count;
        ContentMappings.ContentMap.MaterialNameMap[2]
            .AddRange(CustomCostumes["body_material"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_flesh_male"].InternalPrefix] = GameTextures.BJOHIDFGJJL[2];
        GameTextures.BJOHIDFGJJL[2] += CustomCostumes["body_flesh_male"].Count;
        ContentMappings.ContentMap.FleshNameMap[2]
            .AddRange(CustomCostumes["body_flesh_male"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_flesh_female"].InternalPrefix] = GameTextures.HPDNFLDAEEJ;
        GameTextures.HPDNFLDAEEJ += CustomCostumes["body_flesh_female"].Count;
        ContentMappings.ContentMap.BodyFemaleNameMap.AddRange(CustomCostumes["body_flesh_female"].CustomObjects
            .Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_shape"].InternalPrefix] = GameTextures.EBIAAOEBLLB[2];
        GameTextures.EBIAAOEBLLB[2] += CustomCostumes["body_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[2]
            .AddRange(CustomCostumes["body_shape"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_female"].InternalPrefix] = GameTextures.MDLGLIKFHHL;
        GameTextures.MDLGLIKFHHL += CustomCostumes["face_female"].Count;
        ContentMappings.ContentMap.FaceFemaleNameMap.AddRange(CustomCostumes["face_female"].CustomObjects
            .Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_male"].InternalPrefix] = GameTextures.CEJEFCFAEOB[3];
        GameTextures.CEJEFCFAEOB[3] += CustomCostumes["face_male"].Count;
        ContentMappings.ContentMap.MaterialNameMap[3]
            .AddRange(CustomCostumes["face_male"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[3] += 0; // face_flesh (default 0)
        ContentPatch._internalCostumeCounts[CustomCostumes["face_shape"].InternalPrefix] = GameTextures.BJOHIDFGJJL[3];
        GameTextures.EBIAAOEBLLB[3] += CustomCostumes["face_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[3]
            .AddRange(CustomCostumes["face_shape"].CustomObjects.Select(c => c.Item1));
        for (GameTextures.AAIJMJLPAFC = 4; GameTextures.AAIJMJLPAFC <= 7; GameTextures.AAIJMJLPAFC++)
        {
            GameTextures.CEJEFCFAEOB[GameTextures.AAIJMJLPAFC] +=
                CustomCostumes["face_male"].Count; // Unknown (default face_male)
            ContentMappings.ContentMap.MaterialNameMap[GameTextures.AAIJMJLPAFC]
                .AddRange(CustomCostumes["face_male"].CustomObjects.Select(c => c.Item1));
            GameTextures.BJOHIDFGJJL[GameTextures.AAIJMJLPAFC] += 0; // face_flesh2 (default face_flesh)
            GameTextures.EBIAAOEBLLB[GameTextures.AAIJMJLPAFC] +=
                CustomCostumes["face_shape"].Count; // Unknown (default face shapes)
            ContentMappings.ContentMap.ShapeNameMap[GameTextures.AAIJMJLPAFC]
                .AddRange(CustomCostumes["face_shape"].CustomObjects.Select(c => c.Item1));
        }

        for (GameTextures.AAIJMJLPAFC = 8; GameTextures.AAIJMJLPAFC <= 12; GameTextures.AAIJMJLPAFC++)
        {
            if (GameTextures.AAIJMJLPAFC != 10)
            {
                ContentPatch._internalCostumeCounts[CustomCostumes["arms_material"].InternalPrefix] =
                    GameTextures.CEJEFCFAEOB[GameTextures.AAIJMJLPAFC];
                GameTextures.CEJEFCFAEOB[GameTextures.AAIJMJLPAFC] += CustomCostumes["arms_material"].Count;
                ContentMappings.ContentMap.MaterialNameMap[GameTextures.AAIJMJLPAFC]
                    .AddRange(CustomCostumes["arms_material"].CustomObjects.Select(c => c.Item1));
                ContentPatch._internalCostumeCounts[CustomCostumes["arms_flesh"].InternalPrefix] =
                    GameTextures.BJOHIDFGJJL[GameTextures.AAIJMJLPAFC];
                GameTextures.BJOHIDFGJJL[GameTextures.AAIJMJLPAFC] += CustomCostumes["arms_flesh"].Count;
                ContentMappings.ContentMap.FleshNameMap[GameTextures.AAIJMJLPAFC]
                    .AddRange(CustomCostumes["arms_flesh"].CustomObjects.Select(c => c.Item1));
                ContentPatch._internalCostumeCounts[CustomCostumes["arms_shape"].InternalPrefix] =
                    GameTextures.EBIAAOEBLLB[GameTextures.AAIJMJLPAFC];
                GameTextures.EBIAAOEBLLB[GameTextures.AAIJMJLPAFC] += CustomCostumes["arms_shape"].Count;
                ContentMappings.ContentMap.ShapeNameMap[GameTextures.AAIJMJLPAFC]
                    .AddRange(CustomCostumes["arms_shape"].CustomObjects.Select(c => c.Item1));
            }
        }

        ContentPatch._internalCostumeCounts[CustomCostumes["arms_glove"].InternalPrefix] = GameTextures.CEJEFCFAEOB[10];
        GameTextures.CEJEFCFAEOB[10] += CustomCostumes["arms_glove"].Count;
        ContentMappings.ContentMap.MaterialNameMap[10]
            .AddRange(CustomCostumes["arms_glove"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[10] += 0; // arms_glove_flesh (default 1)
        GameTextures.EBIAAOEBLLB[10] += 0; // arms_glove_shape (default 1)
        GameTextures.CEJEFCFAEOB[13] += CustomCostumes["arms_glove"].Count;
        ContentMappings.ContentMap.MaterialNameMap[13]
            .AddRange(CustomCostumes["arms_glove"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[13] += 0; // arms_glove_flesh2 (default arms_glove_flesh)
        GameTextures.EBIAAOEBLLB[13] += 0; // arms_glove_shape2 (default arms_glove_shape)
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_footwear_special"].InternalPrefix] = GameTextures.OMBJMMEGDAI;
        GameTextures.OMBJMMEGDAI += CustomCostumes["legs_footwear_special"].Count;
        ContentMappings.ContentMap.SpecialFootwearNameMap.AddRange(CustomCostumes["legs_footwear_special"]
            .CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_footwear"].InternalPrefix] = GameTextures.CEJEFCFAEOB[14];
        GameTextures.CEJEFCFAEOB[14] += CustomCostumes["legs_footwear"].Count;
        ContentMappings.ContentMap.MaterialNameMap[14]
            .AddRange(CustomCostumes["legs_footwear"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[14] += 0; // legs_footwear_flesh (default 0)
        GameTextures.EBIAAOEBLLB[14] += 0; // legs_footwear_shape (default 0)
        GameTextures.CEJEFCFAEOB[15] += CustomCostumes["legs_footwear"].Count;
        ContentMappings.ContentMap.MaterialNameMap[15]
            .AddRange(CustomCostumes["legs_footwear"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[15] += 0; // legs_footwear_flesh2 (default legs_footwear_flesh)
        GameTextures.EBIAAOEBLLB[15] += 0; // legs_footwear_shape2 (default legs_footwear_shape)
        ContentPatch._internalCostumeCounts[CustomCostumes["body_collar"].InternalPrefix] = GameTextures.CEJEFCFAEOB[16];
        GameTextures.CEJEFCFAEOB[16] += CustomCostumes["body_collar"].Count;
        ContentMappings.ContentMap.MaterialNameMap[16]
            .AddRange(CustomCostumes["body_collar"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[16] += 0; // body_collar_flesh (default 1)  
        GameTextures.EBIAAOEBLLB[16] += 0; // body_collar_shape (default 0)
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_texture_transparent"].InternalPrefix] = GameTextures.MLMACJBKCMJ;
        GameTextures.MLMACJBKCMJ += CustomCostumes["hair_texture_transparent"].Count;
        ContentMappings.ContentMap.TransparentHairMaterialNameMap.AddRange(
            CustomCostumes["hair_texture_transparent"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_texture_solid"].InternalPrefix] = GameTextures.CEJEFCFAEOB[17];
        GameTextures.CEJEFCFAEOB[17] += CustomCostumes["hair_texture_solid"].Count;
        ContentMappings.ContentMap.MaterialNameMap[17]
            .AddRange(CustomCostumes["hair_texture_solid"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[17] += 0; // hair_texture_solid_flesh (default 100)
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_hairstyle_solid"].InternalPrefix] = GameTextures.EBIAAOEBLLB[17];
        GameTextures.EBIAAOEBLLB[17] += CustomCostumes["hair_hairstyle_solid"].Count;
        ContentMappings.ContentMap.ShapeNameMap[17]
            .AddRange(CustomCostumes["hair_hairstyle_solid"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_hairstyle_transparent"].InternalPrefix] = GameTextures.CMCOJECKMOG;
        GameTextures.CMCOJECKMOG += CustomCostumes["hair_hairstyle_transparent"].Count;
        ContentMappings.ContentMap.TransparentHairHairstyleNameMap.AddRange(
            CustomCostumes["hair_hairstyle_transparent"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[18] += 0; // hair_hairstyle_transparent_texture (default 2)
        GameTextures.BJOHIDFGJJL[18] += 0; // hair_hairstyle_transparent_flesh (default 100)
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_extension"].InternalPrefix] = GameTextures.EBIAAOEBLLB[18];
        GameTextures.EBIAAOEBLLB[18] += CustomCostumes["hair_extension"].Count;
        ContentMappings.ContentMap.ShapeNameMap[18]
            .AddRange(CustomCostumes["hair_extension"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["hair_shave"].InternalPrefix] = GameTextures.CEJEFCFAEOB[19];
        GameTextures.CEJEFCFAEOB[19] += CustomCostumes["hair_shave"].Count;
        ContentMappings.ContentMap.MaterialNameMap[19]
            .AddRange(CustomCostumes["hair_shave"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_beard"].InternalPrefix] = GameTextures.CEJEFCFAEOB[20];
        GameTextures.CEJEFCFAEOB[20] += CustomCostumes["face_beard"].Count;
        ContentMappings.ContentMap.MaterialNameMap[20]
            .AddRange(CustomCostumes["face_beard"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["face_mask"].InternalPrefix] = GameTextures.CEJEFCFAEOB[21];
        GameTextures.CEJEFCFAEOB[21] += CustomCostumes["face_mask"].Count;
        ContentMappings.ContentMap.MaterialNameMap[21]
            .AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[22] += CustomCostumes["face_mask"].Count;
        ContentMappings.ContentMap.MaterialNameMap[22]
            .AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[23] += CustomCostumes["body_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[23]
            .AddRange(CustomCostumes["body_pattern"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["body_pattern"].InternalPrefix] = GameTextures.CEJEFCFAEOB[24];
        GameTextures.CEJEFCFAEOB[24] += CustomCostumes["body_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[24]
            .AddRange(CustomCostumes["body_pattern"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_kneepad"].InternalPrefix] = GameTextures.MKKEAAPOJMC;
        GameTextures.MKKEAAPOJMC += CustomCostumes["legs_kneepad"].Count;
        ContentMappings.ContentMap.KneepadNameMap.AddRange(CustomCostumes["legs_kneepad"].CustomObjects
            .Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_pattern"].InternalPrefix] = GameTextures.CEJEFCFAEOB[25];
        GameTextures.CEJEFCFAEOB[25] += CustomCostumes["legs_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[25]
            .AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[26] += CustomCostumes["legs_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[26]
            .AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["legs_laces"].InternalPrefix] = GameTextures.CEJEFCFAEOB[27];
        GameTextures.CEJEFCFAEOB[27] += CustomCostumes["legs_laces"].Count;
        ContentMappings.ContentMap.MaterialNameMap[27]
            .AddRange(CustomCostumes["legs_laces"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[28] += 0; // face_eyewear_texture (default 1)
        ContentPatch._internalCostumeCounts[CustomCostumes["face_headwear"].InternalPrefix] = GameTextures.EBIAAOEBLLB[28];
        GameTextures.EBIAAOEBLLB[28] += CustomCostumes["face_headwear"].Count;
        ContentMappings.ContentMap.ShapeNameMap[28]
            .AddRange(CustomCostumes["face_headwear"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["arms_elbow_pad"].InternalPrefix] = GameTextures.CEJEFCFAEOB[29];
        GameTextures.CEJEFCFAEOB[29] += CustomCostumes["arms_elbow_pad"].Count;
        ContentMappings.ContentMap.MaterialNameMap[29]
            .AddRange(CustomCostumes["arms_elbow_pad"].CustomObjects.Select(c => c.Item1));
        ContentPatch._internalCostumeCounts[CustomCostumes["arms_wristband"].InternalPrefix] = GameTextures.CEJEFCFAEOB[30];
        GameTextures.CEJEFCFAEOB[30] += CustomCostumes["arms_wristband"].Count;
        ContentMappings.ContentMap.MaterialNameMap[30]
            .AddRange(CustomCostumes["arms_wristband"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[31] += 0; // face_headwear_texture (default face_eyewear_texture)
        GameTextures.EBIAAOEBLLB[31] += CustomCostumes["face_headwear"].Count;
        ContentMappings.ContentMap.ShapeNameMap[31]
            .AddRange(CustomCostumes["face_headwear"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[32] += CustomCostumes["arms_elbow_pad"].Count;
        ContentMappings.ContentMap.MaterialNameMap[32]
            .AddRange(CustomCostumes["arms_elbow_pad"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[33] += CustomCostumes["arms_wristband"].Count;
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