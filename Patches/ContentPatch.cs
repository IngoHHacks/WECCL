using UnityEngine.UI;
using WECCL.Content;
using WECCL.Saves;
using WECCL.Updates;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
internal class ContentPatch
{
    private static readonly Dictionary<string, int> _internalCostumeCounts = new();

    /*
     * GameGlobals.AMPMBILAJNM loads an object from an AssetBundle.
     * This patch is used to load custom objects from the Assets folder(s).
     */
    [HarmonyPatch(typeof(GameGlobals), nameof(GameGlobals.AMPMBILAJNM))]
    [HarmonyPrefix]
    public static bool GameGlobals_AMPMBILAJNM(ref Object __result, string JJGJAHCHODI, string AGKNLHFIEBO)
    {
        if (JJGJAHCHODI.StartsWith("Music"))
        {
            if (AGKNLHFIEBO.StartsWith("Theme") && int.Parse(AGKNLHFIEBO.Substring(5)) > VanillaCounts.MusicCount)
            {
                __result = CustomClips[int.Parse(AGKNLHFIEBO.Substring(5)) - VanillaCounts.MusicCount - 1];
                return false;
            }
        }
        else if (JJGJAHCHODI.StartsWith("Costumes"))
        {
            int numberIndex = 0;
            while (numberIndex < AGKNLHFIEBO.Length && !char.IsDigit(AGKNLHFIEBO[numberIndex]))
            {
                numberIndex++;
            }

            if (numberIndex == 0 || numberIndex == AGKNLHFIEBO.Length)
            {
                return true;
            }

            string prefix = AGKNLHFIEBO.Substring(0, numberIndex);
            int index = 0;
            if (int.TryParse(AGKNLHFIEBO.Substring(numberIndex), out index))
            {
                if (_internalCostumeCounts.ContainsKey(prefix) && index > _internalCostumeCounts[prefix])
                {
                    __result = CustomCostumes.Single(c => c.Value.InternalPrefix == prefix).Value
                        .CustomObjects[index - _internalCostumeCounts[prefix] - 1].Item2;
                    return false;
                }
            }
        }

        return true;
    }

