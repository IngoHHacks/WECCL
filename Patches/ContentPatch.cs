using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using WECCL.Content;
using WECCL.Updates;
using static WECCL.Utils.NumberFormatUtils;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
internal class ContentPatch
{
    internal static readonly Dictionary<string, int> _internalCostumeCounts = new();

    internal static bool _contentLoaded;

    /*
     * Patch:
     * - Changes content to be loaded from custom content instead of the vanilla content if the content is custom.
     */
    [HarmonyPatch(typeof(UnmappedGlobals), nameof(UnmappedGlobals.JFHPHDKKECG))]
    [HarmonyPrefix]
    public static bool Globals_JFHPHDKKECG(ref Object __result, string PMNNPFOOCOH, string NONJIHAJAKC)
    {
        if (PMNNPFOOCOH.StartsWith("Music"))
        {
            if (NONJIHAJAKC.StartsWith("Theme") && int.Parse(NONJIHAJAKC.Substring(5)) > VanillaCounts.Data.MusicCount)
            {
                __result = CustomClips[int.Parse(NONJIHAJAKC.Substring(5)) - VanillaCounts.Data.MusicCount - 1].AudioClip;
                return false;
            }
        }
        else if (PMNNPFOOCOH.StartsWith("Costumes"))
        {
            int numberIndex = 0;
            while (numberIndex < NONJIHAJAKC.Length && !char.IsDigit(NONJIHAJAKC[numberIndex]))
            {
                numberIndex++;
            }

            if (numberIndex == 0 || numberIndex == NONJIHAJAKC.Length)
            {
                return true;
            }

            string prefix = NONJIHAJAKC.Substring(0, numberIndex);
            int index = 0;
            if (int.TryParse(NONJIHAJAKC.Substring(numberIndex), out index))
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
     * Patch:
     * - Updates the vanilla content constants with the custom content counts.
     * - Writes the version data to the debug folder if debug mode is enabled.
     */
    [HarmonyPatch(typeof(UnmappedTextures), nameof(UnmappedTextures.PNKNBNBFCFC))]
    [HarmonyPostfix]
    public static void Textures_PNKNBNBFCFC()
    {
        VanillaCounts.Data.MaterialCounts = UnmappedTextures.BFEEOEOEFPG.ToList();
        VanillaCounts.Data.FleshCounts = UnmappedTextures.HJHFIPCDPBD.ToList();
        VanillaCounts.Data.ShapeCounts = UnmappedTextures.ONGGFJMFJDP.ToList();
        VanillaCounts.Data.MaterialCounts[28] = VanillaCounts.Data.ShapeCounts[28];
        VanillaCounts.Data.MaterialCounts[31] = VanillaCounts.Data.ShapeCounts[31];
        VanillaCounts.Data.BodyFemaleCount = UnmappedTextures.DAKBNPKLNGC;
        VanillaCounts.Data.FaceFemaleCount = UnmappedTextures.GFKAKKGLBOI;
        VanillaCounts.Data.SpecialFootwearCount = UnmappedTextures.ABKAOIHDJPA;
        VanillaCounts.Data.TransparentHairMaterialCount = UnmappedTextures.MHOGCFLJEFN;
        VanillaCounts.Data.TransparentHairHairstyleCount = UnmappedTextures.NLHJDAEKLAB;
        VanillaCounts.Data.KneepadCount = UnmappedTextures.BKMPEAALEEM;
        VanillaCounts.Data.IsInitialized = true;

        if (Plugin.Debug.Value)
        {
            VersionData.WriteVersionData();
        }

        _contentLoaded = true;
    }

    /*
     * Patch:
     * Overrides the textures of the game with overrides for objects that are loaded from asset bundles.
     */
    [HarmonyPatch(typeof(AssetBundle), nameof(AssetBundle.LoadAsset), typeof(string), typeof(Type))]
    [HarmonyPostfix]
    public static void AssetBundle_LoadAsset(ref Object __result, string name)
    {
        if (ResourceOverridesTextures.ContainsKey(name.ToLower()))
        {
            if (__result == null) // Manual overrides
            {
                if (Regex.IsMatch(name.ToLower(), @"fed[0-9]+_texture[0-9]+"))  // Missing fed belt textures
                {
                    __result = new Texture2D(512, 256);
                }
                else if (Regex.IsMatch(name.ToLower(), @"fed[0-9]+_sprite[0-9]+"))   // Missing fed belt sprites
                {
                    __result = Sprite.Create(new Texture2D(256, 64), new Rect(0, 0, 256, 64), new Vector2(0.5f, 0.5f), 50);
                }
            }
            if (__result is Texture2D texture)
            {
                Texture2D overrideTexture = GetHighestPriorityTextureOverride(name.ToLower());
                if ((texture.width != overrideTexture.width || texture.height != overrideTexture.height) && !Plugin.UseFullQualityTextures.Value)
                {
                    overrideTexture = ResizeTexture(overrideTexture, texture.width, texture.height);
                    SetHighestPriorityTextureOverride(name.ToLower(), overrideTexture);
                }

                __result = overrideTexture;
            }
            else if (__result is Sprite sprite)
            {
                Texture2D overrideTexture = GetHighestPriorityTextureOverride(name.ToLower());
                if (sprite.texture.width != overrideTexture.width || sprite.texture.height != overrideTexture.height)
                {
                    overrideTexture = ResizeTexture(overrideTexture, sprite.texture.width, sprite.texture.height);
                    SetHighestPriorityTextureOverride(name.ToLower(), overrideTexture);
                }

                Vector2 relativePivot = new(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);

                Rect rect = new(Mathf.Round(sprite.rect.x), Mathf.Round(sprite.rect.y), Mathf.Round(sprite.rect.width),
                    Mathf.Round(sprite.rect.height));

                __result = Sprite.Create(overrideTexture, rect, relativePivot, sprite.pixelsPerUnit);
            }
            else
            {
                LogWarning("Asset " + name + " is not a texture or sprite, cannot override");
            }
        }

        if (ResourceOverridesAudio.ContainsKey(name.ToLower()))
        {
            if (__result is AudioClip)
            {
                __result = GetHighestPriorityAudioOverride(name.ToLower());
            }
            else
            {
                LogWarning("Asset " + name + " is not an audio clip, cannot override");
            }
        }
    }

    /*
     * Patch:
     * - Overrides the textures of the game with overrides for objects that are cloned.
     */
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
                    
                    if (ResourceOverridesTextures.ContainsKey(material.mainTexture.name.ToLower()) ||
                        ResourceOverridesTextures.ContainsKey(gameObject.name.Replace("(Clone)", "").Trim().ToLower() + "_" +
                                                              material.mainTexture.ToString().ToLower()))
                    {
                        Texture2D tex = GetHighestPriorityTextureOverride(material.mainTexture.name.ToLower());
                        if ((material.mainTexture.width != tex.width ||
                            material.mainTexture.height != tex.height) &&
                             !Plugin.UseFullQualityTextures.Value)
                        {
                            tex = ResizeTexture(tex, material.mainTexture.width, material.mainTexture.height);
                            SetHighestPriorityTextureOverride(material.mainTexture.name.ToLower(), tex);
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

                if (ResourceOverridesAudio.ContainsKey(audioSource.clip.name.ToLower()))
                {
                    audioSource.clip = GetHighestPriorityAudioOverride(audioSource.clip.name.ToLower());
                }
            }
        }
    }

    /*
     * Patch:
     * - Overrides the textures of the game with overrides for images loaded at runtime.
     */
    [HarmonyPatch(typeof(Image), nameof(UnityEngine.UI.Image.OnPopulateMesh))]
    [HarmonyPostfix]
    public static void Image(ref Image __instance)
    {
        if (__instance.m_Sprite != null && ResourceOverridesTextures.ContainsKey(__instance.m_Sprite.name.ToLower()))
        {
            Texture2D overrideTexture = GetHighestPriorityTextureOverride(__instance.m_Sprite.name.ToLower());
            if (__instance.m_Sprite.texture.width != overrideTexture.width ||
                __instance.m_Sprite.texture.height != overrideTexture.height)
            {
                overrideTexture = ResizeTexture(overrideTexture, __instance.m_Sprite.texture.width,
                    __instance.m_Sprite.texture.height);
            }

            Vector2 relativePivot = new(__instance.m_Sprite.pivot.x / __instance.m_Sprite.rect.width,
                __instance.m_Sprite.pivot.y / __instance.m_Sprite.rect.height);

            Rect rect = new(Mathf.Round(__instance.m_Sprite.rect.x),
                Mathf.Round(__instance.m_Sprite.rect.y), Mathf.Round(__instance.m_Sprite.rect.width),
                Mathf.Round(__instance.m_Sprite.rect.height));

            __instance.m_Sprite = Sprite.Create(overrideTexture, rect, relativePivot,
                __instance.m_Sprite.pixelsPerUnit);
        }
    }

    internal static int temp = -1;

    /*
     * Patch:
     * - Sets the temp variable to the current limb variable if it's custom, using the value 0 for the original method.
     */
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.JMOLAPIFDFE))]
    [HarmonyPrefix]
    public static void Player_JMOLAPIFDFE_Pre(ref UnmappedPlayer __instance, ref int IKBHGAKKJMM)
    {
        if (__instance.OEGJEBDBGJA.shape[IKBHGAKKJMM] > VanillaCounts.Data.ShapeCounts[IKBHGAKKJMM] ||
           (IKBHGAKKJMM == 17 && -__instance.OEGJEBDBGJA.shape[IKBHGAKKJMM] > VanillaCounts.Data.TransparentHairHairstyleCount))
        {
            temp = __instance.OEGJEBDBGJA.shape[IKBHGAKKJMM];
            __instance.OEGJEBDBGJA.shape[IKBHGAKKJMM] = 1;
        }
    }

    /*
     * Patch:
     * - Applies custom meshes to the player when the player is loaded.
     */
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.JMOLAPIFDFE))]
    [HarmonyPostfix]
    public static void Player_JMOLAPIFDFE_Post(ref UnmappedPlayer __instance, int IKBHGAKKJMM)
    {
        if (temp != -1)
        {
            __instance.OEGJEBDBGJA.shape[IKBHGAKKJMM] = temp;
            temp = -1;
        }
        var limb = IKBHGAKKJMM;
        if ((limb == 4 && __instance.OEGJEBDBGJA.shape[limb] > 50 &&
             __instance.OEGJEBDBGJA.shape[limb] % 10 == 0) || VanillaCounts.Data.ShapeCounts[limb] == 0)
        {
            return;
        }

        try
        {
            if (__instance.OEGJEBDBGJA.shape[limb] > VanillaCounts.Data.ShapeCounts[limb] ||
                (limb == 17 &&
                 -__instance.OEGJEBDBGJA.shape[limb] > VanillaCounts.Data.TransparentHairHairstyleCount))
            {
                int shape = __instance.OEGJEBDBGJA.shape[limb] > 0
                    ? __instance.OEGJEBDBGJA.shape[limb] - VanillaCounts.Data.ShapeCounts[limb] - 1
                    : -__instance.OEGJEBDBGJA.shape[limb] - VanillaCounts.Data.TransparentHairHairstyleCount - 1;
                if (CustomCostumes.Values.Any(x => x.InternalPrefix.Contains("shape" + limb)))
                {
                    Tuple<string, Object, Dictionary<string, string>> c = CustomCostumes.Values
                        .First(x => x.InternalPrefix.Contains("shape" + limb))
                        .CustomObjects[shape];
                    ;
                    Mesh mesh = c.Item2 as Mesh;
                    Dictionary<string, string> meta = c.Item3;
                    if (mesh != null)
                    {
                        //if (limb == 14 || limb == 15 || limb == 18 || limb == 28 || limb == 31)
                        //{
                            __instance.PCNHIIPBNEK[limb].GetComponent<MeshFilter>().mesh = mesh;
                            __instance.PCNHIIPBNEK[limb].GetComponent<MeshRenderer>().material = new Material(Shader.Find("Custom/My Solid"));
                        //}
                        //else
                        //{
                            //Feel it needs something like below but can't get it working
                        //    __instance.PCNHIIPBNEK[limb].GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;
                        //}
                        if (meta.ContainsKey("scale"))
                        {
                            if (meta["scale"].Contains(","))
                            {
                                string[] scale = meta["scale"].Split(',');
                                __instance.PCNHIIPBNEK[limb].transform.localScale = new Vector3(
                                    float.Parse(scale[0], Nfi), float.Parse(scale[1], Nfi), float.Parse(scale[2], Nfi));
                            }
                            else
                            {
                                __instance.PCNHIIPBNEK[limb].transform.localScale = new Vector3(
                                    float.Parse(meta["scale"], Nfi), float.Parse(meta["scale"], Nfi),
                                    float.Parse(meta["scale"], Nfi));
                            }
                        }

                        if (meta.ContainsKey("position"))
                        {
                            string[] position = meta["position"].Split(',');
                            __instance.PCNHIIPBNEK[limb].transform.localPosition = new Vector3(
                                float.Parse(position[0], Nfi), float.Parse(position[1], Nfi),
                                float.Parse(position[2], Nfi));
                        }

                        if (meta.ContainsKey("rotation"))
                        {
                            string[] rotation = meta["rotation"].Split(',');
                            __instance.PCNHIIPBNEK[limb].transform.localRotation = Quaternion.Euler(
                                float.Parse(rotation[0], Nfi), float.Parse(rotation[1], Nfi),
                                float.Parse(rotation[2], Nfi));
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            LogError(e);
            LogError("Limb: " + limb + " (" + __instance.OEGJEBDBGJA.shape[limb] + ")");
        }
    }

    /*
     * Patch:
     * - Fixes the meshes of the player when updated.
     */
    [HarmonyPatch(typeof(UnmappedTextures), nameof(UnmappedTextures.HKJHJGIJPAN))]
    [HarmonyPostfix]
    public static void Textures_HKJHJGIJPAN(UnmappedPlayer OAAMGFLINOB)
    {
        FixMeshes(OAAMGFLINOB);
    }

    /*
     * Patch:
     * - Fixes the meshes of the player when a new costume is applied.
     */
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.ABHDOPBDDPB))]
    [HarmonyPostfix]
    public static void Player_ABHDOPBDDPB(UnmappedPlayer __instance)
    {
        FixMeshes(__instance);
    }

    private static void FixMeshes(MappedPlayer player)
    {
        try
        {
            for (MappedTextures.limb = 1;
                 MappedTextures.limb <= MappedTextures.no_limbs;
                 MappedTextures.limb++)
            {
                if ((MappedTextures.limb == 4 && player.costume.shape[MappedTextures.limb] > 50 &&
                     player.costume.shape[MappedTextures.limb] % 10 == 0) ||
                    player.model[MappedTextures.limb] == null ||
                    VanillaCounts.Data.ShapeCounts[MappedTextures.limb] == 0)
                {
                    continue;
                }

                if (player.costume.shape[MappedTextures.limb] >
                    VanillaCounts.Data.ShapeCounts[MappedTextures.limb]
                    || (MappedTextures.limb == 17 && -player.costume.shape[MappedTextures.limb] >
                        VanillaCounts.Data.TransparentHairHairstyleCount))
                {
                    Mesh mesh = player.model[MappedTextures.limb].GetComponent<MeshFilter>().mesh;
                    if (player.model[MappedTextures.limb].GetComponent<MeshRenderer>().materials.Length <
                        mesh.subMeshCount)
                    {
                        int shape = player.costume.shape[MappedTextures.limb] > 0
                            ? player.costume.shape[MappedTextures.limb] -
                              VanillaCounts.Data.ShapeCounts[MappedTextures.limb] - 1
                            : -player.costume.shape[MappedTextures.limb] -
                              VanillaCounts.Data.TransparentHairHairstyleCount - 1;
                        Dictionary<string, string> meta = new();
                        if (CustomCostumes.Values.Any(
                                x => x.InternalPrefix.Contains("shape" + MappedTextures.limb)))
                        {
                            meta = CustomCostumes.Values
                                .First(x => x.InternalPrefix.Contains("shape" + MappedTextures.limb))
                                .CustomObjects[shape]
                                .Item3;
                        }

                        Material[] materials = new Material[mesh.subMeshCount];
                        materials[0] = player.model[MappedTextures.limb].GetComponent<MeshRenderer>()
                            .material;
                        for (int i = 1; i < materials.Length; i++)
                        {
                            materials[i] = new Material(player.model[MappedTextures.limb]
                                .GetComponent<MeshRenderer>().material);

                            if (meta.ContainsKey("submesh" + i + "color"))
                            {
                                string[] split = meta["submesh" + i + "color"].Split(',');
                                if (split.Length == 3)
                                {
                                    materials[i].color = new Color(float.Parse(split[0], Nfi),
                                        float.Parse(split[1], Nfi),
                                        float.Parse(split[2], Nfi));
                                }

                                materials[i].color =
                                    ColorUtility.TryParseHtmlString(meta["submesh" + i + "color"], out Color color)
                                        ? color
                                        : Color.white;
                            }
                        }

                        player.model[MappedTextures.limb].GetComponent<MeshRenderer>().materials =
                            materials;
                    }
                }
            }
        }
        catch (Exception e)
        {
            LogError(e);
            LogError("Limb: " + MappedTextures.limb + " (" +
                                player.costume.shape[MappedTextures.limb] + ")");
        }
    }
}