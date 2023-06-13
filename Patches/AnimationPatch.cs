using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
internal class AnimationPatch
{
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.HBMEELACHFO))]
    [HarmonyPrefix]
    public static void GamePlayer_HBMEELACHFO(ref GamePlayer __instance)
    {
        if (__instance.MDOCJJELCBG >= 1000000)
        {
            var anim = __instance.MKEAFINLGIO;
            var controller = (AnimatorOverrideController) anim.runtimeAnimatorController;
            if (CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item2.ReceiveAnim != null) return;
            if (controller.name != "CustomAnimation" + __instance.MDOCJJELCBG)
            {
                controller.name = "CustomAnimation" + __instance.MDOCJJELCBG;
                controller["Custom00"] = CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item1;
                NBPIEPNKBDG.KCIBKFDHPPD[100] = CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item1.length * CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item1.frameRate;
                NBPIEPNKBDG.AKKFFGMMCHD[100] = 1f / NBPIEPNKBDG.KCIBKFDHPPD[100];
            }
            Animations.DoCustomAnimation(__instance, __instance.MDOCJJELCBG, CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item2.ForwardSpeedMultiplier);
        }
    }
    
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.JAHCFDMANAI))]
    [HarmonyPrefix]
    public static bool GamePlayer_JAHCFDMANAI(ref GamePlayer __instance)
    {
        if (__instance.MDOCJJELCBG >= 1000000)
        {
            var anim = __instance.MKEAFINLGIO;
            var controller = (AnimatorOverrideController) anim.runtimeAnimatorController;
            if (CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item2.ReceiveAnim == null) return true;
            __instance.CMMMEGLFLNJ = 0;
            __instance.OJHKNFHFOCO = 0f;
            __instance.MPNLGGMEJOC = 0;
            __instance.CLGGNCLADPG = 0f;
            if (controller.name != "CustomAnimation" + __instance.MDOCJJELCBG)
            {
                controller.name = "CustomAnimation" + __instance.MDOCJJELCBG;
                controller["Custom00"] = CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item1;
                NBPIEPNKBDG.KCIBKFDHPPD[100] = CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item1.length * CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item1.frameRate;
                NBPIEPNKBDG.AKKFFGMMCHD[100] = 1f / NBPIEPNKBDG.KCIBKFDHPPD[100];
            }
            var opponent = AMJONEKIAID.NCPIJJFEDFL[__instance.PDMDFGNJCPN];
            if (opponent?.MKEAFINLGIO?.runtimeAnimatorController == null) return true;
            var oppController = (AnimatorOverrideController) opponent.MKEAFINLGIO.runtimeAnimatorController;
            if (oppController.name != "CustomAnimationReceive" + __instance.MDOCJJELCBG)
            {
                oppController.name = "CustomAnimationReceive" + __instance.MDOCJJELCBG;
                oppController["Custom01"] = CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item2.ReceiveAnim;
                NBPIEPNKBDG.KCIBKFDHPPD[101] = CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item2.ReceiveAnim.length * CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item2.ReceiveAnim.frameRate;
                NBPIEPNKBDG.AKKFFGMMCHD[101] = 1f / NBPIEPNKBDG.KCIBKFDHPPD[101];
            }
            //Animations.DoCustomAnimation(__instance, __instance.MDOCJJELCBG, CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item2.ForwardSpeedMultiplier);
            Animations.PerformTestAnimation(__instance, __instance.MDOCJJELCBG, CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item2.ForwardSpeedMultiplier);
            Animations.PerformPostGrappleCode(__instance);
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(NBPIEPNKBDG), nameof(NBPIEPNKBDG.LODJNNLLCEM))]
    [HarmonyPrefix]
    public static bool NBPIEPNKBDG_LODJNNLLCEM(ref string __result, int FJIFJEGHJCA)
    {
        if (FJIFJEGHJCA >= 100)
        {
            __result = "Custom" + (FJIFJEGHJCA - 100).ToString("00");
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(NBPIEPNKBDG), nameof(NBPIEPNKBDG.EPCEEDKJHAH))]
    [HarmonyPostfix]
    public static void NBPIEPNKBDG_EPCEEDKJHAH()
    {
        Array.Resize(ref NBPIEPNKBDG.KCIBKFDHPPD, 200);
        Array.Resize(ref NBPIEPNKBDG.AKKFFGMMCHD, 200);
    }
    
    [HarmonyPatch(typeof(NBPIEPNKBDG), nameof(NBPIEPNKBDG.OOKDMJJFLBO))]
    [HarmonyPrefix]
    public static bool NBPIEPNKBDG_OOKDMJJFLBO(ref string __result, int FAFFHCHCLHI)
    {
        if (FAFFHCHCLHI >= 1000000)
        {
            __result = CustomAnimationClips[FAFFHCHCLHI - 1000000].Item2.Name ?? "CustomAnimation" + (FAFFHCHCLHI - 1000000).ToString("00");
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.AJOOKABNAOE))]
    [HarmonyPrefix]
    public static bool GamePlayer_AJOOKABNAOE(ref GamePlayer __instance)
    {
        return __instance.MDOCJJELCBG < 1000000;
    }
    
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.APJAPBDDLOC))]
    [HarmonyPostfix]
    public static void GamePlayer_APJAPBDDLOC(ref GamePlayer __instance)
    {
        var orig = __instance.MKEAFINLGIO.runtimeAnimatorController;
        var overrideController = new AnimatorOverrideController(AO.AnimationController);
        foreach (var clip in orig.animationClips)
        {
            overrideController[clip.name] = clip;
        }
        __instance.MKEAFINLGIO.runtimeAnimatorController = overrideController;
    }
}