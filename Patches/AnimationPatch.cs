#if ANIMATION_TEST

using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
internal class AnimationPatch
{
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.BCHJKLDMDFB))]
    [HarmonyPrefix]
    public static void Player_BCHJKLDMDFB(UnmappedPlayer __instance)
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
    
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.JPNLADBMDNK))]
    [HarmonyPrefix]
    public static bool Player_JPNLADBMDNK(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        if (p.anim >= 1000000)
        {
            var anim = p.animator;
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
            var opponent = NJBJIIIACEP.OAAMGFLINOB[p.foc];
            if (opponent?.MPMGGCCFCOP?.runtimeAnimatorController == null) return true;
            var oppController = (AnimatorOverrideController) opponent.MPMGGCCFCOP.runtimeAnimatorController;
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

    [HarmonyPatch(typeof(UnmappedAnims), nameof(UnmappedAnims.NNEMALOMALN))]
    [HarmonyPrefix]
    public static bool Anims_NNEMALOMALN(ref string __result, int PFDGHMKKHOF)
    {
        if (PFDGHMKKHOF >= 100)
        {
            __result = "Custom" + (PFDGHMKKHOF - 100).ToString("00");
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(UnmappedAnims), nameof(UnmappedAnims.EAPGGFCLELG))]
    [HarmonyPostfix]
    public static void Anims_EAPGGFCLELG()
    {
        Array.Resize(ref UnmappedAnims.NIMHPNKOPAE, 200);
        Array.Resize(ref UnmappedAnims.BKCAJIALAPC, 200);
    }
    
    [HarmonyPatch(typeof(UnmappedAnims), nameof(UnmappedAnims.DDIJBPJLEBF))]
    [HarmonyPrefix]
    public static bool Anims_DDIJBPJLEBF(ref string __result, int NOLKIINBMHA)
    {
        if (NOLKIINBMHA >= 1000000)
        {
            __result = CustomAnimationClips[NOLKIINBMHA - 1000000].Item2.Name ?? "CustomAnimation" + (NOLKIINBMHA - 1000000).ToString("00");
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.OFJBOCGFCBC))]
    [HarmonyPrefix]
    public static bool Player_OFJBOCGFCBC(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        return p.anim < 1000000;
    }
    
    [HarmonyPatch(typeof(UnmappedPlayer), nameof(UnmappedPlayer.DDKAGOBJGBC))]
    [HarmonyPostfix]
    public static void Player_DDKAGOBJGBC(ref UnmappedPlayer __instance)
    {
        MappedPlayer p = __instance;
        var orig = p.animator.runtimeAnimatorController;
        var overrideController = new AnimatorOverrideController(AO.AnimationController);
        foreach (var clip in orig.animationClips)
        {
            overrideController[clip.name] = clip;
        }
        p.animator.runtimeAnimatorController = overrideController;
        MappedMenus.screenTim = 0;
    }
}

#endif