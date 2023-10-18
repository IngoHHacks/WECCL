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
            UnmappedMenus.EHLDKHKMHNG();
            UnmappedMenus.FAKHAFKOBPB = 1002;
            UnmappedMenus.CHLJMEPFJOK = 0;
            UnmappedMenus.ODOAPLMOJPD = 0;
            UnmappedMenus.ICGNAJFLAHL();
            __instance.gLicense.SetActive(false);
            UnmappedMenus.GIBJMFJBELJ.SetActive(false);
            Object.Destroy(GameObject.Find("Logo"));
            GameObject obj = Object.Instantiate(UnmappedSprites.IMPJPDIEKDF[1]);
            obj.transform.position = new Vector3(0f, 50f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 5;
            obj.transform.SetParent(UnmappedMenus.JPABICKOAEO.transform, false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text =
                "<color=#EF0000>IMPORTANT NOTICE</color>\n While WECCL tries its best to keep modded save files stable and consistent between game updates and mod changes, you may still encounter issues. WECCL automatically creates backups (up to 100 by default). However, it is recommended to manually create backups of your save file. The save file can be found in %USERPROFILE%/AppData/LocalLow/MDickie/Wrestling Empire. If you encounter any issues, please report them in the Wrestling Empire Modding Discord server.";
        }
        else if (!_initialized && HasConflictingOverrides && !MetaFile.Data.HidePriorityScreenNextTime)
        {
            UnmappedMenus.EHLDKHKMHNG();
            UnmappedMenus.FAKHAFKOBPB = 1001;
            UnmappedMenus.CHLJMEPFJOK = 0;
            UnmappedMenus.ODOAPLMOJPD = 0;
            UnmappedMenus.ICGNAJFLAHL();
            __instance.gLicense.SetActive(false);
            UnmappedMenus.GIBJMFJBELJ.SetActive(false);
            Object.Destroy(GameObject.Find("Logo"));

            GameObject obj = Object.Instantiate(UnmappedSprites.IMPJPDIEKDF[1]);
            obj.transform.position = new Vector3(0f, 315f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 2;
            obj.transform.SetParent(UnmappedMenus.JPABICKOAEO.transform, false);
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
            if (UnmappedMenus.FAKHAFKOBPB == 1001)
            {
                UnmappedControls.NCOEPCFFBJA();
                UnmappedMenus.PIELJFKJFKF();

                if (Input.GetMouseButtonDown(0))
                {
                    foc = UnmappedMenus.NNMDEFLLNBF;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    foc = 0;
                    UnmappedMenus.NNMDEFLLNBF = 0;
                    _dd = 10;
                    dir = 0;
                    if (_menu != null)
                    {
                        UnmappedSprites.BBLJCJMDDLO(_menu.MGHGFEHHEBA, 0.8f, 0.8f, 0.8f);
                    }
                }

                if (UnmappedMenus.NNMDEFLLNBF >= Prefixes.Count + 1)
                {
                    if (UnmappedMenus.NNMDEFLLNBF == Prefixes.Count + 2)
                    {
                        MetaFile.Data.HidePriorityScreenNextTime = true;
                    }

                    UnmappedSound.DNNPEAOCDOG(UnmappedSound.PAJJMPLBDPL, 0f, 0.5f);
                    List<string> prefixes = new();
                    for (int i = 1; i < Prefixes.Count + 1; i++)
                    {
                        string text = UnmappedMenus.FKANHDIMMBJ[i].JGHBIPNIHBK.transform.Find("Value").gameObject
                            .GetComponent<Text>().text;
                        prefixes.Add(text);
                    }

                    Prefixes = prefixes;
                    SavePrefixes();
                    UnmappedSound.OGCBMJIIKPP.Stop();
                    UnmappedSound.OOFPHCHKOBE[0] = UnmappedGlobals.JFHPHDKKECG("Music", "Theme00") as AudioClip;
                    UnmappedSound.OGCBMJIIKPP.clip = UnmappedSound.OOFPHCHKOBE[0];
                    UnmappedSound.OGCBMJIIKPP.time = 0f;
                    UnmappedSound.OGCBMJIIKPP.Play();
                    UnmappedMenus.PMIIOCMHEAE(1);
                }
                else if (foc > 0 && _delay <= 0)
                {
                    UnmappedMenu menu = UnmappedMenus.FKANHDIMMBJ[foc];
                    if (dir == 0)
                    {
                        dir = menu.JPMOFJPKINC >= 0 ? 1 : -1;
                    }

                    if (dir < 0)
                    {
                        if (foc > 1)
                        {
                            UnmappedMenu menu2 = UnmappedMenus.FKANHDIMMBJ[foc - 1];
                            Text text1 = menu.JGHBIPNIHBK.transform.Find("Value").gameObject.GetComponent<Text>();
                            Text text2 = menu2.JGHBIPNIHBK.transform.Find("Value").gameObject.GetComponent<Text>();
                            menu2.NKEDCLBOOMJ = text1.text;
                            menu.NKEDCLBOOMJ = text2.text;
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null)
                            {
                                UnmappedSprites.BBLJCJMDDLO(_menu.MGHGFEHHEBA, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            UnmappedSprites.BBLJCJMDDLO(_menu.MGHGFEHHEBA, 0.3f, 0.9f, 0.3f);
                            UnmappedMenus.NNMDEFLLNBF--;
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
                            UnmappedMenu menu2 = UnmappedMenus.FKANHDIMMBJ[foc + 1];
                            Text text1 = menu.JGHBIPNIHBK.transform.Find("Value").gameObject.GetComponent<Text>();
                            Text text2 = menu2.JGHBIPNIHBK.transform.Find("Value").gameObject.GetComponent<Text>();
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null)
                            {
                                UnmappedSprites.BBLJCJMDDLO(_menu.MGHGFEHHEBA, 0.8f, 0.8f, 0.8f);
                            }

                            _menu = menu2;
                            UnmappedSprites.BBLJCJMDDLO(_menu.MGHGFEHHEBA, 0.9f, 0.3f, 0.3f);
                            UnmappedMenus.NNMDEFLLNBF++;
                            _delay = _dd;
                            if (_dd > 1)
                            {
                                _dd -= 1;
                            }

                            foc++;
                        }
                    }
                }

                UnmappedMenus.PGKOMOIMNJN = UnmappedMenus.NNMDEFLLNBF;

                _delay--;

                return false;
            }

            if (UnmappedMenus.FAKHAFKOBPB == 1002)
            {
                UnmappedControls.NCOEPCFFBJA();
                UnmappedMenus.PIELJFKJFKF();
                UnmappedMenus.BBICLKGGIGB();

                for (int num = 1; num <= UnmappedMenus.HOAOLPGEBKJ; num++)
                {
                    if (UnmappedMenus.FKANHDIMMBJ[num].JPMOFJPKINC != 0f && UnmappedMenus.PIEMLEPEDFN == 0 &&
                        UnmappedMenus.CMOMBJMMOBK > 10f && UnmappedControls.LMADDGDMBGB == 0f &&
                        UnmappedMenus.FKANHDIMMBJ[num].LJJINGNDEJN() == 0)
                    {
                        if (UnmappedMenus.NNMDEFLLNBF > 0)
                        {
                            UnmappedMenus.PIEMLEPEDFN = 1;
                            UnmappedMenus.NNMDEFLLNBF = -1;
                        }
                    }
                }

                if (UnmappedMenus.PIEMLEPEDFN >= 5 && UnmappedMenus.NNMDEFLLNBF == -1)
                {
                    UnmappedSound.DNNPEAOCDOG(UnmappedSound.PAJJMPLBDPL, 0f, 0.5f);
                    MetaFile.Data.FirstLaunch = false;
                    MetaFile.Data.Save();
                    UnmappedMenus.PMIIOCMHEAE(1);
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
        if (UnmappedMenus.FAKHAFKOBPB > 1000)
        {
            UnmappedMenus.EHLDKHKMHNG();
            UnmappedMenus.NNMDEFLLNBF = 0;
            UnmappedMenus.HOAOLPGEBKJ = IPCCBDAFNMC;
            UnmappedMenus.FKANHDIMMBJ = new UnmappedMenu[UnmappedMenus.HOAOLPGEBKJ + 1];
            for (UnmappedMenus.KJELLNJFNGO = 1;
                 UnmappedMenus.KJELLNJFNGO <= UnmappedMenus.HOAOLPGEBKJ;
                 UnmappedMenus.KJELLNJFNGO++)
            {
                UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.KJELLNJFNGO] = new UnmappedMenu();
                UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.KJELLNJFNGO].PLFGKLGCOMD = UnmappedMenus.KJELLNJFNGO;
            }

            UnmappedMenus.BDCAECFNCFK = 0f;
            UnmappedMenus.MEIGJEIKBBN = 0f;
            UnmappedMenus.BHNAPHLDMMI = 0f;
            UnmappedMenus.BNMINLJJJKE = 0f;
            float num = 0f;
            float num2 = 0f;
            float num3 = 80f;
            float num4 = 1.6f;
            if (UnmappedMenus.FAKHAFKOBPB == 1001)
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
                    UnmappedMenus.DFLLBNMHHIH();
                    float x = startX + (index % columns * 210 * scale);
                    float y = startY - (index / columns * 50 * scale);
                    UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].ICGNAJFLAHL(6,
                        "#" + (index + 1) +
                        (index == 0 ? " (highest)" : index == Prefixes.Count - 1 ? " (lowest)" : ""), x, y, scale,
                        scale);
                    UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].JGHBIPNIHBK.transform.Find("Value").gameObject
                        .GetComponent<Text>().text = prefix;
                    UnmappedSprites.BBLJCJMDDLO(UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].MGHGFEHHEBA, 0.8f, 0.8f,
                        0.8f);
                    index++;
                }

                UnmappedMenus.DFLLBNMHHIH();
                UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].ICGNAJFLAHL(1, "Proceed", -150f, -280, 1.25f, 1.25f);
                UnmappedMenus.DFLLBNMHHIH();
                UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ]
                    .ICGNAJFLAHL(1, "Proceed & Hide", 150f, -280, 1.25f, 1.25f);
            }
            else if (UnmappedMenus.FAKHAFKOBPB == 1002)
            {
                UnmappedMenus.DFLLBNMHHIH();
                UnmappedMenus.FKANHDIMMBJ[UnmappedMenus.HOAOLPGEBKJ].ICGNAJFLAHL(1, "Proceed", 0f, -280, 1.25f, 1.25f);
            }

            UnmappedMenus.DADOHOENFJJ();
            if (UnmappedMenus.JAKMHECDDBI() > 0 && UnmappedMenus.HOAOLPGEBKJ > 0 && foc == 0)
            {
                foc = 1;
            }

            UnmappedMenus.PGKOMOIMNJN = foc;
            UnmappedMenus.PIEMLEPEDFN = 0;
            UnmappedKeyboard.GAHGPNAADHF = 0;
            UnmappedMenus.OMNBAENKANN = 0;
            UnmappedMenus.ANCKPGPLICF = 0;
            UnmappedMenus.GPGJCPOJCEP = 0;
            UnmappedMenus.CMOMBJMMOBK = 0f;
            UnmappedControls.LMADDGDMBGB = 10f;
            return false;
        }

        return true;
    }
}