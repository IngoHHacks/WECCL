using System.Text.RegularExpressions;
using CommandType = WECCL.Content.PromoData.AdvFeatures.CommandType;

namespace WECCL.Patches;

[HarmonyPatch]
internal class PromoPatch
{
    [HarmonyPatch(typeof(GameDialog), nameof(GameDialog.OIAOBMLCPMA))]
    [HarmonyPrefix]
    public static void GameDialog_OIAOBMLCPMA()
    {
        var promoId = GameDialog.PEIMNLOEAED - 1000000;
        if (promoId < 0)
        {
            return;
        }
        var promo = PromoData[promoId];

        var page = GameDialog.AGAGHGBLCDA - 1;
        if (page >= promo.NumLines)
        {
            GameDialog.PEIMNLOEAED = 0;
        }
        else
        {
            ExecutePromoLine(promo.PromoLines[page].Line1, promo.PromoLines[page].Line2, promo.PromoLines[page].From, promo.PromoLines[page].To, promo.PromoLines[page].Demeanor, promo.PromoLines[page].TauntAnim);
        }
        if (GameDialog.AHKCECADCAM >= 100f && GameDialog.KJAOOKABIFM < GameDialog.AGAGHGBLCDA)
        {
            foreach (var feature in promo.PromoLines[page].Features)
            {
                var cmd = feature.Command;
                switch (cmd)
                {
                    case CommandType.SetFace:
                        GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[0])].DFELOGFFCKL(0);
                        break;
                    case CommandType.SetHeel:
                        GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[0])].DFELOGFFCKL(-1);
                        break;
                    case CommandType.SetRealEnemy:
                        GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[0])].DJPDDHKHGDL(GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[1])].id, -1, 0);
                        break;
                    case CommandType.SetStoryEnemy:
                        GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[0])].DJPDDHKHGDL(GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[1])].id, -1);
                        break;
                    case CommandType.SetRealFriend:
                        GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[0])].DJPDDHKHGDL(GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[1])].id, 1, 0);
                        break;
                    case CommandType.SetStoryFriend:
                        GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[0])].DJPDDHKHGDL(GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[1])].id, 1);
                        break;
                    case CommandType.SetRealNeutral:
                        GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[0])].DJPDDHKHGDL(GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[1])].id, 0, 0);
                        break;
                    case CommandType.SetStoryNeutral:
                        GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[0])].DJPDDHKHGDL(GameDialog.OBMBDIEGBOK[int.Parse(feature.Args[1])].id, 0);
                        break;
                    case CommandType.PlayAudio:
                        GameAudio.KALAKNIDPKO.PlayOneShot(GameAudio.LEMKMADBAHL[int.Parse(feature.Args[0])], 1);
                        break;
                }
            }
            GameAudio.KICNMIIFKIC(GameDialog.DNNAOLIENKK, -1, 1f);
            GameDialog.KJAOOKABIFM = GameDialog.AGAGHGBLCDA;
        }

    }

    private static void ExecutePromoLine(string line1, string line2, int from, int to, float demeanor, int taunt)
    {
        line1 = ReplaceVars(line1);
        line2 = ReplaceVars(line2);
        
        GameDialog.LGMALDIJNNC(GameDialog.OKDNAFEFJBB[from], GameDialog.OKDNAFEFJBB[to], demeanor, taunt);
        GameDialog.HPCGCFCFBLO[1] = line1;
        GameDialog.HPCGCFCFBLO[2] = line2;
    }

    private static string ReplaceVars(string line)
    {
        // Special case for $name#
        var matches = Regex.Matches(line, @"\$([a-zA-Z]+)(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            var varName = match.Groups[1].Value;
            var varIndex = int.Parse(match.Groups[2].Value);
            var test = "";
            var varValue = "";
            switch (varName)
            {
                case "name":
                    varValue = GameDialog.OBMBDIEGBOK[varIndex].name;
                    break;
                case "promotion":
                    varValue = GameDialog.KLADONJKEHO[varIndex].name;
                    break;
            }

            line = line.Replace(match.Value, varValue + match.Groups[3].Value);
        }
        matches = Regex.Matches(line, @"#([a-zA-Z]+)(\d+)_(\d+)(\W|$)"); //probably can simplify and merge with the $name
        foreach (Match match in matches)
        {
            var varName = match.Groups[1].Value;
            var varIndex1 = int.Parse(match.Groups[2].Value);
            var varIndex2 = int.Parse(match.Groups[3].Value);
            var varValue = "";
            switch (varName)
            {
                case "belt":
                    varValue = GameDialog.KLADONJKEHO[varIndex1].beltName[varIndex2];
                    break;
                case "champ":
                    varValue = Characters.c[GameDialog.KLADONJKEHO[varIndex1].champ[varIndex2, 1]].name;  //1 - current champ, then 2 - previous?
                    break;
            }
            line = line.Replace(match.Value, varValue + match.Groups[4].Value);
        }
        matches = Regex.Matches(line, @"@([a-zA-Z]+)(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            var varName = match.Groups[1].Value;
            var varIndex = int.Parse(match.Groups[2].Value);
            var varValue = GameDialog.OBMBDIEGBOK[varIndex].BMKFIPMAICK(varName);
            
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
        GameDialog.BILCJNLECHK++;
        Array.Resize(ref GameDialog.BOMNNLKCEIP, GameDialog.BILCJNLECHK + 1);
        ResizeArray(ref GameDialog.BGDOGFPDOPK, GameDialog.BILCJNLECHK + 1, Math.Max(40, PromoData.Count));
        Array.Resize(ref GameDialog.FHBOMMGABMM, GameDialog.BILCJNLECHK + 1);
        GameDialog.FHBOMMGABMM[GameDialog.BILCJNLECHK] = PromoData.Count;
        GameDialog.BOMNNLKCEIP[GameDialog.BILCJNLECHK] = "Custom";
        GameDialog.BGDOGFPDOPK[GameDialog.BILCJNLECHK, 0] = 0;
        for (int i = 0; i < PromoData.Count; i++)
        {
            GameDialog.BGDOGFPDOPK[GameDialog.BILCJNLECHK, i + 1] = 1000000 + i;
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
    
    
    [HarmonyPatch(typeof(GameDialog), nameof(GameDialog.ILJJHNDOFBK))]
    [HarmonyPostfix]
    public static void GameDialog_ILJJHNDOFBK(int CBDCDJLMHFB)
    {
        if (CBDCDJLMHFB < 1000000)
        {
            return;
        }
        var index = CBDCDJLMHFB - 1000000;

        GameDialog.IEMHDAFJKAK = PromoData[index].Title;
        GameDialog.FOKOLBEDANF = PromoData[index].Description;
        GameDialog.NHPLKKIKMLI = PromoData[index].Characters;
    }
}