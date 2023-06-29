using UnityEngine.UI;
using WECCL.Content;

namespace WECCL.Patches;

using static LoadContent;

[HarmonyPatch]
public class LoadingPatch
{
    private static bool _uncolor;
    private static Text _text;
    private static Text _text2;

    [HarmonyPatch(typeof(Scene_Loading), nameof(Scene_Loading.Update))]
    [HarmonyPrefix]
    public static bool Scene_Loading_Update(Scene_Loading __instance)
    {
        if (_text == null)
        {
            Transform parent = __instance.gLoader.transform.parent;
            GameObject go = new("LoadingText", typeof(Text));
            go.transform.SetParent(parent);
            go.transform.position = __instance.gLoader.transform.position + new Vector3(0f, -100f, 0f);
            _text = go.GetComponent<Text>();
            _text.text = "Loading...";
            _text.fontSize = 30;
            _text.color = new Color(1f, 1f, 1f, 1f);
            _text.alignment = TextAnchor.MiddleCenter;
            _text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            _text.rectTransform.sizeDelta = new Vector2(10000f, 100f);
            GameObject go2 = new("LoadingTextSub", typeof(Text));
            go2.transform.SetParent(parent);
            go2.transform.position = __instance.gLoader.transform.position + new Vector3(0f, -130f, 0f);
            _text2 = go2.GetComponent<Text>();
            _text2.text = "";
            _text2.fontSize = 20;
            _text2.color = new Color(1f, 1f, 1f, 1f);
            _text2.alignment = TextAnchor.MiddleCenter;
            _text2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            _text2.rectTransform.sizeDelta = new Vector2(10000f, 100f);
        }

        if (_modsLoaded)
        {
            if (!_uncolor)
            {
                __instance.gLoadMeter.GetComponent<Image>().color =
                    new Color(1f, 1f, 1f, 1f);
                _text.color = new Color(1f, 1f, 1f, 1f);
                _uncolor = true;
            }

            _text.text = $"Vanilla content: {__instance.loadProgress * 100f:0.0}%";
            _text2.text = "";
            return true;
        }

        __instance.gLoader.SetActive(true);
        _progressGradual = GameGlobals.EBGOIAGEPLN(_progressGradual, _progress, 0.2f, 0.01f);
        __instance.gLoadMeter.transform.localScale =
            new Vector3(_progressGradual, 1f, 1f);
        __instance.gLoadMeter.GetComponent<Image>().color =
            new Color(0f, 0.9f, 0f, 1f);
        _text.text = $"Modded content: {_loadedAssets}/{_totalAssets} - {_progress * 100f:0.0}%";
        _text.color = new Color(0f, 0.9f, 0f, 1f);
        _text2.text = _lastItemLoaded;
        _text2.color = new Color(0f, 0.5f, 0f, 1f);
        return false;
    }
}