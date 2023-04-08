using UnityEngine.UI;
using WECCL.Content;
using WECCL.Updates;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
internal class ContentPatch
{
    internal static readonly Dictionary<string, int> _internalCostumeCounts = new();
    
    internal static bool _contentLoaded = false;

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
        
        _contentLoaded = true;
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
    
    /*
     * BBKEGFNNMGF is the method that is called when the game applies the meshes to the player.
     */
    [HarmonyPatch(typeof(OCOFGNPADGM), "BBKEGFNNMGF")]
    [HarmonyPostfix]
    public static void OCOFGNPADGM_BBKEGFNNMGF(ref OCOFGNPADGM __instance, int AAIJMJLPAFC)
    {
        if (AAIJMJLPAFC == 4 && (__instance.LAFFIDJJGIE.shape[AAIJMJLPAFC] > 50 && __instance.LAFFIDJJGIE.shape[AAIJMJLPAFC] % 10 == 0)) return;
        try {
            if (__instance.LAFFIDJJGIE.shape[AAIJMJLPAFC] > VanillaCounts.ShapeCounts[AAIJMJLPAFC] || (AAIJMJLPAFC == 17 &&
                    -__instance.LAFFIDJJGIE.shape[AAIJMJLPAFC] > VanillaCounts.TransparentHairHairstyleCount))
            {
                var shape = __instance.LAFFIDJJGIE.shape[AAIJMJLPAFC] > 0 ? __instance.LAFFIDJJGIE.shape[AAIJMJLPAFC] - VanillaCounts.ShapeCounts[AAIJMJLPAFC] - 1: -__instance.LAFFIDJJGIE.shape[AAIJMJLPAFC] - VanillaCounts.TransparentHairHairstyleCount - 1;
                if (CustomCostumes.Values.Any(x => x.InternalPrefix.Contains("shape" + AAIJMJLPAFC))) {
                    var mesh = CustomCostumes.Values.First(x => x.InternalPrefix.Contains("shape" + AAIJMJLPAFC)).CustomObjects[shape].Item2 as Mesh;
                    if (mesh != null)
                    {
                        __instance.AEHENPKDKEG[AAIJMJLPAFC].GetComponent<MeshFilter>().mesh = mesh;
                        __instance.AEHENPKDKEG[AAIJMJLPAFC].transform.localScale = new Vector3(15f, 15f, 15f);
                    }
                }
            }
        } catch (Exception e)
        {
            Plugin.Log.LogError(e);
            Plugin.Log.LogError("AAIJMJLPAFC: " + AAIJMJLPAFC + " (" + __instance.LAFFIDJJGIE.shape[AAIJMJLPAFC] + ")");
        }
    }

    [HarmonyPatch(typeof(IINHFOHEAJB), "NDIGGJPCLCL")]
    [HarmonyPostfix]
    public static void IINHFOHEAJB_NDIGGJPCLCL(OCOFGNPADGM NMOJJPKABCC)
    {
        try
        {
            for (IINHFOHEAJB.AAIJMJLPAFC = 1;
                 IINHFOHEAJB.AAIJMJLPAFC <= IINHFOHEAJB.KPEKAODDBPN;
                 IINHFOHEAJB.AAIJMJLPAFC++)
                {
                    if (IINHFOHEAJB.AAIJMJLPAFC == 4 && NMOJJPKABCC.LAFFIDJJGIE.shape[IINHFOHEAJB.AAIJMJLPAFC] > 50 && NMOJJPKABCC.LAFFIDJJGIE.shape[IINHFOHEAJB.AAIJMJLPAFC] % 10 == 0 || NMOJJPKABCC.AEHENPKDKEG[IINHFOHEAJB.AAIJMJLPAFC] == null)
                    {
                        continue;
                    }
                    if (NMOJJPKABCC.LAFFIDJJGIE.shape[IINHFOHEAJB.AAIJMJLPAFC] > VanillaCounts.ShapeCounts[IINHFOHEAJB.AAIJMJLPAFC]
                        || (IINHFOHEAJB.AAIJMJLPAFC == 17 && -NMOJJPKABCC.LAFFIDJJGIE.shape[IINHFOHEAJB.AAIJMJLPAFC] > VanillaCounts.TransparentHairHairstyleCount))
                    {
                    var mesh = NMOJJPKABCC.AEHENPKDKEG[IINHFOHEAJB.AAIJMJLPAFC].GetComponent<MeshFilter>().mesh;
                    if (NMOJJPKABCC.AEHENPKDKEG[IINHFOHEAJB.AAIJMJLPAFC].GetComponent<MeshRenderer>().materials.Length <
                        mesh.subMeshCount)
                    {
                        var materials = new Material[mesh.subMeshCount];
                        materials[0] = NMOJJPKABCC.AEHENPKDKEG[IINHFOHEAJB.AAIJMJLPAFC].GetComponent<MeshRenderer>().material;
                        for (int i = 1; i < materials.Length; i++)
                        {
                            materials[i] = new Material(NMOJJPKABCC.AEHENPKDKEG[IINHFOHEAJB.AAIJMJLPAFC].GetComponent<MeshRenderer>().material);
                            materials[i].color = Color.black;;
                        }

                        NMOJJPKABCC.AEHENPKDKEG[IINHFOHEAJB.AAIJMJLPAFC].GetComponent<MeshRenderer>().materials =
                            materials;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
            Plugin.Log.LogError("AAIJMJLPAFC: " + IINHFOHEAJB.AAIJMJLPAFC + " (" + NMOJJPKABCC.LAFFIDJJGIE.shape[IINHFOHEAJB.AAIJMJLPAFC] + ")");
        }
    }
}