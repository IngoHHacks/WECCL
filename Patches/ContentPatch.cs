using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WECCL.Content;
using WECCL.Saves;
using static WECCL.Content.CustomContent;
using Object = UnityEngine.Object;

namespace WECCL.Patches
{
    [HarmonyPatch]
    internal class ContentPatch
    {
        private static Dictionary<string, int> _internalCostumeCounts = new();

        /*
         * KILNEHBPDGI.AMPMBILAJNM loads an object from an AssetBundle.
         * This patch is used to load custom objects from the Assets folder(s).
         */
        [HarmonyPatch(typeof(KILNEHBPDGI), nameof(KILNEHBPDGI.AMPMBILAJNM))]
        [HarmonyPrefix]
        public static bool KILNEHBPDGI_AMPMBILAJNM(ref Object __result, string JJGJAHCHODI, string AGKNLHFIEBO)
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
                while (numberIndex < AGKNLHFIEBO.Length && !char.IsDigit(AGKNLHFIEBO[numberIndex])) numberIndex++;
                if (numberIndex == 0 || numberIndex == AGKNLHFIEBO.Length) return true;
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
         * IINHFOHEAJB.CGHJPFMGCAO is called when the game loads the vanilla content constants.
         * This patch is used to update the vanilla content constants with the custom content counts.
         */
        [HarmonyPatch(typeof(IINHFOHEAJB), nameof(IINHFOHEAJB.CGHJPFMGCAO))]
        [HarmonyPostfix]
        public static void IINHFOHEAJB_CGHJPFMGCAO()
        {
            VanillaCounts.MaterialCounts = IINHFOHEAJB.CEJEFCFAEOB.ToList();
            VanillaCounts.FleshCounts = IINHFOHEAJB.BJOHIDFGJJL.ToList();
            VanillaCounts.ShapeCounts = IINHFOHEAJB.EBIAAOEBLLB.ToList();
            VanillaCounts.BodyFemaleCount = IINHFOHEAJB.HPDNFLDAEEJ;
            VanillaCounts.FaceFemaleCount = IINHFOHEAJB.MDLGLIKFHHL;
            VanillaCounts.SpecialFootwearCount = IINHFOHEAJB.OMBJMMEGDAI;
            VanillaCounts.TransparentHairMaterialCount = IINHFOHEAJB.MLMACJBKCMJ;
            VanillaCounts.TransparentHairHairstyleCount = IINHFOHEAJB.CMCOJECKMOG;
            VanillaCounts.KneepadCount = IINHFOHEAJB.MKKEAAPOJMC;
            VanillaCounts.IsInitialized = true;
            
            _internalCostumeCounts[CustomCostumes["legs_material"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[1];
            IINHFOHEAJB.CEJEFCFAEOB[1] += CustomCostumes["legs_material"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[1].AddRange(CustomCostumes["legs_material"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["legs_flesh"].InternalPrefix] = IINHFOHEAJB.BJOHIDFGJJL[1];
            IINHFOHEAJB.BJOHIDFGJJL[1] += CustomCostumes["legs_flesh"].Count;
            CustomContentSaveFile.ContentMap.FleshNameMap[1].AddRange(CustomCostumes["legs_flesh"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["legs_shape"].InternalPrefix] = IINHFOHEAJB.EBIAAOEBLLB[1];
            IINHFOHEAJB.EBIAAOEBLLB[1] += CustomCostumes["legs_shape"].Count;
            CustomContentSaveFile.ContentMap.ShapeNameMap[1].AddRange(CustomCostumes["legs_shape"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["body_material"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[2];
            IINHFOHEAJB.CEJEFCFAEOB[2] += CustomCostumes["body_material"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[2].AddRange(CustomCostumes["body_material"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["body_flesh_male"].InternalPrefix] = IINHFOHEAJB.BJOHIDFGJJL[2];
            IINHFOHEAJB.BJOHIDFGJJL[2] += CustomCostumes["body_flesh_male"].Count;
            CustomContentSaveFile.ContentMap.FleshNameMap[2].AddRange(CustomCostumes["body_flesh_male"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["body_flesh_female"].InternalPrefix] = IINHFOHEAJB.HPDNFLDAEEJ;
            IINHFOHEAJB.HPDNFLDAEEJ += CustomCostumes["body_flesh_female"].Count;
            CustomContentSaveFile.ContentMap.BodyFemaleNameMap.AddRange(CustomCostumes["body_flesh_female"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["body_shape"].InternalPrefix] = IINHFOHEAJB.EBIAAOEBLLB[2];
            IINHFOHEAJB.EBIAAOEBLLB[2] += CustomCostumes["body_shape"].Count;
            CustomContentSaveFile.ContentMap.ShapeNameMap[2].AddRange(CustomCostumes["body_shape"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["face_female"].InternalPrefix] = IINHFOHEAJB.MDLGLIKFHHL;
            IINHFOHEAJB.MDLGLIKFHHL += CustomCostumes["face_female"].Count;
            CustomContentSaveFile.ContentMap.FaceFemaleNameMap.AddRange(CustomCostumes["face_female"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["face_male"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[3];
            IINHFOHEAJB.CEJEFCFAEOB[3] += CustomCostumes["face_male"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[3].AddRange(CustomCostumes["face_male"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.BJOHIDFGJJL[3] += 0; // face_flesh (default 0)
            _internalCostumeCounts[CustomCostumes["face_shape"].InternalPrefix] = IINHFOHEAJB.BJOHIDFGJJL[3];
            IINHFOHEAJB.EBIAAOEBLLB[3] += CustomCostumes["face_shape"].Count;  
            CustomContentSaveFile.ContentMap.ShapeNameMap[3].AddRange(CustomCostumes["face_shape"].CustomObjects.Select(c => c.Item1));
            for (IINHFOHEAJB.AAIJMJLPAFC = 4; IINHFOHEAJB.AAIJMJLPAFC <= 7; IINHFOHEAJB.AAIJMJLPAFC++)
            {
                IINHFOHEAJB.CEJEFCFAEOB[IINHFOHEAJB.AAIJMJLPAFC] += CustomCostumes["face_male"].Count; // Unknown (default face_male)
                CustomContentSaveFile.ContentMap.MaterialNameMap[IINHFOHEAJB.AAIJMJLPAFC].AddRange(CustomCostumes["face_male"].CustomObjects.Select(c => c.Item1));
                IINHFOHEAJB.BJOHIDFGJJL[IINHFOHEAJB.AAIJMJLPAFC] += 0; // face_flesh2 (default face_flesh)
                IINHFOHEAJB.EBIAAOEBLLB[IINHFOHEAJB.AAIJMJLPAFC] += CustomCostumes["face_shape"].Count; // Unknown (default face shapes)
                CustomContentSaveFile.ContentMap.ShapeNameMap[IINHFOHEAJB.AAIJMJLPAFC].AddRange(CustomCostumes["face_shape"].CustomObjects.Select(c => c.Item1));
            }
            for (IINHFOHEAJB.AAIJMJLPAFC = 8; IINHFOHEAJB.AAIJMJLPAFC <= 12; IINHFOHEAJB.AAIJMJLPAFC++)
            {
                if (IINHFOHEAJB.AAIJMJLPAFC != 10)
                {
                    _internalCostumeCounts[CustomCostumes["arms_material"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[IINHFOHEAJB.AAIJMJLPAFC];
                    IINHFOHEAJB.CEJEFCFAEOB[IINHFOHEAJB.AAIJMJLPAFC] += CustomCostumes["arms_material"].Count;
                    CustomContentSaveFile.ContentMap.MaterialNameMap[IINHFOHEAJB.AAIJMJLPAFC].AddRange(CustomCostumes["arms_material"].CustomObjects.Select(c => c.Item1));
                    _internalCostumeCounts[CustomCostumes["arms_flesh"].InternalPrefix] = IINHFOHEAJB.BJOHIDFGJJL[IINHFOHEAJB.AAIJMJLPAFC];
                    IINHFOHEAJB.BJOHIDFGJJL[IINHFOHEAJB.AAIJMJLPAFC] += CustomCostumes["arms_flesh"].Count;
                    CustomContentSaveFile.ContentMap.FleshNameMap[IINHFOHEAJB.AAIJMJLPAFC].AddRange(CustomCostumes["arms_flesh"].CustomObjects.Select(c => c.Item1));
                    _internalCostumeCounts[CustomCostumes["arms_shape"].InternalPrefix] = IINHFOHEAJB.EBIAAOEBLLB[IINHFOHEAJB.AAIJMJLPAFC];
                    IINHFOHEAJB.EBIAAOEBLLB[IINHFOHEAJB.AAIJMJLPAFC] += CustomCostumes["arms_shape"].Count;
                    CustomContentSaveFile.ContentMap.ShapeNameMap[IINHFOHEAJB.AAIJMJLPAFC].AddRange(CustomCostumes["arms_shape"].CustomObjects.Select(c => c.Item1));
                }
            }
            _internalCostumeCounts[CustomCostumes["arms_glove"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[10];
            IINHFOHEAJB.CEJEFCFAEOB[10] += CustomCostumes["arms_glove"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[10].AddRange(CustomCostumes["arms_glove"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.BJOHIDFGJJL[10] += 0; // arms_glove_flesh (default 1)
            IINHFOHEAJB.EBIAAOEBLLB[10] += 0; // arms_glove_shape (default 1)
            IINHFOHEAJB.CEJEFCFAEOB[13] += CustomCostumes["arms_glove"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[13].AddRange(CustomCostumes["arms_glove"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.BJOHIDFGJJL[13] += 0; // arms_glove_flesh2 (default arms_glove_flesh)
            IINHFOHEAJB.EBIAAOEBLLB[13] += 0; // arms_glove_shape2 (default arms_glove_shape)
            _internalCostumeCounts[CustomCostumes["legs_footwear_special"].InternalPrefix] = IINHFOHEAJB.OMBJMMEGDAI;
            IINHFOHEAJB.OMBJMMEGDAI += CustomCostumes["legs_footwear_special"].Count;
            CustomContentSaveFile.ContentMap.SpecialFootwearNameMap.AddRange(CustomCostumes["legs_footwear_special"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["legs_footwear"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[14];
            IINHFOHEAJB.CEJEFCFAEOB[14] += CustomCostumes["legs_footwear"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[14].AddRange(CustomCostumes["legs_footwear"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.BJOHIDFGJJL[14] += 0; // legs_footwear_flesh (default 0)
            IINHFOHEAJB.EBIAAOEBLLB[14] += 0; // legs_footwear_shape (default 0)
            IINHFOHEAJB.CEJEFCFAEOB[15] += CustomCostumes["legs_footwear"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[15].AddRange(CustomCostumes["legs_footwear"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.BJOHIDFGJJL[15] += 0; // legs_footwear_flesh2 (default legs_footwear_flesh)
            IINHFOHEAJB.EBIAAOEBLLB[15] += 0; // legs_footwear_shape2 (default legs_footwear_shape)
            _internalCostumeCounts[CustomCostumes["body_collar"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[16];
            IINHFOHEAJB.CEJEFCFAEOB[16] += CustomCostumes["body_collar"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[16].AddRange(CustomCostumes["body_collar"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.BJOHIDFGJJL[16] += 0; // body_collar_flesh (default 1)  
            IINHFOHEAJB.EBIAAOEBLLB[16] += 0; // body_collar_shape (default 0)
            _internalCostumeCounts[CustomCostumes["hair_texture_transparent"].InternalPrefix] = IINHFOHEAJB.MLMACJBKCMJ;
            IINHFOHEAJB.MLMACJBKCMJ += CustomCostumes["hair_texture_transparent"].Count;
            CustomContentSaveFile.ContentMap.TransparentHairMaterialNameMap.AddRange(CustomCostumes["hair_texture_transparent"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["hair_texture_solid"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[17];
            IINHFOHEAJB.CEJEFCFAEOB[17] += CustomCostumes["hair_texture_solid"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[17].AddRange(CustomCostumes["hair_texture_solid"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.BJOHIDFGJJL[17] += 0; // hair_texture_solid_flesh (default 100)
            _internalCostumeCounts[CustomCostumes["hair_hairstyle_solid"].InternalPrefix] = IINHFOHEAJB.EBIAAOEBLLB[17];
            IINHFOHEAJB.EBIAAOEBLLB[17] += CustomCostumes["hair_hairstyle_solid"].Count;  
            CustomContentSaveFile.ContentMap.ShapeNameMap[17].AddRange(CustomCostumes["hair_hairstyle_solid"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["hair_hairstyle_transparent"].InternalPrefix] = IINHFOHEAJB.CMCOJECKMOG;
            IINHFOHEAJB.CMCOJECKMOG += CustomCostumes["hair_hairstyle_transparent"].Count;
            CustomContentSaveFile.ContentMap.TransparentHairHairstyleNameMap.AddRange(CustomCostumes["hair_hairstyle_transparent"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.CEJEFCFAEOB[18] += 0; // hair_hairstyle_transparent_texture (default 2)
            IINHFOHEAJB.BJOHIDFGJJL[18] += 0; // hair_hairstyle_transparent_flesh (default 100)
            _internalCostumeCounts[CustomCostumes["hair_extension"].InternalPrefix] = IINHFOHEAJB.EBIAAOEBLLB[18];
            IINHFOHEAJB.EBIAAOEBLLB[18] += CustomCostumes["hair_extension"].Count;
            CustomContentSaveFile.ContentMap.ShapeNameMap[18].AddRange(CustomCostumes["hair_extension"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["hair_shave"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[19];
            IINHFOHEAJB.CEJEFCFAEOB[19] += CustomCostumes["hair_shave"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[19].AddRange(CustomCostumes["hair_shave"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["face_beard"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[20];
            IINHFOHEAJB.CEJEFCFAEOB[20] += CustomCostumes["face_beard"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[20].AddRange(CustomCostumes["face_beard"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["face_mask"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[21];
            IINHFOHEAJB.CEJEFCFAEOB[21] += CustomCostumes["face_mask"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[21].AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.CEJEFCFAEOB[22] += CustomCostumes["face_mask"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[22].AddRange(CustomCostumes["face_mask"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.CEJEFCFAEOB[23] += CustomCostumes["body_pattern"].Count;
            _internalCostumeCounts[CustomCostumes["body_pattern"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[24];
            IINHFOHEAJB.CEJEFCFAEOB[24] += CustomCostumes["body_pattern"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[24].AddRange(CustomCostumes["body_pattern"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["legs_kneepad"].InternalPrefix] = IINHFOHEAJB.MKKEAAPOJMC;    
            IINHFOHEAJB.MKKEAAPOJMC += CustomCostumes["legs_kneepad"].Count;
            CustomContentSaveFile.ContentMap.KneepadNameMap.AddRange(CustomCostumes["legs_kneepad"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["legs_pattern"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[25];
            IINHFOHEAJB.CEJEFCFAEOB[25] += CustomCostumes["legs_pattern"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[25].AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.CEJEFCFAEOB[26] += CustomCostumes["legs_pattern"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[26].AddRange(CustomCostumes["legs_pattern"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["legs_laces"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[27];
            IINHFOHEAJB.CEJEFCFAEOB[27] += CustomCostumes["legs_laces"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[27].AddRange(CustomCostumes["legs_laces"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.CEJEFCFAEOB[28] += 0; // face_eyewear_texture (default 1)
            _internalCostumeCounts[CustomCostumes["face_headwear"].InternalPrefix] = IINHFOHEAJB.EBIAAOEBLLB[28];
            IINHFOHEAJB.EBIAAOEBLLB[28] += CustomCostumes["face_headwear"].Count;
            CustomContentSaveFile.ContentMap.ShapeNameMap[28].AddRange(CustomCostumes["face_headwear"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["arms_elbow_pad"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[29];
            IINHFOHEAJB.CEJEFCFAEOB[29] += CustomCostumes["arms_elbow_pad"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[29].AddRange(CustomCostumes["arms_elbow_pad"].CustomObjects.Select(c => c.Item1));
            _internalCostumeCounts[CustomCostumes["arms_wristband"].InternalPrefix] = IINHFOHEAJB.CEJEFCFAEOB[30];
            IINHFOHEAJB.CEJEFCFAEOB[30] += CustomCostumes["arms_wristband"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[30].AddRange(CustomCostumes["arms_wristband"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.CEJEFCFAEOB[31] += 0; // face_headwear_texture (default face_eyewear_texture)
            IINHFOHEAJB.EBIAAOEBLLB[31] += CustomCostumes["face_headwear"].Count;
            CustomContentSaveFile.ContentMap.ShapeNameMap[31].AddRange(CustomCostumes["face_headwear"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.CEJEFCFAEOB[32] += CustomCostumes["arms_elbow_pad"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[32].AddRange(CustomCostumes["arms_elbow_pad"].CustomObjects.Select(c => c.Item1));
            IINHFOHEAJB.CEJEFCFAEOB[33] += CustomCostumes["arms_wristband"].Count;
            CustomContentSaveFile.ContentMap.MaterialNameMap[33].AddRange(CustomCostumes["arms_wristband"].CustomObjects.Select(c => c.Item1));
            
            
            if (Plugin.AllModsImportDirs.Count > 0)
            {
                Plugin.Log.LogInfo($"Found {Plugin.AllModsImportDirs.Count} mod(s) with Import directories.");
            }

            if (Plugin.Instance.AllowImportingCharacters.Value)
            {
                foreach (var modImportDir in Plugin.AllModsImportDirs)
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
                    var overrideTexture = ResourceOverridesTextures[name];
                    if (texture.width != overrideTexture.width || texture.height != overrideTexture.height)
                    {
                        overrideTexture = ResizeTexture(overrideTexture, texture.width, texture.height);
                    }
                    __result = overrideTexture;
                }
                else if (__result is Sprite sprite)
                {
                    var overrideTexture = ResourceOverridesTextures[name];
                    if (sprite.texture.width != overrideTexture.width || sprite.texture.height != overrideTexture.height)
                    {
                        overrideTexture = ResizeTexture(overrideTexture, sprite.texture.width, sprite.texture.height);
                    }
                    __result = Sprite.Create(overrideTexture, sprite.rect, sprite.pivot, sprite.pixelsPerUnit);
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
                    __result = ResourceOverridesAudio[name];
                }
                else
                {
                    Plugin.Log.LogWarning("Asset " + name + " is not an audio clip, cannot override");
                }
            }
        }
        
        private static Texture2D ResizeTexture(Texture2D original, int width, int height)
        {
            RenderTexture rt = RenderTexture.GetTemporary(width, height);
            RenderTexture.active = rt;
            Graphics.Blit(original, rt);
            Texture2D resized = new(width, height);
            resized.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            resized.Apply();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            return resized;
        }
    }
}
