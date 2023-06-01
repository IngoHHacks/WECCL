using System.Text.RegularExpressions;
using WECCL.Utils;
using CommandType = WECCL.Content.PromoData.AdvFeatures.CommandType;

namespace WECCL.Patches;

[HarmonyPatch]
internal class PromoPatch
{
    [HarmonyPatch(typeof(GameDialog), nameof(GameDialog.CJNHDOHIKNJ))]
    [HarmonyPrefix]
    public static void GameDialog_CJNHDOHIKNJ()
    {
        var promoId = GameDialog.JJHCKDLBACE - 1000000;
        if (promoId < 0)
        {
            return;
        }
        var promo = PromoData[promoId];

        var page = GameDialog.BABHEGOMNLJ - 1;
        if (page >= promo.NumLines)
        {
            GameDialog.JJHCKDLBACE = 0;
        }
        else
        {
            ExecutePromoLine(promo.PromoLines[page].Line1, promo.PromoLines[page].Line2, promo.PromoLines[page].From, promo.PromoLines[page].To, promo.PromoLines[page].Demeanor, promo.PromoLines[page].TauntAnim);
        }
        if (GameDialog.GKDOOPDCBMD >= 100f && GameDialog.KOJMLOEJKCN < GameDialog.BABHEGOMNLJ)
        {
            if (promo.PromoLines[page].Features != null)
            {
                foreach (var feature in promo.PromoLines[page].Features)
                {
                    var cmd = feature.Command;
                    switch (cmd)
                    {
                        case CommandType.SetFace:
                            GameDialog.GNNMHIENJIA[int.Parse(feature.Args[0])].MFHHNDICNEF(0);
                            break;
                        case CommandType.SetHeel:
                            GameDialog.GNNMHIENJIA[int.Parse(feature.Args[0])].MFHHNDICNEF(-1);
                            break;
                        case CommandType.SetRealEnemy:
                            GameDialog.GNNMHIENJIA[int.Parse(feature.Args[0])]
                                .NHOANJHPFEE(GameDialog.GNNMHIENJIA[int.Parse(feature.Args[1])].id, -1, 0);
                            break;
                        case CommandType.SetStoryEnemy:
                            GameDialog.GNNMHIENJIA[int.Parse(feature.Args[0])]
                                .NHOANJHPFEE(GameDialog.GNNMHIENJIA[int.Parse(feature.Args[1])].id, -1);
                            break;
                        case CommandType.SetRealFriend:
                            GameDialog.GNNMHIENJIA[int.Parse(feature.Args[0])]
                                .NHOANJHPFEE(GameDialog.GNNMHIENJIA[int.Parse(feature.Args[1])].id, 1, 0);
                            break;
                        case CommandType.SetStoryFriend:
                            GameDialog.GNNMHIENJIA[int.Parse(feature.Args[0])]
                                .NHOANJHPFEE(GameDialog.GNNMHIENJIA[int.Parse(feature.Args[1])].id, 1);
                            break;
                        case CommandType.SetRealNeutral:
                            GameDialog.GNNMHIENJIA[int.Parse(feature.Args[0])]
                                .NHOANJHPFEE(GameDialog.GNNMHIENJIA[int.Parse(feature.Args[1])].id, 0, 0);
                            break;
                        case CommandType.SetStoryNeutral:
                            GameDialog.GNNMHIENJIA[int.Parse(feature.Args[0])]
                                .NHOANJHPFEE(GameDialog.GNNMHIENJIA[int.Parse(feature.Args[1])].id, 0);
                            break;
                        case CommandType.PlayAudio:
                            if (feature.Args[0] == "-1")
                            {
                                GameAudio.HPICKPAJOKP(GameDialog.PDMDFGNJCPN, -1, 1f);
                            }
                            else
                            {
                                GameAudio.NCOKFNDGDME.PlayOneShot(
                                    GameAudio.JPOFBDKBOMB[Indices.ParseCrowdAudio(feature.Args[0])], 1);
                            }
                            break;
                    }
                }
            }
            
            GameDialog.KOJMLOEJKCN = GameDialog.BABHEGOMNLJ;
        }

    }

