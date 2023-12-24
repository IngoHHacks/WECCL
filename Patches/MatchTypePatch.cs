using UnityEngine.SceneManagement;
using WECCL.API;

namespace WECCL.Patches;

[HarmonyPatch]
public class MatchTypePatch
{
    private static float OldPresetMin;
    private static float OldCageMax;
    private static float OldCageMin;
   
    /*
     * Patch:
     * - Adds custom match types from the selection menu.
     */
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.ODONMLDCHHF))]
    [HarmonyPrefix]
    private static void Menu_ODONMLDCHHF_Pre1(UnmappedMenu __instance, float __result, ref float CAAJBNHEFJJ, float OEGLNPMNEOE, float NOGFHHECJBM, ref float JBPAELFIDOP, ref float LKBOHHGFJFO, int LKJHMOHMKCM)
    {
        if (SceneManager.GetActiveScene().name != "Match_Setup") return;
        if (LIPNHOMGGHF.CHLJMEPFJOK == 2)
        {
            if (LIPNHOMGGHF.FKANHDIMMBJ[1] == __instance)
            {
                OldPresetMin = JBPAELFIDOP;
                JBPAELFIDOP -= CustomMatch.CustomPresetsNeg.Count;
                if (CAAJBNHEFJJ <= -10000)
                {
                    CAAJBNHEFJJ = CAAJBNHEFJJ + 10000 - OldPresetMin;
                }
            }
        }    
    }
    
    /*
     * Patch:
     * - Second patch for custom match types from the selection menu.
     */
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.ODONMLDCHHF))]
    [HarmonyPostfix]
    private static void Menu_ODONMLDCHHF_Post1(UnmappedMenu __instance, ref float __result, float CAAJBNHEFJJ, float OEGLNPMNEOE, float NOGFHHECJBM, ref float JBPAELFIDOP, ref float LKBOHHGFJFO, int LKJHMOHMKCM)
    {
        if (SceneManager.GetActiveScene().name != "Match_Setup") return;
        if (LIPNHOMGGHF.CHLJMEPFJOK == 2)
        {
            if (LIPNHOMGGHF.FKANHDIMMBJ[1] == __instance)
            {
                if (__result < OldPresetMin) __result = -10000 - (OldPresetMin - __result);
            }
        }
    }
    
    /*
     * Patch:
     * - Adds custom cages to the selection menu.
     */
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.ODONMLDCHHF))]
    [HarmonyPrefix]
    private static void Menu_ODONMLDCHHF_Pre2(UnmappedMenu __instance, float __result, ref float CAAJBNHEFJJ, float OEGLNPMNEOE, float NOGFHHECJBM, ref float JBPAELFIDOP, ref float LKBOHHGFJFO, int LKJHMOHMKCM)
    {
        if (SceneManager.GetActiveScene().name != "Match_Setup") return;
        if (LIPNHOMGGHF.CHLJMEPFJOK == 1 && LIPNHOMGGHF.ODOAPLMOJPD == 1)
        {
            if (LIPNHOMGGHF.FKANHDIMMBJ[9] == __instance)
            {
                ChangeHardcodedPrefix(ref OldCageMin, ref JBPAELFIDOP, CustomMatch.CustomCagesNeg.Count, ref OldCageMax, ref LKBOHHGFJFO, CustomMatch.CustomCagesPos.Count, ref CAAJBNHEFJJ);
            }
        }
    }
    
    /*
     * Patch:
     * - Second patch for custom cages to the selection menu.
     */
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.ODONMLDCHHF))]
    [HarmonyPostfix]
    private static void Menu_ODONMLDCHHF_Post2(UnmappedMenu __instance, ref float __result, float CAAJBNHEFJJ, float OEGLNPMNEOE, float NOGFHHECJBM, ref float JBPAELFIDOP, ref float LKBOHHGFJFO, int LKJHMOHMKCM)
    {
        if (SceneManager.GetActiveScene().name != "Match_Setup") return;
        if (LIPNHOMGGHF.CHLJMEPFJOK == 1 && LIPNHOMGGHF.ODOAPLMOJPD == 1)
        {
            if (LIPNHOMGGHF.FKANHDIMMBJ[9] == __instance)
            {
                ChangeHardcodedPostfix(ref __result, OldCageMin, OldCageMax);
            }
        }
    }
    private static void ChangeHardcodedPrefix(ref float OldMin, ref float GameMin, int NegativesCount, ref float OldMax, ref float GameMax, int PositivesCount, ref float CurrentValue)
    {
        OldMax = GameMax;
        OldMin = GameMin;
        GameMin -= NegativesCount;
        GameMax += PositivesCount;
        if (CurrentValue >= 10000)
        {
            CurrentValue = CurrentValue - 10000 + OldMax;
        }
        if (CurrentValue <= -10000)
        {
            CurrentValue = CurrentValue + 10000 - OldMin;
        }
    }
    private static void ChangeHardcodedPostfix(ref float __result, float OldMin, float OldMax)
    {
        if (__result > OldMax)
        {
            __result = 10000 + (__result - OldMax);
        }
        if (__result < OldMin) __result = -10000 - (OldMin - __result);
    }
}