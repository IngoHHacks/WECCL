using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
internal class AnimationPatch
{
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.CECEJCFDOEP))]
    [HarmonyPrefix]
    public static void GamePlayer_CECEJCFDOEP(ref GamePlayer __instance)
    {
        if (__instance.JGPFJIBNLFC >= 1000000)
        {
            var anim = __instance.BFGNHPHILHK;
            var controller = (AnimatorOverrideController) anim.runtimeAnimatorController;
            if (CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item2.ReceiveAnim != null) return;
            if (controller.name != "CustomAnimation" + __instance.JGPFJIBNLFC)
            {
                controller.name = "CustomAnimation" + __instance.JGPFJIBNLFC;
                controller["Custom00"] = CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item1;
                BCKHHMIMAEN.AEAMFLIMHGN[100] = CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item1.length * CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item1.frameRate;
                BCKHHMIMAEN.EENMACFDDEE[100] = 1f / BCKHHMIMAEN.AEAMFLIMHGN[100];
            }
            Animations.DoCustomAnimation(__instance, __instance.JGPFJIBNLFC, CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item2.ForwardSpeedMultiplier);
        }
    }
    
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.GJPOIDFLOLO))]
    [HarmonyPrefix]
    public static bool GamePlayer_GJPOIDFLOLO(ref GamePlayer __instance)
    {
        if (__instance.JGPFJIBNLFC >= 1000000)
        {
            var anim = __instance.BFGNHPHILHK;
            var controller = (AnimatorOverrideController) anim.runtimeAnimatorController;
            if (CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item2.ReceiveAnim == null) return true;
            __instance.AKLPBFADAMB = 0;
            __instance.NMJNOMIGHPF = 0f;
            __instance.JGKCHHHIDFE = 0;
            __instance.DPIMBMHANLC = 0f;
            if (controller.name != "CustomAnimation" + __instance.JGPFJIBNLFC)
            {
                controller.name = "CustomAnimation" + __instance.JGPFJIBNLFC;
                controller["Custom00"] = CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item1;
                BCKHHMIMAEN.AEAMFLIMHGN[100] = CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item1.length * CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item1.frameRate;
                BCKHHMIMAEN.EENMACFDDEE[100] = 1f / BCKHHMIMAEN.AEAMFLIMHGN[100];
            }
            var opponent = FFKMIEMAJML.FJCOPECCEKN[__instance.CJGHFHCHDNN];
            if (opponent?.BFGNHPHILHK?.runtimeAnimatorController == null) return true;
            var oppController = (AnimatorOverrideController) opponent.BFGNHPHILHK.runtimeAnimatorController;
            if (oppController.name != "CustomAnimationReceive" + __instance.JGPFJIBNLFC)
            {
                oppController.name = "CustomAnimationReceive" + __instance.JGPFJIBNLFC;
                oppController["Custom01"] = CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item2.ReceiveAnim;
                BCKHHMIMAEN.AEAMFLIMHGN[101] = CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item2.ReceiveAnim.length * CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item2.ReceiveAnim.frameRate;
                BCKHHMIMAEN.EENMACFDDEE[101] = 1f / BCKHHMIMAEN.AEAMFLIMHGN[101];
            }
            //Animations.DoCustomAnimation(__instance, __instance.JGPFJIBNLFC, CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item2.ForwardSpeedMultiplier);
            Animations.PerformTestAnimation(__instance, __instance.JGPFJIBNLFC, CustomAnimationClips[__instance.JGPFJIBNLFC - 1000000].Item2.ForwardSpeedMultiplier);
            Animations.PerformPostGrappleCode(__instance);
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(BCKHHMIMAEN), nameof(BCKHHMIMAEN.JCEIBJPONOM))]
    [HarmonyPrefix]
    public static bool BCKHHMIMAEN_JCEIBJPONOM(ref string __result, int NNIHHMADKHI)
    {
        if (NNIHHMADKHI >= 100)
        {
            __result = "Custom" + (NNIHHMADKHI - 100).ToString("00");
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(BCKHHMIMAEN), nameof(BCKHHMIMAEN.IIPHLNJKONO))]
    [HarmonyPostfix]
    public static void BCKHHMIMAEN_IIPHLNJKONO()
    {
        Array.Resize(ref BCKHHMIMAEN.AEAMFLIMHGN, 200);
        Array.Resize(ref BCKHHMIMAEN.EENMACFDDEE, 200);
    }
    
    [HarmonyPatch(typeof(BCKHHMIMAEN), nameof(BCKHHMIMAEN.OOKPOBBPPOD))]
    [HarmonyPrefix]
    public static bool BCKHHMIMAEN_OOKPOBBPPOD(ref string __result, int FLGDADINOPK)
    {
        if (FLGDADINOPK >= 1000000)
        {
            __result = CustomAnimationClips[FLGDADINOPK - 1000000].Item2.Name ?? "CustomAnimation" + (FLGDADINOPK - 1000000).ToString("00");
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.GPADNIMNGBA))]
    [HarmonyPrefix]
    public static bool GamePlayer_GPADNIMNGBA(ref GamePlayer __instance)
    {
        return __instance.JGPFJIBNLFC < 1000000;
    }
    
    [HarmonyPatch(typeof(GamePlayer), nameof(GamePlayer.CMHHOLOLIAM))]
    [HarmonyPostfix]
    public static void GamePlayer_CMHHOLOLIAM(ref GamePlayer __instance)
    {
        var orig = __instance.BFGNHPHILHK.runtimeAnimatorController;
        var overrideController = new AnimatorOverrideController(AO.AnimationController);
        foreach (var clip in orig.animationClips)
        {
            overrideController[clip.name] = clip;
        }
        __instance.BFGNHPHILHK.runtimeAnimatorController = overrideController;
    }
}