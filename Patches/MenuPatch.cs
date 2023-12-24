using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using WECCL.Animation;
using WECCL.Content;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
internal class MenuPatch
{
    private static int _lastFed = 1;
    internal static readonly Dictionary<int, Tuple<int, int, float, int, int>> _optimalLayouts = new();

    private static int _expectedNextId = -1;

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
     * Menu.ICGNAJFLAHL is called when the player opens the editor (including the fed editor)
     * This patch is used to resize the character editor to fit the roster size if it is larger than 48 (vanilla max)
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
            Plugin.Log.LogError(e);
        }
    }
    
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.EHLDKHKMHNG))]
    [HarmonyPrefix]
    public static void UnmappedMenus_EHLDKHKMHNG()
    {
        PrevScale = 1.0f;
    }
    
    private static string _searchString = "";
    private static bool _searchUpdate = false;

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
    
    [HarmonyPatch(typeof(Scene_Roster_Editor), nameof(Scene_Roster_Editor.Start))]
    [HarmonyPrefix]
    public static void Scene_Roster_Editor_Start(Scene_Roster_Editor __instance)
    {
        // Make sure the search screen gets disabled when the roster editor is opened
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
                MappedSound.Play(MappedSound.death[3]);
                Plugin.Log.LogInfo("Deleting wrestler " + Characters.c[Characters.foc].name);
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
            Plugin.Log.LogInfo("Creating new wrestler");
            CharacterUtils.CreateRandomCharacter();
            MappedSaveSystem.request = 1;
            MappedMenus.Load();
        }
    }

    [HarmonyPatch(typeof(UnmappedSprites), nameof(UnmappedSprites.BBLJCJMDDLO))]
    [HarmonyPostfix]
    public static void UnmappedSprites_BBLJCJMDDLO()
    {
        Characters.no_feds = VanillaCounts.Data.NoFeds;
    }
    
    private static List<GameObject> _tempObjects = new();

    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.ICGNAJFLAHL))]
    [HarmonyPostfix]
    public static void UnmappedMenus_ICGNAJFLAHL()
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

    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.GBLDMIAPNEP))]
    [HarmonyPrefix]
    public static bool UnmappedMenu_GBLDMIAPNEP(ref int __result, UnmappedMenu __instance, float MMBJPONJJGM, float EJOKLBHLEEJ, float GJGFOKOEANG)
    {
        if (__instance.NKEDCLBOOMJ.Equals("\u200BSearch\u200B"))
        {
            __result = 0;
            return false;
        }
        return true;
    }
    
    [HarmonyPatch(typeof(UnmappedMenu), nameof(UnmappedMenu.BBICLKGGIGB))]
    [HarmonyPostfix]
    public static void UnmappedMenu_BBICLKGGIGB(UnmappedMenu __instance)
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
    
    [HarmonyPatch(typeof(Scene_Editor), nameof(Scene_Editor.Update))]
    [HarmonyPostfix]
    public static void Scene_Editor_Update()
    {
        if (MappedMenus.tab == 1 && MappedMenus.page == 0)
        {
            UnmappedPlayer gMIKIMHFABP = NJBJIIIACEP.OAAMGFLINOB[1];
            Character iPNKFGHIDJP = gMIKIMHFABP.EMDMDLNJFKP;
            if (iPNKFGHIDJP.music > VanillaCounts.Data.MusicCount)
            {
                int index = iPNKFGHIDJP.music - VanillaCounts.Data.MusicCount - 1;
                string name = CustomClips[index].Name;
                UnmappedMenus.FKANHDIMMBJ[8].FFCNPGPALPD = name;
            }
            else if (iPNKFGHIDJP.music == 0)
            {
                UnmappedMenus.FKANHDIMMBJ[8].FFCNPGPALPD = "None";
            }
            else if (CustomClips.Count > 0)
            {
                UnmappedMenus.FKANHDIMMBJ[8].FFCNPGPALPD = "Vanilla " + iPNKFGHIDJP.music;
            }
            UnmappedMenus.BBICLKGGIGB();
            
        }
    }
    
    // Patch to browse large rosters without skipping rows/columns
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.JCHEJAJJCMJ))]
    [HarmonyTranspiler]
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public static IEnumerable<CodeInstruction> UnmappedMenus_JCHEJAJJCMJ_Transpiler(
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

    [HarmonyPatch(typeof(Scene_Options), nameof(Scene_Options.Start))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Scene_Options_Start_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Call && (MethodInfo)instruction.operand == AccessTools.Method(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.LKCDENJNCHL)))
            {
                // +1 to the number of tabs
                yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                yield return new CodeInstruction(OpCodes.Add);
            }
            yield return instruction;
        }
    }

    [HarmonyPatch(typeof(Scene_Options), nameof(Scene_Options.Start))]
    [HarmonyPrefix]
    public static void Scene_Options_Start(Scene_Options __instance)
    {
        var tab = GameObject.Find("Tab00");
        // Duplicate the tab
        var newTab = Object.Instantiate(tab, tab.transform.parent);
        newTab.name = "Tab05";
        newTab.transform.Find("Title").GetComponent<Text>().text = "Mods";
        float diffX = 0;
        for (int i = 0; i < 6; i++)
        {
            var tabObj = GameObject.Find("Tab" + i.ToString("00"));
            tabObj.transform.localScale = new Vector3(tabObj.transform.localScale.x * (5f / 6f),
                tabObj.transform.localScale.y, tabObj.transform.localScale.z);
            if (tab != tabObj)
            {
                if (diffX == 0)
                {
                    var diffO = tabObj.transform.localPosition.x - tab.transform.localPosition.x;
                    diffX = diffO * (5f / 6f);
                    var diffDiffHalf = (diffO - diffX) / 2f;
                    
                    tab.transform.localPosition = new Vector3(tab.transform.localPosition.x - diffDiffHalf,
                        tab.transform.localPosition.y, tab.transform.localPosition.z);
                    
                    tabObj.transform.localPosition = new Vector3(tab.transform.localPosition.x + diffX,
                        tabObj.transform.localPosition.y, tabObj.transform.localPosition.z);
                }
                else
                {
                    tabObj.transform.localPosition = new Vector3(tab.transform.localPosition.x + diffX,
                        tabObj.transform.localPosition.y, tabObj.transform.localPosition.z);
                }
            }
            
            tab = tabObj;
        }
    }

    public static int modSettingSelected = 0;
    public static bool mReset = true;
    public static int editing = -1;
    public static bool commit = false;
    public static string currentString = "";
    public static string prevString = "";
    public static bool getInput = false;
    public static GameObject info;
    
    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.OPPFJDEMLAP))]
    [HarmonyPostfix]
    public static void LIPNHOMGGHF_OPPFJDEMLAP()
    {
        mReset = true;
    }
    
    [HarmonyPatch(typeof(Scene_Options), nameof(Scene_Options.Update))]
    [HarmonyPostfix]
    public static void Scene_Options_Update(Scene_Options __instance)
    {
        if (MappedMenus.tab == 5)
        {
            int oldModSettingSelected = modSettingSelected;
            modSettingSelected = modSettingSelected < 0 ? 0 : Mathf.RoundToInt(((MappedMenu)MappedMenus.menu[1]).ChangeValue(modSettingSelected, 1, 10, 0, AllMods.Instance.NumMods-1, 1));
            ((MappedMenu)MappedMenus.menu[1]).value = AllMods.Instance.Mods[modSettingSelected].Metadata.Name;
            int diff = modSettingSelected - oldModSettingSelected;
            if (diff > 1) diff = -1;
            if (diff < -1) diff = 1;
            var config = AllMods.Instance.Mods[modSettingSelected].Instance.Config;
            if (diff != 0 || mReset)
            {
                mReset = false;
                while (config == null || config.Keys.Count == 0)
                {
                    modSettingSelected += diff;
                    if (modSettingSelected < 0)
                    {
                        modSettingSelected = AllMods.Instance.NumMods - 1;
                    }
                    else if (modSettingSelected >= AllMods.Instance.NumMods)
                    {
                        modSettingSelected = 0;
                    }

                    config = AllMods.Instance.Mods[modSettingSelected].Instance.Config;
                }

                if (MappedMenus.no_menus > 1)
                {
                    for (int i = 2; i <= MappedMenus.no_menus; i++)
                    {
                        var box = ((MappedMenu)MappedMenus.menu[i]).box;
                        if (box != null)
                        {
                            Object.Destroy(box);
                        }
                    }
                    MappedMenus.no_menus = 1;
                }
                
                int minX = -500;
                int maxX = 500;
                int minY = -160;
                int maxY = 110;
                int columns;
                float scale;
                int startX;
                int startY;
                FindBestFit(config.Keys.Count, minX, minY, maxX, maxY, out int rows, out columns, out scale, out startX, out startY);
                int nMaxX = (int)(startX + ((columns-1) * 210f * scale));
                var sz = nMaxX - startX;
                startX = (int)(-sz / 2f);
                foreach (var pair in config)
                {
                    float x = startX + ((MappedMenus.no_menus - 1) % (float) columns * 210f * scale);
                    // ReSharper disable once PossibleLossOfFraction
                    float y = startY - ((MappedMenus.no_menus - 1) / columns * 60f * scale);
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(2, pair.Key.Key, x, y, scale, scale);
                    var v = pair.Value.BoxedValue;
                    if (v is bool b)
                    {
                        ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).value = b ? "On" : "Off";
                    }
                    else
                    {
                        ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).value = v.ToString();
                    }
                }
                
                if (info == null)
                {
                    info = Object.Instantiate(MappedSprites.gMenu[1]);
                    info.transform.position = new Vector3(0f, -280f, 0f);
                    info.transform.localScale = new Vector3(1f, 1f, 1f);
                    RectTransform rt = info.transform.Find("Title").transform as RectTransform;
                    rt.sizeDelta *= 5;
                    info.transform.SetParent(MappedMenus.gDisplay.transform, false);
                    Object.Destroy(info.transform.Find("Background").gameObject);
                    Object.Destroy(info.transform.Find("Border").gameObject);
                    Object.Destroy(info.transform.Find("Sheen").gameObject);
                    Object.Destroy(info.transform.Find("Corners").gameObject);
                    info.transform.Find("Title").gameObject.GetComponent<Text>().text = "";
                }
            }
            
            for (int i = 2; i <= MappedMenus.no_menus; i++)
            {
                if (MappedMenus.foc == i)
                {
                    if (info != null)
                    {
                        var def = config[config.Keys.ToList()[i - 2]].DefaultValue == null
                            ? "None"
                            : config[config.Keys.ToList()[i - 2]].DefaultValue;
                        
                        if (def is bool b)
                        {
                            def = b ? "On" : "Off";
                        }
                        else
                        {
                            def = def.ToString();
                        }

                        info.transform.Find("Title").gameObject.GetComponent<Text>().text =
                            config[config.Keys.ToList()[i - 2]].Description.Description + "\nDefault: " +
                            def;
                    }

                    bool soft = false;
                    var type = config[config.Keys.ToList()[i - 2]].SettingType;
                    if (editing == -1)
                    {
                        if (type == typeof(bool))
                        {
                            bool b = (bool)config[config.Keys.ToList()[i - 2]].BoxedValue;
                            int current = b ? 1 : 0;
                            current = Mathf.RoundToInt(
                                ((MappedMenu)MappedMenus.menu[i]).ChangeValue(current, 1, 10, 0, 1, 1));
                            config[config.Keys.ToList()[i - 2]].BoxedValue = current == 1;
                            ((MappedMenu)MappedMenus.menu[i]).value =
                                (bool) config[config.Keys.ToList()[i - 2]].BoxedValue ? "On" : "Off";
                            soft = true;
                        }
                        else if (type == typeof(int))
                        {
                            int min = int.MinValue;
                            int max = int.MaxValue;
                            if (config[config.Keys.ToList()[i - 2]].Description.AcceptableValues is
                                AcceptableValueRange<int> range)
                            {
                                min = range.MinValue;
                                max = range.MaxValue;
                            }

                            int current = (int)config[config.Keys.ToList()[i - 2]].BoxedValue;
                            current = Mathf.RoundToInt(
                                ((MappedMenu)MappedMenus.menu[i]).ChangeValue(current, 1, 10, min, max, 1));
                            config[config.Keys.ToList()[i - 2]].BoxedValue = current;
                            ((MappedMenu)MappedMenus.menu[i]).value =
                                config[config.Keys.ToList()[i - 2]].BoxedValue.ToString();
                            soft = true;
                        }
                        else if (type == typeof(float))
                        {
                            float min = -float.MaxValue;
                            float max = float.MaxValue;
                            if (config[config.Keys.ToList()[i - 2]].Description.AcceptableValues is
                                AcceptableValueRange<float> range)
                            {
                                min = range.MinValue;
                                max = range.MaxValue;
                            }

                            float def = config[config.Keys.ToList()[i - 2]].DefaultValue == null
                                ? 0
                                : (float)config[config.Keys.ToList()[i - 2]].DefaultValue;
                            int defDec = !def.ToString(CultureInfo.CurrentCulture).Contains(".")
                                ? 0
                                : def.ToString(CultureInfo.CurrentCulture).Split('.')[1].Length;
                            int dec = Math.Max(2, defDec);
                            float inc = (float)Math.Round(1f / Math.Pow(10, dec), dec);

                            float current = (float)config[config.Keys.ToList()[i - 2]].BoxedValue;
                            current = ((MappedMenu)MappedMenus.menu[i]).ChangeValue(current, inc, 10, min, max, 1);
                            current = (float)Math.Round(current, dec);
                            config[config.Keys.ToList()[i - 2]].BoxedValue = current;
                            ((MappedMenu)MappedMenus.menu[i]).value =
                                config[config.Keys.ToList()[i - 2]].BoxedValue.ToString();
                            soft = true;
                        }
                        else if (type == typeof(string))
                        {
                            if (config[config.Keys.ToList()[i - 2]].Description.AcceptableValues is
                                AcceptableValueList<string> list)
                            {
                                var strings = list.AcceptableValues.ToList();
                                int current = strings.IndexOf((string)config[config.Keys.ToList()[i - 2]].BoxedValue);
                                current = Mathf.RoundToInt(
                                    ((MappedMenu)MappedMenus.menu[i]).ChangeValue(current, 1, 10, 0, strings.Count - 1, 1));
                                config[config.Keys.ToList()[i - 2]].BoxedValue = strings[current];
                                ((MappedMenu)MappedMenus.menu[i]).value =
                                    config[config.Keys.ToList()[i - 2]].BoxedValue.ToString();
                                soft = true;
                            }
                        }
                        else if (type == typeof(KeyCode))
                        {
                            getInput = true;
                        }
                        else if (type.IsEnum)
                        {
                            var strings = Enum.GetNames(type);
                            if (config[config.Keys.ToList()[i - 2]].Description.AcceptableValues is
                                AcceptableValueList<string> list)
                            {
                                strings = list.AcceptableValues.ToArray();
                            }

                            int current = Array.IndexOf(strings, config[config.Keys.ToList()[i - 2]].BoxedValue.ToString());
                            current = Mathf.RoundToInt(
                                ((MappedMenu)MappedMenus.menu[i]).ChangeValue(current, 1, 10, 0, strings.Length - 1, 1));
                            config[config.Keys.ToList()[i - 2]].BoxedValue = Enum.Parse(type, strings[current], true);
                            ((MappedMenu)MappedMenus.menu[i]).value =
                                config[config.Keys.ToList()[i - 2]].BoxedValue.ToString();
                            soft = true;
                        }
                        prevString = ((MappedMenu)MappedMenus.menu[i]).value;
                    }
                    currentString = ((MappedMenu)MappedMenus.menu[i]).value;
                    // Keyboard input
                    if (getInput && editing == -1)
                    {
                        if (type != typeof(KeyCode))
                        {
                            getInput = false;
                        }
                        else
                        {
                            if (Input.GetKeyDown(KeyCode.Escape))
                            {
                                getInput = false;
                            }
                            else
                            {
                                KeyCode key = KeyCode.None;
                                bool erase = false;
                                if (Input.anyKeyDown)
                                {
                                    foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                                    {
                                        if (Input.GetKeyDown(kcode))
                                        {
                                            if (kcode >= KeyCode.Mouse0 && kcode <= KeyCode.Mouse6)
                                            {
                                                continue;
                                            }
                                            if (kcode == KeyCode.Delete)
                                            {
                                                key = KeyCode.None;
                                                erase = true;
                                                break;
                                            }
                                            if (kcode == KeyCode.Insert)
                                            {
                                                editing = i;
                                                break;
                                            }
                                            key = kcode;
                                            break;
                                        }
                                    }
                                }
                                if (key != KeyCode.None || erase)
                                {
                                    config[config.Keys.ToList()[i - 2]].BoxedValue = key;
                                    ((MappedMenu)MappedMenus.menu[i]).value =
                                        config[config.Keys.ToList()[i - 2]].BoxedValue.ToString();
                                    getInput = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                        {
                            commit = true;
                        }
                        else if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            currentString = prevString;
                            commit = true;
                        }
                        else
                        {
                            string str = GetKeyboardInput(currentString, soft && editing == -1);
                            if (currentString != str)
                            {
                                currentString = str;
                                editing = i;
                                ((MappedMenu)MappedMenus.menu[i]).value = currentString;
                            }
                        }
                    }
                }
                if (editing == i)
                {
                    var c = config[config.Keys.ToList()[i - 2]];
                    if (TypeUtils.IsValid(c.SettingType, currentString) && IsAllowed(c, currentString))
                    {
                        MappedSprites.ChangeColour(((MappedMenu)MappedMenus.menu[i]).gBackground, 0.5f, 0.5f, 0.8f);
                    }
                    else
                    {
                        MappedSprites.ChangeColour(((MappedMenu)MappedMenus.menu[i]).gBackground, 0.8f, 0.5f, 0.5f);
                    }
                }
                else
                {
                    MappedSprites.ChangeColour(((MappedMenu)MappedMenus.menu[i]).gBackground, 0.8f, 0.8f, 0.8f);
                }

                if (editing != MappedMenus.foc || commit)
                {
                    if (editing != -1)
                    {

                        var type = config[config.Keys.ToList()[editing - 2]].SettingType;
                        if (type == typeof(bool))
                        {
                            var r = AnimationParser.ParseBool(currentString);
                            if (r.Result)
                            {
                                config[config.Keys.ToList()[editing - 2]].BoxedValue = r.Value;
                                ((MappedMenu)MappedMenus.menu[editing]).value =
                                    (bool) config[config.Keys.ToList()[editing - 2]].BoxedValue ? "On" : "Off";
                            }
                            else
                            {
                                ((MappedMenu)MappedMenus.menu[editing]).value = prevString;
                            }
                        }
                        else if (type == typeof(int))
                        {
                            var r = AnimationParser.ParseInt(currentString);
                            if (r.Result)
                            {
                                config[config.Keys.ToList()[editing - 2]].BoxedValue = r.Value;
                                ((MappedMenu)MappedMenus.menu[editing]).value =
                                    config[config.Keys.ToList()[editing - 2]].BoxedValue.ToString();
                            }
                            else
                            {
                                ((MappedMenu)MappedMenus.menu[editing]).value = prevString;
                            }
                        }
                        else if (type == typeof(float))
                        {
                            var r = AnimationParser.ParseFloat(currentString);
                            if (r.Result)
                            {
                                config[config.Keys.ToList()[editing - 2]].BoxedValue = r.Value;
                                ((MappedMenu)MappedMenus.menu[editing]).value =
                                    config[config.Keys.ToList()[editing - 2]].BoxedValue.ToString();
                            }
                            else
                            {
                                ((MappedMenu)MappedMenus.menu[editing]).value = prevString;
                            }
                        }
                        else if (type == typeof(string))
                        {
                            config[config.Keys.ToList()[editing - 2]].BoxedValue = currentString;
                            ((MappedMenu)MappedMenus.menu[editing]).value =
                                config[config.Keys.ToList()[editing - 2]].BoxedValue.ToString();
                        }
                        else if (type.IsEnum)
                        {
                            try
                            {
                                config[config.Keys.ToList()[editing - 2]].BoxedValue = Enum.Parse(type, currentString, true);
                                ((MappedMenu)MappedMenus.menu[editing]).value =
                                    config[config.Keys.ToList()[editing - 2]].BoxedValue.ToString();
                            }
                            catch (Exception)
                            {
                                ((MappedMenu)MappedMenus.menu[editing]).value = prevString;
                            }
                        }
                        editing = -1;
                    }
                    commit = false;
                }
            }
        }
        else
        {
            mReset = true;
            if (info != null)
            {
                Object.Destroy(info);
            }
        }
    }

    private static bool IsAllowed(ConfigEntryBase configEntryBase, string s)
    {
        if (configEntryBase.Description.AcceptableValues is AcceptableValueList<string> list)
        {
            return list.AcceptableValues.Contains(s);
        }
        if (configEntryBase.Description.AcceptableValues is AcceptableValueRange<int> range)
        {
            return int.TryParse(s, out int i) && i >= range.MinValue && i <= range.MaxValue;
        }
        if (configEntryBase.Description.AcceptableValues is AcceptableValueRange<float> range2)
        {
            return float.TryParse(s, out float f) && f >= range2.MinValue && f <= range2.MaxValue;
        }
        return true;
    }

    private static string GetKeyboardInput(string s, bool soft)
    {
        if ((Input.inputString == "\b" || Input.GetKeyDown(KeyCode.Delete)) && s.Length > 0)
        {
            return s.Substring(0, s.Length - 1);
        }
        if (soft) return s;
        if (Input.inputString != "" && Input.inputString != "\b")
        {
            String str = Input.inputString.Replace("\b", "").Replace("\n", "").Replace("\r", "")
                .Replace("\t", "")
                .Replace("\0", "");
            return s + str;
        }
        return s;
    }
    
    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.MOKABGFFGLC))]
    [HarmonyPostfix]
    public static void LIPNHOMGGHF_MOKABGFFGLC()
    {
         var text = GameObject.Find("Version").GetComponent<Text>(); 
         text.text = "WECCL " + Plugin.PluginVerLong + "\t\t Game " + text.text;
         text.horizontalOverflow = HorizontalWrapMode.Overflow;
    }

    private static int rcFoc = 0;

    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.PIELJFKJFKF))]
    [HarmonyPostfix]
    public static void LIPNHOMGGHF_PIELJFKJFKF()
    {
        rcFoc = 0;
        MappedController controller = MappedControls.pad[MappedControls.host];
        for (MappedMenus.cyc = 1; MappedMenus.cyc <= MappedMenus.no_menus; MappedMenus.cyc++)
        {
            if (Input.GetMouseButton((int)MouseButton.RightMouse))
            {
                MappedMenu menu = MappedMenus.menu[MappedMenus.cyc];
                var clickX = Input.mousePosition.x;
                var clickY = Input.mousePosition.y;
                if (menu.Inside(clickX, clickY, 10f) <= 0 || MappedKeyboard.preventInput != 0)
                {
                    continue;
                }
                rcFoc = menu.id;
                MappedMenus.foc = rcFoc;
            }
            else if (MappedMenus.Control() > 0 && controller.type > 1 && MappedMenus.cyc == MappedMenus.foc)
            {
                if (controller.button[1] > 0)
                {
                    rcFoc = MappedMenus.foc;
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(Scene_Editor), nameof(Scene_Editor.Update))]
    [HarmonyPostfix]
    public static void Scene_Editor_Update(Scene_Editor __instance)
    {
        if (rcFoc > 0 && MappedMenus.page == 0 && MappedMenus.foc == 8 && MappedMenus.tab == 1)
        {
            MappedSound.Play(MappedSound.proceed, 1f);
            MappedMenus.tabOldFoc[MappedMenus.tab] = MappedMenus.foc;
            MappedMenus.CreateAlphabet();
        }
    }
    
    [HarmonyPatch(typeof(Scene_Editor), nameof(Scene_Editor.Update))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Scene_Editor_Update_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction prev = null;
        CodeInstruction prev2 = null;
        foreach (CodeInstruction instruction in instructions)
        {
            yield return instruction;
            if (prev2 != null) {
                if (prev2.opcode == OpCodes.Ldsfld && (FieldInfo)prev2.operand ==
                    AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.CHLJMEPFJOK)))
                {
                    if (prev.opcode == OpCodes.Ldc_I4_1)
                    {
                        yield return new CodeInstruction(OpCodes.Ldsfld,
                            AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.ODOAPLMOJPD)));
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                        yield return new CodeInstruction(instruction);
                    }
                } else if (prev2.opcode == OpCodes.Ldsfld && (FieldInfo)prev2.operand ==
                    AccessTools.Field(typeof(CHLPMKEGJBJ), nameof(CHLPMKEGJBJ.CNNKEACKKCD)))
                {
                    if (prev.opcode == OpCodes.Ldsfld && (FieldInfo)prev.operand ==
                        AccessTools.Field(typeof(CHLPMKEGJBJ), nameof(CHLPMKEGJBJ.GEDDILDLILI)))
                    {
                        yield return new CodeInstruction(OpCodes.Ldsfld,
                            AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.ODOAPLMOJPD)));
                        yield return new CodeInstruction(OpCodes.Ldc_I4_M1);
                        yield return new CodeInstruction(instruction);
                    }
                }
            } 
            prev2 = prev;
            prev = instruction;
        }
    }
    
    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.ICGNAJFLAHL))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> LIPNHOMGGHF_ICGNAJFLAHL_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction prev = null;
        CodeInstruction prev2 = null;
        int screen = 0;
        foreach (CodeInstruction instruction in instructions)
        {
            yield return instruction;
            if (prev != null && prev.opcode == OpCodes.Ldsfld && (FieldInfo)prev.operand ==
                AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.FAKHAFKOBPB)))
            {
                if (instruction.opcode == OpCodes.Ldc_I4_S)
                {
                    screen = (sbyte)instruction.operand;
                }
            }
            else if (screen == 60)
            {
                if (prev2 != null && prev2.opcode == OpCodes.Ldsfld && (FieldInfo)prev2.operand ==
                    AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.CHLJMEPFJOK)))
                {
                    if (prev.opcode == OpCodes.Ldc_I4_1)
                    {
                        yield return new CodeInstruction(OpCodes.Ldsfld,
                            AccessTools.Field(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.ODOAPLMOJPD)));
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                        yield return new CodeInstruction(instruction);
                    }
                }
            }
            prev2 = prev;
            prev = instruction;
        }
    }
    
    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.BCGNJIGEDBM))]
    [HarmonyPrefix]
    public static void LIPNHOMGGHF_BCGNJIGEDBM()
    {
        MappedMenus.listReturnPage = MappedMenus.page;
        //MappedMenus.listReturnFoc = MappedMenus.foc;
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1)
        {
            MappedMenus.listSource = Enumerable.Range(0, MappedSound.no_themes).ToArray();
        }
    }
    
    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.LFEHAGOEBNK))]
    [HarmonyPrefix]
    public static bool LIPNHOMGGHF_LFEHAGOEBNK(ref string __result, int KJELLNJFNGO)
    {
        var cyc = KJELLNJFNGO;
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1)
        {
            if (cyc > VanillaCounts.Data.MusicCount)
            {
                int index = cyc - VanillaCounts.Data.MusicCount - 1;
                string name = CustomClips[index].Name;
                __result = name;
                return false;
            }
            if (cyc == 0)
            {
                __result = "None";
                return false;
            }
            if (CustomClips.Count > 0)
            {
                __result = "Vanilla " + cyc;
                return false;
            }
        }
        return true;
    }
    
    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.CFOIMDIGPDC))]
    [HarmonyPrefix]
    public static bool LIPNHOMGGHF_CFOIMDIGPDC(ref int __result, int GOOKPABIPBC)
    {
        var charID = GOOKPABIPBC;
        if (charID <= 0)
        {
            return true;
        }
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1)
        {
            MappedCharacter character = MappedCharacters.c[charID];
            for (int i = 0; i <= MappedMenus.listSize; i++)
            {
                var id = MappedMenus.listID[i];
                if (id == character.music)
                {
                    __result = i;
                    return false;
                }
            }
        }
        return true;
    }
    
    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.EEIPIFGEDNP))]
    [HarmonyPrefix]
    public static bool LIPNHOMGGHF_EEIPIFGEDNP(int GOOKPABIPBC)
    {
        var charID = GOOKPABIPBC;
        if (charID <= 0)
        {
            return true;
        }
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1)
        {
            MappedCharacter character = MappedCharacters.c[charID];
            character.music = MappedMenus.listID[MappedMenus.listFoc];
        }
        return true;
    }
    
    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.OGOMMBJBBDB))]
    [HarmonyPostfix]
    public static void LIPNHOMGGHF_OGOMMBJBBDB()
    {
        var d = Input.mouseScrollDelta;
        if (d.y != 0)
        {
            MappedMenus.scrollSpeedY -= 20f * d.y / MappedGlobals.resY;
            MappedMenus.scrollDelay = 200f;
        }
    }
    
    [HarmonyPatch(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.OGOMMBJBBDB))]
    [HarmonyPostfix]
    public static void LIPNHOMGGHF_OGOMMBJBBDB_Post()
    {
        if (MappedMenus.listReturnPage == 0 && MappedMenus.tab == 1)
        {
            var music = MappedMenus.listID[MappedMenus.listFoc];
            if (MappedSound.musicPlaying != music)
            {
                if (MappedSound.musicPlaying == MappedSound.musicMain)
                {
                    Debug.Log("Interrupting main theme at " + MappedSound.musicChannel.time);
                    MappedSound.musicRestore = MappedSound.musicChannel.time;
                }
                MappedSound.PlayMusic(music, Characters.c[Characters.edit].musicSpeed, (MappedSound.musicVol + MappedSound.soundVol) / 2f);
            }
        }
    }
}