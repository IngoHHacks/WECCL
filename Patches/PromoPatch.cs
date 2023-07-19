using System.Text.RegularExpressions;
using WECCL.Content;
using PromoData = WECCL.Content.PromoData;

namespace WECCL.Patches;

[HarmonyPatch]
internal class PromoPatch
{
    [HarmonyPatch(typeof(GameDialog), nameof(GameDialog.CJNHDOHIKNJ))]
    [HarmonyPrefix]
    public static void GameDialog_CJNHDOHIKNJ()
    {
        int promoId = GameDialog.JJHCKDLBACE - 1000000;
        if (promoId < 0)
        {
            return;
        }

        PromoData promo = CustomContent.PromoData[promoId];

        int page = GameDialog.BABHEGOMNLJ - 1;
        if (page >= promo.NumLines)
        {
            GameDialog.JJHCKDLBACE = 0;
        }
        else
        {
            if (promo.UseCharacterNames)
            {
                if (!promo.NameToID.TryGetValue(promo.PromoLines[page].FromName, out int fromid))
                {
                    fromid = 0;
                }
                if (!promo.NameToID.TryGetValue(promo.PromoLines[page].ToName, out int toid))
                {
                    toid = 0;
                }
                ExecutePromoLine(promo.PromoLines[page].Line1, promo.PromoLines[page].Line2, fromid,
                    toid, promo.PromoLines[page].Demeanor, promo.PromoLines[page].TauntAnim, true);
            }
            else
            {
                ExecutePromoLine(promo.PromoLines[page].Line1, promo.PromoLines[page].Line2, promo.PromoLines[page].From,
                    promo.PromoLines[page].To, promo.PromoLines[page].Demeanor, promo.PromoLines[page].TauntAnim, false);
            }
        }

        if (GameDialog.GKDOOPDCBMD >= 100f && GameDialog.KOJMLOEJKCN < GameDialog.BABHEGOMNLJ)
        {
            if (promo.PromoLines[page].Features != null)
            {
                foreach (PromoData.AdvFeatures feature in promo.PromoLines[page].Features)
                {
                    PromoData.AdvFeatures.CommandType cmd = feature.Command;
                    switch (cmd)
                    {
                        case PromoData.AdvFeatures.CommandType.SetFace:
                            promo.GetCharacterForCmd(feature.Args[0]).MFHHNDICNEF(0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetHeel:
                            promo.GetCharacterForCmd(feature.Args[0]).MFHHNDICNEF(-1);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetRealEnemy:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .NHOANJHPFEE(promo.GetCharacterForCmd(feature.Args[1]).id, -1, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetStoryEnemy:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .NHOANJHPFEE(promo.GetCharacterForCmd(feature.Args[1]).id, -1);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetRealFriend:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .NHOANJHPFEE(promo.GetCharacterForCmd(feature.Args[1]).id, 1, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetStoryFriend:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .NHOANJHPFEE(promo.GetCharacterForCmd(feature.Args[1]).id, 1);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetRealNeutral:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .NHOANJHPFEE(promo.GetCharacterForCmd(feature.Args[1]).id, 0, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetStoryNeutral:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .NHOANJHPFEE(promo.GetCharacterForCmd(feature.Args[1]).id, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.PlayAudio:
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
    private static void ExecutePromoLine(string line1, string line2, int from, int to, float demeanor, int taunt, bool useNames)
    {
        line1 = ReplaceVars(line1);
        line2 = ReplaceVars(line2);
        if(useNames)
        {
            GameDialog.IDGEDBDFPCK(from, to, demeanor, taunt);
        }
        else
        {
            GameDialog.IDGEDBDFPCK(GameDialog.CABJFFKNOGN[from], GameDialog.CABJFFKNOGN[to], demeanor, taunt);
        }

        GameDialog.ODIDPPCKKJF[1] = line1;
        GameDialog.ODIDPPCKKJF[2] = line2;
    }

    private static string ReplaceVars(string line)
    {
        // Special case for $name#
        MatchCollection matches = Regex.Matches(line, @"\$([a-zA-Z]+)(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            try
            {
                string varName = match.Groups[1].Value;
                int varIndex = int.Parse(match.Groups[2].Value);
                string test = "";
                string varValue = varName switch
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

        matches = Regex.Matches(line,
            @"\$\$([a-zA-Z]+)(\d+)_(\d+)(\W|$)"); //probably can simplify and merge with the $name
        foreach (Match match in matches)
        {
            try
            {
                string varName = match.Groups[1].Value;
                int varIndex1 = int.Parse(match.Groups[2].Value);
                int varIndex2 = int.Parse(match.Groups[3].Value);
                string varValue = varName switch
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
            string varName = match.Groups[1].Value;
            int varIndex = int.Parse(match.Groups[2].Value);
            string varValue = GameDialog.GNNMHIENJIA[varIndex].LDIOHGBMLHB(varName);

            line = line.Replace(match.Value, varValue + match.Groups[3].Value);
        }

        return line;
    }

    public static void PatchPromoInfo()
    {
        if (CustomContent.PromoData.Count == 0)
        {
            return;
        }

        GameDialog.JKAAAHLMLKN++;
        Array.Resize(ref GameDialog.KOPNJGECPIA, GameDialog.JKAAAHLMLKN + 1);
        ResizeArray(ref GameDialog.MFDNNCOMBBN, GameDialog.JKAAAHLMLKN + 1,
            Math.Max(40, CustomContent.PromoData.Count));
        Array.Resize(ref GameDialog.KJBBEFOFGIP, GameDialog.JKAAAHLMLKN + 1);
        GameDialog.KJBBEFOFGIP[GameDialog.JKAAAHLMLKN] = CustomContent.PromoData.Count;
        GameDialog.KOPNJGECPIA[GameDialog.JKAAAHLMLKN] = "Custom";
        GameDialog.MFDNNCOMBBN[GameDialog.JKAAAHLMLKN, 0] = 0;
        for (int i = 0; i < CustomContent.PromoData.Count; i++)
        {
            GameDialog.MFDNNCOMBBN[GameDialog.JKAAAHLMLKN, i + 1] = 1000000 + i;
        }
    }


    internal static void ResizeArray<T>(ref T[,] original, int cols, int rows)
    {
        T[,] newArray = new T[cols, rows];
        int columnCount = original.GetLength(1);
        int columnCount2 = rows;
        int columns = original.GetUpperBound(0);
        for (int col = 0; col <= columns; col++)
        {
            Array.Copy(original, col * columnCount, newArray, col * columnCount2, columnCount);
        }

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

        int index = PLAEOMODMCI - 1000000;

        GameDialog.AABGEEFANFM = CustomContent.PromoData[index].Title;
        GameDialog.DKLBGLEAEJC = CustomContent.PromoData[index].Description;
        GameDialog.ALFDHAOKHGN = CustomContent.PromoData[index].Characters;
    }

    [HarmonyPatch(typeof(AMJONEKIAID), nameof(AMJONEKIAID.PFGKPCEKOLJ))]
    [HarmonyPostfix]
    public static void AMJONEKIAID_PFGKPCEKOLJ()  //setting up surprises
    {
        if (ONACPDNNNMM.PLAEOMODMCI < 1000000 || DNDIEGNJOKN.OBNLIIMODBI != 50)
        {
            return;
        }

        int index = ONACPDNNNMM.PLAEOMODMCI - 1000000;
        CustomContent.PromoData[index].NameToID = new();
        CustomContent.PromoData[index].SurpirseExtras = new();
        for (int i = 1; i < AMJONEKIAID.NCPIJJFEDFL.Length; i++)
        {
            GMIKIMHFABP person = AMJONEKIAID.NCPIJJFEDFL[i];
            if (CustomContent.PromoData[index].UseCharacterNames)
            {
                CustomContent.PromoData[index].NameToID.Add(person.IPNKFGHIDJP.name, i);
            }

            if(CustomContent.PromoData[index].IsCharacterSurprise(person))
            {
                RespawnSurprise(person);
            }
            if (CustomContent.PromoData[index].IsCharacterSurprise(AMJONEKIAID.NCPIJJFEDFL[person.AKDLEIDAHFL])) //person they are managing
            {
                CustomContent.PromoData[index].SurpirseExtras.Add(i);
                RespawnTogetherWith(person, AMJONEKIAID.NCPIJJFEDFL[person.AKDLEIDAHFL]);
            }
        }
    }
    public static void RespawnSurprise(GMIKIMHFABP __instance)
    {
            __instance.FLFDNLEILGC = -1;  //setting surprise entrants
            DIAOCEFCCAE diaocefccae;
            int num2;
            do
            {
                diaocefccae = AKHBGBPEJHB.LJFPLPEHCPK[JGKBBDPDIBC.OCMIPAODMHH(1, AKHBGBPEJHB.AOONPPMPJAD, 0)]; //resetting their spawns in case they spawn in ring
                num2 = 1;
                if (World.location == 9 && diaocefccae.OEACALPGDOM() > 50f)
                {
                    num2 = 0;
                }
                if (diaocefccae.FLFDNLEILGC == 0)
                {
                    num2 = 0;
                }
            }
            while (num2 == 0);
            __instance.PPFFBIPHOEE = diaocefccae.GPKCOKPBJHB();
            __instance.OIHBMKLFEBJ = diaocefccae.OEACALPGDOM();
            __instance.MPPIDPBCCEM = diaocefccae.LHBBEOPJHHD;
            __instance.PBFJIDAPJGL = World.FOOLMKOOCGH(__instance.PPFFBIPHOEE, __instance.EDHBIOFAKNL, __instance.OIHBMKLFEBJ);
            if (AKHBGBPEJHB.BGJJHFDHLEP(__instance.PPFFBIPHOEE, __instance.OIHBMKLFEBJ, 0f) > 0)
            {
                __instance.PBFJIDAPJGL = World.ringGround;
            }
            __instance.EDHBIOFAKNL = __instance.PBFJIDAPJGL;
            if (__instance.EDHBIOFAKNL < __instance.PBFJIDAPJGL)
            {
                __instance.EDHBIOFAKNL = __instance.PBFJIDAPJGL;
            }
            __instance.AOAEIHIOPIG = 0;  //seat
            __instance.CBLJCJMAPGH[0].SetActive(false);
            __instance.PPOLHLIGBMP.SetActive(false);
    }
    public static void RespawnTogetherWith(GMIKIMHFABP manager, GMIKIMHFABP wrestler)
    {
        manager.FLFDNLEILGC = -1;
        manager.AOAEIHIOPIG = 0;
        manager.PPFFBIPHOEE = wrestler.PPFFBIPHOEE;
        manager.OIHBMKLFEBJ = wrestler.OIHBMKLFEBJ;
        manager.MPPIDPBCCEM = wrestler.MPPIDPBCCEM;
        manager.PBFJIDAPJGL = wrestler.PBFJIDAPJGL;
        manager.EDHBIOFAKNL = wrestler.EDHBIOFAKNL;
        manager.CBLJCJMAPGH[0].SetActive(false);
        manager.PPOLHLIGBMP.SetActive(false);

    }

    [HarmonyPatch(typeof(ONACPDNNNMM), nameof(ONACPDNNNMM.KHCNKEAGNPP))]
    [HarmonyPostfix]
    public static void ONACPDNNNMM_KHCNKEAGNPP(string MONBHNGICKE, string CIJMKHIGLFG = "", string BHKFLMJMIDL = "")  //mimicking "Guest Partner" promo behaviour to hide the names
    {
        if (Mathf.Abs(ONACPDNNNMM.PLAEOMODMCI) < 1000000 || DNDIEGNJOKN.OBNLIIMODBI != 50 || ONACPDNNNMM.ENHMOFPOBJL <= 0 || ONACPDNNNMM.JKAKKHJACHD == 0)
        {
            return;
        }
        int index = Mathf.Abs(ONACPDNNNMM.PLAEOMODMCI) - 1000000;
        if (CustomContent.PromoData[index].SurpriseEntrants.Count == 0 && CustomContent.PromoData[index].SurpriseEntrantsNames.Count == 0)
        {
            return;
        }
        GMIKIMHFABP character = AMJONEKIAID.NCPIJJFEDFL[ONACPDNNNMM.JKAKKHJACHD];
        int num = character.IPNKFGHIDJP.HIFIFLNMJHO(0);
        ONACPDNNNMM.BMPOAANONKJ[2].text = character.IPNKFGHIDJP.name;
        if (num > 0)
        {
            ONACPDNNNMM.BMPOAANONKJ[1].text = Characters.fedData[character.IPNKFGHIDJP.fed].beltName[num] + " Champion";
        }
    }

    [HarmonyPatch(typeof(ONACPDNNNMM), nameof(ONACPDNNNMM.EEJPINEJAFI))]
    [HarmonyPrefix]
    public static void ONACPDNNNMM_EEJPINEJAFI()  //releasing surprise entrants
    {
        if (ONACPDNNNMM.PLAEOMODMCI > -1000000)
        {
            return;
        }

        int index = -ONACPDNNNMM.PLAEOMODMCI - 1000000;
        if(CustomContent.PromoData[index].SurpriseEntrants.Count == 0 && CustomContent.PromoData[index].SurpriseEntrantsNames.Count == 0)
        {
            return;
        }

        if (CustomContent.PromoData[index].UseCharacterNames)
        {
            CustomContent.PromoData[index].NameToID.TryGetValue(CustomContent.PromoData[index].SurpriseEntrantsNames.First(), out int first);
            if (AMJONEKIAID.NCPIJJFEDFL[first].FLFDNLEILGC == -1)
            {
                foreach (string name in CustomContent.PromoData[index].SurpriseEntrantsNames)
                {
                    if (CustomContent.PromoData[index].NameToID.TryGetValue(name, out int id))
                    {
                        AMJONEKIAID.NCPIJJFEDFL[id].FLFDNLEILGC = 0;
                    }
                }
                foreach (int id in CustomContent.PromoData[index].SurpirseExtras)
                {
                    AMJONEKIAID.NCPIJJFEDFL[id].FLFDNLEILGC = 0;
                }
            }
        }
        else
        {
            if (AMJONEKIAID.NCPIJJFEDFL[CustomContent.PromoData[index].SurpriseEntrants.First()].FLFDNLEILGC == -1)
            {
                foreach (int id in CustomContent.PromoData[index].SurpriseEntrants)
                {
                    AMJONEKIAID.NCPIJJFEDFL[id].FLFDNLEILGC = 0;
                }
                foreach (int id in CustomContent.PromoData[index].SurpirseExtras)
                {
                    AMJONEKIAID.NCPIJJFEDFL[id].FLFDNLEILGC = 0;
                }
            }
        }
    }
}