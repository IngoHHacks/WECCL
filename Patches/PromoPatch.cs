using System.Text.RegularExpressions;
using WECCL.Content;
using PromoData = WECCL.Content.PromoData;

namespace WECCL.Patches;

[HarmonyPatch]
internal class PromoPatch
{
    [HarmonyPatch(typeof(GameDialog), nameof(GameDialog.NOFIKEFIHFO))]
    [HarmonyPrefix]
    public static void GameDialog_NOFIKEFIHFO()
    {
        int promoId = GameDialog.JEEDCAOBBKN - 1000000;
        if (promoId < 0)
        {
            return;
        }

        PromoData promo = CustomContent.PromoData[promoId];

        int page = GameDialog.NJJPPLCPOIA - 1;
        if (page >= promo.NumLines)
        {
            GameDialog.JEEDCAOBBKN = 0;
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

        if (GameDialog.AMBGCJOBKFN >= 100f && GameDialog.IIMHNJLHJFM < GameDialog.NJJPPLCPOIA)
        {
            if (promo.PromoLines[page].Features != null)
            {
                foreach (PromoData.AdvFeatures feature in promo.PromoLines[page].Features)
                {
                    PromoData.AdvFeatures.CommandType cmd = feature.Command;
                    switch (cmd)
                    {
                        case PromoData.AdvFeatures.CommandType.SetFace:
                            promo.GetCharacterForCmd(feature.Args[0]).ICONKAPNPGL(0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetHeel:
                            promo.GetCharacterForCmd(feature.Args[0]).ICONKAPNPGL(-1);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetRealEnemy:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .EGHFNFMHOIL(promo.GetCharacterForCmd(feature.Args[1]).id, -1, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetStoryEnemy:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .EGHFNFMHOIL(promo.GetCharacterForCmd(feature.Args[1]).id, -1);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetRealFriend:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .EGHFNFMHOIL(promo.GetCharacterForCmd(feature.Args[1]).id, 1, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetStoryFriend:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .EGHFNFMHOIL(promo.GetCharacterForCmd(feature.Args[1]).id, 1);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetRealNeutral:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .EGHFNFMHOIL(promo.GetCharacterForCmd(feature.Args[1]).id, 0, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetStoryNeutral:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .EGHFNFMHOIL(promo.GetCharacterForCmd(feature.Args[1]).id, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.PlayAudio:
                            if (feature.Args[0] == "-1")
                            {
                                GameAudio.GNILNDHIFEG(GameDialog.CJGHFHCHDNN, -1, 1f);
                            }
                            else
                            {
                                GameAudio.IJDAPEMMDJC.PlayOneShot(
                                    GameAudio.NAGGJJNCEEK[Indices.ParseCrowdAudio(feature.Args[0])], 1);
                            }

                            break;
                    }
                }
            }

            GameDialog.IIMHNJLHJFM = GameDialog.NJJPPLCPOIA;
        }
    }
    private static void ExecutePromoLine(string line1, string line2, int from, int to, float demeanor, int taunt, bool useNames)
    {
        line1 = ReplaceVars(line1);
        line2 = ReplaceVars(line2);
        if(useNames)
        {
            GameDialog.GLHDGKIGFLG(from, to, demeanor, taunt);
        }
        else
        {
            GameDialog.GLHDGKIGFLG(GameDialog.FMGHIAMFFCJ[from], GameDialog.FMGHIAMFFCJ[to], demeanor, taunt);
        }

        GameDialog.PHAAJGLNMAP[1] = line1;
        GameDialog.PHAAJGLNMAP[2] = line2;
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
                    "name" => GameDialog.HGJNAEDAMDO[varIndex].name,
                    "promotion" => GameDialog.KHDBDEJBIAL[varIndex].name,
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
                    "belt" => GameDialog.KHDBDEJBIAL[varIndex1].beltName[varIndex2],
                    "champ" => Characters.c[GameDialog.KHDBDEJBIAL[varIndex1].champ[varIndex2, 1]]
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
            string varValue = GameDialog.HGJNAEDAMDO[varIndex].LCEFDFPALCJ(varName);

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

        GameDialog.LEKNPAAEBAE++;
        Array.Resize(ref GameDialog.JDJMBJHKNAB, GameDialog.LEKNPAAEBAE + 1);
        ResizeArray(ref GameDialog.KIGJPKBDCKK, GameDialog.LEKNPAAEBAE + 1,
            Math.Max(40, CustomContent.PromoData.Count));
        Array.Resize(ref GameDialog.OIDMOHCDEHK, GameDialog.LEKNPAAEBAE + 1);
        GameDialog.OIDMOHCDEHK[GameDialog.LEKNPAAEBAE] = CustomContent.PromoData.Count;
        GameDialog.JDJMBJHKNAB[GameDialog.LEKNPAAEBAE] = "Custom";
        GameDialog.KIGJPKBDCKK[GameDialog.LEKNPAAEBAE, 0] = 0;
        for (int i = 0; i < CustomContent.PromoData.Count; i++)
        {
            GameDialog.KIGJPKBDCKK[GameDialog.LEKNPAAEBAE, i + 1] = 1000000 + i;
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


    [HarmonyPatch(typeof(GameDialog), nameof(GameDialog.BHFAHIOEPFJ))]
    [HarmonyPostfix]
    public static void GameDialog_BHFAHIOEPFJ(int LPHFJGGHBED)
    {
        if (LPHFJGGHBED < 1000000)
        {
            return;
        }

        int index = LPHFJGGHBED - 1000000;

        GameDialog.CCFHFGBDIHE = CustomContent.PromoData[index].Title;
        GameDialog.EPPMEBCMEMD = CustomContent.PromoData[index].Description;
        GameDialog.LKEGBIECEGM = CustomContent.PromoData[index].Characters;
    }

    [HarmonyPatch(typeof(FFKMIEMAJML), nameof(FFKMIEMAJML.AICCEAHJDCB))]
    [HarmonyPostfix]
    public static void FFKMIEMAJML_AICCEAHJDCB()  //setting up surprises
    {
        if (PHECEOMIMND.LPHFJGGHBED < 1000000 || JJDCNALMPCI.AAAIDOOHBCM != 50)
        {
            return;
        }

        int index = PHECEOMIMND.LPHFJGGHBED - 1000000;
        CustomContent.PromoData[index].NameToID = new();
        CustomContent.PromoData[index].SurpirseExtras = new();
        for (int i = 1; i < FFKMIEMAJML.FJCOPECCEKN.Length; i++)
        {
            DJEKCMMMFJM person = FFKMIEMAJML.FJCOPECCEKN[i];
            if (CustomContent.PromoData[index].UseCharacterNames)
            {
                CustomContent.PromoData[index].NameToID.Add(person.LLEGGMCIALJ.name, i);
            }

            if(CustomContent.PromoData[index].IsCharacterSurprise(person))
            {
                RespawnSurprise(person);
            }
            if (CustomContent.PromoData[index].IsCharacterSurprise(FFKMIEMAJML.FJCOPECCEKN[person.KFHDEANBHDB])) //person they are managing
            {
                CustomContent.PromoData[index].SurpirseExtras.Add(i);
                RespawnTogetherWith(person, FFKMIEMAJML.FJCOPECCEKN[person.KFHDEANBHDB]);
            }
        }
    }
    public static void RespawnSurprise(DJEKCMMMFJM __instance)
    {
            __instance.FKPIGOJCEAK = -1;  //setting surprise entrants
            ILPOGGNCJEN diaocefccae;
            int num2;
            do
            {
                diaocefccae = LFDENAEGJBJ.JNBNDGANKDE[LFNJDEGJLLJ.NBNFJOFFMHO(1, LFDENAEGJBJ.IGLHKCAMDKO, 0)]; //resetting their spawns in case they spawn in ring
                num2 = 1;
                if (World.location == 9 && diaocefccae.NCHCAJDNMKK() > 50f)
                {
                    num2 = 0;
                }
                if (diaocefccae.FKPIGOJCEAK == 0)
                {
                    num2 = 0;
                }
            }
            while (num2 == 0);
            __instance.DCLLKPILCBP = diaocefccae.JKJBCGJHCIG();
            __instance.FFEONFCEHDF = diaocefccae.NCHCAJDNMKK();
            __instance.PDEGHHLLFDG = diaocefccae.NAMDOACBNED;
            __instance.DOOCGGBPAFM = World.JMFAKOLINLF(__instance.DCLLKPILCBP, __instance.BEHMHIINOGM, __instance.FFEONFCEHDF);
            if (LFDENAEGJBJ.LLAGNIPIDOO(__instance.DCLLKPILCBP, __instance.FFEONFCEHDF, 0f) > 0)
            {
                __instance.DOOCGGBPAFM = World.ringGround;
            }
            __instance.BEHMHIINOGM = __instance.DOOCGGBPAFM;
            if (__instance.BEHMHIINOGM < __instance.DOOCGGBPAFM)
            {
                __instance.BEHMHIINOGM = __instance.DOOCGGBPAFM;
            }
            __instance.FCKMCPMNLJM = 0;  //seat
            __instance.OOPKPKCHBEN[0].SetActive(false);
            __instance.FPELIOPLHFE.SetActive(false);
    }
    public static void RespawnTogetherWith(DJEKCMMMFJM manager, DJEKCMMMFJM wrestler)
    {
        manager.FKPIGOJCEAK = -1;
        manager.FCKMCPMNLJM = 0;
        manager.DCLLKPILCBP = wrestler.DCLLKPILCBP;
        manager.FFEONFCEHDF = wrestler.FFEONFCEHDF;
        manager.PDEGHHLLFDG = wrestler.PDEGHHLLFDG;
        manager.DOOCGGBPAFM = wrestler.DOOCGGBPAFM;
        manager.BEHMHIINOGM = wrestler.BEHMHIINOGM;
        manager.OOPKPKCHBEN[0].SetActive(false);
        manager.FPELIOPLHFE.SetActive(false);

    }

    [HarmonyPatch(typeof(PHECEOMIMND), nameof(PHECEOMIMND.MPPIMFNMGJK))]
    [HarmonyPostfix]
    public static void PHECEOMIMND_MPPIMFNMGJK(string BEBDCBJKINE, string BLMNLKOPMBJ = "", string FMEMPJHPGKL = "")  //mimicking "Guest Partner" promo behaviour to hide the names
    {
        if (Mathf.Abs(PHECEOMIMND.LPHFJGGHBED) < 1000000 || JJDCNALMPCI.AAAIDOOHBCM != 50 || PHECEOMIMND.IINDGFPADFM <= 0 || PHECEOMIMND.KGFJGDMFNLL == 0)
        {
            return;
        }
        int index = Mathf.Abs(PHECEOMIMND.LPHFJGGHBED) - 1000000;
        if (CustomContent.PromoData[index].SurpriseEntrants.Count == 0 && CustomContent.PromoData[index].SurpriseEntrantsNames.Count == 0)
        {
            return;
        }
        DJEKCMMMFJM character = FFKMIEMAJML.FJCOPECCEKN[PHECEOMIMND.KGFJGDMFNLL];
        int num = character.LLEGGMCIALJ.KDKGBDLCHLJ(0);
        PHECEOMIMND.OFANDPIMPNA[2].text = character.LLEGGMCIALJ.name;
        if (num > 0)
        {
            PHECEOMIMND.OFANDPIMPNA[1].text = Characters.fedData[character.LLEGGMCIALJ.fed].beltName[num] + " Champion";
        }
    }

    [HarmonyPatch(typeof(PHECEOMIMND), nameof(PHECEOMIMND.GMIGIAFOAAF))]
    [HarmonyPrefix]
    public static void PHECEOMIMND_GMIGIAFOAAF()  //releasing surprise entrants
    {
        if (PHECEOMIMND.LPHFJGGHBED > -1000000)
        {
            return;
        }

        int index = -PHECEOMIMND.LPHFJGGHBED - 1000000;
        if(CustomContent.PromoData[index].SurpriseEntrants.Count == 0 && CustomContent.PromoData[index].SurpriseEntrantsNames.Count == 0)
        {
            return;
        }

        if (CustomContent.PromoData[index].UseCharacterNames)
        {
            CustomContent.PromoData[index].NameToID.TryGetValue(CustomContent.PromoData[index].SurpriseEntrantsNames.First(), out int first);
            if (FFKMIEMAJML.FJCOPECCEKN[first].FKPIGOJCEAK == -1)
            {
                foreach (string name in CustomContent.PromoData[index].SurpriseEntrantsNames)
                {
                    if (CustomContent.PromoData[index].NameToID.TryGetValue(name, out int id))
                    {
                        FFKMIEMAJML.FJCOPECCEKN[id].FKPIGOJCEAK = 0;
                    }
                }
                foreach (int id in CustomContent.PromoData[index].SurpirseExtras)
                {
                    FFKMIEMAJML.FJCOPECCEKN[id].FKPIGOJCEAK = 0;
                }
            }
        }
        else
        {
            if (FFKMIEMAJML.FJCOPECCEKN[CustomContent.PromoData[index].SurpriseEntrants.First()].FKPIGOJCEAK == -1)
            {
                foreach (int id in CustomContent.PromoData[index].SurpriseEntrants)
                {
                    FFKMIEMAJML.FJCOPECCEKN[id].FKPIGOJCEAK = 0;
                }
                foreach (int id in CustomContent.PromoData[index].SurpirseExtras)
                {
                    FFKMIEMAJML.FJCOPECCEKN[id].FKPIGOJCEAK = 0;
                }
            }
        }

        if(CustomContent.PromoData[index].NextPromo != "")
        {
            int next =  CustomContent.PromoData.FindIndex(a => a.Title == CustomContent.PromoData[index].NextPromo);
            if (next != -1)
            {
                PHECEOMIMND.LPHFJGGHBED = next + 1000000;
                if (CustomContent.PromoData[next].UseCharacterNames)
                {
                    CustomContent.PromoData[next].NameToID = new();
                    for (int i = 1; i < FFKMIEMAJML.FJCOPECCEKN.Length; i++)
                    {
                        DJEKCMMMFJM person = FFKMIEMAJML.FJCOPECCEKN[i];

                        CustomContent.PromoData[next].NameToID.Add(person.LLEGGMCIALJ.name, i);
                    }
                }
            }
        }
    }
}