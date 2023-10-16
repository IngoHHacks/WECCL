using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
internal class MenuPatch
{
    private static int _lastFed = 1;
    internal static readonly Dictionary<int, Tuple<int, int, float, int, int>> _optimalLayouts = new();

    private static int _expectedNextId = -1;

    [HarmonyPatch(typeof(Characters), nameof(Characters.EJPIIEPCBHO))]
    [HarmonyPrefix]
    public static bool Characters_EJPIIEPCBHO(int INHHDMCONJD, int NALFBHCGLGA, int KBBBIJIOMIP)
    {
        if (SceneManager.GetActiveScene().name != "Select_Char")
        {
            return true;
        }
        _lastFed = NALFBHCGLGA;
        _expectedNextId = 0;
        if (NALFBHCGLGA == VanillaCounts.NoFeds + 1)
        {
            if (_searchString == "")
            {
                UnmappedMenus.OICIMMOPFPG = Characters.no_chars;
                UnmappedMenus.ELGGCLPAICP = Characters.c.Skip(1).SortBy(INHHDMCONJD).Select(x => x.id).Prepend(0).ToArray();
                UnmappedMenus.AHJHELDAAJP = new int[Characters.no_chars + 1];
            }
            else
            {
                UnmappedMenus.ELGGCLPAICP = Characters.c.Skip(1).Where(x => x.name.ToLower().Contains(_searchString.ToLower())).SortBy(INHHDMCONJD).Select(x => x.id).Prepend(0).ToArray();
                UnmappedMenus.OICIMMOPFPG = UnmappedMenus.ELGGCLPAICP.Length - 1;
                UnmappedMenus.AHJHELDAAJP = new int[Characters.no_chars + 1];
                if (UnmappedMenus.OICIMMOPFPG < 25 && INHHDMCONJD == 0)
                {
                    IEnumerable<Character> c = Characters.c.Skip(1).Where(x => x.name.ToLower().Contains(_searchString.ToLower())).Concat(Characters.c.Skip(1).Where(x => !x.name.ToLower().Contains(_searchString.ToLower())).OrderBy(x => SubstringDamerauLevenshteinDistance(x.name.ToLower(), _searchString.ToLower()))).Take(25);
                    UnmappedMenus.ELGGCLPAICP = c.Select(x => x.id).Prepend(0).ToArray();
                    UnmappedMenus.OICIMMOPFPG = UnmappedMenus.ELGGCLPAICP.Length - 1;
                }
            }
            for (int i = 0; i < UnmappedMenus.ELGGCLPAICP.Length; i++)
            {
                UnmappedMenus.AHJHELDAAJP[UnmappedMenus.ELGGCLPAICP[i]] = i;
            }

            Characters.fedData[NALFBHCGLGA].size = UnmappedMenus.OICIMMOPFPG;
            return false;
        }
        return true;
    }
    
    // Damerau Levenshtein distance algorithm from https://programm.top/en/c-sharp/algorithm/damerau-levenshtein-distance/
    private static int Minimum(int a, int b) => a < b ? a : b;
    private static int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;
    private static int DamerauLevenshteinDistance(string firstText, string secondText)
    {
        var n = firstText.Length + 1;
        var m = secondText.Length + 1;
        var arrayD = new int[n, m];

        for (var i = 0; i < n; i++)
        {
            arrayD[i, 0] = i;
        }

        for (var j = 0; j < m; j++)
        {
            arrayD[0, j] = j;
        }

        for (var i = 1; i < n; i++)
        {
            for (var j = 1; j < m; j++)
            {
                var cost = firstText[i - 1] == secondText[j - 1] ? 0 : 1;

                arrayD[i, j] = Minimum(arrayD[i - 1, j] + 1, // delete
                    arrayD[i, j - 1] + 1, // insert
                    arrayD[i - 1, j - 1] + cost); // replacement

                if (i > 1 && j > 1
                          && firstText[i - 1] == secondText[j - 2]
                          && firstText[i - 2] == secondText[j - 1])
                {
                    arrayD[i, j] = Minimum(arrayD[i, j],
                        arrayD[i - 2, j - 2] + cost); // permutation
                }
            }
        }

        return arrayD[n - 1, m - 1];
    }

    private static int SubstringDamerauLevenshteinDistance(string firstText, string secondText)
    {
        int len1 = firstText.Length;
        int len2 = secondText.Length;
        if (len1 < len2)
        {
            return DamerauLevenshteinDistance(firstText, secondText);
        }
        string shortestWord = secondText;
        string longestWord = firstText;
        int start = 0;
        int len = shortestWord.Length;
        int min = int.MaxValue;
        while (start + len <= longestWord.Length)
        {
            int dist = DamerauLevenshteinDistance(shortestWord, longestWord.Substring(start, len));
            if (dist < min)
            {
                min = dist;
            }
            start++;
        }
        return min;
    }