    private static void ExecutePromoLine(string line1, string line2, int from, int to, float demeanor, int taunt)
    {
        line1 = ReplaceVars(line1);
        line2 = ReplaceVars(line2);
        
        GameDialog.IDGEDBDFPCK(GameDialog.CABJFFKNOGN[from], GameDialog.CABJFFKNOGN[to], demeanor, taunt);
        GameDialog.ODIDPPCKKJF[1] = line1;
        GameDialog.ODIDPPCKKJF[2] = line2;
    }

    private static string ReplaceVars(string line)
    {
        // Special case for $name#
        var matches = Regex.Matches(line, @"\$([a-zA-Z]+)(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            try
            {
                var varName = match.Groups[1].Value;
                var varIndex = int.Parse(match.Groups[2].Value);
                var test = "";
                var varValue = varName switch
                {
                    "name" => GameDialog.GNNMHIENJIA[varIndex].name,
                    "promotion" => GameDialog.AOOGKCAICMI[varIndex].name,
                    _ => "UNKNOWN"
                };

                line = line.Replace(match.Value, varValue + match.Groups[3].Value);
            }
            catch (Exception e)
            {
                line = line.Replace(match.Value, "INVALID");
                Plugin.Log.LogError(e);
            }
        }
        matches = Regex.Matches(line, @"\$\$([a-zA-Z]+)(\d+)_(\d+)(\W|$)"); //probably can simplify and merge with the $name
        foreach (Match match in matches)
        {
            try
            {
                var varName = match.Groups[1].Value;
                var varIndex1 = int.Parse(match.Groups[2].Value);
                var varIndex2 = int.Parse(match.Groups[3].Value);
                var varValue = varName switch
                {
                    "belt" => GameDialog.AOOGKCAICMI[varIndex1].beltName[varIndex2],
                    "champ" => Characters.c[GameDialog.AOOGKCAICMI[varIndex1].champ[varIndex2, 1]]
                        .name //1 - current champ, then 2 - previous?
                    ,
                    _ => "UNKNOWN"
                };

                line = line.Replace(match.Value, varValue + match.Groups[4].Value);
            }
            catch (Exception e)
            {
                line = line.Replace(match.Value, "INVALID");
                Plugin.Log.LogError(e);
            }
        }
        matches = Regex.Matches(line, @"@([a-zA-Z]+)(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            var varName = match.Groups[1].Value;
            var varIndex = int.Parse(match.Groups[2].Value);
            var varValue = GameDialog.GNNMHIENJIA[varIndex].LDIOHGBMLHB(varName);
            
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
        GameDialog.JKAAAHLMLKN++;
        Array.Resize(ref GameDialog.KOPNJGECPIA, GameDialog.JKAAAHLMLKN + 1);
        ResizeArray(ref GameDialog.MFDNNCOMBBN, GameDialog.JKAAAHLMLKN + 1, Math.Max(40, PromoData.Count));
        Array.Resize(ref GameDialog.KJBBEFOFGIP, GameDialog.JKAAAHLMLKN + 1);
        GameDialog.KJBBEFOFGIP[GameDialog.JKAAAHLMLKN] = PromoData.Count;
        GameDialog.KOPNJGECPIA[GameDialog.JKAAAHLMLKN] = "Custom";
        GameDialog.MFDNNCOMBBN[GameDialog.JKAAAHLMLKN, 0] = 0;
        for (int i = 0; i < PromoData.Count; i++)
        {
            GameDialog.MFDNNCOMBBN[GameDialog.JKAAAHLMLKN, i + 1] = 1000000 + i;
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
    
    
    [HarmonyPatch(typeof(GameDialog), nameof(GameDialog.IPEMHNGGINI))]
    [HarmonyPostfix]
    public static void GameDialog_IPEMHNGGINI(int PLAEOMODMCI)
    {
        if (PLAEOMODMCI < 1000000)
        {
            return;
        }
        var index = PLAEOMODMCI - 1000000;

        GameDialog.AABGEEFANFM = PromoData[index].Title;
        GameDialog.DKLBGLEAEJC = PromoData[index].Description;
        GameDialog.ALFDHAOKHGN = PromoData[index].Characters;
    }
}