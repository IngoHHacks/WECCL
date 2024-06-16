using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using WECCL.Saves;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
public class ScreenPatch
{
    private static bool _initialized;
    private static int _delay;
    private static int _dd = 10;
    private static int _foc;
    private static int _dir;
    private static MappedMenu _menu;
    private static bool _newScreen;
    
    private static int? _nextTitleScreenId;
    private static int? _nextScreenId;

    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.PMIIOCMHEAE))]
    [HarmonyPrefix]
    public static void Menus_PMIIOCMHEAE_Pre(ref int KBEAJEIMNMI)
    {
        if (StackTracePatch.HasException)
        {
            _nextScreenId = KBEAJEIMNMI;
            _nextTitleScreenId = 1003;
            StackTracePatch.HasException = false;
            KBEAJEIMNMI = 1;
        }
    }

    /*
     * Patch:
     * - Adds a screen to warn the user about potential issues with using mods.
     * - Adds a screen to set the priority of override mods.
     */
    [HarmonyPatch(typeof(Scene_Titles), nameof(Scene_Titles.Start))]
    [HarmonyPostfix]
    public static void SceneTitles_Start(Scene_Titles __instance)
    {
        _foc = 0;
        _dir = 0;
        _delay = 0;
        _dd = 10;
        if (_nextTitleScreenId != null)
        {
            switch (_nextTitleScreenId)
            {
                case 1003:
                    MappedMenus.screen = _nextTitleScreenId.Value;
                    _nextTitleScreenId = null;
                    MappedMenus.tab = 0;
                    MappedMenus.page = 0;
                    MappedMenus.Load();
                    __instance.gLicense.SetActive(false);
                    MappedMenus.gBackArrow.SetActive(false);
                    Object.Destroy(GameObject.Find("Logo"));
                    GameObject obj = Object.Instantiate(MappedSprites.gMenu[1]);
                    obj.transform.position = new Vector3(0f, 50f, 0f);
                    obj.transform.localScale = new Vector3(1f, 1f, 1f);
                    RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
                    rt.sizeDelta *= 5;
                    obj.transform.SetParent(MappedMenus.gDisplay.transform, false);
                    Object.Destroy(obj.transform.Find("Background").gameObject);
                    Object.Destroy(obj.transform.Find("Border").gameObject);
                    Object.Destroy(obj.transform.Find("Sheen").gameObject);
                    Object.Destroy(obj.transform.Find("Corners").gameObject);
                    var text = "<color=#EF0000>EXCEPTION</color>\nAn exception has occurred " + Describe(StackTracePatch.ExceptionScreen) + ".\nPlease check the log file for more information and report it to the mod author if necessary.\n" +
                               "You can try to continue playing, but the game may not function correctly. Click the button below to continue.\n\nException:\n" +
                               StackTracePatch.ExceptionMessage;
                    if (StackTracePatch.ExceptionScreen == "Save")
                    {
                        text += "\n\n<color=#EF0000>WARNING</color>\nThe error occurred while saving the game. It is recommended to load a backup save file, as the save file may be corrupted.";
                    } else if (StackTracePatch.ExceptionScreen == "Loading")
                    {
                        text += "\n\n<color=#EF0000>WARNING</color>\nThis error occurred while loading the game. This may cause the save file to not be loaded correctly, so continuing is highly discouraged.";
                    }
                    obj.transform.Find("Title").gameObject.GetComponent<Text>().text = text;
                        
                    break;
                default:
                    MappedMenus.screen = _nextTitleScreenId.Value;
                    MappedMenus.tab = 0;
                    MappedMenus.page = 0;
                    MappedMenus.Load();
                    __instance.gLicense.SetActive(false);
                    MappedMenus.gBackArrow.SetActive(false);
                    Object.Destroy(GameObject.Find("Logo"));
                    GameObject obj2 = Object.Instantiate(MappedSprites.gMenu[1]);
                    obj2.transform.position = new Vector3(0f, 50f, 0f);
                    obj2.transform.localScale = new Vector3(1f, 1f, 1f);
                    RectTransform rt2 = obj2.transform.Find("Title").transform as RectTransform;
                    rt2.sizeDelta *= 5;
                    obj2.transform.SetParent(MappedMenus.gDisplay.transform, false);
                    Object.Destroy(obj2.transform.Find("Background").gameObject);
                    Object.Destroy(obj2.transform.Find("Border").gameObject);
                    Object.Destroy(obj2.transform.Find("Sheen").gameObject);
                    Object.Destroy(obj2.transform.Find("Corners").gameObject);
                    obj2.transform.Find("Title").gameObject.GetComponent<Text>().text =
                        "<color=#EF0000>NO SCREEN</color>\nThis screen does not exist! Screen ID: " +
                        _nextTitleScreenId;
                    _nextTitleScreenId = null;
                    break;
            }
        }
        else if (MetaFile.Data.FirstLaunch)
        {
            MappedMenus.RemoveExisting();
            MappedMenus.screen = 1002;
            MappedMenus.tab = 0;
            MappedMenus.page = 0;
            MappedMenus.Load();
            __instance.gLicense.SetActive(false);
            MappedMenus.gBackArrow.SetActive(false);
            Object.Destroy(GameObject.Find("Logo"));
            GameObject obj = Object.Instantiate(MappedSprites.gMenu[1]);
            obj.transform.position = new Vector3(0f, 50f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 5;
            obj.transform.SetParent(MappedMenus.gDisplay.transform, false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text =
                "<color=#EF0000>IMPORTANT NOTICE</color>\nWhile WECCL tries its best to keep modded save files stable and consistent between game updates and mod changes, you may still encounter issues. WECCL automatically creates backups (up to 100 by default). However, it is recommended to manually create backups of your save file. The save file can be found in %USERPROFILE%/AppData/LocalLow/MDickie/Wrestling Empire. If you encounter any issues, please report them in the Wrestling Empire Modding Discord server.";
        }
        else if (!_initialized && HasConflictingOverrides && !MetaFile.Data.HidePriorityScreenNextTime)
        {
            MappedMenus.RemoveExisting();
            MappedMenus.screen = 1001;
            MappedMenus.tab = 0;
            MappedMenus.page = 0;
            MappedMenus.Load();
            __instance.gLicense.SetActive(false);
            MappedMenus.gBackArrow.SetActive(false);
            Object.Destroy(GameObject.Find("Logo"));

            GameObject obj = Object.Instantiate(UnmappedSprites.IMPJPDIEKDF[1]);
            obj.transform.position = new Vector3(0f, 315f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 2;
            obj.transform.SetParent(MappedMenus.gDisplay.transform, false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text =
                "Order the override mods by the desired priority:";


            _initialized = true;
        }
    }

    private static string Describe(string screen)
    {
        switch (screen)
        {
            case "Loading":
                return "when loading the game";
            case "Titles":
                return "in the title screen";
            case "Trophies":
                return "in the achievements screen";
            case "Select_Character":
                return "in the character selection screen";
            case "Roster_Editor":
                return "in the federation editor screen";
            case "Game":
                return "during gameplay";
            case "Save":
                return "when saving the game";
            default:
                return "in the " + screen.ToLower() + " screen";
        }
    }

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    /*
     * Patch:
     * - Tick loop for the extra screens in Titles.
     */
    [HarmonyPatch(typeof(Scene_Titles), nameof(Scene_Titles.Update))]
    [HarmonyPrefix]
    public static bool SceneTitles_Update()
    {
        try
        {
            if (MappedMenus.screen > 1000)
            {
                MappedControls.GetInput();
                MappedMenus.FindClicks();
                if (_newScreen)
                {
                    bool flag = false;
                    for (MappedMenus.cyc = 1;
                         MappedMenus.cyc <= MappedMenus.no_menus;
                         MappedMenus.cyc++)
                    {
                            if (((MappedMenu)MappedMenus.menu[MappedMenus.cyc]).clicked != 0)
                            {
                                ((MappedMenu)MappedMenus.menu[MappedMenus.cyc]).clicked = 0;
                                flag = true;
                            }
                    }
                    if (!flag)
                    {
                        _newScreen = false;
                    }
                }
            }
            if (MappedMenus.screen == 1001)
            {
                MappedMenus.UpdateDisplay();
                if (MappedMenus.foc > 0 && ((MappedMenu)MappedMenus.menu[MappedMenus.foc]).clicked != 0)
                {
                    _foc = MappedMenus.foc;
                }
                else if (MappedMenus.foc > 0 && _menu != null)
                {
                    _foc = 0;
                    _dd = 10;
                    _dir = 0;
                    MappedSprites.ChangeColour(_menu.gBackground, 0.8f, 0.8f, 0.8f);
                    _menu = null;
                }

                if (_foc >= Prefixes.Count + 1)
                {
                    if (_foc == Prefixes.Count + 2)
                    {
                        MetaFile.Data.HidePriorityScreenNextTime = true;
                    }

                    MappedSound.Play(MappedSound.proceed, 0f, 0.5f);
                    List<string> prefixes = new();
                    for (int i = 1; i < Prefixes.Count + 1; i++)
                    {
                        string text = ((MappedMenu)MappedMenus.menu[i]).value;
                        prefixes.Add(text);
                    }

                    Prefixes = prefixes;
                    SavePrefixes();
                    MappedSound.musicChannel.Stop();
                    MappedSound.musicFile[0] = MappedGlobals.LoadAsset("Music", "Theme00") as AudioClip;
                    MappedSound.musicChannel.clip = MappedSound.musicFile[0];
                    MappedSound.musicChannel.time = 0f;
                    MappedSound.musicChannel.Play();
                    MappedMenus.ChangeScreen(1);
                }
                else if (_foc > 0 && _delay <= 0)
                {
                    MappedMenu menu = MappedMenus.menu[_foc];
                    if (_dir == 0)
                    {
                        _dir = menu.clicked >= 0 ? 1 : -1;
                    }
                    MappedController controller = MappedControls.pad[MappedControls.host];
                    if (_dir < 0)
                    {
                        if (_foc > 1)
                        {
                            MappedMenu menu2 = MappedMenus.menu[_foc - 1];
                            (menu.value, menu2.value) = (menu2.value, menu.value);
                            if (_menu != null)
                            {
                                MappedSprites.ChangeColour(_menu.gBackground, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            MappedMenus.foc--;
                            if (controller.type <= 1)
                            {
                                var prevPos = menu2.box.transform.position;
                                var newPos = menu.box.transform.position;
                                var diff = prevPos - newPos;
                                POINT point;
                                GetCursorPos(out point);
                                SetCursorPos(point.X + (int)diff.x, point.Y - (int)diff.y);
                            }

                            _delay = _dd;
                            if (_dd > 1)
                            {
                                _dd -= 1;
                            }

                            _foc--;
                        }
                    }
                    else if (_dir > 0)
                    {
                        if (_foc < Prefixes.Count)
                        {
                            MappedMenu menu2 = MappedMenus.menu[_foc + 1];
                            (menu.value, menu2.value) = (menu2.value, menu.value);
                            if (_menu != null)
                            {
                                MappedSprites.ChangeColour(_menu.gBackground, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            MappedMenus.foc++;
                            if (controller.type <= 1)
                            {
                                var prevPos = menu2.box.transform.position;
                                var newPos = menu.box.transform.position;
                                var diff = prevPos - newPos;
                                POINT point;
                                GetCursorPos(out point);
                                SetCursorPos(point.X + (int)diff.x, point.Y - (int)diff.y);
                            }

                            _delay = _dd;
                            if (_dd > 1)
                            {
                                _dd -= 1;
                            }

                            _foc++;
                        }
                    }
                }

                if (_menu != null)
                {
                    if (_dir < 0)
                    {
                        MappedSprites.ChangeColour(_menu.gBackground, 0.3f, 0.9f, 0.3f);
                    } else if (_dir > 0)
                    {
                        MappedSprites.ChangeColour(_menu.gBackground, 0.9f, 0.3f, 0.3f);
                    }
                }
                MappedMenus.oldFoc = MappedMenus.foc;
                _delay--;

                return false;
            }

            if (MappedMenus.screen == 1002)
            {
                MappedMenus.UpdateDisplay();
                
                if (((MappedMenu)MappedMenus.menu[1]).clicked != 0)
                {
                    MappedSound.Play(MappedSound.proceed, 0f, 0.5f);
                    MetaFile.Data.FirstLaunch = false;
                    MetaFile.Data.Save();
                    MappedMenus.ChangeScreen(1);
                }

                return false;
            }
            
            if (MappedMenus.screen == 1003)
            {
                MappedMenus.UpdateDisplay();
                
                if (((MappedMenu)MappedMenus.menu[1]).clicked != 0)
                {
                    MappedSound.Play(MappedSound.proceed, 0f, 0.5f);
                    var next = _nextScreenId!.Value;
                    _nextScreenId = null;
                    MappedMenus.ChangeScreen(next);
                }

                return false;
            }
        }
        catch (Exception e)
        {
            LogError(e);
        }

        return true;
    }

    /*
     * Patch:
     * - Loads the menus for the extra screens.
     */
    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.ICGNAJFLAHL))]
    [HarmonyPrefix]
    public static bool Menus_ICGNAJFLAHL_Pre(int IPCCBDAFNMC)
    {
        if (MappedMenus.screen > 1000 || (MappedMenus.screen == 50 && (MappedMenus.page == 0 || MappedMenus.page == 5) && MappedMatch.state == 0))
        {
            _newScreen = true;
            MappedMenus.RemoveExisting();
            MappedMenus.foc = 0;
            MappedMenus.no_menus = IPCCBDAFNMC;
            MappedMenus.menu = new UnmappedMenu[MappedMenus.no_menus + 1];
            for (MappedMenus.cyc = 1;
                 MappedMenus.cyc <= MappedMenus.no_menus;
                 MappedMenus.cyc++)
            {
                MappedMenus.menu[MappedMenus.cyc] = new UnmappedMenu();
                ((MappedMenu) MappedMenus.menu[MappedMenus.cyc]).id = MappedMenus.cyc;
            }

            MappedMenus.scrollX = 0f;
            MappedMenus.scrollSpeedX = 0f;
            MappedMenus.scrollY = 0f;
            MappedMenus.scrollSpeedY = 0f;
            if (MappedMenus.screen == 1001)
            {
                int rows;
                int columns;
                float scale;
                int startX;
                int startY;
                FindBestFit(Prefixes.Count, -450, -200, 450, 250, out rows, out columns, out scale, out startX,
                    out startY);
                int index = 0;
                foreach (string prefix in SortPrev(Prefixes))
                {
                    MappedMenus.Add();
                    float x = startX + (index % columns * 210 * scale);
                    float y = startY - (index / columns * 50 * scale);
                    ((MappedMenu) MappedMenus.menu[MappedMenus.no_menus]).Load(2,
                        "#" + (index + 1) +
                        (index == 0 ? " (highest)" : index == Prefixes.Count - 1 ? " (lowest)" : ""), x, y, scale,
                        scale);
                    ((MappedMenu) MappedMenus.menu[MappedMenus.no_menus]).value = prefix;
                    MappedSprites.ChangeColour(((MappedMenu) MappedMenus.menu[MappedMenus.no_menus]).gBackground, 0.8f, 0.8f,
                        0.8f);
                    index++;
                }

                MappedMenus.Add();
                ((MappedMenu) MappedMenus.menu[MappedMenus.no_menus]).Load(1, "Proceed", -150f, -280, 1.25f, 1.25f);
                MappedMenus.Add();
                ((MappedMenu) MappedMenus.menu[MappedMenus.no_menus]).Load(1, "Proceed & Hide", 150f, -280, 1.25f, 1.25f);
            }
            else if (MappedMenus.screen == 1002)
            {
                MappedMenus.Add();
                ((MappedMenu) MappedMenus.menu[MappedMenus.no_menus]).Load(1, "Proceed", 0f, -280, 1.25f, 1.25f);
            }
            else if (MappedMenus.screen == 1003)
            {
                MappedMenus.Add();
                ((MappedMenu) MappedMenus.menu[MappedMenus.no_menus]).Load(1, "Bummer", 0f, -280, 1.25f, 1.25f);
            }
            else if (MappedMenus.screen == 50 && MappedMatch.state == 0)
            {
                if (MappedMenus.page == 0)
                {
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(1, "> Resume >", 0f, 220f, 1.6f, 1.6f);
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(1, "Camera", 0f, 100f, 1.6f, 1.6f);
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(1, "Map", 0f, 0f, 1.6f, 1.6f);
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(1, "Fast Travel", 0, -100f, 1.6f, 1.6f);
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(1, "< Exit <", 0f, -220f, 1.6f, 1.6f);
                }
                else
                {
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(2, "Location", 0f, 120f, 1.6f, 1.6f);
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(1, "> Travel >", 0f, -20f, 1.6f, 1.6f);
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(1, "< Exit <", 0f, -120f, 1.6f, 1.6f);
                    MappedMenus.Add();
                    ((MappedMenu)MappedMenus.menu[MappedMenus.no_menus]).Load(1, "Cost: Free", 0f, 50f, 0.8f, 0.8f);

                }
            }
            
            MappedMenus.MeasureOptions();
            if (MappedMenus.Control() > 0 && MappedMenus.no_menus > 0 && _foc == 0)
            {
                _foc = 1;
            }

            MappedMenus.oldFoc = _foc;
            MappedMenus.commit = 0;
            MappedKeyboard.entryFoc = 0;
            MappedMenus.moveFoc = 0;
            MappedMenus.tapStart = 0;
            MappedMenus.tapEnd = 0;
            MappedMenus.gotim = 0f;
            MappedControls.keytim = 10f;
            return false;
        }
        return true;
    }
    private static IEnumerable<string> SortPrev(List<string> prefixes)
    {
        var prev = MetaFile.Data.PrefixPriorityOrder;
        var sorted = new List<string>();
        foreach (var prefix in prev)
        {
            if (prefixes.Contains(prefix))
            {
                sorted.Add(prefix);
            }
        }
        foreach (var prefix in prefixes)
        {
            if (!sorted.Contains(prefix))
            {
                sorted.Add(prefix);
            }
        }
        return sorted;
    }

    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.PMIIOCMHEAE))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Menus_PMIIOCMHEAE(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction prev = null;
        CodeInstruction prev2 = null;
        foreach (var instruction in instructions)
        {
            yield return instruction;
            if (prev2 != null && (instruction.opcode == OpCodes.Beq_S || instruction.opcode == OpCodes.Beq) &&
                prev.opcode == OpCodes.Ldc_I4_S && (sbyte)prev.operand == 90 && prev2.opcode == OpCodes.Ldsfld &&
                (FieldInfo)prev2.operand == AccessTools.Field(typeof(UnmappedMenus), nameof(UnmappedMenus.FAKHAFKOBPB)))
            {
                yield return new CodeInstruction(prev2);
                yield return new CodeInstruction(OpCodes.Ldc_I4, 1000);
                if (instruction.opcode == OpCodes.Beq_S)
                {
                    yield return new CodeInstruction(OpCodes.Bge_S, instruction.operand);
                }
                else
                {
                    yield return new CodeInstruction(OpCodes.Bge, instruction.operand);
                }
            }
            prev2 = prev;
            prev = instruction;
        }
    }
}