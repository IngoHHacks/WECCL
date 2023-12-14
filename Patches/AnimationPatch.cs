using System.Reflection;
using System.Reflection.Emit;

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
            if (CustomAnimations[p.anim - 1000000].ReceiveAnim != null) return;
            if (controller.name != "CustomAnimation" + p.anim)
            {
                LogInfo($"Setting up animation for {p.charData.name}");
                controller.name = "CustomAnimation" + p.anim;
                controller["Custom00"] = CustomAnimations[p.anim - 1000000].Anim;
                MappedAnims.length[100] = CustomAnimations[p.anim - 1000000].Anim.length * CustomAnimations[p.anim - 1000000].Anim.frameRate;
                MappedAnims.timing[100] = 1f / MappedAnims.length[100];
            }
            Animations.DoCustomAnimation(p, p.anim, CustomAnimations[p.anim - 1000000].ForwardSpeedMultiplier);
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
            if (CustomAnimations[p.anim - 1000000].ReceiveAnim == null) return true;
            p.fileA = 0;
            p.frameA = 0f;
            p.fileB = 0;
            p.frameB = 0f;
            if (p.sellTim > 0f)
            {
                p.sellTim = 0f;
            }
            if (controller.name != "CustomAnimation" + p.anim)
            {
                controller.name = "CustomAnimation" + p.anim;
                controller["Custom00"] = CustomAnimations[p.anim - 1000000].Anim;
                MappedAnims.length[100] = CustomAnimations[p.anim - 1000000].Anim.length * CustomAnimations[p.anim - 1000000].Anim.frameRate;
                MappedAnims.timing[100] = 1f / MappedAnims.length[100];
            }
            var opponent = p.pV;
            if (opponent?.animator.runtimeAnimatorController == null) return true;
            var oppController = (AnimatorOverrideController) opponent.animator.runtimeAnimatorController;
            if (oppController.name != "CustomAnimationReceive" + p.anim)
            {
                oppController.name = "CustomAnimationReceive" + p.anim;
                oppController["Custom01"] = CustomAnimations[p.anim - 1000000].ReceiveAnim;
                MappedAnims.length[101] = CustomAnimations[p.anim - 1000000].ReceiveAnim.length * CustomAnimations[p.anim - 1000000].ReceiveAnim.frameRate;
                MappedAnims.timing[101] = 1f / MappedAnims.length[101];
            }
            Animations.DoCustomAnimation(p, p.anim, CustomAnimations[p.anim - 1000000].ForwardSpeedMultiplier);
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
            __result = CustomAnimations[NOLKIINBMHA - 1000000].Name ?? "CustomAnimation" + (NOLKIINBMHA - 1000000).ToString("00");
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
    
    [HarmonyPatch(typeof(Scene_Editor), nameof(Scene_Editor.Update))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Scene_Editor_Update_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        int flag = 0;
        int flag2 = 0;
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldloc_S)
            {
                var operand = instruction.operand;
                PropertyInfo property = operand.GetType().GetProperty("LocalIndex");
                if (property != null && (int)property.GetValue(operand) == 27)
                {
                    if (++flag == 2)
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(Animations), nameof(Animations.IsGrappleMove), new[] {typeof(int)}));
                    }
                }
            }
            if (instruction.opcode == OpCodes.Ldsfld && (FieldInfo)instruction.operand == AccessTools.Field(typeof(NJBJIIIACEP), nameof(NJBJIIIACEP.OAAMGFLINOB)))
            {
                if (flag2 < 4)
                {
                    flag2++;
                }
            }

            if (flag is 2 or 3)
            {
                if (instruction.opcode == OpCodes.Ldloc_0)
                {
                    flag = 4;
                    yield return instruction;
                }
                else if (instruction.opcode == OpCodes.Bge_S || instruction.opcode == OpCodes.Bge)
                {
                    yield return new CodeInstruction(OpCodes.Brfalse, instruction.operand);
                }
            } else if (flag2 is 4 or 5) {
                if (flag2 == 4)
                {
                    if (instruction.opcode == OpCodes.Ldc_I4_0)
                    {
                        flag2 = 5;
                        yield return new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(Animations), nameof(Animations.IsRegularMove), new[] {typeof(int)}));
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
                else
                {
                    if (instruction.opcode == OpCodes.Bge_S || instruction.opcode == OpCodes.Bge)
                    {
                        yield return new CodeInstruction(OpCodes.Brfalse, instruction.operand);
                        flag2 = 6;
                    }
                }
            }
            else
            {
                yield return instruction;
            }
        }
    }
}
