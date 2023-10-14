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
    private static UnmappedMenu _menu;

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
            UnmappedMenus.IIFOHKLHALM();
            UnmappedMenus.AAAIDOOHBCM = 1002;
            UnmappedMenus.PJHNMEEFCME = 0;
            UnmappedMenus.NJJPPLCPOIA = 0;
            UnmappedMenus.GBDGLHHCLCI();
            __instance.gLicense.SetActive(false);
            UnmappedMenus.EOANIBNAACO.SetActive(false);
            Object.Destroy(GameObject.Find("Logo"));
            GameObject obj = Object.Instantiate(UnmappedSprites.JCNCOJEJDJC[1]);
            obj.transform.position = new Vector3(0f, 50f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 5;
            obj.transform.SetParent(UnmappedMenus.BJNOPDGKOEI.transform, false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text =
                "<color=#EF0000>IMPORTANT NOTICE</color>\n While WECCL tries its best to keep modded save files stable and consistent between game updates and mod changes, you may still encounter issues. WECCL automatically creates backups (up to 100 by default). However, it is recommended to manually create backups of your save file. The save file can be found in %USERPROFILE%/AppData/LocalLow/MDickie/Wrestling Empire. If you encounter any issues, please report them in the Wrestling Empire Modding Discord server.";
        }
        else if (!_initialized && HasConflictingOverrides && !MetaFile.Data.HidePriorityScreenNextTime)
        {
            UnmappedMenus.IIFOHKLHALM();
            UnmappedMenus.AAAIDOOHBCM = 1001;
            UnmappedMenus.PJHNMEEFCME = 0;
            UnmappedMenus.NJJPPLCPOIA = 0;
            UnmappedMenus.GBDGLHHCLCI();
            __instance.gLicense.SetActive(false);
            UnmappedMenus.EOANIBNAACO.SetActive(false);
            Object.Destroy(GameObject.Find("Logo"));

            GameObject obj = Object.Instantiate(UnmappedSprites.JCNCOJEJDJC[1]);
            obj.transform.position = new Vector3(0f, 315f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 2;
            obj.transform.SetParent(UnmappedMenus.BJNOPDGKOEI.transform, false);
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
            if (UnmappedMenus.AAAIDOOHBCM == 1001)
            {
                UnmappedControls.PAOEHLEJKIJ();
                UnmappedMenus.MDLFJLFKLJH();

                if (Input.GetMouseButtonDown(0))
                {
                    foc = UnmappedMenus.CJGHFHCHDNN;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    foc = 0;
                    UnmappedMenus.CJGHFHCHDNN = 0;
                    _dd = 10;
                    dir = 0;
                    if (_menu != null)
                    {
                        UnmappedSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.8f, 0.8f, 0.8f);
                    }
                }

                if (UnmappedMenus.CJGHFHCHDNN >= Prefixes.Count + 1)
                {
                    if (UnmappedMenus.CJGHFHCHDNN == Prefixes.Count + 2)
                    {
                        MetaFile.Data.HidePriorityScreenNextTime = true;
                    }

                    UnmappedSound.NMHGFGAEKEJ(UnmappedSound.IMLNJEMFJNK, 0f, 0.5f);
                    List<string> prefixes = new();
                    for (int i = 1; i < Prefixes.Count + 1; i++)
                    {
                        string text = UnmappedMenus.ECEJOIDPOCN[i].NBNIGADOOJD.transform.Find("Value").gameObject
                            .GetComponent<Text>().text;
                        prefixes.Add(text);
                    }

                    Prefixes = prefixes;
                    SavePrefixes();
                    UnmappedSound.HMEHNDBNCEB.Stop();
                    UnmappedSound.FFMBDMFIJHL[0] = UnmappedGlobals.HBNLBOOPFOJ("Music", "Theme00") as AudioClip;
                    UnmappedSound.HMEHNDBNCEB.clip = UnmappedSound.FFMBDMFIJHL[0];
                    UnmappedSound.HMEHNDBNCEB.time = 0f;
                    UnmappedSound.HMEHNDBNCEB.Play();
                    UnmappedMenus.NCIBLEAKGFH(1);
                }
                else if (foc > 0 && _delay <= 0)
                {
                    UnmappedMenu menu = UnmappedMenus.ECEJOIDPOCN[foc];
                    if (dir == 0)
                    {
                        dir = menu.DFFIHDMCJGL >= 0 ? 1 : -1;
                    }

                    if (dir < 0)
                    {
                        if (foc > 1)
                        {
                            UnmappedMenu menu2 = UnmappedMenus.ECEJOIDPOCN[foc - 1];
                            Text text1 = menu.NBNIGADOOJD.transform.Find("Value").gameObject.GetComponent<Text>();
                            Text text2 = menu2.NBNIGADOOJD.transform.Find("Value").gameObject.GetComponent<Text>();
                            menu2.CCFHFGBDIHE = text1.text;
                            menu.CCFHFGBDIHE = text2.text;
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null)
                            {
                                UnmappedSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            UnmappedSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.3f, 0.9f, 0.3f);
                            UnmappedMenus.CJGHFHCHDNN--;
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
                            UnmappedMenu menu2 = UnmappedMenus.ECEJOIDPOCN[foc + 1];
                            Text text1 = menu.NBNIGADOOJD.transform.Find("Value").gameObject.GetComponent<Text>();
                            Text text2 = menu2.NBNIGADOOJD.transform.Find("Value").gameObject.GetComponent<Text>();
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null)
                            {
                                UnmappedSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            UnmappedSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.9f, 0.3f, 0.3f);
                            UnmappedMenus.CJGHFHCHDNN++;
                            _delay = _dd;
                            if (_dd > 1)
                            {
                                _dd -= 1;
                            }

                            foc++;
                        }
                    }
                }

                UnmappedMenus.NLNBFBCGKCN = UnmappedMenus.CJGHFHCHDNN;

                _delay--;

                return false;
            }

            if (UnmappedMenus.AAAIDOOHBCM == 1002)
            {
                UnmappedControls.PAOEHLEJKIJ();
                UnmappedMenus.MDLFJLFKLJH();
                UnmappedMenus.BKCLHHDGBEC();

                for (int num = 1; num <= UnmappedMenus.LHEGOJODLAF; num++)
                {
                    if (UnmappedMenus.ECEJOIDPOCN[num].DFFIHDMCJGL != 0f && UnmappedMenus.EJDHJPCLOBP == 0 &&
                        UnmappedMenus.FKLIHLHEECO > 10f && UnmappedControls.JOPPDHFINKD == 0f &&
                        UnmappedMenus.ECEJOIDPOCN[num].DBEKNIFAGLK() == 0)
                    {
                        if (UnmappedMenus.CJGHFHCHDNN > 0)
                        {
                            UnmappedMenus.EJDHJPCLOBP = 1;
                            UnmappedMenus.CJGHFHCHDNN = -1;
                        }
                    }
                }

                if (UnmappedMenus.EJDHJPCLOBP >= 5 && UnmappedMenus.CJGHFHCHDNN == -1)
                {
                    UnmappedSound.NMHGFGAEKEJ(UnmappedSound.IMLNJEMFJNK, 0f, 0.5f);
                    MetaFile.Data.FirstLaunch = false;
                    MetaFile.Data.Save();
                    UnmappedMenus.NCIBLEAKGFH(1);
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

    [HarmonyPatch(typeof(UnmappedMenus), nameof(UnmappedMenus.GBDGLHHCLCI))]
    [HarmonyPrefix]
    public static bool Menus_GBDGLHHCLCI(int EJFCAOFBHMC)
    {
        if (UnmappedMenus.AAAIDOOHBCM > 1000)
        {
            UnmappedMenus.IIFOHKLHALM();
            UnmappedMenus.CJGHFHCHDNN = 0;
            UnmappedMenus.LHEGOJODLAF = EJFCAOFBHMC;
            UnmappedMenus.ECEJOIDPOCN = new UnmappedMenu[UnmappedMenus.LHEGOJODLAF + 1];
            for (UnmappedMenus.LBDCLOPBBJF = 1;
                 UnmappedMenus.LBDCLOPBBJF <= UnmappedMenus.LHEGOJODLAF;
                 UnmappedMenus.LBDCLOPBBJF++)
            {
                UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LBDCLOPBBJF] = new UnmappedMenu();
                UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LBDCLOPBBJF].DHBIELODIAN = UnmappedMenus.LBDCLOPBBJF;
            }

            UnmappedMenus.KFMIBDFGKJL = 0f;
            UnmappedMenus.OPGFFOJJHBL = 0f;
            UnmappedMenus.BHFMJHOPEAL = 0f;
            UnmappedMenus.JLGBEDGJKIP = 0f;
            float num = 0f;
            float num2 = 0f;
            float num3 = 80f;
            float num4 = 1.6f;
            if (UnmappedMenus.AAAIDOOHBCM == 1001)
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
                    UnmappedMenus.MFDCLFKDDFB();
                    float x = startX + (index % columns * 210 * scale);
                    float y = startY - (index / columns * 50 * scale);
                    UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].GBDGLHHCLCI(6,
                        "#" + (index + 1) +
                        (index == 0 ? " (highest)" : index == Prefixes.Count - 1 ? " (lowest)" : ""), x, y, scale,
                        scale);
                    UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].NBNIGADOOJD.transform.Find("Value").gameObject
                        .GetComponent<Text>().text = prefix;
                    UnmappedSprites.IJHBLDHOEOH(UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].FCFPMEDHPML, 0.8f, 0.8f,
                        0.8f);
                    index++;
                }

                UnmappedMenus.MFDCLFKDDFB();
                UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].GBDGLHHCLCI(1, "Proceed", -150f, -280, 1.25f, 1.25f);
                UnmappedMenus.MFDCLFKDDFB();
                UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF]
                    .GBDGLHHCLCI(1, "Proceed & Hide", 150f, -280, 1.25f, 1.25f);
            }
            else if (UnmappedMenus.AAAIDOOHBCM == 1002)
            {
                UnmappedMenus.MFDCLFKDDFB();
                UnmappedMenus.ECEJOIDPOCN[UnmappedMenus.LHEGOJODLAF].GBDGLHHCLCI(1, "Proceed", 0f, -280, 1.25f, 1.25f);
            }

            UnmappedMenus.KBAEPPOEEIG();
            if (UnmappedMenus.NNEAIJGCPNF() > 0 && UnmappedMenus.LHEGOJODLAF > 0 && foc == 0)
            {
                foc = 1;
            }

            UnmappedMenus.NLNBFBCGKCN = foc;
            UnmappedMenus.EJDHJPCLOBP = 0;
            UnmappedKeyboard.FPAFHMCMOEF = 0;
            UnmappedMenus.NFEAFIEMMOJ = 0;
            UnmappedMenus.NCAKGGALEFE = 0;
            UnmappedMenus.OOILEGABJLC = 0;
            UnmappedMenus.FKLIHLHEECO = 0f;
            UnmappedControls.JOPPDHFINKD = 10f;
            return false;
        }

        return true;
    }
}