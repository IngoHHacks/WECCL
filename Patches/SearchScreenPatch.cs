using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WECCL.Content;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
internal class SearchScreenPatch
{
    private static int _lastFed = 1;

    private static int _expectedNextId = -1;

    /*
     * Patch:
     * - Enables search screen in the character select menu.
     */
    [HarmonyPatch(typeof(Characters), nameof(Characters.MKFNIFJNLEK))]
    [HarmonyPrefix]
    public static bool Characters_MKFNIFJNLEK(int DLMLPINGCBA, int GMJKGKDFHOH, int ADKBAGHAIGH)
    {
        if (SceneManager.GetActiveScene().name != "Select_Char")
        {
            return true;
        }
        _lastFed = GMJKGKDFHOH;
        _expectedNextId = 0;
        if (GMJKGKDFHOH == VanillaCounts.Data.NoFeds + 1)
        {
            if (_searchString == "")
            {
                UnmappedMenus.JGNMFJLONMA = Characters.no_chars;
                UnmappedMenus.JJKLBHGFJNF = Characters.c.Skip(1).SortBy(DLMLPINGCBA).Select(x => x.id).Prepend(0).ToArray();
                UnmappedMenus.ILEGPMAAJAJ = new int[Characters.no_chars + 1];
            }
            else
            {
                UnmappedMenus.JJKLBHGFJNF = Characters.c.Skip(1).Where(x => x.name.ToLower().Contains(_searchString.ToLower())).SortBy(DLMLPINGCBA).Select(x => x.id).Prepend(0).ToArray();
                UnmappedMenus.JGNMFJLONMA = UnmappedMenus.JJKLBHGFJNF.Length - 1;
                UnmappedMenus.ILEGPMAAJAJ = new int[Characters.no_chars + 1];
                if (UnmappedMenus.JGNMFJLONMA < 25 && DLMLPINGCBA == 0)
                {
                    IEnumerable<Character> c = Characters.c.Skip(1).Where(x => x.name.ToLower().Contains(_searchString.ToLower())).Concat(Characters.c.Skip(1).Where(x => !x.name.ToLower().Contains(_searchString.ToLower())).OrderBy(x => SubstringDamerauLevenshteinDistance(x.name.ToLower(), _searchString.ToLower()))).Take(25);
                    UnmappedMenus.JJKLBHGFJNF = c.Select(x => x.id).Prepend(0).ToArray();
                    UnmappedMenus.JGNMFJLONMA = UnmappedMenus.JJKLBHGFJNF.Length - 1;
                }
            }
            for (int i = 0; i < UnmappedMenus.JJKLBHGFJNF.Length; i++)
            {
                UnmappedMenus.ILEGPMAAJAJ[UnmappedMenus.JJKLBHGFJNF[i]] = i;
            }

            Characters.fedData[GMJKGKDFHOH].size = UnmappedMenus.JGNMFJLONMA;
            return false;
        }
        return true;
    }