    /*
     * Menu.GBDGLHHCLCI is called when the player opens the editor (including the fed editor)
     * This patch is used to resize the character editor to fit the roster size if it is larger than 48 (vanilla max)
     */
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.GBDGLHHCLCI))]
    [HarmonyPrefix]
    public static void Menu_GBDGLHHCLCI(int ACBEHIAKAPB, string IGAPGCFCPKC, ref float FINKCELCANI,
        ref float BIHCCGJAHAB, ref float BALKEOIBHFM, ref float AHGEKIBLHPM)
    {
        try
        {
            if (ACBEHIAKAPB != 5)
            {
                return;
            }

            int fedSize = Characters.fedData[_lastFed].size;
            if (fedSize > 48 || _lastFed == VanillaCounts.NoFeds + 1)
            {
                int actualIndex = (((int)FINKCELCANI + 525) / 210) + ((-(int)BIHCCGJAHAB + 110) / 60 * 6);
                if (fedSize <= 35)
                {
                    actualIndex = (((int)FINKCELCANI + 490) / 245) + ((-(int)BIHCCGJAHAB + 110) / 70 * 5);
                }
                if (actualIndex != _expectedNextId)
                {
                    return;
                }

                _expectedNextId++;

                int columns;
                float scale;
                int startX;
                int startY;

                var y = 110;
                
                if (_lastFed == VanillaCounts.NoFeds + 1)
                {
                    if (fedSize > 48)
                    {
                        y = 70;
                    }
                    y = 50;
                }
                
                FindBestFit(fedSize, -525, -310, 525, y, out int _, out columns, out scale, out startX, out startY);

                BALKEOIBHFM = scale;
                AHGEKIBLHPM = scale;
                int itemWidth = fedSize > 35 ? 210 : 245;
                int itemHeight = fedSize > 48 ? 50 : 60;
                FINKCELCANI = startX + (actualIndex % columns * itemWidth * scale);
                BIHCCGJAHAB = startY - (actualIndex / columns * itemHeight * scale);
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }
    
    private static string _searchString = "";
    private static bool _searchUpdate = false;

    [HarmonyPatch(typeof(Scene_Select_Char), nameof(Scene_Select_Char.Update))]
    [HarmonyPrefix]
    public static bool Scene_Select_Char_Update(Scene_Select_Char __instance)
    {
        if (Plugin.EnableWrestlerSearchScreen.Value)
        {
            if (_lastFed == VanillaCounts.NoFeds + 1)
            {
                if (UnmappedMenus.CJGHFHCHDNN == 0)
                {
                    if (Input.inputString != "" && Input.inputString != "\b")
                    {
                        String str = Input.inputString.Replace("\b", "").Replace("\n", "").Replace("\r", "")
                            .Replace("\t", "")
                            .Replace("\0", "");
                        UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].BHAAOLGFAOD += str;
                        _searchString = UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].BHAAOLGFAOD;
                        _searchUpdate = true;
                        UnmappedMenus.GBDGLHHCLCI();
                        return false;
                    }

                    if (Input.inputString == "\b" || Input.GetKeyDown(KeyCode.Delete))
                    {
                        if (UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].BHAAOLGFAOD.Length > 0)
                        {
                            UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].BHAAOLGFAOD = UnmappedMenus
                                .ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].BHAAOLGFAOD.Substring(0,
                                    UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].BHAAOLGFAOD.Length - 1);
                            _searchString = UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].BHAAOLGFAOD;
                            _searchUpdate = true;
                            UnmappedMenus.GBDGLHHCLCI();
                        }

                        return false;
                    }

                    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Z))
                    {
                        return _searchString.Length == 0;
                    }

                    __instance.gFed.GetComponent<Image>().sprite = UnmappedSprites.MEHGBLJMPBF[VanillaCounts.NoFeds + 1];
                }
                else
                {
                    __instance.gFed.GetComponent<Image>().sprite =
                        UnmappedSprites.MEHGBLJMPBF[Characters.c[Characters.foc].fed];
                }
            }

            Characters.no_feds = VanillaCounts.NoFeds + 1;
            if (Characters.fedData.Length <= Characters.no_feds)
            {
                Array.Resize(ref Characters.fedData, Characters.no_feds + 1);
                Characters.fedData[Characters.no_feds] = new Roster();
                Characters.fedData[Characters.no_feds].roster = Characters.c.Select(x => x.id).ToArray();
                Characters.fedData[Characters.no_feds].FFGHCMGIDOB(-1);
                Characters.fedData[Characters.no_feds].CACCOPDFPGP();
                Characters.fedData[Characters.no_feds].size = Characters.no_chars;
            }
        }
        return true;
    }

    [HarmonyPatch(typeof(UnmappedSprites), nameof(UnmappedSprites.IJHBLDHOEOH))]
    [HarmonyPostfix]
    public static void UnmappedSprites_IJHBLDHOEOH()
    {
        Characters.no_feds = VanillaCounts.NoFeds;
    }

    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.GBDGLHHCLCI))]
    [HarmonyPostfix]
    public static void UnmappedMenus_GBDGLHHCLCI()
    {
        if (UnmappedMenus.AAAIDOOHBCM == 11 && Characters.fed == VanillaCounts.NoFeds + 1)
        {
            UnmappedMenus.MFDCLFKDDFB();
            UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].GBDGLHHCLCI(2, "\u200BSearch\u200B", 0, 110, 1, 1);
            UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].BHAAOLGFAOD = _searchString;
            UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].DHBIELODIAN = 999999999;
            if (_searchUpdate)
            {
                UnmappedMenus.CJGHFHCHDNN = 0;
                _searchUpdate = false;
            }
        }
        else
        {
            _searchString = "";
        }
    }

    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.DLADNAFPGPJ))]
    [HarmonyPrefix]
    public static bool UnmappedMenu_DLADNAFPGPJ(ref int __result, UnmappedMenu __instance, float DKOBDIJJOGO, float IBDKLAELPND, float MFCNEPBJODD)
    {
        if (__instance.CCFHFGBDIHE.Equals("\u200BSearch\u200B"))
        {
            __result = 0;
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.BKCLHHDGBEC))]
    [HarmonyPostfix]
    public static void UnmappedMenu_BKCLHHDGBEC(UnmappedMenu __instance)
    {
        if (__instance.CCFHFGBDIHE.Equals("\u200BSearch\u200B") && UnmappedMenus.CJGHFHCHDNN == 0)
        {
            UnmappedSprites.IJHBLDHOEOH(__instance.FCFPMEDHPML, UnmappedMenus.NGDHHIIMOFK.r, UnmappedMenus.NGDHHIIMOFK.g, UnmappedMenus.NGDHHIIMOFK.b);
            UnmappedSprites.IJHBLDHOEOH(__instance.ALCICLMKKDB, UnmappedMenus.DEOFJPGNMMC.r, UnmappedMenus.DEOFJPGNMMC.g, UnmappedMenus.DEOFJPGNMMC.b);
            if (__instance.FNIDHNNCLBB == 3)
            {
                UnmappedSprites.IJHBLDHOEOH(__instance.CBINJLAMDCI, UnmappedMenus.DEOFJPGNMMC.r, UnmappedMenus.DEOFJPGNMMC.g, UnmappedMenus.DEOFJPGNMMC.b);
            }
            if (__instance.KGBFPNENBBP != null)
            {
                __instance.KGBFPNENBBP.color = new Color(UnmappedMenus.EHPPGJOCJJI.r, UnmappedMenus.EHPPGJOCJJI.g, UnmappedMenus.EHPPGJOCJJI.b, __instance.KGBFPNENBBP.color.a);
            }
            if (__instance.BPKEDBIMCJG != null)
            {
                __instance.BPKEDBIMCJG.color = new Color(UnmappedMenus.OCHCAGGLEOD.r, UnmappedMenus.OCHCAGGLEOD.g, UnmappedMenus.OCHCAGGLEOD.b, __instance.BPKEDBIMCJG.color.a);
            }
        }
    }
    
    [HarmonyPatch(typeof(Scene_Editor), nameof(Scene_Editor.Update))]
    [HarmonyPostfix]
    public static void Scene_Editor_Update()
    {
        if (UnmappedMenus.PJHNMEEFCME == 1)
        {
            UnmappedPlayer gMIKIMHFABP = FFKMIEMAJML.FJCOPECCEKN[1];
            Character iPNKFGHIDJP = gMIKIMHFABP.LLEGGMCIALJ;
            if (iPNKFGHIDJP.music > VanillaCounts.MusicCount)
            {
                int index = iPNKFGHIDJP.music - VanillaCounts.MusicCount - 1;
                string name = CustomClips[index].Name;
                UnmappedMenus.ECEJOIDPOCN[8].BHAAOLGFAOD = name;
                
            }
            else if (iPNKFGHIDJP.music == 0)
            {
                UnmappedMenus.ECEJOIDPOCN[8].BHAAOLGFAOD = "None";
            }
            else if (CustomClips.Count > 0)
            {
                UnmappedMenus.ECEJOIDPOCN[8].BHAAOLGFAOD = "Vanilla " + iPNKFGHIDJP.music;
            }
            UnmappedMenus.BKCLHHDGBEC();
            
        }
    }
}