using System.Text.RegularExpressions;

namespace WECCL.Patches;

[HarmonyPatch]
internal class PromoPatch
{
    [HarmonyPatch(typeof(HJBJPCDFLGL), "EMFEHLOHNEC")]
    [HarmonyPrefix]
    public static void HJBJPCDFLGL_EMFEHLOHNEC()
    {
        var promoId = HJBJPCDFLGL.GPOMGOFJBBJ - 1000000;
        if (promoId < 0)
        {
            return;
        }
        var promo = PromoData[promoId];

        var page = HJBJPCDFLGL.OKILOINLLAO - 1;
        if (page >= promo.NumLines)
        {
            HJBJPCDFLGL.GPOMGOFJBBJ = 0;
        }
        else
        {
            ExecutePromoLine(promo.PromoLines[page].Line1, promo.PromoLines[page].Line2, promo.PromoLines[page].From, promo.PromoLines[page].To, promo.PromoLines[page].Demeanor, promo.PromoLines[page].TauntAnim);
        }
        if (HJBJPCDFLGL.IAOODIBGNFI >= 100f && HJBJPCDFLGL.PHLNGBLJPCD < HJBJPCDFLGL.OKILOINLLAO)
        {
            CKAMIAJJDBP.LLEOPJCKBNE(HJBJPCDFLGL.NAGCDENHJNE, -1, 1f);
            HJBJPCDFLGL.PHLNGBLJPCD = HJBJPCDFLGL.OKILOINLLAO;
        }

    }

    private static void ExecutePromoLine(string line1, string line2, int from, int to, float demeanor, int taunt)
    {
        line1 = ReplaceVars(line1);
        line2 = ReplaceVars(line2);
        
        HJBJPCDFLGL.PCBHDGIILMM(HJBJPCDFLGL.ACCCNCAHDNO[from], HJBJPCDFLGL.ACCCNCAHDNO[to], demeanor, taunt);
        HJBJPCDFLGL.PLPGONCJOMC[1] = line1;
        HJBJPCDFLGL.PLPGONCJOMC[2] = line2;
    }

    private static string ReplaceVars(string line)
    {
        // Special case for $name#
        var matches = Regex.Matches(line, @"\$([a-zA-Z]+)(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            var varName = match.Groups[1].Value;
            var varIndex = int.Parse(match.Groups[2].Value);
            var varValue = "";
            switch (varName)
            {
                case "name":
                    varValue = HJBJPCDFLGL.LFMMJKBFOHO[varIndex].name;
                    break;
            }

            line = line.Replace(match.Value, varValue + match.Groups[3].Value);
        }
        matches = Regex.Matches(line, @"@([a-zA-Z]+)(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            var varName = match.Groups[1].Value;
            var varIndex = int.Parse(match.Groups[2].Value);
            var varValue = HJBJPCDFLGL.LFMMJKBFOHO[varIndex].DBFECDPDIFN(varName);
            
            line = line.Replace(match.Value, varValue + match.Groups[3].Value);
        }
        return line;
    }
    
    public static void PatchPromoInfo()
    {
        if (PromoData.Count == 0)
        {
            return;
        }
        HJBJPCDFLGL.IMNHBANDBMM++;
        Array.Resize(ref HJBJPCDFLGL.IPIGJMJCNAM, HJBJPCDFLGL.IMNHBANDBMM + 1);
        ResizeArray(ref HJBJPCDFLGL.EMOGCGOONAA, HJBJPCDFLGL.IMNHBANDBMM + 1, Math.Max(40, PromoData.Count));
        Array.Resize(ref HJBJPCDFLGL.NADPMKIMOCE, HJBJPCDFLGL.IMNHBANDBMM + 1);
        HJBJPCDFLGL.NADPMKIMOCE[HJBJPCDFLGL.IMNHBANDBMM] = PromoData.Count;
        HJBJPCDFLGL.IPIGJMJCNAM[HJBJPCDFLGL.IMNHBANDBMM] = "Custom";
        HJBJPCDFLGL.EMOGCGOONAA[HJBJPCDFLGL.IMNHBANDBMM, 0] = 0;
        for (int i = 0; i < PromoData.Count; i++)
        {
            HJBJPCDFLGL.EMOGCGOONAA[HJBJPCDFLGL.IMNHBANDBMM, i + 1] = 1000000 + i;
        }
    }

    
    internal static void ResizeArray<T>(ref T[,] original, int cols, int rows)
    {
        var newArray = new T[cols,rows];
        int columnCount = original.GetLength(1);
        int columnCount2 = rows;
        int columns = original.GetUpperBound(0);
        for (int col = 0; col <= columns; col++)
            Array.Copy(original, col * columnCount, newArray, col * columnCount2, columnCount);
        original = newArray;
    }
    
    
    [HarmonyPatch(typeof(HJBJPCDFLGL), "CHACHKONPAI")]
    [HarmonyPostfix]
    public static void HJBJPCDFLGL_CHACHKONPAI(int POMLANJPIML)
    {
        if (POMLANJPIML < 1000000)
        {
            return;
        }
        var index = POMLANJPIML - 1000000;

        HJBJPCDFLGL.IBOAEEIDBGL = PromoData[index].Title;
        HJBJPCDFLGL.KAEKKEABJBN = PromoData[index].Description;
        HJBJPCDFLGL.NNKDFAJFGMC = PromoData[index].Characters;
    }
}