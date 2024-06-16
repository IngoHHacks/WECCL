using System.Text.RegularExpressions;
using WECCL.Content;
using PromoData = WECCL.Content.PromoData;

namespace WECCL.Patches;

[HarmonyPatch]
internal class PromoPatch
{
    
    /*
     * Patch:
     * - Runs promo script of custom promos.
     */
    [HarmonyPatch(typeof(UnmappedPromo), nameof(UnmappedPromo.KMFBNADPGMF))]
    [HarmonyPrefix]
    public static void Promo_KMFBNADPGMF()
    {
        int promoId = UnmappedPromo.LODPJDDLEKI - 1000000;
        if (promoId < 0)
        {
            return;
        }

        PromoData promo = CustomContent.PromoData[promoId];

        int page = UnmappedPromo.ODOAPLMOJPD - 1;
        if (page >= promo.NumLines)
        {
            UnmappedPromo.LODPJDDLEKI = 0;
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

        if (UnmappedPromo.IMJHCHECCED >= 100f && UnmappedPromo.KJPJODMIPGO < UnmappedPromo.ODOAPLMOJPD)
        {
            if (promo.PromoLines[page].Features != null)
            {
                foreach (PromoData.AdvFeatures feature in promo.PromoLines[page].Features)
                {
                    PromoData.AdvFeatures.CommandType cmd = feature.Command;
                    switch (cmd)
                    {
                        case PromoData.AdvFeatures.CommandType.SetFace:
                            promo.GetCharacterForCmd(feature.Args[0]).MFICBPFFDLC(0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetHeel:
                            promo.GetCharacterForCmd(feature.Args[0]).MFICBPFFDLC(-1);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetRealEnemy:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .DADEOGCFAAN(promo.GetCharacterForCmd(feature.Args[1]).id, -1, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetStoryEnemy:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .DADEOGCFAAN(promo.GetCharacterForCmd(feature.Args[1]).id, -1);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetRealFriend:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .DADEOGCFAAN(promo.GetCharacterForCmd(feature.Args[1]).id, 1, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetStoryFriend:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .DADEOGCFAAN(promo.GetCharacterForCmd(feature.Args[1]).id, 1);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetRealNeutral:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .DADEOGCFAAN(promo.GetCharacterForCmd(feature.Args[1]).id, 0, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.SetStoryNeutral:
                            promo.GetCharacterForCmd(feature.Args[0])
                                .DADEOGCFAAN(promo.GetCharacterForCmd(feature.Args[1]).id, 0);
                            break;
                        case PromoData.AdvFeatures.CommandType.PlayAudio:
                            if (feature.Args[0] == "-1")
                            {
                                UnmappedSound.KIKKPCJGDLM(UnmappedPromo.NNMDEFLLNBF, -1, 1f);
                            }
                            else
                            {
                                UnmappedSound.AFLOJPBKNFB.PlayOneShot(
                                    UnmappedSound.JOPLJHCBICG[Indices.ParseCrowdAudio(feature.Args[0])], 1);
                            }

                            break;
                    }
                }
            }

            UnmappedPromo.KJPJODMIPGO = UnmappedPromo.ODOAPLMOJPD;
        }
    }
    
#pragma warning disable Harmony003
    private static void ExecutePromoLine(string line1, string line2, int from, int to, float demeanor, int taunt, bool useNames)
    {
        line1 = ReplaceVars(line1);
        line2 = ReplaceVars(line2);
        if(useNames)
        {
            UnmappedPromo.LIHAMAIBHFN(from, to, demeanor, taunt);
        }
        else
        {
            UnmappedPromo.LIHAMAIBHFN(UnmappedPromo.ACELMDKGHEK[from], UnmappedPromo.ACELMDKGHEK[to], demeanor, taunt);
        }

        UnmappedPromo.MLLPFEKAONO[1] = line1;
        UnmappedPromo.MLLPFEKAONO[2] = line2;
    }

    private static string ReplaceVars(string line)
    {
        // Replace $variables
        MatchCollection matches = Regex.Matches(line, @"\$\$?([a-zA-Z]+)(\W|$)");
        foreach (Match match in matches)
        {
            try
            {
                string varName = match.Groups[1].Value.ToLower();
                string varValue = varName switch
                {
                    "location" => MappedWorld.DescribeLocation(World.location),
                    "date" => MappedProgress.DescribeDate(MappedProgress.date, MappedProgress.year),
                    "match" => MappedMatch.DescribeMatch(2),
                    _ => "UNKNOWN"
                };
                line = line.Replace(match.Value, varValue + match.Groups[2].Value);
            }
            catch (Exception e)
            {
                line = line.Replace(match.Value, "INVALID");
                LogError(e);
            }
        }
        matches = Regex.Matches(line, @"\$\$?([a-zA-Z]+)-?(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            try
            {
                string varName = match.Groups[1].Value.ToLower();
                int varIndex = int.Parse(match.Groups[2].Value);
                string varValue;
                if (varName == "date")
                {
                    var date = MappedProgress.date + varIndex;
                    var year = MappedProgress.year;
                    while (date > 48)
                    {
                        date -= 48;
                        year++;
                    }
                    while (date < 1)
                    {
                        date += 48;
                        year--;
                    }
                    varValue = MappedProgress.DescribeDate(date, year);
                }
                else
                {
                    varValue = varName switch
                    {
                        "name" => MappedPromo.c[varIndex].name,
                        "promotion" => MappedPromo.fed[varIndex].name,
                        "prop" => MappedWeapons.Describe(MappedPromo.c[varIndex].prop),
                        "team" => MappedPromo.c[varIndex].teamName,
                        _ => "UNKNOWN"
                    };
                }
                line = line.Replace(match.Value, varValue + match.Groups[3].Value);
            }
            catch (Exception e)
            {
                line = line.Replace(match.Value, "INVALID");
                LogError(e);
            }
        }
        matches = Regex.Matches(line,
            @"\$\$?([a-zA-Z]+)-?(\d+)_-?(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            try
            {
                string varName = match.Groups[1].Value.ToLower();
                int varIndex1 = int.Parse(match.Groups[2].Value);
                int varIndex2 = int.Parse(match.Groups[3].Value);
                string varValue = varName switch
                {
                    "belt" => MappedPromo.fed[varIndex1].beltName[varIndex2],
                    "champ" => Characters.c[MappedPromo.fed[varIndex1].champ[varIndex2, 1]]
                        .name, //1 - current champ, then 2 - previous?
                    "movefront" => MappedAnims.DescribeMove(MappedPromo.c[varIndex1].moveFront[varIndex2]),
                    "moveback" => MappedAnims.DescribeMove(MappedPromo.c[varIndex1].moveBack[varIndex2]),
                    "moveground" => MappedAnims.DescribeMove(MappedPromo.c[varIndex1].moveGround[varIndex2]),
                    "moveattack" => MappedAnims.DescribeMove(MappedPromo.c[varIndex1].moveAttack[varIndex2]),
                    "movecrush" => MappedAnims.DescribeMove(MappedPromo.c[varIndex1].moveCrush[varIndex2]),
                    "taunt" => ((MappedTaunt) MappedAnims.taunt[MappedPromo.c[varIndex1].taunt[varIndex2]]).name,
                    "stat" => MappedPromo.c[varIndex1].stat[varIndex2].ToString("0"),
                    _ => "UNKNOWN"
                };

                line = line.Replace(match.Value, varValue + match.Groups[4].Value);
            }
            catch (Exception e)
            {
                line = line.Replace(match.Value, "INVALID");
                LogError(e);
            }
        }

        matches = Regex.Matches(line, @"@([a-zA-Z]+)(\d+)(\W|$)");
        foreach (Match match in matches)
        {
            string varName = match.Groups[1].Value;
            int varIndex = int.Parse(match.Groups[2].Value);
            string varValue = UnmappedPromo.BBPPMGDKCBJ[varIndex].POOMHHMDABP(varName);

            line = line.Replace(match.Value, varValue + match.Groups[3].Value);
        }

        return line;
    }
#pragma warning restore Harmony003
    
    public static void PatchPromoInfo()
    {
        if (CustomContent.PromoData.Count == 0)
        {
            return;
        }
        
        foreach (PromoData promo in CustomContent.PromoData)
        {
            var cl = UnmappedPromo.KIDIEPFFPOO.GetLength(1);
            if (!MappedPromo.libraryName.Contains(promo.Category))
            {
                MappedPromo.no_categories++;
                Array.Resize(ref UnmappedPromo.ALPFKHEBGII, MappedPromo.no_categories + 1);
                Array.Resize(ref UnmappedPromo.FNMHOMDBCJE, MappedPromo.no_categories + 1);
                ResizeArray(ref UnmappedPromo.KIDIEPFFPOO, MappedPromo.no_categories + 1, cl);
                MappedPromo.libraryName[MappedPromo.no_categories] = promo.Category;
                MappedPromo.library[MappedPromo.no_categories, 0] = 0;
            }
            int cat = Array.IndexOf(MappedPromo.libraryName, promo.Category);
            MappedPromo.librarySize[cat]++;
            if (cl <= MappedPromo.librarySize[cat])
            {
                ResizeArray(ref UnmappedPromo.KIDIEPFFPOO, MappedPromo.no_categories + 1, 2 * cl);
            }
            MappedPromo.library[cat, MappedPromo.librarySize[cat]] = promo._id;
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

    /*
     * Patch:
     * Sets promo properties for custom promos.
     */
    [HarmonyPatch(typeof(UnmappedPromo), nameof(UnmappedPromo.GGCFFJNAKFM))]
    [HarmonyPostfix]
    public static void Promo_GGCFFJNAKFM(int NJPKCMBLMLG)
    {
        if (NJPKCMBLMLG < 1000000)
        {
            return;
        }

        int index = NJPKCMBLMLG - 1000000;

        UnmappedPromo.NKEDCLBOOMJ = CustomContent.PromoData[index].Title;
        UnmappedPromo.OLHNMPPACFE = CustomContent.PromoData[index].Description;
        UnmappedPromo.JPOHPEOOIMK = CustomContent.PromoData[index].Characters;
    }

    /*
     * Patch:
     * Enables surprise entrants for custom promos.
     */
    [HarmonyPatch(typeof(UnmappedPlayers), nameof(UnmappedPlayers.BGIDMGILIEG))]
    [HarmonyPostfix]
    public static void Players_BGIDMGILIEG()  //setting up surprises
    {
        if (FFCEGMEAIBP.NJPKCMBLMLG < 1000000 || UnmappedMenus.FAKHAFKOBPB != 50)
        {
            return;
        }
        int index = FFCEGMEAIBP.NJPKCMBLMLG - 1000000;

        UnmappedPromo.NKEDCLBOOMJ = CustomContent.PromoData[index].Title;
        UnmappedPromo.OLHNMPPACFE = CustomContent.PromoData[index].Description;
        UnmappedPromo.JPOHPEOOIMK = CustomContent.PromoData[index].Characters;

        CustomContent.PromoData[index].NameToID = new();
        CustomContent.PromoData[index].SurpirseExtras = new();
        for (int i = 1; i < NJBJIIIACEP.OAAMGFLINOB.Length; i++)
        {
            UnmappedPlayer person = NJBJIIIACEP.OAAMGFLINOB[i];
            if (CustomContent.PromoData[index].UseCharacterNames)
            {
                CustomContent.PromoData[index].NameToID.Add(person.EMDMDLNJFKP.name, i);
            }

            if(CustomContent.PromoData[index].IsCharacterSurprise(person))
            {
                RespawnSurprise(person);
            }
            if (CustomContent.PromoData[index].IsCharacterSurprise(NJBJIIIACEP.OAAMGFLINOB[person.FHMLNCBCALP])) //person they are managing
            {
                CustomContent.PromoData[index].SurpirseExtras.Add(i);
                RespawnTogetherWith(person, NJBJIIIACEP.OAAMGFLINOB[person.FHMLNCBCALP]);
            }
        }
    }
  
    public static void RespawnSurprise(UnmappedPlayer __instance)
    {
        __instance.AHBNKMMMGFI = -1;  //setting surprise entrants
        OGAJMOPCPLJ diaocefccae;
        int num2;
        do
        {
            diaocefccae = UnmappedBlocks.FBEMAEDLBLN[UnmappedGlobals.PMEEFNOLAGF(1, UnmappedBlocks.BAOOLJCLBIH, 0)]; //resetting their spawns in case they spawn in ring
            num2 = 1;
            if (World.location == 9 && diaocefccae.OCPIPKGHABK() > 50f)
            {
                num2 = 0;
            }
            if (diaocefccae.AHBNKMMMGFI == 0)
            {
                num2 = 0;
            }
        }
        while (num2 == 0);
        __instance.NJDGEELLAKG = diaocefccae.APJMFOCJLNJ();
        __instance.BMFDFFLPBOJ = diaocefccae.OCPIPKGHABK();
        __instance.MPFFANIIEDG = diaocefccae.AAPMLHAGBGF;
        __instance.EKOHAKPAOGN = World.KJOEBADBOME(__instance.NJDGEELLAKG, __instance.FNNBCDPJBIO, __instance.BMFDFFLPBOJ);
        if (UnmappedBlocks.MDLFHNCMFDO(__instance.NJDGEELLAKG, __instance.BMFDFFLPBOJ, 0f) > 0)
        {
            __instance.EKOHAKPAOGN = World.ringGround;
        }
        __instance.FNNBCDPJBIO = __instance.EKOHAKPAOGN;
        if (__instance.FNNBCDPJBIO < __instance.EKOHAKPAOGN)
        {
            __instance.FNNBCDPJBIO = __instance.EKOHAKPAOGN;
        }
        __instance.NELODEMHJHN = 0;  //seat
        __instance.PCNHIIPBNEK[0].SetActive(false);
        __instance.IFOCOECLBAF.SetActive(false);
    }
    public static void RespawnTogetherWith(UnmappedPlayer manager, UnmappedPlayer wrestler)
    {
        manager.AHBNKMMMGFI = -1;
        manager.NELODEMHJHN = 0;
        manager.NJDGEELLAKG = wrestler.NJDGEELLAKG;
        manager.BMFDFFLPBOJ = wrestler.BMFDFFLPBOJ;
        manager.MPFFANIIEDG = wrestler.MPFFANIIEDG;
        manager.EKOHAKPAOGN = wrestler.EKOHAKPAOGN;
        manager.FNNBCDPJBIO = wrestler.FNNBCDPJBIO;
        manager.PCNHIIPBNEK[0].SetActive(false);
        manager.IFOCOECLBAF.SetActive(false);

    }

    /*
     * Patch:
     * - Overrides 'comment' for promos with guest partners to hide the names.
     */
    [HarmonyPatch(typeof(UnmappedMatch), nameof(UnmappedMatch.CDKIEOBHCKE))]
    [HarmonyPostfix]
    public static void Match_CDKIEOBHCKE(string LLGKFOKJILF, string EBKFKAKBBKI = "", string GMGANODNCGH = "")
    {
        if (Mathf.Abs(FFCEGMEAIBP.NJPKCMBLMLG) < 1000000 || UnmappedMenus.FAKHAFKOBPB != 50 || FFCEGMEAIBP.OLJFOJOLLOM <= 0 || FFCEGMEAIBP.LPBCEGPJNMF == 0)
        {
            return;
        }
        int index = Mathf.Abs(FFCEGMEAIBP.NJPKCMBLMLG) - 1000000;
        if (CustomContent.PromoData[index].SurpriseEntrants.Count == 0 && CustomContent.PromoData[index].SurpriseEntrantsNames.Count == 0)
        {
            return;
        }
        UnmappedPlayer character = NJBJIIIACEP.OAAMGFLINOB[FFCEGMEAIBP.LPBCEGPJNMF];
        int num = character.EMDMDLNJFKP.FBIIPBMKAGK(0);
        FFCEGMEAIBP.PGPFHDLODFG[2].text = character.EMDMDLNJFKP.name;
        if (num > 0)
        {
            FFCEGMEAIBP.PGPFHDLODFG[1].text = Characters.fedData[character.EMDMDLNJFKP.fed].beltName[num] + " Champion";
        }
    }

    /*
     * Patch:
     * - Releases surprise entrants.
     */
    [HarmonyPatch(typeof(UnmappedMatch), nameof(UnmappedMatch.FGFMHFNMLMA))]
    [HarmonyPrefix]
    public static void Match_FGFMHFNMLMA()
    {
        if (FFCEGMEAIBP.NJPKCMBLMLG > -1000000)
        {
            return;
        }

        int index = -FFCEGMEAIBP.NJPKCMBLMLG - 1000000;
        if(CustomContent.PromoData[index].SurpriseEntrants.Count == 0 && CustomContent.PromoData[index].SurpriseEntrantsNames.Count == 0)
        {
            return;
        }

        if (CustomContent.PromoData[index].UseCharacterNames)
        {
            CustomContent.PromoData[index].NameToID.TryGetValue(CustomContent.PromoData[index].SurpriseEntrantsNames.First(), out int first);
            if (NJBJIIIACEP.OAAMGFLINOB[first].AHBNKMMMGFI == -1)
            {
                foreach (string name in CustomContent.PromoData[index].SurpriseEntrantsNames)
                {
                    if (CustomContent.PromoData[index].NameToID.TryGetValue(name, out int id))
                    {
                        NJBJIIIACEP.OAAMGFLINOB[id].AHBNKMMMGFI = 0;
                    }
                }
                foreach (int id in CustomContent.PromoData[index].SurpirseExtras)
                {
                    NJBJIIIACEP.OAAMGFLINOB[id].AHBNKMMMGFI = 0;
                }
            }
        }
        else
        {
            if (NJBJIIIACEP.OAAMGFLINOB[CustomContent.PromoData[index].SurpriseEntrants.First()].AHBNKMMMGFI == -1)
            {
                foreach (int id in CustomContent.PromoData[index].SurpriseEntrants)
                {
                    NJBJIIIACEP.OAAMGFLINOB[id].AHBNKMMMGFI = 0;
                }
                foreach (int id in CustomContent.PromoData[index].SurpirseExtras)
                {
                    NJBJIIIACEP.OAAMGFLINOB[id].AHBNKMMMGFI = 0;
                }
            }
        }

        if(CustomContent.PromoData[index].NextPromo != "")
        {
            int next =  CustomContent.PromoData.FindIndex(a => a.Title == CustomContent.PromoData[index].NextPromo);
            if (next != -1)
            {
                FFCEGMEAIBP.NJPKCMBLMLG = next + 1000000;
                if (CustomContent.PromoData[next].UseCharacterNames)
                {
                    CustomContent.PromoData[next].NameToID = new();
                    for (int i = 1; i < NJBJIIIACEP.OAAMGFLINOB.Length; i++)
                    {
                        UnmappedPlayer person = NJBJIIIACEP.OAAMGFLINOB[i];

                        CustomContent.PromoData[next].NameToID.Add(person.EMDMDLNJFKP.name, i);
                    }
                }
            }
        }
    }
    
    /*
     * Patch:
     * - Assigns custom promos to matches in wrestler career mode.
     */
    [HarmonyPatch(typeof(Progress), nameof(Progress.EEANPLJLLMA))]
    [HarmonyPrefix]
    public static void Progress_EEANPLJLLMA_Pre()
    {
        if (CustomContent.PromoData.Count == 0 || Progress.promo[Progress.date] != 0)
        {
            return;
        }

        PromoData.AssignPromo(true);
    }
    
    /*
     * Patch:
     * - Second patch for assigning custom promos to matches in wrestler career mode.
     */
    [HarmonyPatch(typeof(Progress), nameof(Progress.EEANPLJLLMA))]
    [HarmonyPostfix]
    public static void Progress_EEANPLJLLMA_Post()
    {
        if (CustomContent.PromoData.Count == 0 || Progress.promo[Progress.date] != 0)
        {
            return;
        }

        if (PromoData.AssignPromo())
        {
            MappedMatch.promo = 0;
            if (Progress.promo[Progress.date] != 0)
            {
                MappedMatch.promo = Mathf.Abs(Progress.promo[Progress.date]);
                MappedPromo.variable = Progress.promoVariable[Progress.date];
                MappedPromo.star = Characters.star;
                MappedPromo.opponent = Progress.opponent[Progress.date];
                if (Progress.promo[Progress.date] < 0)
                {
                    MappedPromo.star = Progress.opponent[Progress.date];
                    MappedPromo.opponent = Characters.star;
                }
            }
        }
    }
}