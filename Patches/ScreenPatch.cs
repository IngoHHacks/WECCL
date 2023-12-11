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
    private static int foc;
    private static int dir;
    private static MappedMenu _menu;

    [HarmonyPatch(typeof(Scene_Titles), "Start")]
    [HarmonyPostfix]
    public static void SceneTitles_Start(Scene_Titles __instance)
    {
        foc = 0;
        dir = 0;
        _delay = 0;
        _dd = 10;
        if (MetaFile.Data.FirstLaunch)
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
                "<color=#EF0000>IMPORTANT NOTICE</color>\n While WECCL tries its best to keep modded save files stable and consistent between game updates and mod changes, you may still encounter issues. WECCL automatically creates backups (up to 100 by default). However, it is recommended to manually create backups of your save file. The save file can be found in %USERPROFILE%/AppData/LocalLow/MDickie/Wrestling Empire. If you encounter any issues, please report them in the Wrestling Empire Modding Discord server.";
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

    [HarmonyPatch(typeof(Scene_Titles), "Update")]
    [HarmonyPrefix]
    public static bool SceneTitles_Update()
    {
        try
        {
            if (MappedMenus.screen > 1000)
            {
                MappedControls.GetInput();
                MappedMenus.FindClicks();
            }
            if (MappedMenus.screen == 1001)
            {
                MappedMenus.UpdateDisplay();
                if (Input.GetMouseButtonDown(0))
                {
                    foc = MappedMenus.foc;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    foc = 0;
                    MappedMenus.foc = 0;
                    _dd = 10;
                    dir = 0;
                    if (_menu != null)
                    {
                        MappedSprites.ChangeColour(_menu.gBackground, 0.8f, 0.8f, 0.8f);
                    }
                }

                if (MappedMenus.foc >= Prefixes.Count + 1)
                {
                    if (MappedMenus.foc == Prefixes.Count + 2)
                    {
                        MetaFile.Data.HidePriorityScreenNextTime = true;
                    }

                    MappedSound.Play(MappedSound.proceed, 0f, 0.5f);
                    List<string> prefixes = new();
                    for (int i = 1; i < Prefixes.Count + 1; i++)
                    {
                        string text = MappedMenus.menu[i].JGHBIPNIHBK.transform.Find("Value").gameObject
                            .GetComponent<Text>().text;
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
                else if (foc > 0 && _delay <= 0)
                {
                    MappedMenu menu = MappedMenus.menu[foc];
                    if (dir == 0)
                    {
                        dir = menu.alpha >= 0 ? 1 : -1;
                    }

                    if (dir < 0)
                    {
                        if (foc > 1)
                        {
                            MappedMenu menu2 = MappedMenus.menu[foc - 1];
                            Text text1 = menu.box.transform.Find("Value").gameObject.GetComponent<Text>();
                            Text text2 = menu2.box.transform.Find("Value").gameObject.GetComponent<Text>();
                            menu2.title = text1.text;
                            menu.title = text2.text;
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null)
                            {
                                MappedSprites.ChangeColour(_menu.gBackground, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            MappedSprites.ChangeColour(_menu.gBackground, 0.3f, 0.9f, 0.3f);
                            MappedMenus.foc--;
                            _delay = _dd;
                            if (_dd > 1)
                            {
                                _dd -= 1;
                            }

                            foc--;
                        }
                    }
                    else if (dir > 0)
                    {
                        if (foc < Prefixes.Count)
                        {
                            MappedMenu menu2 = MappedMenus.menu[foc + 1];
                            Text text1 = menu.box.transform.Find("Value").gameObject.GetComponent<Text>();
                            Text text2 = menu2.box.transform.Find("Value").gameObject.GetComponent<Text>();
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null)
                            {
                                MappedSprites.ChangeColour(_menu.gBackground, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            MappedSprites.ChangeColour(_menu.gBackground, 0.9f, 0.3f, 0.3f);
                            MappedMenus.foc++;
                            _delay = _dd;
                            if (_dd > 1)
                            {
                                _dd -= 1;
                            }

                            foc++;
                        }
                    }
                }
                MappedMenus.oldFoc = MappedMenus.foc;
                _delay--;

                return false;
            }

            if (MappedMenus.screen == 1002)
            {
                MappedMenus.UpdateDisplay();
                
                for (int num = 1; num <= MappedMenus.no_menus; num++)
                {
                    if (((MappedMenu) MappedMenus.menu[num]).alpha != 0f && MappedMenus.commit == 0 &&
                        MappedMenus.gotim > 10f && MappedControls.keytim == 0f &&
                        ((MappedMenu) MappedMenus.menu[num]).BlockAccess() == 0)
                    {
                        if (MappedMenus.foc > 0)
                        {
                            MappedMenus.commit = 1;
                            MappedMenus.foc = -1;
                        }
                    }
                }

                if (MappedMenus.commit >= 5 && MappedMenus.foc == -1)
                {
                    MappedSound.Play(MappedSound.proceed, 0f, 0.5f);
                    MetaFile.Data.FirstLaunch = false;
                    MetaFile.Data.Save();
                    MappedMenus.ChangeScreen(1);
                }

                return false;
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }

        return true;
    }

    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.ICGNAJFLAHL))]
    [HarmonyPrefix]
    public static bool Menus_ICGNAJFLAHL(int IPCCBDAFNMC)
    {
        if (MappedMenus.screen > 1000)
        {
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
                foreach (string prefix in Prefixes)
                {
                    MappedMenus.Add();
                    float x = startX + (index % columns * 210 * scale);
                    float y = startY - (index / columns * 50 * scale);
                    ((MappedMenu) MappedMenus.menu[MappedMenus.no_menus]).Load(6,
                        "#" + (index + 1) +
                        (index == 0 ? " (highest)" : index == Prefixes.Count - 1 ? " (lowest)" : ""), x, y, scale,
                        scale);
                    ((MappedMenu) MappedMenus.menu[MappedMenus.no_menus]).box.transform.Find("Value").gameObject
                        .GetComponent<Text>().text = prefix;
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

            MappedMenus.MeasureOptions();
            if (MappedMenus.Control() > 0 && MappedMenus.no_menus > 0 && foc == 0)
            {
                foc = 1;
            }

            MappedMenus.oldFoc = foc;
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
}