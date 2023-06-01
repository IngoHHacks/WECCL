using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
public class CostumePatch
{
    [HarmonyPatch(typeof(Costume), nameof(Costume.IFDJGOBNCMG))]
    [HarmonyPostfix]
    public static void CostumePostfix(ref Color __result, Costume __instance)
    {
        if (__instance.texture[3] > VanillaCounts.MaterialCounts[3])
        {
            __result *= GetSkinColor(__instance.texture[3]);
        }
        else if (__instance.texture[3] < -VanillaCounts.FaceFemaleCount)
        {
            __result *= GetSkinColor(__instance.texture[3]);
        }
    }
}