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
            GameScreens.NCMGGBCDILG();
            GameScreens.OBNLIIMODBI = 1002;
            GameScreens.HMMHJLDCGFJ = 0;
            GameScreens.BABHEGOMNLJ = 0;
            GameScreens.ICKGKDOKJEN();
            __instance.gLicense.SetActive(value: false);
            GameScreens.DCBAOOEJKDN.SetActive(value: false);
            Object.Destroy(GameObject.Find("Logo"));
            var obj = Object.Instantiate(GameSprites.LHJLHHFLELN[1]);
            obj.transform.position = new Vector3(0f, 50f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 5;
            obj.transform.SetParent(GameScreens.NMLFLIDGDAF.transform, worldPositionStays: false);
            Object.Destroy(obj.transform.Find("Background").gameObject);
            Object.Destroy(obj.transform.Find("Border").gameObject);
            Object.Destroy(obj.transform.Find("Sheen").gameObject);
            Object.Destroy(obj.transform.Find("Corners").gameObject);
            obj.transform.Find("Title").gameObject.GetComponent<Text>().text = "<color=#EF0000>IMPORTANT NOTICE</color>\n While WECCL tries its best to keep modded save files stable and consistent between game updates and mod changes, you may still encounter issues. WECCL automatically creates backups (up to 100 by default). However, it is recommended to manually create backups of your save file. The save file can be found in %USERPROFILE%/AppData/LocalLow/MDickie/Wrestling Empire. If you encounter any issues, please report them in the Wrestling Empire Modding Discord server.";
        }
        else if (!_initialized && HasConflictingOverrides && !MetaFile.Data.HidePriorityScreenNextTime)
        {
            GameScreens.NCMGGBCDILG();
            GameScreens.OBNLIIMODBI = 1001;
            GameScreens.HMMHJLDCGFJ = 0;
            GameScreens.BABHEGOMNLJ = 0;
            GameScreens.ICKGKDOKJEN();
            __instance.gLicense.SetActive(value: false);
            GameScreens.DCBAOOEJKDN.SetActive(value: false);
            Object.Destroy(GameObject.Find("Logo"));
            
            var obj = Object.Instantiate(GameSprites.LHJLHHFLELN[1]);
            obj.transform.position = new Vector3(0f, 315f, 0f);
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            RectTransform rt = obj.transform.Find("Title").transform as RectTransform;
            rt.sizeDelta *= 2;
            obj.transform.SetParent(GameScreens.NMLFLIDGDAF.transform, worldPositionStays: false);
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
            if (GameScreens.OBNLIIMODBI == 1001)
            {
                GameControls.GCGDPDLEHPH();
                GameScreens.OFFHPAMPIHN();

                if (Input.GetMouseButtonDown(0))
                {
                    foc = GameScreens.PDMDFGNJCPN;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    foc = 0;
                    GameScreens.PDMDFGNJCPN = 0;
                    _dd = 10;
                    dir = 0;
                    if (_menu != null) GameSprites.JAAOOCGLBNN(_menu.KNOKHLBGDKO, 0.8f, 0.8f, 0.8f);
                }

                if (GameScreens.PDMDFGNJCPN >= Prefixes.Count + 1)
                {
                    if (GameScreens.PDMDFGNJCPN == Prefixes.Count + 2)
                    {
                        MetaFile.Data.HidePriorityScreenNextTime = true;
                    }

                    GameAudio.MDIKCPGDILK(GameAudio.KNIDFHCNJPF, 0f, 0.5f);
                    List<string> prefixes = new();
                    for (int i = 1; i < Prefixes.Count + 1; i++)
                    {
                        var text = GameScreens.FPLAGLKCKII[i].HBJNBEDIFIG.transform.Find("Value").gameObject
                            .GetComponent<Text>().text;
                        prefixes.Add(text);
                    }

                    Prefixes = prefixes;
                    SavePrefixes();
                    GameAudio.JOLFNKOGLLL.Stop();
                    GameAudio.BDMIHNKDBDF[0] = GameGlobals.KNPGMHAEKJO("Music", "Theme00") as AudioClip;
                    GameAudio.JOLFNKOGLLL.clip = GameAudio.BDMIHNKDBDF[0];
                    GameAudio.JOLFNKOGLLL.time = 0f;
                    GameAudio.JOLFNKOGLLL.Play();
                    GameScreens.KGAMHBKDPCB(1);
                }
                else if (foc > 0 && _delay <= 0)
                {

                    var menu = GameScreens.FPLAGLKCKII[foc];
                    if (dir == 0) dir = menu.LDPBBENEMHM >= 0 ? 1 : -1;
                    if (dir < 0)
                    {
                        if (foc > 1)
                        {
                            var menu2 = GameScreens.FPLAGLKCKII[foc - 1];
                            var text1 = menu.HBJNBEDIFIG.transform.Find("Value").gameObject.GetComponent<Text>();
                            var text2 = menu2.HBJNBEDIFIG.transform.Find("Value").gameObject.GetComponent<Text>();
                            menu2.AABGEEFANFM = text1.text;
                            menu.AABGEEFANFM = text2.text;
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null) GameSprites.JAAOOCGLBNN(_menu.KNOKHLBGDKO, 0.8f, 0.8f, 0.8f);
                            _menu = menu2;
                            GameSprites.JAAOOCGLBNN(_menu.KNOKHLBGDKO, 0.3f, 0.9f, 0.3f);
                            GameScreens.PDMDFGNJCPN--;
                            _delay = _dd;
                            if (_dd > 1) _dd -= 1;
                            foc--;
                        }
                    }
                    else if (dir > 0)
                    {
                        if (foc < Prefixes.Count)
                        {
                            var menu2 = GameScreens.FPLAGLKCKII[foc + 1];
                            var text1 = menu.HBJNBEDIFIG.transform.Find("Value").gameObject.GetComponent<Text>();
                            var text2 = menu2.HBJNBEDIFIG.transform.Find("Value").gameObject.GetComponent<Text>();
                            (text1.text, text2.text) = (text2.text, text1.text);
                            if (_menu != null) GameSprites.JAAOOCGLBNN(_menu.KNOKHLBGDKO, 0.8f, 0.8f, 0.8f);
                            _menu = menu2;
                            GameSprites.JAAOOCGLBNN(_menu.KNOKHLBGDKO, 0.9f, 0.3f, 0.3f);
                            GameScreens.PDMDFGNJCPN++;
                            _delay = _dd;
                            if (_dd > 1) _dd -= 1;
                            foc++;
                        }
                    }
                }

                GameScreens.JJDBPPGNGHL = GameScreens.PDMDFGNJCPN;

                _delay--;

                return false;
            }

            if (GameScreens.OBNLIIMODBI == 1002)
            {
                GameControls.GCGDPDLEHPH();
                GameScreens.OFFHPAMPIHN();
                GameScreens.DGIBKBFIJJD();

                for (int num = 1; num <= GameScreens.CFPJFAKOKMD; num++)
                {
                    if (GameScreens.FPLAGLKCKII[num].LDPBBENEMHM != 0f && GameScreens.KGNNPFLHMPF == 0 &&
                        GameScreens.OLPCEEJPNAF > 10f && GameControls.DBEEFCPGGCP == 0f && GameScreens.FPLAGLKCKII[num].LFBLCLADKOM() == 0)
                    {
                        if (GameScreens.PDMDFGNJCPN > 0)
                        {
                            GameScreens.KGNNPFLHMPF = 1;
                            DNDIEGNJOKN.PDMDFGNJCPN = -1;
                        }
                    }
                }

                if (GameScreens.KGNNPFLHMPF >= 5 && GameScreens.PDMDFGNJCPN == -1)
                {
                    GameAudio.MDIKCPGDILK(GameAudio.KNIDFHCNJPF, 0f, 0.5f);
                    MetaFile.Data.FirstLaunch = false;
                    MetaFile.Data.Save();
                    GameScreens.KGAMHBKDPCB(1);
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

    [HarmonyPatch(typeof(GameScreens), nameof(GameScreens.ICKGKDOKJEN))]
    [HarmonyPrefix]
    public static bool GameScreens_ICKGKDOKJEN(int PBKICCGONGP)
    {
        if (GameScreens.OBNLIIMODBI > 1000)
        {
            GameScreens.NCMGGBCDILG();
            GameScreens.PDMDFGNJCPN = 0;
            GameScreens.CFPJFAKOKMD = PBKICCGONGP;
            GameScreens.FPLAGLKCKII = new GameMenus[GameScreens.CFPJFAKOKMD + 1];
            for (GameScreens.BCAKDEKDCHC = 1; GameScreens.BCAKDEKDCHC <= GameScreens.CFPJFAKOKMD; GameScreens.BCAKDEKDCHC++)
            {
                GameScreens.FPLAGLKCKII[GameScreens.BCAKDEKDCHC] = new GameMenus();
                GameScreens.FPLAGLKCKII[GameScreens.BCAKDEKDCHC].NMKACNOOPPC = GameScreens.BCAKDEKDCHC;
            }
            GameScreens.MADKBIDIKAN = 0f;
            GameScreens.JDECBBFHPPG = 0f;
            GameScreens.MHJBIDLIGKP = 0f;
            GameScreens.BIGIPLDLMOB = 0f;
            float num = 0f;
            float num2 = 0f;
            float num3 = 80f;
            float num4 = 1.6f;
            if (GameScreens.OBNLIIMODBI == 1001)
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
                    GameScreens.LKMAEOFENHG();
                    var x = startX + (index % columns * 210 * scale);
                    var y = startY - (index / columns * 50 * scale);
                    GameScreens.FPLAGLKCKII[GameScreens.CFPJFAKOKMD].ICKGKDOKJEN(6,
                        "#" + (index + 1) +
                        (index == 0 ? " (highest)" : index == Prefixes.Count - 1 ? " (lowest)" : ""), x, y, scale,
                        scale);
                    GameScreens.FPLAGLKCKII[GameScreens.CFPJFAKOKMD].HBJNBEDIFIG.transform.Find("Value").gameObject
                        .GetComponent<Text>().text = prefix;
                    GameSprites.JAAOOCGLBNN(GameScreens.FPLAGLKCKII[GameScreens.CFPJFAKOKMD].KNOKHLBGDKO, 0.8f, 0.8f,
                        0.8f);
                    index++;
                }

                GameScreens.LKMAEOFENHG();
                GameScreens.FPLAGLKCKII[GameScreens.CFPJFAKOKMD].ICKGKDOKJEN(1, "Proceed", -150f, -280, 1.25f, 1.25f);
                GameScreens.LKMAEOFENHG();
                GameScreens.FPLAGLKCKII[GameScreens.CFPJFAKOKMD]
                    .ICKGKDOKJEN(1, "Proceed & Hide", 150f, -280, 1.25f, 1.25f);
            }
            else if (GameScreens.OBNLIIMODBI == 1002)
            {
                GameScreens.LKMAEOFENHG();
                GameScreens.FPLAGLKCKII[GameScreens.CFPJFAKOKMD].ICKGKDOKJEN(1, "Proceed", 0f, -280, 1.25f, 1.25f);
            }
            
            GameScreens.CHJIBAPDFEL();
            if (GameScreens.DACPNCCINEK() > 0 && GameScreens.CFPJFAKOKMD > 0 && foc == 0)
            {
                foc = 1;
            }
            GameScreens.JJDBPPGNGHL = foc;
            GameScreens.KGNNPFLHMPF = 0;
            GameKeyboard.OHFANNKCCJE = 0;
            GameScreens.PEPAOPHOJBL = 0;
            GameScreens.LILMCJLMDPL = 0;
            GameScreens.LGLOIBPBCBP = 0;
            GameScreens.OLPCEEJPNAF = 0f;
            GameControls.DBEEFCPGGCP = 10f;
            return false;
        }

        return true;
    }
}