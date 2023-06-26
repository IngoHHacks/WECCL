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
     * GameGlobals.KNPGMHAEKJO loads an object from an AssetBundle.
     * This patch is used to load custom objects from the Assets folder(s).
     */
    [HarmonyPatch(typeof(GameGlobals), nameof(GameGlobals.KNPGMHAEKJO))]
    [HarmonyPrefix]
    public static bool GameGlobals_KNPGMHAEKJO(ref Object __result, string FAAOKJPDICE, string CKLFGNMEILD)
    {
        if (FAAOKJPDICE.StartsWith("Music"))
        {
            if (CKLFGNMEILD.StartsWith("Theme") && int.Parse(CKLFGNMEILD.Substring(5)) > VanillaCounts.MusicCount)
            {
                __result = CustomClips[int.Parse(CKLFGNMEILD.Substring(5)) - VanillaCounts.MusicCount - 1];
                return false;
            }
        }
        else if (FAAOKJPDICE.StartsWith("Costumes"))
        {
            int numberIndex = 0;
            while (numberIndex < CKLFGNMEILD.Length && !char.IsDigit(CKLFGNMEILD[numberIndex]))
            {
                numberIndex++;
            }

            if (numberIndex == 0 || numberIndex == CKLFGNMEILD.Length)
            {
                return true;
            }

            string prefix = CKLFGNMEILD.Substring(0, numberIndex);
            int index = 0;
            if (int.TryParse(CKLFGNMEILD.Substring(numberIndex), out index))
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
     * GameTextures.FOAOMMHHENI is called when the game loads the vanilla content constants.
     * This patch is used to update the vanilla content constants with the custom content counts.
     */
    [HarmonyPatch(typeof(GameTextures), nameof(GameTextures.FOAOMMHHENI))]
    [HarmonyPostfix]
    public static void GameTextures_FOAOMMHHENI()
    {
        VanillaCounts.MaterialCounts = GameTextures.LNMBKFHJKBD.ToList();
        VanillaCounts.FleshCounts = GameTextures.COAEICPABJO.ToList();
        VanillaCounts.ShapeCounts = GameTextures.DNFPMKLACHM.ToList();
        VanillaCounts.BodyFemaleCount = GameTextures.AMOGDCDAIPN;
        VanillaCounts.FaceFemaleCount = GameTextures.DAIOGKFPBML;
        VanillaCounts.SpecialFootwearCount = GameTextures.ILFFOFONPPO;
        VanillaCounts.TransparentHairMaterialCount = GameTextures.NCDJIAOEADB;
        VanillaCounts.TransparentHairHairstyleCount = GameTextures.HBCJJDDDEJO;
        VanillaCounts.KneepadCount = GameTextures.IBFPEKPLBIJ;
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

    [HarmonyPatch(typeof(AssetBundle), nameof(AssetBundle.LoadAsset), typeof(string), typeof(Type))]
    [HarmonyPostfix]
    public static void AssetBundle_LoadAsset(ref Object __result, string name)
    {
        if (ResourceOverridesTextures.ContainsKey(name))
        {
            if (__result == null)   //manual overrides
            {
                switch (true)
                {
                    case bool when Regex.IsMatch(name, @"Fed[0-9]+_Texture[0-9]+"):  //missing fed belt textures
                        __result = new Texture2D(512, 256);
                        break;

                       case bool when Regex.IsMatch(name, @"Fed[0-9]+_Sprite[0-9]+"):   //missing fed belt sprites
                           __result = Sprite.Create(new Texture2D(256, 64), new Rect(0, 0, 256, 64), new Vector2(0.5f, 0.5f), 50);
                           break;

                    default:
                        break;
                }
            }
            if (__result is Texture2D texture)
            {
                Texture2D overrideTexture = GetHighestPriorityTextureOverride(name);
                if ((texture.width != overrideTexture.width || texture.height != overrideTexture.height) && !Plugin.UseFullQualityTextures.Value)
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

                Vector2 relativePivot = new(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);

                Rect rect = new(Mathf.Round(sprite.rect.x), Mathf.Round(sprite.rect.y), Mathf.Round(sprite.rect.width),
                    Mathf.Round(sprite.rect.height));

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

                    if (ResourceOverridesTextures.ContainsKey(material.mainTexture.name) ||
                        ResourceOverridesTextures.ContainsKey(gameObject.name.Replace("(Clone)", "").Trim() + "_" +
                                                              material.mainTexture))
                    {
                        Texture2D tex = GetHighestPriorityTextureOverride(material.mainTexture.name);
                        if ((material.mainTexture.width != tex.width ||
                            material.mainTexture.height != tex.height) &&
                             !Plugin.UseFullQualityTextures.Value)
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

    [HarmonyPatch(typeof(Image), nameof(UnityEngine.UI.Image.OnPopulateMesh))]
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

            Vector2 relativePivot = new(__instance.m_Sprite.pivot.x / __instance.m_Sprite.rect.width,
                __instance.m_Sprite.pivot.y / __instance.m_Sprite.rect.height);

            Rect rect = new(Mathf.Round(__instance.m_Sprite.rect.x),
                Mathf.Round(__instance.m_Sprite.rect.y), Mathf.Round(__instance.m_Sprite.rect.width),
                Mathf.Round(__instance.m_Sprite.rect.height));

            __instance.m_Sprite = Sprite.Create(overrideTexture, rect, relativePivot,
                __instance.m_Sprite.pixelsPerUnit);
        }
    }

    /*
     * CFHMKIICHMB is the method that is called when the game applies the meshes to the player.
     */
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.CFHMKIICHMB))]
    [HarmonyPostfix]
    public static void GamePlayer_CFHMKIICHMB(ref GamePlayer __instance, int FNLOGBBADOD)
    {
        if ((FNLOGBBADOD == 4 && __instance.HBFGHFIFIEN.shape[FNLOGBBADOD] > 50 &&
             __instance.HBFGHFIFIEN.shape[FNLOGBBADOD] % 10 == 0) || VanillaCounts.ShapeCounts[FNLOGBBADOD] == 0)
        {
            return;
        }

        try
        {
            if (__instance.HBFGHFIFIEN.shape[FNLOGBBADOD] > VanillaCounts.ShapeCounts[FNLOGBBADOD] ||
                (FNLOGBBADOD == 17 &&
                 -__instance.HBFGHFIFIEN.shape[FNLOGBBADOD] > VanillaCounts.TransparentHairHairstyleCount))
            {
                int shape = __instance.HBFGHFIFIEN.shape[FNLOGBBADOD] > 0
                    ? __instance.HBFGHFIFIEN.shape[FNLOGBBADOD] - VanillaCounts.ShapeCounts[FNLOGBBADOD] - 1
                    : -__instance.HBFGHFIFIEN.shape[FNLOGBBADOD] - VanillaCounts.TransparentHairHairstyleCount - 1;
                if (CustomCostumes.Values.Any(x => x.InternalPrefix.Contains("shape" + FNLOGBBADOD)))
                {
                    Tuple<string, Object, Dictionary<string, string>> c = CustomCostumes.Values
                        .First(x => x.InternalPrefix.Contains("shape" + FNLOGBBADOD))
                        .CustomObjects[shape];
                    ;
                    Mesh mesh = c.Item2 as Mesh;
                    Dictionary<string, string> meta = c.Item3;
                    if (mesh != null)
                    {
                        __instance.CBLJCJMAPGH[FNLOGBBADOD].GetComponent<MeshFilter>().mesh = mesh;
                        if (meta.ContainsKey("scale"))
                        {
                            if (meta["scale"].Contains(","))
                            {
                                string[] scale = meta["scale"].Split(',');
                                __instance.CBLJCJMAPGH[FNLOGBBADOD].transform.localScale = new Vector3(
                                    float.Parse(scale[0], Nfi), float.Parse(scale[1], Nfi), float.Parse(scale[2], Nfi));
                            }
                            else
                            {
                                __instance.CBLJCJMAPGH[FNLOGBBADOD].transform.localScale = new Vector3(
                                    float.Parse(meta["scale"], Nfi), float.Parse(meta["scale"], Nfi),
                                    float.Parse(meta["scale"], Nfi));
                            }
                        }

                        if (meta.ContainsKey("position"))
                        {
                            string[] position = meta["position"].Split(',');
                            __instance.CBLJCJMAPGH[FNLOGBBADOD].transform.localPosition = new Vector3(
                                float.Parse(position[0], Nfi), float.Parse(position[1], Nfi),
                                float.Parse(position[2], Nfi));
                        }

                        if (meta.ContainsKey("rotation"))
                        {
                            string[] rotation = meta["rotation"].Split(',');
                            __instance.CBLJCJMAPGH[FNLOGBBADOD].transform.localRotation = Quaternion.Euler(
                                float.Parse(rotation[0], Nfi), float.Parse(rotation[1], Nfi),
                                float.Parse(rotation[2], Nfi));
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
            Plugin.Log.LogError("FNLOGBBADOD: " + FNLOGBBADOD + " (" + __instance.HBFGHFIFIEN.shape[FNLOGBBADOD] + ")");
        }
    }

    [HarmonyPatch(typeof(GameTextures), nameof(GameTextures.HAOCAMJJPMM))]
    [HarmonyPostfix]
    public static void GameTextures_HAOCAMJJPMM(GamePlayer NCPIJJFEDFL)
    {
        FixMeshes(NCPIJJFEDFL);
    }

    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.POBBMNHMBGI))]
    [HarmonyPostfix]
    public static void GamePlayer_POBBMNHMBGI(GamePlayer __instance)
    {
        FixMeshes(__instance);
    }

    private static void FixMeshes(GamePlayer player)
    {
        try
        {
            for (GameTextures.FNLOGBBADOD = 1;
                 GameTextures.FNLOGBBADOD <= GameTextures.OFFCLMNEJCA;
                 GameTextures.FNLOGBBADOD++)
            {
                if ((GameTextures.FNLOGBBADOD == 4 && player.HBFGHFIFIEN.shape[GameTextures.FNLOGBBADOD] > 50 &&
                     player.HBFGHFIFIEN.shape[GameTextures.FNLOGBBADOD] % 10 == 0) ||
                    player.CBLJCJMAPGH[GameTextures.FNLOGBBADOD] == null ||
                    VanillaCounts.ShapeCounts[GameTextures.FNLOGBBADOD] == 0)
                {
                    continue;
                }

                if (player.HBFGHFIFIEN.shape[GameTextures.FNLOGBBADOD] >
                    VanillaCounts.ShapeCounts[GameTextures.FNLOGBBADOD]
                    || (GameTextures.FNLOGBBADOD == 17 && -player.HBFGHFIFIEN.shape[GameTextures.FNLOGBBADOD] >
                        VanillaCounts.TransparentHairHairstyleCount))
                {
                    Mesh mesh = player.CBLJCJMAPGH[GameTextures.FNLOGBBADOD].GetComponent<MeshFilter>().mesh;
                    if (player.CBLJCJMAPGH[GameTextures.FNLOGBBADOD].GetComponent<MeshRenderer>().materials.Length <
                        mesh.subMeshCount)
                    {
                        int shape = player.HBFGHFIFIEN.shape[GameTextures.FNLOGBBADOD] > 0
                            ? player.HBFGHFIFIEN.shape[GameTextures.FNLOGBBADOD] -
                              VanillaCounts.ShapeCounts[GameTextures.FNLOGBBADOD] - 1
                            : -player.HBFGHFIFIEN.shape[GameTextures.FNLOGBBADOD] -
                              VanillaCounts.TransparentHairHairstyleCount - 1;
                        Dictionary<string, string> meta = new();
                        if (CustomCostumes.Values.Any(
                                x => x.InternalPrefix.Contains("shape" + GameTextures.FNLOGBBADOD)))
                        {
                            meta = CustomCostumes.Values
                                .First(x => x.InternalPrefix.Contains("shape" + GameTextures.FNLOGBBADOD))
                                .CustomObjects[shape]
                                .Item3;
                        }

                        Material[] materials = new Material[mesh.subMeshCount];
                        materials[0] = player.CBLJCJMAPGH[GameTextures.FNLOGBBADOD].GetComponent<MeshRenderer>()
                            .material;
                        for (int i = 1; i < materials.Length; i++)
                        {
                            materials[i] = new Material(player.CBLJCJMAPGH[GameTextures.FNLOGBBADOD]
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

                        player.CBLJCJMAPGH[GameTextures.FNLOGBBADOD].GetComponent<MeshRenderer>().materials =
                            materials;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
            Plugin.Log.LogError("FNLOGBBADOD: " + GameTextures.FNLOGBBADOD + " (" +
                                player.HBFGHFIFIEN.shape[GameTextures.FNLOGBBADOD] + ")");
        }
    }
}