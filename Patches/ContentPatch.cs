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
     * GameGlobals.HBNLBOOPFOJ loads an object from an AssetBundle.
     * This patch is used to load custom objects from the Assets folder(s).
     */
    [HarmonyPatch(typeof(GameGlobals), nameof(GameGlobals.HBNLBOOPFOJ))]
    [HarmonyPrefix]
    public static bool GameGlobals_HBNLBOOPFOJ(ref Object __result, string GMAHLKOBDBF, string IBDBKFNKGPI)
    {
        if (GMAHLKOBDBF.StartsWith("Music"))
        {
            if (IBDBKFNKGPI.StartsWith("Theme") && int.Parse(IBDBKFNKGPI.Substring(5)) > VanillaCounts.MusicCount)
            {
                __result = CustomClips[int.Parse(IBDBKFNKGPI.Substring(5)) - VanillaCounts.MusicCount - 1].AudioClip;
                return false;
            }
        }
        else if (GMAHLKOBDBF.StartsWith("Costumes"))
        {
            int numberIndex = 0;
            while (numberIndex < IBDBKFNKGPI.Length && !char.IsDigit(IBDBKFNKGPI[numberIndex]))
            {
                numberIndex++;
            }

            if (numberIndex == 0 || numberIndex == IBDBKFNKGPI.Length)
            {
                return true;
            }

            string prefix = IBDBKFNKGPI.Substring(0, numberIndex);
            int index = 0;
            if (int.TryParse(IBDBKFNKGPI.Substring(numberIndex), out index))
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
     * GameTextures.FFHGCMDOHJL is called when the game loads the vanilla content constants.
     * This patch is used to update the vanilla content constants with the custom content counts.
     */
    [HarmonyPatch(typeof(GameTextures), nameof(GameTextures.FFHGCMDOHJL))]
    [HarmonyPostfix]
    public static void GameTextures_FFHGCMDOHJL()
    {
        VanillaCounts.MaterialCounts = GameTextures.PGGBCDPCBDC.ToList();
        VanillaCounts.FleshCounts = GameTextures.PECMEJKJOJJ.ToList();
        VanillaCounts.ShapeCounts = GameTextures.JMOFBHKFODO.ToList();
        VanillaCounts.BodyFemaleCount = GameTextures.GGDEHKODMKK;
        VanillaCounts.FaceFemaleCount = GameTextures.PJMNJIFAGCO;
        VanillaCounts.SpecialFootwearCount = GameTextures.FFKLFABJAGA;
        VanillaCounts.TransparentHairMaterialCount = GameTextures.OEMIEAFDIAB;
        VanillaCounts.TransparentHairHairstyleCount = GameTextures.KJDGKNNINBC;
        VanillaCounts.KneepadCount = GameTextures.LDJBMKDKOBO;
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
     * DLKADFPEAGC is the method that is called when the game applies the meshes to the player.
     */
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.DLKADFPEAGC))]
    [HarmonyPostfix]
    public static void GamePlayer_DLKADFPEAGC(ref GamePlayer __instance, int MHFLLOFKLLF)
    {
        if ((MHFLLOFKLLF == 4 && __instance.JKPAIJGCMPH.shape[MHFLLOFKLLF] > 50 &&
             __instance.JKPAIJGCMPH.shape[MHFLLOFKLLF] % 10 == 0) || VanillaCounts.ShapeCounts[MHFLLOFKLLF] == 0)
        {
            return;
        }

        try
        {
            if (__instance.JKPAIJGCMPH.shape[MHFLLOFKLLF] > VanillaCounts.ShapeCounts[MHFLLOFKLLF] ||
                (MHFLLOFKLLF == 17 &&
                 -__instance.JKPAIJGCMPH.shape[MHFLLOFKLLF] > VanillaCounts.TransparentHairHairstyleCount))
            {
                int shape = __instance.JKPAIJGCMPH.shape[MHFLLOFKLLF] > 0
                    ? __instance.JKPAIJGCMPH.shape[MHFLLOFKLLF] - VanillaCounts.ShapeCounts[MHFLLOFKLLF] - 1
                    : -__instance.JKPAIJGCMPH.shape[MHFLLOFKLLF] - VanillaCounts.TransparentHairHairstyleCount - 1;
                if (CustomCostumes.Values.Any(x => x.InternalPrefix.Contains("shape" + MHFLLOFKLLF)))
                {
                    Tuple<string, Object, Dictionary<string, string>> c = CustomCostumes.Values
                        .First(x => x.InternalPrefix.Contains("shape" + MHFLLOFKLLF))
                        .CustomObjects[shape];
                    ;
                    Mesh mesh = c.Item2 as Mesh;
                    Dictionary<string, string> meta = c.Item3;
                    if (mesh != null)
                    {
                        __instance.OOPKPKCHBEN[MHFLLOFKLLF].GetComponent<MeshFilter>().mesh = mesh;
                        if (meta.ContainsKey("scale"))
                        {
                            if (meta["scale"].Contains(","))
                            {
                                string[] scale = meta["scale"].Split(',');
                                __instance.OOPKPKCHBEN[MHFLLOFKLLF].transform.localScale = new Vector3(
                                    float.Parse(scale[0], Nfi), float.Parse(scale[1], Nfi), float.Parse(scale[2], Nfi));
                            }
                            else
                            {
                                __instance.OOPKPKCHBEN[MHFLLOFKLLF].transform.localScale = new Vector3(
                                    float.Parse(meta["scale"], Nfi), float.Parse(meta["scale"], Nfi),
                                    float.Parse(meta["scale"], Nfi));
                            }
                        }

                        if (meta.ContainsKey("position"))
                        {
                            string[] position = meta["position"].Split(',');
                            __instance.OOPKPKCHBEN[MHFLLOFKLLF].transform.localPosition = new Vector3(
                                float.Parse(position[0], Nfi), float.Parse(position[1], Nfi),
                                float.Parse(position[2], Nfi));
                        }

                        if (meta.ContainsKey("rotation"))
                        {
                            string[] rotation = meta["rotation"].Split(',');
                            __instance.OOPKPKCHBEN[MHFLLOFKLLF].transform.localRotation = Quaternion.Euler(
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
            Plugin.Log.LogError("MHFLLOFKLLF: " + MHFLLOFKLLF + " (" + __instance.JKPAIJGCMPH.shape[MHFLLOFKLLF] + ")");
        }
    }

    [HarmonyPatch(typeof(GameTextures), nameof(GameTextures.PIMHNLDNBOE))]
    [HarmonyPostfix]
    public static void GameTextures_PIMHNLDNBOE(GamePlayer FJCOPECCEKN)
    {
        FixMeshes(FJCOPECCEKN);
    }

    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.JAIOJDGDCLP))]
    [HarmonyPostfix]
    public static void GamePlayer_JAIOJDGDCLP(GamePlayer __instance)
    {
        FixMeshes(__instance);
    }

    private static void FixMeshes(GamePlayer player)
    {
        try
        {
            for (GameTextures.MHFLLOFKLLF = 1;
                 GameTextures.MHFLLOFKLLF <= GameTextures.IJDEFKDCNAN;
                 GameTextures.MHFLLOFKLLF++)
            {
                if ((GameTextures.MHFLLOFKLLF == 4 && player.JKPAIJGCMPH.shape[GameTextures.MHFLLOFKLLF] > 50 &&
                     player.JKPAIJGCMPH.shape[GameTextures.MHFLLOFKLLF] % 10 == 0) ||
                    player.OOPKPKCHBEN[GameTextures.MHFLLOFKLLF] == null ||
                    VanillaCounts.ShapeCounts[GameTextures.MHFLLOFKLLF] == 0)
                {
                    continue;
                }

                if (player.JKPAIJGCMPH.shape[GameTextures.MHFLLOFKLLF] >
                    VanillaCounts.ShapeCounts[GameTextures.MHFLLOFKLLF]
                    || (GameTextures.MHFLLOFKLLF == 17 && -player.JKPAIJGCMPH.shape[GameTextures.MHFLLOFKLLF] >
                        VanillaCounts.TransparentHairHairstyleCount))
                {
                    Mesh mesh = player.OOPKPKCHBEN[GameTextures.MHFLLOFKLLF].GetComponent<MeshFilter>().mesh;
                    if (player.OOPKPKCHBEN[GameTextures.MHFLLOFKLLF].GetComponent<MeshRenderer>().materials.Length <
                        mesh.subMeshCount)
                    {
                        int shape = player.JKPAIJGCMPH.shape[GameTextures.MHFLLOFKLLF] > 0
                            ? player.JKPAIJGCMPH.shape[GameTextures.MHFLLOFKLLF] -
                              VanillaCounts.ShapeCounts[GameTextures.MHFLLOFKLLF] - 1
                            : -player.JKPAIJGCMPH.shape[GameTextures.MHFLLOFKLLF] -
                              VanillaCounts.TransparentHairHairstyleCount - 1;
                        Dictionary<string, string> meta = new();
                        if (CustomCostumes.Values.Any(
                                x => x.InternalPrefix.Contains("shape" + GameTextures.MHFLLOFKLLF)))
                        {
                            meta = CustomCostumes.Values
                                .First(x => x.InternalPrefix.Contains("shape" + GameTextures.MHFLLOFKLLF))
                                .CustomObjects[shape]
                                .Item3;
                        }

                        Material[] materials = new Material[mesh.subMeshCount];
                        materials[0] = player.OOPKPKCHBEN[GameTextures.MHFLLOFKLLF].GetComponent<MeshRenderer>()
                            .material;
                        for (int i = 1; i < materials.Length; i++)
                        {
                            materials[i] = new Material(player.OOPKPKCHBEN[GameTextures.MHFLLOFKLLF]
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

                        player.OOPKPKCHBEN[GameTextures.MHFLLOFKLLF].GetComponent<MeshRenderer>().materials =
                            materials;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
            Plugin.Log.LogError("MHFLLOFKLLF: " + GameTextures.MHFLLOFKLLF + " (" +
                                player.JKPAIJGCMPH.shape[GameTextures.MHFLLOFKLLF] + ")");
        }
    }
}