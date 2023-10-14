using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
internal class AnimationPatch
{
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.CECEJCFDOEP))]
    [HarmonyPrefix]
    public static void Player_CECEJCFDOEP(UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        if (p.anim >= 1000000)
        {
            var anim = p.animator;
            var controller = (AnimatorOverrideController) anim.runtimeAnimatorController;
            if (CustomAnimationClips[p.anim - 1000000].Item2.ReceiveAnim != null) return;
            if (controller.name != "CustomAnimation" + p.anim)
            {
                controller.name = "CustomAnimation" + p.anim;
                controller["Custom00"] = CustomAnimationClips[p.anim - 1000000].Item1;
                MappedAnims.length[100] = CustomAnimationClips[p.anim - 1000000].Item1.length * CustomAnimationClips[p.anim - 1000000].Item1.frameRate;
                MappedAnims.timing[100] = 1f / MappedAnims.length[100];
            }
            Animations.DoCustomAnimation(p, p.anim, CustomAnimationClips[p.anim - 1000000].Item2.ForwardSpeedMultiplier);
        }
    }
    
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.GJPOIDFLOLO))]
    [HarmonyPrefix]
    public static bool Player_GJPOIDFLOLO(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        if (p.anim >= 1000000)
        {
            var anim = p.BFGNHPHILHK;
            var controller = (AnimatorOverrideController) anim.runtimeAnimatorController;
            if (CustomAnimationClips[p.anim - 1000000].Item2.ReceiveAnim == null) return true;
            p.fileA = 0;
            p.frameA = 0f;
            p.fileB = 0;
            p.frameB = 0f;
            if (controller.name != "CustomAnimation" + p.anim)
            {
                controller.name = "CustomAnimation" + p.anim;
                controller["Custom00"] = CustomAnimationClips[p.anim - 1000000].Item1;
                MappedAnims.length[100] = CustomAnimationClips[p.anim - 1000000].Item1.length * CustomAnimationClips[p.anim - 1000000].Item1.frameRate;
                MappedAnims.timing[100] = 1f / MappedAnims.length[100];
            }
            var opponent = FFKMIEMAJML.FJCOPECCEKN[p.foc];
            if (opponent?.BFGNHPHILHK?.runtimeAnimatorController == null) return true;
            var oppController = (AnimatorOverrideController) opponent.BFGNHPHILHK.runtimeAnimatorController;
            if (oppController.name != "CustomAnimationReceive" + p.anim)
            {
                oppController.name = "CustomAnimationReceive" + p.anim;
                oppController["Custom01"] = CustomAnimationClips[p.anim - 1000000].Item2.ReceiveAnim;
                MappedAnims.length[101] = CustomAnimationClips[p.anim - 1000000].Item2.ReceiveAnim.length * CustomAnimationClips[p.anim - 1000000].Item2.ReceiveAnim.frameRate;
                MappedAnims.timing[101] = 1f / MappedAnims.length[101];
            }
            //Animations.DoCustomAnimation(p, p.anim, CustomAnimationClips[p.anim - 1000000].Item2.ForwardSpeedMultiplier);
            Animations.PerformTestAnimation(p, p.anim, CustomAnimationClips[p.anim - 1000000].Item2.ForwardSpeedMultiplier);
            Animations.PerformPostGrappleCode(p);
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(MappedAnims), nameof(MappedAnims.JCEIBJPONOM))]
    [HarmonyPrefix]
    public static bool MappedAnims_JCEIBJPONOM(ref string __result, int NNIHHMADKHI)
    {
        if (NNIHHMADKHI >= 100)
        {
            __result = "Custom" + (NNIHHMADKHI - 100).ToString("00");
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(MappedAnims), nameof(MappedAnims.IIPHLNJKONO))]
    [HarmonyPostfix]
    public static void MappedAnims_IIPHLNJKONO()
    {
        Array.Resize(ref UnmappedAnims.AEAMFLIMHGN, 200);
        Array.Resize(ref UnmappedAnims.EENMACFDDEE, 200);
    }
    
    [HarmonyPatch(typeof(MappedAnims), nameof(MappedAnims.OOKPOBBPPOD))]
    [HarmonyPrefix]
    public static bool MappedAnims_OOKPOBBPPOD(ref string __result, int FLGDADINOPK)
    {
        if (FLGDADINOPK >= 1000000)
        {
            __result = CustomAnimationClips[FLGDADINOPK - 1000000].Item2.Name ?? "CustomAnimation" + (FLGDADINOPK - 1000000).ToString("00");
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.GPADNIMNGBA))]
    [HarmonyPrefix]
    public static bool Player_GPADNIMNGBA(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        return p.anim < 1000000;
    }
    
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.CMHHOLOLIAM))]
    [HarmonyPostfix]
    public static void Player_CMHHOLOLIAM(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        var orig = p.animator.runtimeAnimatorController;
        var overrideController = new AnimatorOverrideController(AO.AnimationController);
        foreach (var clip in orig.animationClips)
        {
            overrideController[clip.name] = clip;
        }
        p.animator.runtimeAnimatorController = overrideController;
    }
}