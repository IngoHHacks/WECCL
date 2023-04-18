using UnityEngine.UI;
using WECCL.Saves;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
public class ScreenPatch
{
    private static bool _initialized = false;
    private static int _delay = 0;
    private static int _dd = 10;
    private static int foc = 0;
    private static int dir = 0;
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
            GameScreens.GGPMGNGIAKG();
            GameScreens.GLDIFJOEOIO = 1002;
            GameScreens.FPBNAMNIOKK = 0;
            GameScreens.AGAGHGBLCDA = 0;
            GameScreens.DMGJOHGEOKF();
            __instance.gLicense.SetActive(value: false);
            GameScreens.FBGBCKNJKCD.SetActive(value: false);
            Object.Destroy(GameObject.Find("Logo"));
            var obj = Object.Instantiate(GameSprites.FGEFCDGEEHA[1]);
            obj.transform.position = new Vector3(0f, 50f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 5;
            obj.transform.SetParent(GameScreens.LIEFBDMPLNB.transform, worldPositionStays: false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text = "<color=#EF0000>IMPORTANT NOTICE</color>\n While WECCL tries its best to keep modded save files stable and consistent between game updates and mod changes, you may still encounter issues. WECCL automatically creates backups (up to 100 by default). However, it is recommended to manually create backups of your save file. The save file can be found in %USERPROFILE%/AppData/LocalLow/MDickie/Wrestling Empire. If you encounter any issues, please report them in the Wrestling Empire Modding Discord server.";
        }
        else if (!_initialized && HasConflictingOverrides && !MetaFile.Data.HidePriorityScreenNextTime)
        {
            GameScreens.GGPMGNGIAKG();
            GameScreens.GLDIFJOEOIO = 1001;
            GameScreens.FPBNAMNIOKK = 0;
            GameScreens.AGAGHGBLCDA = 0;
            GameScreens.DMGJOHGEOKF();
            __instance.gLicense.SetActive(value: false);
            GameScreens.FBGBCKNJKCD.SetActive(value: false);
            Object.Destroy(GameObject.Find("Logo"));
            
            var obj = Object.Instantiate(GameSprites.FGEFCDGEEHA[1]);
            obj.transform.position = new Vector3(0f, 315f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 2;
            obj.transform.SetParent(GameScreens.LIEFBDMPLNB.transform, worldPositionStays: false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text = "Order the override mods by the desired priority:";
            

            _initialized = true;
        }
    }
    
    [HarmonyPatch(typeof(Scene_Titles), "Update")]
    [HarmonyPrefix]
    public static bool SceneTitles_Update()
    {
        try
        {
            if (GameScreens.GLDIFJOEOIO == 1001)
            {
                GameControls.FKAFNEKGFEB();
                GameScreens.HLNKAPEEMAG();

                if (Input.GetMouseButtonDown(0))
                {
                    foc = GameScreens.DNNAOLIENKK;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    foc = 0;
                    GameScreens.DNNAOLIENKK = 0;
                    _dd = 10;
                    dir = 0;
                    if (_menu != null) GameSprites.JCGAHJGKOKA(_menu.HDNGNBKADOP, 0.8f, 0.8f, 0.8f);
                }

                if (GameScreens.DNNAOLIENKK >= Prefixes.Count + 1)
                {
                    if (GameScreens.DNNAOLIENKK == Prefixes.Count + 2)
                    {
                        MetaFile.Data.HidePriorityScreenNextTime = true;
                    }

                    GameAudio.GBJOOBJBPEM(GameAudio.HPAMLGBIJLJ, 0f, 0.5f);
                    List<string> prefixes = new();
                    for (int i = 1; i < Prefixes.Count + 1; i++)
                    {
                        var text = GameScreens.AEKFGAHLMBF[i].KOFIFIBJKGH.transform.Find("Value").gameObject
                            .GetComponent<Text>().text;
                        prefixes.Add(text);
                    }

                    Prefixes = prefixes;
                    SavePrefixes();
                    GameAudio.EDHODGHLFJF.Stop();
                    GameAudio.GBMBIDMNKOK[0] = GameGlobals.GPPPEEGMGGP("Music", "Theme00") as AudioClip;
                    GameAudio.EDHODGHLFJF.clip = GameAudio.GBMBIDMNKOK[0];
                    GameAudio.EDHODGHLFJF.time = 0f;
                    GameAudio.EDHODGHLFJF.Play();
                    GameScreens.KLNDLKEPNEF(1);
                }
                else if (foc > 0 && _delay <= 0)
                {

                    var menu = GameScreens.AEKFGAHLMBF[foc];
                    if (dir == 0) dir = menu.LKFPJKJBDMK >= 0 ? 1 : -1;
                    if (dir < 0)
                    {
                        if (foc > 1)
                        {
                            var menu2 = GameScreens.AEKFGAHLMBF[foc - 1];
                            var text1 = menu.KOFIFIBJKGH.transform.Find("Value").gameObject.GetComponent<Text>();
                            var text2 = menu2.KOFIFIBJKGH.transform.Find("Value").gameObject.GetComponent<Text>();
                            menu2.IEMHDAFJKAK = text1.text;
                            menu.IEMHDAFJKAK = text2.text;
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null) GameSprites.JCGAHJGKOKA(_menu.HDNGNBKADOP, 0.8f, 0.8f, 0.8f);
                            _menu = menu2;
                            GameSprites.JCGAHJGKOKA(_menu.HDNGNBKADOP, 0.3f, 0.9f, 0.3f);
                            GameScreens.DNNAOLIENKK--;
                            _delay = _dd;
                            if (_dd > 1) _dd -= 1;
                            foc--;
                        }
                    }
                    else if (dir > 0)
                    {
                        if (foc < Prefixes.Count)
                        {
                            var menu2 = GameScreens.AEKFGAHLMBF[foc + 1];
                            var text1 = menu.KOFIFIBJKGH.transform.Find("Value").gameObject.GetComponent<Text>();
                            var text2 = menu2.KOFIFIBJKGH.transform.Find("Value").gameObject.GetComponent<Text>();
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null) GameSprites.JCGAHJGKOKA(_menu.HDNGNBKADOP, 0.8f, 0.8f, 0.8f);
                            _menu = menu2;
                            GameSprites.JCGAHJGKOKA(_menu.HDNGNBKADOP, 0.9f, 0.3f, 0.3f);
                            GameScreens.DNNAOLIENKK++;
                            _delay = _dd;
                            if (_dd > 1) _dd -= 1;
                            foc++;
                        }
                    }
                }

                GameScreens.KDEIEADNAOK = GameScreens.DNNAOLIENKK;

                _delay--;

                return false;
            }

            if (GameScreens.GLDIFJOEOIO == 1002)
            {
                GameControls.FKAFNEKGFEB();
                GameScreens.HLNKAPEEMAG();

                for (int num = 1; num <= GameScreens.GEBEPDNJHGB; num++)
                {
                    if (GameScreens.AEKFGAHLMBF[num].LKFPJKJBDMK != 0f && GameScreens.PEAAJKABMCP == 0 &&
                        GameScreens.NIIPOEGBBFH > 10f && GameControls.PKCABBMAGNE == 0f)
                    {
                        if (GameScreens.AEKFGAHLMBF[num].HOOGIOMCIMP > 0)
                        {
                            GameAudio.GBJOOBJBPEM(GameAudio.HPAMLGBIJLJ);
                            GameScreens.PEAAJKABMCP = 1;
                        }
                        else if (GameControls.PKCABBMAGNE == 0f)
                        {
                            GameAudio.GBJOOBJBPEM(GameAudio.NNIOLGAKPGG, 1f);
                            GameControls.PKCABBMAGNE = 10f;
                        }
                    }
                }

                if (GameScreens.PEAAJKABMCP == 5 && GameScreens.DNNAOLIENKK > 0)
                {
                    GameAudio.GBJOOBJBPEM(GameAudio.HPAMLGBIJLJ, 0f, 0.5f);
                    MetaFile.Data.FirstLaunch = false;
                    MetaFile.Data.Save();
                    GameScreens.KLNDLKEPNEF(1);
                }

                GameScreens.BONHHCACNDG();
                return false;
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }

        return true;
    }

    [HarmonyPatch(typeof(GameScreens), nameof(GameScreens.DMGJOHGEOKF))]
    [HarmonyPrefix]
    public static bool GameScreens_DMGJOHGEOKF(int HHFMDMKEIKJ)
    {
        if (GameScreens.GLDIFJOEOIO > 1000)
        {
            GameScreens.GGPMGNGIAKG();
            GameScreens.DNNAOLIENKK = 0;
            GameScreens.GEBEPDNJHGB = HHFMDMKEIKJ;
            GameScreens.AEKFGAHLMBF = new GameMenus[GameScreens.GEBEPDNJHGB + 1];
            for (GameScreens.LCKJBMKHDFI = 1; GameScreens.LCKJBMKHDFI <= GameScreens.GEBEPDNJHGB; GameScreens.LCKJBMKHDFI++)
            {
                GameScreens.AEKFGAHLMBF[GameScreens.LCKJBMKHDFI] = new GameMenus();
                GameScreens.AEKFGAHLMBF[GameScreens.LCKJBMKHDFI].DKHFDIBPHMK = GameScreens.LCKJBMKHDFI;
            }
            GameScreens.KOAPEMIOKLI = 0f;
            GameScreens.AGFFOFNHHLK = 0f;
            GameScreens.LPBMALHMIGE = 0f;
            GameScreens.AJNEEIMNLFD = 0f;
            float num = 0f;
            float num2 = 0f;
            float num3 = 80f;
            float num4 = 1.6f;
            if (GameScreens.GLDIFJOEOIO == 1001)
            {
                int rows;
                int columns;
                float scale;
                int startX;
                int startY;
                FindBestFit(Prefixes.Count, -450, -200, 450, 250, out rows, out columns, out scale, out startX,
                    out startY);
                int index = 0;
                foreach (var prefix in Prefixes)
                {
                    GameScreens.OHBONDHEDEC();
                    var x = startX + (index % columns * 210 * scale);
                    var y = startY - (index / columns * 50 * scale);
                    GameScreens.AEKFGAHLMBF[GameScreens.GEBEPDNJHGB].DMGJOHGEOKF(6,
                        "#" + (index + 1) +
                        (index == 0 ? " (highest)" : index == Prefixes.Count - 1 ? " (lowest)" : ""), x, y, scale,
                        scale);
                    GameScreens.AEKFGAHLMBF[GameScreens.GEBEPDNJHGB].KOFIFIBJKGH.transform.Find("Value").gameObject
                        .GetComponent<Text>().text = prefix;
                    GameSprites.JCGAHJGKOKA(GameScreens.AEKFGAHLMBF[GameScreens.GEBEPDNJHGB].HDNGNBKADOP, 0.8f, 0.8f,
                        0.8f);
                    index++;
                }

                GameScreens.OHBONDHEDEC();
                GameScreens.AEKFGAHLMBF[GameScreens.GEBEPDNJHGB].DMGJOHGEOKF(1, "Proceed", -150f, -280, 1.25f, 1.25f);
                GameScreens.OHBONDHEDEC();
                GameScreens.AEKFGAHLMBF[GameScreens.GEBEPDNJHGB]
                    .DMGJOHGEOKF(1, "Proceed & Hide", 150f, -280, 1.25f, 1.25f);
            }
            else if (GameScreens.GLDIFJOEOIO == 1002)
            {
                GameScreens.OHBONDHEDEC();
                GameScreens.AEKFGAHLMBF[GameScreens.GEBEPDNJHGB].DMGJOHGEOKF(1, "Proceed", 0f, -280, 1.25f, 1.25f);
            }
            
            GameScreens.GJAJDABPMNH();
            if (GameScreens.LGCOHPNNGLM() > 0 && GameScreens.GEBEPDNJHGB > 0 && foc == 0)
            {
                foc = 1;
            }
            GameScreens.KDEIEADNAOK = foc;
            GameScreens.PEAAJKABMCP = 0;
            GameKeyboard.FNCDEHIDPPM = 0;
            GameScreens.KALAICJONEO = 0;
            GameScreens.GDFKNNGCKML = 0;
            GameScreens.KJLLBNEIAJO = 0;
            GameScreens.NIIPOEGBBFH = 0f;
            GameControls.PKCABBMAGNE = 10f;
            return false;
        }

        return true;
    }
}