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
    
    internal static bool _contentLoaded = false;

    /*
     * GameGlobals.GPPPEEGMGGP loads an object from an AssetBundle.
     * This patch is used to load custom objects from the Assets folder(s).
     */
    [HarmonyPatch(typeof(GameGlobals), nameof(GameGlobals.GPPPEEGMGGP))]
    [HarmonyPrefix]
    public static bool GameGlobals_GPPPEEGMGGP(ref Object __result, string ICKMMIPDKEI, string INCOBPKLMHL)
    {
        if (ICKMMIPDKEI.StartsWith("Music"))
        {
            if (INCOBPKLMHL.StartsWith("Theme") && int.Parse(INCOBPKLMHL.Substring(5)) > VanillaCounts.MusicCount)
            {
                __result = CustomClips[int.Parse(INCOBPKLMHL.Substring(5)) - VanillaCounts.MusicCount - 1];
                return false;
            }
        }
        else if (ICKMMIPDKEI.StartsWith("Costumes"))
        {
            int numberIndex = 0;
            while (numberIndex < INCOBPKLMHL.Length && !char.IsDigit(INCOBPKLMHL[numberIndex]))
            {
                numberIndex++;
            }

            if (numberIndex == 0 || numberIndex == INCOBPKLMHL.Length)
            {
                return true;
            }

            string prefix = INCOBPKLMHL.Substring(0, numberIndex);
            int index = 0;
            if (int.TryParse(INCOBPKLMHL.Substring(numberIndex), out index))
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
     * GameTextures.PKBAFLGBINB is called when the game loads the vanilla content constants.
     * This patch is used to update the vanilla content constants with the custom content counts.
     */
    [HarmonyPatch(typeof(GameTextures), nameof(GameTextures.PKBAFLGBINB))]
    [HarmonyPostfix]
    public static void GameTextures_PKBAFLGBINB()
    {
        VanillaCounts.MaterialCounts = GameTextures.AOBEBHODFKG.ToList();
        VanillaCounts.FleshCounts = GameTextures.GEANFMLIDLN.ToList();
        VanillaCounts.ShapeCounts = GameTextures.AEBNMPCDDAL.ToList();
        VanillaCounts.BodyFemaleCount = GameTextures.IANHIBOPGJD;
        VanillaCounts.FaceFemaleCount = GameTextures.MBDNIAIJFDE;
        VanillaCounts.SpecialFootwearCount = GameTextures.MIBHEJJOBME;
        VanillaCounts.TransparentHairMaterialCount = GameTextures.GHOGKPDHOJH;
        VanillaCounts.TransparentHairHairstyleCount = GameTextures.IGDBDHBFCDE;
        VanillaCounts.KneepadCount = GameTextures.JOKEKDGHCGL;
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
     * IMPDEJAKALA is the method that is called when the game applies the meshes to the player.
     */
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.IMPDEJAKALA))]
    [HarmonyPostfix]
    public static void GamePlayer_IMPDEJAKALA(ref GamePlayer __instance, int DLMFHPGACEP)
    {
        if (DLMFHPGACEP == 4 && (__instance.JKFJKEOEKEM.shape[DLMFHPGACEP] > 50 && __instance.JKFJKEOEKEM.shape[DLMFHPGACEP] % 10 == 0) || VanillaCounts.ShapeCounts[DLMFHPGACEP] == 0) return;
        try {
            if (__instance.JKFJKEOEKEM.shape[DLMFHPGACEP] > VanillaCounts.ShapeCounts[DLMFHPGACEP] || (DLMFHPGACEP == 17 &&
                    -__instance.JKFJKEOEKEM.shape[DLMFHPGACEP] > VanillaCounts.TransparentHairHairstyleCount))
            {
                var shape = __instance.JKFJKEOEKEM.shape[DLMFHPGACEP] > 0 ? __instance.JKFJKEOEKEM.shape[DLMFHPGACEP] - VanillaCounts.ShapeCounts[DLMFHPGACEP] - 1: -__instance.JKFJKEOEKEM.shape[DLMFHPGACEP] - VanillaCounts.TransparentHairHairstyleCount - 1;
                if (CustomCostumes.Values.Any(x => x.InternalPrefix.Contains("shape" + DLMFHPGACEP)))
                {
                    var c = CustomCostumes.Values.First(x => x.InternalPrefix.Contains("shape" + DLMFHPGACEP))
                        .CustomObjects[shape];
                    ;
                    var mesh = c.Item2 as Mesh;
                    var meta = c.Item3;
                    if (mesh != null)
                    {
                        __instance.PNINKKAAPBD[DLMFHPGACEP].GetComponent<MeshFilter>().mesh = mesh;
                        if (meta.ContainsKey("scale"))
                        {
                            if (meta["scale"].Contains(",")) {
                                var scale = meta["scale"].Split(',');
                                __instance.PNINKKAAPBD[DLMFHPGACEP].transform.localScale = new Vector3(float.Parse(scale[0], Nfi), float.Parse(scale[1], Nfi), float.Parse(scale[2], Nfi));
                            } else {
                                __instance.PNINKKAAPBD[DLMFHPGACEP].transform.localScale = new Vector3(float.Parse(meta["scale"], Nfi), float.Parse(meta["scale"], Nfi), float.Parse(meta["scale"], Nfi));
                            }
                        }
                        if (meta.ContainsKey("position"))
                        {
                            var position = meta["position"].Split(',');
                            __instance.PNINKKAAPBD[DLMFHPGACEP].transform.localPosition = new Vector3(float.Parse(position[0], Nfi), float.Parse(position[1], Nfi), float.Parse(position[2], Nfi));
                        }
                        if (meta.ContainsKey("rotation"))
                        {
                            var rotation = meta["rotation"].Split(','); 
                            __instance.PNINKKAAPBD[DLMFHPGACEP].transform.localRotation = Quaternion.Euler(float.Parse(rotation[0], Nfi), float.Parse(rotation[1], Nfi), float.Parse(rotation[2], Nfi));
                        }
                    }
                }
            }
        } catch (Exception e)
        {
            Plugin.Log.LogError(e);
            Plugin.Log.LogError("DLMFHPGACEP: " + DLMFHPGACEP + " (" + __instance.JKFJKEOEKEM.shape[DLMFHPGACEP] + ")");
        }
    }

    [HarmonyPatch(typeof(GameTextures), nameof(GameTextures.OAMIGPHBLOB))]
    [HarmonyPostfix]
    public static void GameTextures_OAMIGPHBLOB(GamePlayer KHMKIGPJPHN)
    {
        FixMeshes(KHMKIGPJPHN);
    }
    
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.FLNCGNOKCJG))]
    [HarmonyPostfix]
    public static void GamePlayer_FLNCGNOKCJG(GamePlayer __instance)
    {
        FixMeshes(__instance);
    }

    private static void FixMeshes(GamePlayer player)
    {
        try
        {
            for (GameTextures.DLMFHPGACEP = 1;
                 GameTextures.DLMFHPGACEP <= GameTextures.KMDKBPEGPKA;
                 GameTextures.DLMFHPGACEP++)
                {
                    if (GameTextures.DLMFHPGACEP == 4 && player.JKFJKEOEKEM.shape[GameTextures.DLMFHPGACEP] > 50 && player.JKFJKEOEKEM.shape[GameTextures.DLMFHPGACEP] % 10 == 0 || player.PNINKKAAPBD[GameTextures.DLMFHPGACEP] == null|| VanillaCounts.ShapeCounts[GameTextures.DLMFHPGACEP] == 0)
                    {
                        continue;
                    }
                    if (player.JKFJKEOEKEM.shape[GameTextures.DLMFHPGACEP] > VanillaCounts.ShapeCounts[GameTextures.DLMFHPGACEP]
                        || (GameTextures.DLMFHPGACEP == 17 && -player.JKFJKEOEKEM.shape[GameTextures.DLMFHPGACEP] > VanillaCounts.TransparentHairHairstyleCount))
                    {
                    var mesh = player.PNINKKAAPBD[GameTextures.DLMFHPGACEP].GetComponent<MeshFilter>().mesh;
                    if (player.PNINKKAAPBD[GameTextures.DLMFHPGACEP].GetComponent<MeshRenderer>().materials.Length <
                        mesh.subMeshCount)
                    {
                        var shape = player.JKFJKEOEKEM.shape[GameTextures.DLMFHPGACEP] > 0 ? player.JKFJKEOEKEM.shape[GameTextures.DLMFHPGACEP] - VanillaCounts.ShapeCounts[GameTextures.DLMFHPGACEP] - 1: -player.JKFJKEOEKEM.shape[GameTextures.DLMFHPGACEP] - VanillaCounts.TransparentHairHairstyleCount - 1;
                        var meta = new Dictionary<string, string>();
                        if (CustomCostumes.Values.Any(x => x.InternalPrefix.Contains("shape" + GameTextures.DLMFHPGACEP)))
                        {
                            meta = CustomCostumes.Values
                                .First(x => x.InternalPrefix.Contains("shape" + GameTextures.DLMFHPGACEP))
                                .CustomObjects[shape]
                                .Item3;
                        }

                        var materials = new Material[mesh.subMeshCount];
                        materials[0] = player.PNINKKAAPBD[GameTextures.DLMFHPGACEP].GetComponent<MeshRenderer>().material;
                        for (int i = 1; i < materials.Length; i++)
                        {
                            materials[i] = new Material(player.PNINKKAAPBD[GameTextures.DLMFHPGACEP].GetComponent<MeshRenderer>().material);

                            if (meta.ContainsKey("submesh" + i + "color"))
                            {
                                var split = meta["submesh" + i + "color"].Split(',');
                                if (split.Length == 3)
                                {
                                    materials[i].color = new Color(float.Parse(split[0], Nfi), float.Parse(split[1], Nfi),
                                        float.Parse(split[2], Nfi));
                                }

                                materials[i].color = ColorUtility.TryParseHtmlString(meta["submesh" + i + "color"], out var color) ? color : Color.white;
                            }
                        }

                        player.PNINKKAAPBD[GameTextures.DLMFHPGACEP].GetComponent<MeshRenderer>().materials =
                            materials;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
            Plugin.Log.LogError("DLMFHPGACEP: " + GameTextures.DLMFHPGACEP + " (" + player.JKFJKEOEKEM.shape[GameTextures.DLMFHPGACEP] + ")");
        }
    }
}