    /*
     * GameTextures.CGHJPFMGCAO is called when the game loads the vanilla content constants.
     * This patch is used to update the vanilla content constants with the custom content counts.
     */
    [HarmonyPatch(typeof(GameTextures), nameof(GameTextures.CGHJPFMGCAO))]
    [HarmonyPostfix]
    public static void GameTextures_CGHJPFMGCAO()
    {
        VanillaCounts.MaterialCounts = GameTextures.CEJEFCFAEOB.ToList();
        VanillaCounts.FleshCounts = GameTextures.BJOHIDFGJJL.ToList();
        VanillaCounts.ShapeCounts = GameTextures.EBIAAOEBLLB.ToList();
        VanillaCounts.BodyFemaleCount = GameTextures.HPDNFLDAEEJ;
        VanillaCounts.FaceFemaleCount = GameTextures.MDLGLIKFHHL;
        VanillaCounts.SpecialFootwearCount = GameTextures.OMBJMMEGDAI;
        VanillaCounts.TransparentHairMaterialCount = GameTextures.MLMACJBKCMJ;
        VanillaCounts.TransparentHairHairstyleCount = GameTextures.CMCOJECKMOG;
        VanillaCounts.KneepadCount = GameTextures.MKKEAAPOJMC;
        VanillaCounts.IsInitialized = true;
        
        if (Plugin.Debug.Value)
        {
            VersionData.WriteVersionData();
        }

        _internalCostumeCounts[CustomCostumes["legs_material"].InternalPrefix] = GameTextures.CEJEFCFAEOB[1];
        GameTextures.CEJEFCFAEOB[1] += CustomCostumes["legs_material"].Count;
        ContentMappings.ContentMap.MaterialNameMap[1]
            .AddRange(CustomCostumes["legs_material"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["legs_flesh"].InternalPrefix] = GameTextures.BJOHIDFGJJL[1];
        GameTextures.BJOHIDFGJJL[1] += CustomCostumes["legs_flesh"].Count;
        ContentMappings.ContentMap.FleshNameMap[1]
            .AddRange(CustomCostumes["legs_flesh"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["legs_shape"].InternalPrefix] = GameTextures.EBIAAOEBLLB[1];
        GameTextures.EBIAAOEBLLB[1] += CustomCostumes["legs_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[1]
            .AddRange(CustomCostumes["legs_shape"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["body_material"].InternalPrefix] = GameTextures.CEJEFCFAEOB[2];
        GameTextures.CEJEFCFAEOB[2] += CustomCostumes["body_material"].Count;
        ContentMappings.ContentMap.MaterialNameMap[2]
            .AddRange(CustomCostumes["body_material"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["body_flesh_male"].InternalPrefix] = GameTextures.BJOHIDFGJJL[2];
        GameTextures.BJOHIDFGJJL[2] += CustomCostumes["body_flesh_male"].Count;
        ContentMappings.ContentMap.FleshNameMap[2]
            .AddRange(CustomCostumes["body_flesh_male"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["body_flesh_female"].InternalPrefix] = GameTextures.HPDNFLDAEEJ;
        GameTextures.HPDNFLDAEEJ += CustomCostumes["body_flesh_female"].Count;
        ContentMappings.ContentMap.BodyFemaleNameMap.AddRange(CustomCostumes["body_flesh_female"].CustomObjects
            .Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["body_shape"].InternalPrefix] = GameTextures.EBIAAOEBLLB[2];
        GameTextures.EBIAAOEBLLB[2] += CustomCostumes["body_shape"].Count;
        ContentMappings.ContentMap.ShapeNameMap[2]
            .AddRange(CustomCostumes["body_shape"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["face_female"].InternalPrefix] = GameTextures.MDLGLIKFHHL;
        GameTextures.MDLGLIKFHHL += CustomCostumes["face_female"].Count;
        ContentMappings.ContentMap.FaceFemaleNameMap.AddRange(CustomCostumes["face_female"].CustomObjects
            .Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["face_male"].InternalPrefix] = GameTextures.CEJEFCFAEOB[3];
        GameTextures.CEJEFCFAEOB[3] += CustomCostumes["face_male"].Count;
        ContentMappings.ContentMap.MaterialNameMap[3]
            .AddRange(CustomCostumes["face_male"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[3] += 0; // face_flesh (default 0)
        _internalCostumeCounts[CustomCostumes["face_shape"].InternalPrefix] = GameTextures.BJOHIDFGJJL[3];
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
                _internalCostumeCounts[CustomCostumes["arms_material"].InternalPrefix] =
                    GameTextures.CEJEFCFAEOB[GameTextures.AAIJMJLPAFC];
                GameTextures.CEJEFCFAEOB[GameTextures.AAIJMJLPAFC] += CustomCostumes["arms_material"].Count;
                ContentMappings.ContentMap.MaterialNameMap[GameTextures.AAIJMJLPAFC]
                    .AddRange(CustomCostumes["arms_material"].CustomObjects.Select(c => c.Item1));
                _internalCostumeCounts[CustomCostumes["arms_flesh"].InternalPrefix] =
                    GameTextures.BJOHIDFGJJL[GameTextures.AAIJMJLPAFC];
                GameTextures.BJOHIDFGJJL[GameTextures.AAIJMJLPAFC] += CustomCostumes["arms_flesh"].Count;
                ContentMappings.ContentMap.FleshNameMap[GameTextures.AAIJMJLPAFC]
                    .AddRange(CustomCostumes["arms_flesh"].CustomObjects.Select(c => c.Item1));
                _internalCostumeCounts[CustomCostumes["arms_shape"].InternalPrefix] =
                    GameTextures.EBIAAOEBLLB[GameTextures.AAIJMJLPAFC];
                GameTextures.EBIAAOEBLLB[GameTextures.AAIJMJLPAFC] += CustomCostumes["arms_shape"].Count;
                ContentMappings.ContentMap.ShapeNameMap[GameTextures.AAIJMJLPAFC]
                    .AddRange(CustomCostumes["arms_shape"].CustomObjects.Select(c => c.Item1));
            }
        }

        _internalCostumeCounts[CustomCostumes["arms_glove"].InternalPrefix] = GameTextures.CEJEFCFAEOB[10];
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
        _internalCostumeCounts[CustomCostumes["legs_footwear_special"].InternalPrefix] = GameTextures.OMBJMMEGDAI;
        GameTextures.OMBJMMEGDAI += CustomCostumes["legs_footwear_special"].Count;
        ContentMappings.ContentMap.SpecialFootwearNameMap.AddRange(CustomCostumes["legs_footwear_special"]
            .CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["legs_footwear"].InternalPrefix] = GameTextures.CEJEFCFAEOB[14];
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
        _internalCostumeCounts[CustomCostumes["body_collar"].InternalPrefix] = GameTextures.CEJEFCFAEOB[16];
        GameTextures.CEJEFCFAEOB[16] += CustomCostumes["body_collar"].Count;
        ContentMappings.ContentMap.MaterialNameMap[16]
            .AddRange(CustomCostumes["body_collar"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[16] += 0; // body_collar_flesh (default 1)  
        GameTextures.EBIAAOEBLLB[16] += 0; // body_collar_shape (default 0)
        _internalCostumeCounts[CustomCostumes["hair_texture_transparent"].InternalPrefix] = GameTextures.MLMACJBKCMJ;
        GameTextures.MLMACJBKCMJ += CustomCostumes["hair_texture_transparent"].Count;
        ContentMappings.ContentMap.TransparentHairMaterialNameMap.AddRange(
            CustomCostumes["hair_texture_transparent"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["hair_texture_solid"].InternalPrefix] = GameTextures.CEJEFCFAEOB[17];
        GameTextures.CEJEFCFAEOB[17] += CustomCostumes["hair_texture_solid"].Count;
        ContentMappings.ContentMap.MaterialNameMap[17]
            .AddRange(CustomCostumes["hair_texture_solid"].CustomObjects.Select(c => c.Item1));
        GameTextures.BJOHIDFGJJL[17] += 0; // hair_texture_solid_flesh (default 100)
        _internalCostumeCounts[CustomCostumes["hair_hairstyle_solid"].InternalPrefix] = GameTextures.EBIAAOEBLLB[17];
        GameTextures.EBIAAOEBLLB[17] += CustomCostumes["hair_hairstyle_solid"].Count;
        ContentMappings.ContentMap.ShapeNameMap[17]
            .AddRange(CustomCostumes["hair_hairstyle_solid"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["hair_hairstyle_transparent"].InternalPrefix] = GameTextures.CMCOJECKMOG;
        GameTextures.CMCOJECKMOG += CustomCostumes["hair_hairstyle_transparent"].Count;
        ContentMappings.ContentMap.TransparentHairHairstyleNameMap.AddRange(
            CustomCostumes["hair_hairstyle_transparent"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[18] += 0; // hair_hairstyle_transparent_texture (default 2)
        GameTextures.BJOHIDFGJJL[18] += 0; // hair_hairstyle_transparent_flesh (default 100)
        _internalCostumeCounts[CustomCostumes["hair_extension"].InternalPrefix] = GameTextures.EBIAAOEBLLB[18];
        GameTextures.EBIAAOEBLLB[18] += CustomCostumes["hair_extension"].Count;
        ContentMappings.ContentMap.ShapeNameMap[18]
            .AddRange(CustomCostumes["hair_extension"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["hair_shave"].InternalPrefix] = GameTextures.CEJEFCFAEOB[19];
        GameTextures.CEJEFCFAEOB[19] += CustomCostumes["hair_shave"].Count;
        ContentMappings.ContentMap.MaterialNameMap[19]
            .AddRange(CustomCostumes["hair_shave"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["face_beard"].InternalPrefix] = GameTextures.CEJEFCFAEOB[20];
        GameTextures.CEJEFCFAEOB[20] += CustomCostumes["face_beard"].Count;
        ContentMappings.ContentMap.MaterialNameMap[20]
            .AddRange(CustomCostumes["face_beard"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["face_mask"].InternalPrefix] = GameTextures.CEJEFCFAEOB[21];
        GameTextures.CEJEFCFAEOB[21] += CustomCostumes["face_mask"].Count;
        ContentMappings.ContentMap.MaterialNameMap[21]
            .AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[22] += CustomCostumes["face_mask"].Count;
        ContentMappings.ContentMap.MaterialNameMap[22]
            .AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[23] += CustomCostumes["body_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[23]
            .AddRange(CustomCostumes["body_pattern"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["body_pattern"].InternalPrefix] = GameTextures.CEJEFCFAEOB[24];
        GameTextures.CEJEFCFAEOB[24] += CustomCostumes["body_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[24]
            .AddRange(CustomCostumes["body_pattern"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["legs_kneepad"].InternalPrefix] = GameTextures.MKKEAAPOJMC;
        GameTextures.MKKEAAPOJMC += CustomCostumes["legs_kneepad"].Count;
        ContentMappings.ContentMap.KneepadNameMap.AddRange(CustomCostumes["legs_kneepad"].CustomObjects
            .Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["legs_pattern"].InternalPrefix] = GameTextures.CEJEFCFAEOB[25];
        GameTextures.CEJEFCFAEOB[25] += CustomCostumes["legs_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[25]
            .AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[26] += CustomCostumes["legs_pattern"].Count;
        ContentMappings.ContentMap.MaterialNameMap[26]
            .AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["legs_laces"].InternalPrefix] = GameTextures.CEJEFCFAEOB[27];
        GameTextures.CEJEFCFAEOB[27] += CustomCostumes["legs_laces"].Count;
        ContentMappings.ContentMap.MaterialNameMap[27]
            .AddRange(CustomCostumes["legs_laces"].CustomObjects.Select(c => c.Item1));
        GameTextures.CEJEFCFAEOB[28] += 0; // face_eyewear_texture (default 1)
        _internalCostumeCounts[CustomCostumes["face_headwear"].InternalPrefix] = GameTextures.EBIAAOEBLLB[28];
        GameTextures.EBIAAOEBLLB[28] += CustomCostumes["face_headwear"].Count;
        ContentMappings.ContentMap.ShapeNameMap[28]
            .AddRange(CustomCostumes["face_headwear"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["arms_elbow_pad"].InternalPrefix] = GameTextures.CEJEFCFAEOB[29];
        GameTextures.CEJEFCFAEOB[29] += CustomCostumes["arms_elbow_pad"].Count;
        ContentMappings.ContentMap.MaterialNameMap[29]
            .AddRange(CustomCostumes["arms_elbow_pad"].CustomObjects.Select(c => c.Item1));
        _internalCostumeCounts[CustomCostumes["arms_wristband"].InternalPrefix] = GameTextures.CEJEFCFAEOB[30];
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
    }

    /*
     * This is a patch to override the textures of the game with ones in the Overrides folder.
     */
    
    [HarmonyPatch(typeof(AssetBundle), "LoadAsset", typeof(string), typeof(Type))]
    [HarmonyPostfix]
    public static void AssetBundle_LoadAsset(ref Object __result, string name)
    {
        if (ResourceOverridesTextures.ContainsKey(name))
        {
            if (__result is Texture2D texture)
            {
                Texture2D overrideTexture = GetHighestPriorityTextureOverride(name);
                if (texture.width != overrideTexture.width || texture.height != overrideTexture.height)
                {
                    overrideTexture = ResizeTexture(overrideTexture, texture.width, texture.height);
                    SetHighestPriorityTextureOverride(name, overrideTexture);
                }

                __result = overrideTexture;
            }
            else if (__result is Sprite sprite)
            {
                Texture2D overrideTexture = GetHighestPriorityTextureOverride(name);
                if (sprite.texture.width != overrideTexture.width || sprite.texture.height != overrideTexture.height)
                {
                    overrideTexture = ResizeTexture(overrideTexture, sprite.texture.width, sprite.texture.height);
                    SetHighestPriorityTextureOverride(name, overrideTexture);
                }
                
                var relativePivot = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);

                var rect = new Rect(Mathf.Round(sprite.rect.x), Mathf.Round(sprite.rect.y), Mathf.Round(sprite.rect.width), Mathf.Round(sprite.rect.height));
                
                __result = Sprite.Create(overrideTexture, rect, relativePivot, sprite.pixelsPerUnit);
            }
            else
            {
                Plugin.Log.LogWarning("Asset " + name + " is not a texture or sprite, cannot override");
            }
        }

        if (ResourceOverridesAudio.ContainsKey(name))
        {
            if (__result is AudioClip)
            {
                __result = GetHighestPriorityAudioOverride(name);
            }
            else
            {
                Plugin.Log.LogWarning("Asset " + name + " is not an audio clip, cannot override");
            }
        }
    }
    
    [HarmonyPatch(typeof(Object), "Internal_CloneSingle", typeof(Object))]
    [HarmonyPostfix]
    public static void Object_CloneSingle(ref Object __result)
    {
        if (__result is GameObject gameObject)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    if (material.mainTexture == null)
                    {
                        continue;
                    }
                    if (ResourceOverridesTextures.ContainsKey(material.mainTexture.name) || ResourceOverridesTextures.ContainsKey(gameObject.name.Replace("(Clone)","").Trim() + "_" + material.mainTexture))
                    {
                        var tex = GetHighestPriorityTextureOverride(material.mainTexture.name);
                        if (material.mainTexture.width != tex.width ||
                            material.mainTexture.height != tex.height)
                        {
                            tex = ResizeTexture(tex, material.mainTexture.width, material.mainTexture.height);
                            SetHighestPriorityTextureOverride(material.mainTexture.name, tex);
                        }

                        material.mainTexture = tex;
                    }
                }
            }
            AudioSource[] audioSources = gameObject.GetComponentsInChildren<AudioSource>(true);
            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource.clip == null)
                {
                    continue;
                }
                if (ResourceOverridesAudio.ContainsKey(audioSource.clip.name))
                {
                    audioSource.clip = GetHighestPriorityAudioOverride(audioSource.clip.name);
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(Image), "OnPopulateMesh")]
    [HarmonyPostfix]
    public static void Image(ref Image __instance)
    {
        if (__instance.m_Sprite != null && ResourceOverridesTextures.ContainsKey(__instance.m_Sprite.name))
        {
            Texture2D overrideTexture = GetHighestPriorityTextureOverride(__instance.m_Sprite.name);
            if (__instance.m_Sprite.texture.width != overrideTexture.width ||
                __instance.m_Sprite.texture.height != overrideTexture.height)
            {
                overrideTexture = ResizeTexture(overrideTexture, __instance.m_Sprite.texture.width,
                    __instance.m_Sprite.texture.height);
            }

            var relativePivot = new Vector2(__instance.m_Sprite.pivot.x / __instance.m_Sprite.rect.width,
                __instance.m_Sprite.pivot.y / __instance.m_Sprite.rect.height);

            var rect = new Rect(Mathf.Round(__instance.m_Sprite.rect.x),
                Mathf.Round(__instance.m_Sprite.rect.y), Mathf.Round(__instance.m_Sprite.rect.width),
                Mathf.Round(__instance.m_Sprite.rect.height));

            __instance.m_Sprite = Sprite.Create(overrideTexture, rect, relativePivot,
                __instance.m_Sprite.pixelsPerUnit);
        }
    }
}