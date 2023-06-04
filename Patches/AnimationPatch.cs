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
            var orig = anim.runtimeAnimatorController;
            if (orig.name != "CustomAnimation" + __instance.MDOCJJELCBG)
            {
                if (orig.name.StartsWith("CustomAnimation"))
                {
                    orig = __instance.MKEAFINLGIO.runtimeAnimatorController;
                }
                var newController = new AnimatorOverrideController(orig);
                newController.name = "CustomAnimation" + __instance.MDOCJJELCBG;
                newController["Standard00"] = CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item1;
                __instance.MKEAFINLGIO.runtimeAnimatorController = newController;
                NBPIEPNKBDG.KCIBKFDHPPD[100] = CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item1.length * CustomAnimationClips[__instance.MDOCJJELCBG - 1000000].Item1.frameRate;
                NBPIEPNKBDG.AKKFFGMMCHD[100] = 1f / NBPIEPNKBDG.KCIBKFDHPPD[100];
            }
            Animations.DoCustomAnimation(__instance, __instance.MDOCJJELCBG);
        }
        else
        {
            var orig = __instance.MKEAFINLGIO.runtimeAnimatorController;
            if (orig.name.StartsWith("CustomAnimation"))
            {
                __instance.MKEAFINLGIO.runtimeAnimatorController = ((AnimatorOverrideController) orig).runtimeAnimatorController;
            }
        }
    }
    
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.LCENEGBMIEJ))]
    [HarmonyPrefix]
    public static void GamePlayer_LCENEGBMIEJ(ref GamePlayer __instance)
    {
        if (__instance.MDOCJJELCBG >= 1000000) return;
        var orig = __instance.MKEAFINLGIO.runtimeAnimatorController;
        if (orig.name.StartsWith("CustomAnimation"))
        {
            __instance.MKEAFINLGIO.runtimeAnimatorController = ((AnimatorOverrideController) orig).runtimeAnimatorController;
        }
    }

    [HarmonyPatch(typeof(NBPIEPNKBDG), nameof(NBPIEPNKBDG.LODJNNLLCEM))]
    [HarmonyPrefix]
    public static bool NBPIEPNKBDG_LODJNNLLCEM(ref string __result, int FJIFJEGHJCA)
    {
        if (FJIFJEGHJCA >= 100)
        {
            __result = "Standard" + (FJIFJEGHJCA - 100).ToString("00");
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
}