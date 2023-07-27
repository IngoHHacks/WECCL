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
    private static GameMenus _menu;

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
            GameScreens.IIFOHKLHALM();
            GameScreens.AAAIDOOHBCM = 1002;
            GameScreens.PJHNMEEFCME = 0;
            GameScreens.NJJPPLCPOIA = 0;
            GameScreens.GBDGLHHCLCI();
            __instance.gLicense.SetActive(false);
            GameScreens.EOANIBNAACO.SetActive(false);
            Object.Destroy(GameObject.Find("Logo"));
            GameObject obj = Object.Instantiate(GameSprites.JCNCOJEJDJC[1]);
            obj.transform.position = new Vector3(0f, 50f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 5;
            obj.transform.SetParent(GameScreens.BJNOPDGKOEI.transform, false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text =
                "<color=#EF0000>IMPORTANT NOTICE</color>\n While WECCL tries its best to keep modded save files stable and consistent between game updates and mod changes, you may still encounter issues. WECCL automatically creates backups (up to 100 by default). However, it is recommended to manually create backups of your save file. The save file can be found in %USERPROFILE%/AppData/LocalLow/MDickie/Wrestling Empire. If you encounter any issues, please report them in the Wrestling Empire Modding Discord server.";
        }
        else if (!_initialized && HasConflictingOverrides && !MetaFile.Data.HidePriorityScreenNextTime)
        {
            GameScreens.IIFOHKLHALM();
            GameScreens.AAAIDOOHBCM = 1001;
            GameScreens.PJHNMEEFCME = 0;
            GameScreens.NJJPPLCPOIA = 0;
            GameScreens.GBDGLHHCLCI();
            __instance.gLicense.SetActive(false);
            GameScreens.EOANIBNAACO.SetActive(false);
            Object.Destroy(GameObject.Find("Logo"));

            GameObject obj = Object.Instantiate(GameSprites.JCNCOJEJDJC[1]);
            obj.transform.position = new Vector3(0f, 315f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 2;
            obj.transform.SetParent(GameScreens.BJNOPDGKOEI.transform, false);
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
            if (GameScreens.AAAIDOOHBCM == 1001)
            {
                GameControls.PAOEHLEJKIJ();
                GameScreens.MDLFJLFKLJH();

                if (Input.GetMouseButtonDown(0))
                {
                    foc = GameScreens.CJGHFHCHDNN;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    foc = 0;
                    GameScreens.CJGHFHCHDNN = 0;
                    _dd = 10;
                    dir = 0;
                    if (_menu != null)
                    {
                        GameSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.8f, 0.8f, 0.8f);
                    }
                }

                if (GameScreens.CJGHFHCHDNN >= Prefixes.Count + 1)
                {
                    if (GameScreens.CJGHFHCHDNN == Prefixes.Count + 2)
                    {
                        MetaFile.Data.HidePriorityScreenNextTime = true;
                    }

                    GameAudio.NMHGFGAEKEJ(GameAudio.IMLNJEMFJNK, 0f, 0.5f);
                    List<string> prefixes = new();
                    for (int i = 1; i < Prefixes.Count + 1; i++)
                    {
                        string text = GameScreens.ECEJOIDPOCN[i].NBNIGADOOJD.transform.Find("Value").gameObject
                            .GetComponent<Text>().text;
                        prefixes.Add(text);
                    }

                    Prefixes = prefixes;
                    SavePrefixes();
                    GameAudio.HMEHNDBNCEB.Stop();
                    GameAudio.FFMBDMFIJHL[0] = GameGlobals.HBNLBOOPFOJ("Music", "Theme00") as AudioClip;
                    GameAudio.HMEHNDBNCEB.clip = GameAudio.FFMBDMFIJHL[0];
                    GameAudio.HMEHNDBNCEB.time = 0f;
                    GameAudio.HMEHNDBNCEB.Play();
                    GameScreens.NCIBLEAKGFH(1);
                }
                else if (foc > 0 && _delay <= 0)
                {
                    BOAPBLKGGHL menu = GameScreens.ECEJOIDPOCN[foc];
                    if (dir == 0)
                    {
                        dir = menu.DFFIHDMCJGL >= 0 ? 1 : -1;
                    }

                    if (dir < 0)
                    {
                        if (foc > 1)
                        {
                            BOAPBLKGGHL menu2 = GameScreens.ECEJOIDPOCN[foc - 1];
                            Text text1 = menu.NBNIGADOOJD.transform.Find("Value").gameObject.GetComponent<Text>();
                            Text text2 = menu2.NBNIGADOOJD.transform.Find("Value").gameObject.GetComponent<Text>();
                            menu2.CCFHFGBDIHE = text1.text;
                            menu.CCFHFGBDIHE = text2.text;
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null)
                            {
                                GameSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            GameSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.3f, 0.9f, 0.3f);
                            GameScreens.CJGHFHCHDNN--;
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
                            BOAPBLKGGHL menu2 = GameScreens.ECEJOIDPOCN[foc + 1];
                            Text text1 = menu.NBNIGADOOJD.transform.Find("Value").gameObject.GetComponent<Text>();
                            Text text2 = menu2.NBNIGADOOJD.transform.Find("Value").gameObject.GetComponent<Text>();
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null)
                            {
                                GameSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            GameSprites.IJHBLDHOEOH(_menu.FCFPMEDHPML, 0.9f, 0.3f, 0.3f);
                            GameScreens.CJGHFHCHDNN++;
                            _delay = _dd;
                            if (_dd > 1)
                            {
                                _dd -= 1;
                            }

                            foc++;
                        }
                    }
                }

                GameScreens.NLNBFBCGKCN = GameScreens.CJGHFHCHDNN;

                _delay--;

                return false;
            }

            if (GameScreens.AAAIDOOHBCM == 1002)
            {
                GameControls.PAOEHLEJKIJ();
                GameScreens.MDLFJLFKLJH();
                GameScreens.BKCLHHDGBEC();

                for (int num = 1; num <= GameScreens.LHEGOJODLAF; num++)
                {
                    if (GameScreens.ECEJOIDPOCN[num].DFFIHDMCJGL != 0f && GameScreens.EJDHJPCLOBP == 0 &&
                        GameScreens.FKLIHLHEECO > 10f && GameControls.JOPPDHFINKD == 0f &&
                        GameScreens.ECEJOIDPOCN[num].DBEKNIFAGLK() == 0)
                    {
                        if (GameScreens.CJGHFHCHDNN > 0)
                        {
                            GameScreens.EJDHJPCLOBP = 1;
                            JJDCNALMPCI.CJGHFHCHDNN = -1;
                        }
                    }
                }

                if (GameScreens.EJDHJPCLOBP >= 5 && GameScreens.CJGHFHCHDNN == -1)
                {
                    GameAudio.NMHGFGAEKEJ(GameAudio.IMLNJEMFJNK, 0f, 0.5f);
                    MetaFile.Data.FirstLaunch = false;
                    MetaFile.Data.Save();
                    GameScreens.NCIBLEAKGFH(1);
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

    [HarmonyPatch(typeof(GameScreens), nameof(GameScreens.GBDGLHHCLCI))]
    [HarmonyPrefix]
    public static bool GameScreens_GBDGLHHCLCI(int EJFCAOFBHMC)
    {
        if (GameScreens.AAAIDOOHBCM > 1000)
        {
            GameScreens.IIFOHKLHALM();
            GameScreens.CJGHFHCHDNN = 0;
            GameScreens.LHEGOJODLAF = EJFCAOFBHMC;
            GameScreens.ECEJOIDPOCN = new GameMenus[GameScreens.LHEGOJODLAF + 1];
            for (GameScreens.LBDCLOPBBJF = 1;
                 GameScreens.LBDCLOPBBJF <= GameScreens.LHEGOJODLAF;
                 GameScreens.LBDCLOPBBJF++)
            {
                GameScreens.ECEJOIDPOCN[GameScreens.LBDCLOPBBJF] = new GameMenus();
                GameScreens.ECEJOIDPOCN[GameScreens.LBDCLOPBBJF].DHBIELODIAN = GameScreens.LBDCLOPBBJF;
            }

            GameScreens.KFMIBDFGKJL = 0f;
            GameScreens.OPGFFOJJHBL = 0f;
            GameScreens.BHFMJHOPEAL = 0f;
            GameScreens.JLGBEDGJKIP = 0f;
            float num = 0f;
            float num2 = 0f;
            float num3 = 80f;
            float num4 = 1.6f;
            if (GameScreens.AAAIDOOHBCM == 1001)
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
                    GameScreens.MFDCLFKDDFB();
                    float x = startX + (index % columns * 210 * scale);
                    float y = startY - (index / columns * 50 * scale);
                    GameScreens.ECEJOIDPOCN[GameScreens.LHEGOJODLAF].GBDGLHHCLCI(6,
                        "#" + (index + 1) +
                        (index == 0 ? " (highest)" : index == Prefixes.Count - 1 ? " (lowest)" : ""), x, y, scale,
                        scale);
                    GameScreens.ECEJOIDPOCN[GameScreens.LHEGOJODLAF].NBNIGADOOJD.transform.Find("Value").gameObject
                        .GetComponent<Text>().text = prefix;
                    GameSprites.IJHBLDHOEOH(GameScreens.ECEJOIDPOCN[GameScreens.LHEGOJODLAF].FCFPMEDHPML, 0.8f, 0.8f,
                        0.8f);
                    index++;
                }

                GameScreens.MFDCLFKDDFB();
                GameScreens.ECEJOIDPOCN[GameScreens.LHEGOJODLAF].GBDGLHHCLCI(1, "Proceed", -150f, -280, 1.25f, 1.25f);
                GameScreens.MFDCLFKDDFB();
                GameScreens.ECEJOIDPOCN[GameScreens.LHEGOJODLAF]
                    .GBDGLHHCLCI(1, "Proceed & Hide", 150f, -280, 1.25f, 1.25f);
            }
            else if (GameScreens.AAAIDOOHBCM == 1002)
            {
                GameScreens.MFDCLFKDDFB();
                GameScreens.ECEJOIDPOCN[GameScreens.LHEGOJODLAF].GBDGLHHCLCI(1, "Proceed", 0f, -280, 1.25f, 1.25f);
            }

            GameScreens.KBAEPPOEEIG();
            if (GameScreens.NNEAIJGCPNF() > 0 && GameScreens.LHEGOJODLAF > 0 && foc == 0)
            {
                foc = 1;
            }

            GameScreens.NLNBFBCGKCN = foc;
            GameScreens.EJDHJPCLOBP = 0;
            GameKeyboard.FPAFHMCMOEF = 0;
            GameScreens.NFEAFIEMMOJ = 0;
            GameScreens.NCAKGGALEFE = 0;
            GameScreens.OOILEGABJLC = 0;
            GameScreens.FKLIHLHEECO = 0f;
            GameControls.JOPPDHFINKD = 10f;
            return false;
        }

        return true;
    }
}