    // Damerau Levenshtein distance algorithm from https://programm.top/en/c-sharp/algorithm/damerau-levenshtein-distance/
    private static int Minimum(int a, int b) => a < b ? a : b;
#pragma warning disable Harmony003
    private static int Minimum(int a, int b, int c) => (a = a < b ? a : b) < c ? a : c;
#pragma warning restore Harmony003
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
     * Patch:
     * - Resizes the character editor to fit the roster size if it is larger than 48 (vanilla max)
     */
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.ICGNAJFLAHL))]
    [HarmonyPrefix]
    public static void Menu_ICGNAJFLAHL(int CHMHJJNEMKB, string NMKKHDOGOGA, ref float DPBNKMPJJOJ,
        ref float NKEMECHAEEJ, ref float BGPLCHIKEAK, ref float JOIPMMGOLFI)
    {
        try
        {
            if (CHMHJJNEMKB != 5)
            {
                return;
            }
            
            int fedSize = Characters.fedData[_lastFed].size;
            if (fedSize > 48 || _lastFed == VanillaCounts.Data.NoFeds + 1)
            {
                int actualIndex = (((int)DPBNKMPJJOJ + 525) / 210) + ((-(int)NKEMECHAEEJ + 110) / 60 * 6);
                if (fedSize <= 35)
                {
                    actualIndex = (((int)DPBNKMPJJOJ + 490) / 245) + ((-(int)NKEMECHAEEJ + 110) / 70 * 5);
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
                
                if (_lastFed == VanillaCounts.Data.NoFeds + 1)
                {
                    if (fedSize > 48)
                    {
                        y = 70;
                    }
                    y = 50;
                }
                
                FindBestFit(fedSize, -525, -310, 525, y, out int _, out columns, out scale, out startX, out startY);

                BGPLCHIKEAK = scale;
                JOIPMMGOLFI = scale;
                int itemWidth = fedSize > 35 ? 210 : 245;
                int itemHeight = fedSize > 48 ? 50 : 60;
                DPBNKMPJJOJ = startX + (actualIndex % columns * itemWidth * scale);
                NKEMECHAEEJ = startY - (actualIndex / columns * itemHeight * scale);
            }
        }
        catch (Exception e)
        {
            LogError(e);
        }
    }
    
    /*
     * Patch:
     * - Resets the scale when the menus are removed.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.EHLDKHKMHNG))]
    [HarmonyPrefix]
    public static void Menus_EHLDKHKMHNG()
    {
        PrevScale = 1.0f;
    }
    
    private static string _searchString = "";
    private static bool _searchUpdate = false;

    /*
     * Patch:
     * - Enables search screen in the character select menu.
     */
    [HarmonyPatch(typeof(Scene_Select_Char), nameof(Scene_Select_Char.Update))]
    [HarmonyPrefix]
    public static bool Scene_Select_Char_Update(Scene_Select_Char __instance)
    {
        if (Plugin.EnableWrestlerSearchScreen.Value)
        {
            if (_lastFed == VanillaCounts.Data.NoFeds + 1)
            {
                MappedMenus.no_options = MappedMenus.no_menus - 1;
                if (MappedMenus.foc == MappedMenus.no_menus)
                {
                    MappedMenus.foc = 0;
                }
                HandleKeybinds();
                if (MappedMenus.foc == 0)
                {
                    if (Input.inputString != "" && Input.inputString != "\b")
                    {
                        String str = Input.inputString.Replace("\b", "").Replace("\n", "").Replace("\r", "")
                            .Replace("\t", "")
                            .Replace("\0", "");
                        UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].FFCNPGPALPD += str;
                        _searchString = UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].FFCNPGPALPD;
                        _searchUpdate = true;
                        UnmappedMenus.ICGNAJFLAHL();
                        return false;
                    }

                    if (Input.inputString == "\b" || Input.GetKeyDown(KeyCode.Delete))
                    {
                        if (UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].FFCNPGPALPD.Length > 0)
                        {
                            UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].FFCNPGPALPD = UnmappedMenus
                                .FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].FFCNPGPALPD.Substring(0,
                                    UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].FFCNPGPALPD.Length - 1);
                            _searchString = UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].FFCNPGPALPD;
                            _searchUpdate = true;
                            UnmappedMenus.ICGNAJFLAHL();
                        }

                        return false;
                    }

                    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Z))
                    {
                        return _searchString.Length == 0;
                    }

                    __instance.gFed.GetComponent<Image>().sprite = UnmappedSprites.HJMMBCFGCKA[VanillaCounts.Data.NoFeds + 1];
                }
                else
                {
                    __instance.gFed.GetComponent<Image>().sprite =
                        UnmappedSprites.HJMMBCFGCKA[Characters.c[Characters.foc].fed];
                }
            }

            Characters.no_feds = VanillaCounts.Data.NoFeds + 1;
            if (Characters.fedData.Length <= Characters.no_feds)
            {
                Array.Resize(ref Characters.fedData, Characters.no_feds + 1);
                Characters.fedData[Characters.no_feds] = new Roster();
                Characters.fedData[Characters.no_feds].roster = Characters.c.Select(x => x.id).ToArray();
                Characters.fedData[Characters.no_feds].PIMGMPBCODM(-1);
                Characters.fedData[Characters.no_feds].DCBBAJADDFH();
                Characters.fedData[Characters.no_feds].size = Characters.no_chars;
            }
        }
        return true;
    }
    
    /*
     * Patch:
     * - Makes sure the search screen gets disabled when the roster editor is opened.
     */
    [HarmonyPatch(typeof(Scene_Roster_Editor), nameof(Scene_Roster_Editor.Start))]
    [HarmonyPrefix]
    public static void Scene_Roster_Editor_Start(Scene_Roster_Editor __instance)
    {
        if (MappedCharacters.fed == VanillaCounts.Data.NoFeds + 1) {
            MappedCharacters.fed = VanillaCounts.Data.NoFeds;
        }
    }
    
    private static void HandleKeybinds()
    {
        // Delete
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Delete))
        {
            if (Characters.foc > 0 && MappedMenus.foc > 0)
            {
                if (Characters.no_chars == 1)
                {
                    MappedSound.Play(MappedSound.block);
                    LogInfo("You can't delete the last wrestler!");
                    return;
                }
                
                MappedSound.Play(MappedSound.death[3]);
                LogInfo("Deleting wrestler " + Characters.c[Characters.foc].name);
                CharacterUtils.DeleteCharacter(Characters.foc);
                Characters.foc--;
                MappedMenus.foc--;
                for (int m = 1; m <= MappedPlayers.no_plays; m++)
                {
                    if (Characters.profileChar[m] > 0)
                    {
                        Characters.profileChar[m] = 0;
                    }
                }
                MappedSaveSystem.request = 1;
                MappedMenus.Load();
            }
        }
        // New
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.N))
        {
            MappedSound.Play(MappedSound.tanoy);
            LogInfo("Creating new wrestler");
            CharacterUtils.CreateRandomCharacter();
            MappedSaveSystem.request = 1;
            MappedMenus.Load();
        }
    }

    /*
     * Patch:
     * - Reverts fed size when the roster editor is closed.
     */
    [HarmonyPatch(typeof(UnmappedSprites), nameof(UnmappedSprites.BBLJCJMDDLO))]
    [HarmonyPostfix]
    public static void Sprites_BBLJCJMDDLO()
    {
        Characters.no_feds = VanillaCounts.Data.NoFeds;
    }
    
    private static List<GameObject> _tempObjects = new();

    /*
     * Patch:
     * - Loads menus for the search screen.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.ICGNAJFLAHL))]
    [HarmonyPostfix]
    public static void Menus_ICGNAJFLAHL()
    {
        if (MappedMenus.screen == 11 && Characters.fed == VanillaCounts.Data.NoFeds + 1)
        {
            MappedMenus.Add();
            ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(2, "\u200BSearch\u200B", 0, 110, 1, 1);
            ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).value = _searchString;
            ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).id = 999999999;

            GameObject obj = Object.Instantiate(MappedSprites.gMenu[1]);
            _tempObjects.Add(obj);
            obj.transform.position = new Vector3(350f, 110f, 0f);
            obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 5;
            obj.transform.SetParent(MappedMenus.gDisplay.transform, false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text =
                "Press [Ctrl+DEL] to delete the selected wrestler.";

            obj = Object.Instantiate(MappedSprites.gMenu[1]);
            _tempObjects.Add(obj);
            obj.transform.position = new Vector3(-350f, 110f, 0f);
            obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 5;
            obj.transform.SetParent(MappedMenus.gDisplay.transform, false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text =
                "Press [Ctrl+N] to create a new wrestler.";

            if (_searchUpdate)
            {
                MappedMenus.foc = 0;
                _searchUpdate = false;
            }
        }
        else
        {
            _searchString = "";
            if (_tempObjects.Count > 0)
            {
                foreach (GameObject obj in _tempObjects)
                {
                    if (obj != null)
                    {
                        Object.Destroy(obj);
                    }
                }
                _tempObjects.Clear();
            }
        }

        if (MappedMenus.screen == 2)
        {
            if (MappedMenus.tab == 5)
            {
                MappedMenus.Add();
                ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(2, "Mod", 0, 220, 1.5f, 1.5f);
            }
        }
    }

    /*
     * Patch:
     * - Makes 'Search' unselectable in the character search screen.
     */
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.GBLDMIAPNEP))]
    [HarmonyPrefix]
    public static bool Menu_GBLDMIAPNEP(ref int __result, UnmappedMenu __instance, float MMBJPONJJGM, float EJOKLBHLEEJ, float GJGFOKOEANG)
    {
        if (__instance.NKEDCLBOOMJ.Equals("\u200BSearch\u200B"))
        {
            __result = 0;
            return false;
        }
        return true;
    }
    
    /*
     * Patch:
     * - Tick loop for the search screen, inside menus.
     */
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.BBICLKGGIGB))]
    [HarmonyPostfix]
    public static void Menu_BBICLKGGIGB(UnmappedMenu __instance)
    {
        if (__instance.NKEDCLBOOMJ != null && __instance.NKEDCLBOOMJ.Equals("\u200BSearch\u200B") && UnmappedMenus.NNMDEFLLNBF == 0)
        {
            UnmappedSprites.BBLJCJMDDLO(__instance.MGHGFEHHEBA, UnmappedMenus.DEGLGENADOK.r, UnmappedMenus.DEGLGENADOK.g, UnmappedMenus.DEGLGENADOK.b);
            UnmappedSprites.BBLJCJMDDLO(__instance.KELNLAINAFB, UnmappedMenus.PLIABNOBFDO.r, UnmappedMenus.PLIABNOBFDO.g, UnmappedMenus.PLIABNOBFDO.b);
            if (__instance.BPJFLJPKKJK == 3)
            {
                UnmappedSprites.BBLJCJMDDLO(__instance.GPBKAFJHLML, UnmappedMenus.PLIABNOBFDO.r, UnmappedMenus.PLIABNOBFDO.g, UnmappedMenus.PLIABNOBFDO.b);
            }
            if (__instance.FHOEKMHCCEM != null)
            {
                __instance.FHOEKMHCCEM.color = new Color(UnmappedMenus.DKNOFHAFPHJ.r, UnmappedMenus.DKNOFHAFPHJ.g, UnmappedMenus.DKNOFHAFPHJ.b, __instance.FHOEKMHCCEM.color.a);
            }
            if (__instance.JAFNFBLIALC != null)
            {
                __instance.JAFNFBLIALC.color = new Color(UnmappedMenus.DDPBNKAHLFI.r, UnmappedMenus.DDPBNKAHLFI.g, UnmappedMenus.DDPBNKAHLFI.b, __instance.JAFNFBLIALC.color.a);
            }
        }
    }
    
    /*
     * Patch:
     * - Allows browsing large rosters without skipping rows/columns by reducing controller movement range.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.JCHEJAJJCMJ))]
    [HarmonyTranspiler]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public static IEnumerable<CodeInstruction> Menus_JCHEJAJJCMJ(
        IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 100f)
            {
                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(MenuUtils), nameof(Scale), new Type[] { typeof(float) }));
                yield return new CodeInstruction(OpCodes.Ldc_R4, 100f);
                continue;
            }
            if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 50f)
            {
                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(MenuUtils), nameof(Scale), new Type[] { typeof(float) }));
                yield return new CodeInstruction(OpCodes.Ldc_R4, 50f);
                continue;
            }
            yield return instruction;
        }
    }
}