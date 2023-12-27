using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine.UI;
using WECCL.Animation;
using WECCL.API;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
internal class ModTabPatch
{
    /*
     * Patch:
     * - Adds another tab to the options menu.
     */
    [HarmonyPatch(typeof(Scene_Options), nameof(Scene_Options.Start))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Scene_Options_Start(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Call && (MethodInfo)instruction.operand == AccessTools.Method(typeof(LIPNHOMGGHF), nameof(LIPNHOMGGHF.LKCDENJNCHL)))
            {
                yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                yield return new CodeInstruction(OpCodes.Add);
            }
            yield return instruction;
        }
    }

    /*
     * Patch:
     * - Applies the new tab to the options menu as 'Mods'.
     */
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
    
    /*
     * Patch:
     * - Resets mod tab when tab is changed.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.OPPFJDEMLAP))]
    [HarmonyPostfix]
    public static void Menus_OPPFJDEMLAP()
    {
        mReset = true;
    }
    
    /*
     * Patch:
     * - Tick loop for the mod tab.
     */
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
            if (mReset)
            {
                modSettingSelected = 0;
                diff = 1;
                mReset = false;
            }

            var m = AllMods.Instance.Mods[modSettingSelected];
            var config = m.Instance.Config;
            if (diff != 0)
            {
                int infiniteLoopPreventer = 0;
                while (!Buttons.CustomButtons.ContainsKey(m.Metadata.Name) && (config == null || config.Keys.Count == 0) && infiniteLoopPreventer++ < AllMods.Instance.NumMods)
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
                var btnCount = Buttons.CustomButtons.TryGetValue(m.Metadata.Name, out List<Buttons.Button> customButton) ? customButton.Count : 0;
                FindBestFit(config.Keys.Count + btnCount, minX, minY, maxX, maxY, out int rows, out columns, out scale, out startX, out startY);
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
                if (Buttons.CustomButtons.ContainsKey(m.Metadata.Name) && Buttons.CustomButtons[m.Metadata.Name].Count > 0)
                {
                    foreach (var button in Buttons.CustomButtons[m.Metadata.Name])
                    {
                        float x = startX + ((MappedMenus.no_menus - 1) % (float) columns * 210f * scale);
                        // ReSharper disable once PossibleLossOfFraction
                        float y = startY - ((MappedMenus.no_menus - 1) / columns * 60f * scale);
                        MappedMenus.Add();
                        ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(1, button.Text, x, y, scale, scale);
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

            info.transform.Find("Title").gameObject.GetComponent<Text>().text = "";
            
            for (int i = 2; i <= MappedMenus.no_menus; i++)
            {
                if (((MappedMenu)MappedMenus.menu[i]).type == 1)
                {
                    Buttons.Button b = Buttons.CustomButtons[m.Metadata.Name][i - 2 - config.Keys.Count];
                    ((MappedMenu)MappedMenus.menu[i]).title = b.Text;
                    b.Update();
                }
                if (MappedMenus.foc == i)
                {
                    if (((MappedMenu)MappedMenus.menu[i]).type == 1)
                    {
                        Buttons.Button b = Buttons.CustomButtons[m.Metadata.Name][i - 2 - config.Keys.Count];
                        if (((MappedMenu)MappedMenus.menu[i]).clicked != 0 && b.Active)
                        {
                            b.Invoke();
                        }
                        continue;
                    }
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
                else if (((MappedMenu)MappedMenus.menu[i]).type == 1)
                {
                    Buttons.Button b = Buttons.CustomButtons[m.Metadata.Name][i - 2 - config.Keys.Count];
                    b.Reset();
                }
                if (editing == i && ((MappedMenu)MappedMenus.menu[i]).type == 2)
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
}