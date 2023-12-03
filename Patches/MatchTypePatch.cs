using UnityEngine.SceneManagement;
using WECCL.Content;
namespace WECCL.Patches;

[HarmonyPatch]
public class MatchTypePatch
{
    private static float OldPresetMax;
    private static float OldPresetMin;
    private static float OldCageMax;
    private static float OldCageMin;
   
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.ODONMLDCHHF))]
    [HarmonyPrefix]
    private static void CustomPresetPrefix(UnmappedMenu __instance, float __result, ref float CAAJBNHEFJJ, float OEGLNPMNEOE, float NOGFHHECJBM, ref float JBPAELFIDOP, ref float LKBOHHGFJFO, int LKJHMOHMKCM)
    {
        if (SceneManager.GetActiveScene().name != "Match_Setup") return;
        if (LIPNHOMGGHF.CHLJMEPFJOK == 2)
        {
            if (LIPNHOMGGHF.FKANHDIMMBJ[1] == __instance)
            {
                OldPresetMax = LKBOHHGFJFO;
                OldPresetMin = JBPAELFIDOP;
                JBPAELFIDOP -= CustomMatch.CustomPresetsNeg.Count;
                LKBOHHGFJFO += CustomMatch.CustomPresetsPos.Count;
                if (CAAJBNHEFJJ >= 10000)
                {
                    CAAJBNHEFJJ = CAAJBNHEFJJ - 10000 + OldPresetMax;
                }
                if (CAAJBNHEFJJ <= -10000)
                {
                    CAAJBNHEFJJ = CAAJBNHEFJJ + 10000 - OldPresetMin;
                }
            }
        }    
    }
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.ODONMLDCHHF))]
    [HarmonyPostfix]
    private static void CustomPresetPostfix(UnmappedMenu __instance, ref float __result, float CAAJBNHEFJJ, float OEGLNPMNEOE, float NOGFHHECJBM, ref float JBPAELFIDOP, ref float LKBOHHGFJFO, int LKJHMOHMKCM)
    {
        if (SceneManager.GetActiveScene().name != "Match_Setup") return;
        if (LIPNHOMGGHF.CHLJMEPFJOK == 2)
        {
            if (LIPNHOMGGHF.FKANHDIMMBJ[1] == __instance)
            {
                if (__result > OldPresetMax)
                {
                    __result = 10000 + (__result - OldPresetMax);
                }
                if (__result < OldPresetMin) __result = -10000 - (OldPresetMin - __result);
            }
        }
    }
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.ODONMLDCHHF))]
    [HarmonyPrefix]
    private static void CustomCagePrefix(UnmappedMenu __instance, float __result, ref float CAAJBNHEFJJ, float OEGLNPMNEOE, float NOGFHHECJBM, ref float JBPAELFIDOP, ref float LKBOHHGFJFO, int LKJHMOHMKCM)
    {
        if (SceneManager.GetActiveScene().name != "Match_Setup") return;
        if (LIPNHOMGGHF.CHLJMEPFJOK == 1 && LIPNHOMGGHF.ODOAPLMOJPD == 1)
        {
            if (LIPNHOMGGHF.FKANHDIMMBJ[9] == __instance)
            {
                OldCageMax = LKBOHHGFJFO;
                OldCageMin = JBPAELFIDOP;
                JBPAELFIDOP -= CustomMatch.CustomCagesNeg.Count;
                LKBOHHGFJFO += CustomMatch.CustomCagesPos.Count;
                if (CAAJBNHEFJJ >= 10000)
                {
                    CAAJBNHEFJJ = CAAJBNHEFJJ - 10000 + OldCageMax;
                }
                if (CAAJBNHEFJJ <= -10000)
                {
                    CAAJBNHEFJJ = CAAJBNHEFJJ + 10000 - OldCageMin;
                }
            }
        }
    }
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.ODONMLDCHHF))]
    [HarmonyPostfix]
    private static void CustomCagePostfix(UnmappedMenu __instance, ref float __result, float CAAJBNHEFJJ, float OEGLNPMNEOE, float NOGFHHECJBM, ref float JBPAELFIDOP, ref float LKBOHHGFJFO, int LKJHMOHMKCM)
    {
        if (SceneManager.GetActiveScene().name != "Match_Setup") return;
        if (LIPNHOMGGHF.CHLJMEPFJOK == 1 && LIPNHOMGGHF.ODOAPLMOJPD == 1)
        {
            if (LIPNHOMGGHF.FKANHDIMMBJ[9] == __instance)
            {
                if (__result > OldCageMax)
                {
                    __result = 10000 + (__result - OldCageMax);
                }
                if (__result < OldCageMin) __result = -10000 - (OldCageMin - __result);
            }
        }
    }
}