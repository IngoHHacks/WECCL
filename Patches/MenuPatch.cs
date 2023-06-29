using UnityEngine.UI;
using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
internal class MenuPatch
{
    private static int _lastFed = 1;
    internal static readonly Dictionary<int, Tuple<int, int, float, int, int>> _optimalLayouts = new();

    private static int _expectedNextId = -1;

    [HarmonyPatch(typeof(Characters), nameof(Characters.PBNPILLGGLH))]
    [HarmonyPrefix]
    public static bool Characters_PBNPILLGGLH(int LOIILHLDKKE, int HCKDAHDFLIF, int IDNKMKFMPOG) // Second argument
    {
        _lastFed = HCKDAHDFLIF;
        _expectedNextId = 0;
        if (HCKDAHDFLIF == VanillaCounts.NoFeds + 1)
        {
            if (_searchString == "")
            {
                DNDIEGNJOKN.MNJEEKGMEEC = Characters.no_chars;
                DNDIEGNJOKN.FDJHLFJLILM = Characters.c.Skip(1).SortBy(LOIILHLDKKE).Select(x => x.id).Prepend(0).ToArray();
                DNDIEGNJOKN.GJNFOKIHEON = new int[Characters.no_chars + 1];
            }
            else
            {
                DNDIEGNJOKN.FDJHLFJLILM = Characters.c.Skip(1).Where(x => x.name.ToLower().Contains(_searchString.ToLower())).SortBy(LOIILHLDKKE).Select(x => x.id).Prepend(0).ToArray();
                DNDIEGNJOKN.MNJEEKGMEEC = DNDIEGNJOKN.FDJHLFJLILM.Length - 1;
                DNDIEGNJOKN.GJNFOKIHEON = new int[Characters.no_chars + 1];
                if (DNDIEGNJOKN.MNJEEKGMEEC < 25 && LOIILHLDKKE == 0)
                {
                    IEnumerable<Character> c = Characters.c.Skip(1).Where(x => x.name.ToLower().Contains(_searchString.ToLower())).Concat(Characters.c.Skip(1).Where(x => !x.name.ToLower().Contains(_searchString.ToLower())).OrderBy(x => SubstringDamerauLevenshteinDistance(x.name.ToLower(), _searchString.ToLower()))).Take(25);
                    DNDIEGNJOKN.FDJHLFJLILM = c.Select(x => x.id).Prepend(0).ToArray();
                    DNDIEGNJOKN.MNJEEKGMEEC = DNDIEGNJOKN.FDJHLFJLILM.Length - 1;
                }
            }
            for (int i = 0; i < DNDIEGNJOKN.FDJHLFJLILM.Length; i++)
            {
                DNDIEGNJOKN.GJNFOKIHEON[DNDIEGNJOKN.FDJHLFJLILM[i]] = i;
            }

            Characters.fedData[HCKDAHDFLIF].size = DNDIEGNJOKN.MNJEEKGMEEC;
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
     * GameMenus.ICKGKDOKJEN is called when the player opens the editor (including the fed editor)
     * This patch is used to resize the character editor to fit the roster size if it is larger than 48 (vanilla max)
     */
    [HarmonyPatch(typeof(GameMenus), nameof(GameMenus.ICKGKDOKJEN))]
    [HarmonyPrefix]
    public static void GameMenus_ICKGKDOKJEN(int IJLDPEFGOOL, string NPDFJAEJIND, ref float GBKANPHAPIG,
        ref float AHMKMFPJFJA, ref float GLMFADFPECG, ref float BKBCELICBON)
    {
        try
        {
            if (IJLDPEFGOOL != 5)
            {
                return;
            }

            int fedSize = Characters.fedData[_lastFed].size;
            if (fedSize > 48 || _lastFed == VanillaCounts.NoFeds + 1)
            {
                int actualIndex = (((int)GBKANPHAPIG + 525) / 210) + ((-(int)AHMKMFPJFJA + 110) / 60 * 6);
                if (fedSize <= 35)
                {
                    actualIndex = (((int)GBKANPHAPIG + 490) / 245) + ((-(int)AHMKMFPJFJA + 110) / 70 * 5);
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

                GLMFADFPECG = scale;
                BKBCELICBON = scale;
                int itemWidth = fedSize > 35 ? 210 : 245;
                int itemHeight = fedSize > 48 ? 50 : 60;
                GBKANPHAPIG = startX + (actualIndex % columns * itemWidth * scale);
                AHMKMFPJFJA = startY - (actualIndex / columns * itemHeight * scale);
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
        if (_lastFed == VanillaCounts.NoFeds + 1) {
            if (DNDIEGNJOKN.PDMDFGNJCPN == 0)
            {
                if (Input.inputString != "" && Input.inputString != "\b")
                {
                    String str = Input.inputString.Replace("\b", "").Replace("\n", "").Replace("\r", "")
                        .Replace("\t", "")
                        .Replace("\0", "");
                    DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ANONBHFAOEP += str;
                    _searchString = DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ANONBHFAOEP;
                    _searchUpdate = true;
                    DNDIEGNJOKN.ICKGKDOKJEN();
                    return false;
                }

                if (Input.inputString == "\b" || Input.GetKeyDown(KeyCode.Delete))
                {
                    if (DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ANONBHFAOEP.Length > 0)
                    {
                        DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ANONBHFAOEP = DNDIEGNJOKN
                            .FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ANONBHFAOEP.Substring(0,
                                DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ANONBHFAOEP.Length - 1);
                        _searchString = DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ANONBHFAOEP;
                        _searchUpdate = true;
                        DNDIEGNJOKN.ICKGKDOKJEN();
                    }

                    return false;
                }

                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Z))
                {
                    return _searchString.Length == 0;
                }
                __instance.gFed.GetComponent<Image>().sprite = CEBGDNMEBCE.OBFKPJJILGB[VanillaCounts.NoFeds + 1];
            }
            else
            {
                __instance.gFed.GetComponent<Image>().sprite = CEBGDNMEBCE.OBFKPJJILGB[Characters.c[Characters.foc].fed];
            }
        }

        Characters.no_feds = VanillaCounts.NoFeds + 1;
        if (Characters.fedData.Length <= Characters.no_feds)
        {
            Array.Resize(ref Characters.fedData, Characters.no_feds + 1);
            Characters.fedData[Characters.no_feds] = new Roster();
            Characters.fedData[Characters.no_feds].size = Characters.no_chars;
        }
        return true;
    }

    [HarmonyPatch(typeof(Scene_Select_Char), nameof(Scene_Select_Char.Update))]
    [HarmonyPostfix]
    public static void Scene_Select_Char_Update_Postfix()
    {
        Characters.no_feds = VanillaCounts.NoFeds;
    }

    [HarmonyPatch(typeof(DNDIEGNJOKN), nameof(DNDIEGNJOKN.ICKGKDOKJEN))]
    [HarmonyPostfix]
    public static void DNDIEGNJOKN_ICKGKDOKJEN()
    {
        if (DNDIEGNJOKN.OBNLIIMODBI == 11 && Characters.fed == VanillaCounts.NoFeds + 1)
        {
            DNDIEGNJOKN.LKMAEOFENHG();
            DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ICKGKDOKJEN(2, "\u200BSearch\u200B", 0, 110, 1, 1);
            DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ANONBHFAOEP = _searchString;
            DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].NMKACNOOPPC = 999999999;
            if (_searchUpdate)
            {
                DNDIEGNJOKN.PDMDFGNJCPN = 0;
                _searchUpdate = false;
            }
        }
        else
        {
            _searchString = "";
        }
    }

    [HarmonyPatch(typeof(OHAJKFJEAFN), nameof(OHAJKFJEAFN.IJICAMIHPFF))]
    [HarmonyPrefix]
    public static bool OHAJKFJEAFN_IJICAMIHPFF(ref int __result, OHAJKFJEAFN __instance, float DPDOBMIPMKE, float HONKBFJOEMG, float LICNCGKMLLL)
    {
        if (__instance.AABGEEFANFM.Equals("\u200BSearch\u200B"))
        {
            __result = 0;
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(OHAJKFJEAFN), nameof(OHAJKFJEAFN.DGIBKBFIJJD))]
    [HarmonyPostfix]
    public static void OHAJKFJEAFN_DGIBKBFIJJD(OHAJKFJEAFN __instance)
    {
        if (__instance.AABGEEFANFM.Equals("\u200BSearch\u200B") && DNDIEGNJOKN.PDMDFGNJCPN == 0)
        {
            CEBGDNMEBCE.JAAOOCGLBNN(__instance.KNOKHLBGDKO, DNDIEGNJOKN.HEKGJDJHEOF.r, DNDIEGNJOKN.HEKGJDJHEOF.g, DNDIEGNJOKN.HEKGJDJHEOF.b);
            CEBGDNMEBCE.JAAOOCGLBNN(__instance.CACIMGKEJNH, DNDIEGNJOKN.HLDMPGKEHMN.r, DNDIEGNJOKN.HLDMPGKEHMN.g, DNDIEGNJOKN.HLDMPGKEHMN.b);
            if (__instance.AHPNDLJNCFK == 3)
            {
                CEBGDNMEBCE.JAAOOCGLBNN(__instance.HHLBEIPIEFH, DNDIEGNJOKN.HLDMPGKEHMN.r, DNDIEGNJOKN.HLDMPGKEHMN.g, DNDIEGNJOKN.HLDMPGKEHMN.b);
            }
            if (__instance.MKFBOEIFOCP != null)
            {
                __instance.MKFBOEIFOCP.color = new Color(DNDIEGNJOKN.PBJKLPOIELG.r, DNDIEGNJOKN.PBJKLPOIELG.g, DNDIEGNJOKN.PBJKLPOIELG.b, __instance.MKFBOEIFOCP.color.a);
            }
            if (__instance.FMBJNHIJMFD != null)
            {
                __instance.FMBJNHIJMFD.color = new Color(DNDIEGNJOKN.JCLJCFOMJFN.r, DNDIEGNJOKN.JCLJCFOMJFN.g, DNDIEGNJOKN.JCLJCFOMJFN.b, __instance.FMBJNHIJMFD.color.a);
            }
        }
    }
}