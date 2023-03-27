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
        if (!_initialized && HasConflictingOverrides && !CustomConfigsSaveFile.Config.HideNextTime)
        {
            KPGIEHHDIDA.CGLNPGPJPJE();
            KPGIEHHDIDA.LHOICDLLMID = 1001;
            KPGIEHHDIDA.LHJLOPGFFPE = 0;
            KPGIEHHDIDA.OKILOINLLAO = 0;
            KPGIEHHDIDA.GHGPDLAMLFL();
            __instance.gLicense.SetActive(value: false);
            KPGIEHHDIDA.DGMLHKKIPEC.SetActive(value: false);
            Object.Destroy(GameObject.Find("Logo"));
            
            var obj = Object.Instantiate(BNNAONOIMBL.KCPCEFMNNNJ[1]);
            obj.transform.position = new Vector3(0f, 315f, 0f);
            obj.transform.localScale = new Vector3(2f, 2f, 1f);
            obj.transform.SetParent(KPGIEHHDIDA.APACKAAEEAH.transform, worldPositionStays: false);
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
        if (KPGIEHHDIDA.LHOICDLLMID == 1001)
        {
            JINPJBLJOMA.BOOMMCGAEEO();
            KPGIEHHDIDA.FFKDNJEKKNG();

            if (Input.GetMouseButtonDown(0))
            {
                foc = KPGIEHHDIDA.NAGCDENHJNE;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                foc = 0;
                KPGIEHHDIDA.NAGCDENHJNE = 0;
                _dd = 10;
                dir = 0;
                if (_menu != null) BNNAONOIMBL.AJEPIKJNHMP(_menu.ENKGCMNBJMP, 0.8f, 0.8f, 0.8f);
            }
            
            if (KPGIEHHDIDA.NAGCDENHJNE >= Prefixes.Count + 1)
            {
                if (KPGIEHHDIDA.NAGCDENHJNE == Prefixes.Count + 2)
                {
                    CustomConfigsSaveFile.Config.HideNextTime = true;
                }
                
                CKAMIAJJDBP.NMHCGFCNGPP(CKAMIAJJDBP.KCHIBLPPMAH, 0f, 0.5f);
                List<string> prefixes = new();
                for (int i = 1; i < Prefixes.Count + 1; i++)
                {
                    var text = KPGIEHHDIDA.KFGOFBAGLDH[i].JDAHAFDKBJG.transform.Find("Value").gameObject.GetComponent<Text>().text;
                    prefixes.Add(text);
                }
                Prefixes = prefixes;
                SavePrefixes();
                CKAMIAJJDBP.ILIKOLOBLBN.Stop();
                CKAMIAJJDBP.CKEJAMLGLAL[0] = GameGlobals.AMPMBILAJNM("Music", "Theme00") as AudioClip;
                CKAMIAJJDBP.ILIKOLOBLBN.clip = CKAMIAJJDBP.CKEJAMLGLAL[0];
                CKAMIAJJDBP.ILIKOLOBLBN.time = 0f;
                CKAMIAJJDBP.ILIKOLOBLBN.Play();
                KPGIEHHDIDA.BANOJFCLKIM(1);
            }
            else if (foc > 0 && _delay <= 0)
            {
                
                var menu = KPGIEHHDIDA.KFGOFBAGLDH[foc];
                if (dir == 0) dir = menu.AIIODMAGBKI >= 0 ? 1 : -1;
                if (dir < 0)
                {
                    if (foc > 1)
                    {
                        var menu2 = KPGIEHHDIDA.KFGOFBAGLDH[foc-1];
                        var text1 = menu.JDAHAFDKBJG.transform.Find("Value").gameObject.GetComponent<Text>();
                        var text2 = menu2.JDAHAFDKBJG.transform.Find("Value").gameObject.GetComponent<Text>();
                        menu2.IBOAEEIDBGL = text1.text;
                        menu.IBOAEEIDBGL = text2.text;
                        (text1.text, text2.text) = (text2.text, text1.text);
                        if (_menu != null) BNNAONOIMBL.AJEPIKJNHMP(_menu.ENKGCMNBJMP, 0.8f, 0.8f, 0.8f);
                        _menu = menu2;
                        BNNAONOIMBL.AJEPIKJNHMP(_menu.ENKGCMNBJMP, 0.3f, 0.9f, 0.3f);
                        KPGIEHHDIDA.NAGCDENHJNE--;
                        _delay = _dd;
                        if (_dd > 1) _dd -= 1;
                        foc--;
                    }
                } else if (dir > 0)
                {
                    if (foc < KPGIEHHDIDA.KFGOFBAGLDH.Length - 2)
                    {
                        var menu2 = KPGIEHHDIDA.KFGOFBAGLDH[foc+1];
                        var text1 = menu.JDAHAFDKBJG.transform.Find("Value").gameObject.GetComponent<Text>();
                        var text2 = menu2.JDAHAFDKBJG.transform.Find("Value").gameObject.GetComponent<Text>();
                        (text1.text, text2.text) = (text2.text, text1.text);
                        if (_menu != null) BNNAONOIMBL.AJEPIKJNHMP(_menu.ENKGCMNBJMP, 0.8f, 0.8f, 0.8f);
                        _menu = menu2;
                        BNNAONOIMBL.AJEPIKJNHMP(_menu.ENKGCMNBJMP, 0.9f, 0.3f, 0.3f);
                        KPGIEHHDIDA.NAGCDENHJNE++;
                        _delay = _dd;
                        if (_dd > 1) _dd -= 1;
                        foc++;
                    }
                }
            }
            KPGIEHHDIDA.CMFJNCPCPIO = KPGIEHHDIDA.NAGCDENHJNE;
            
            _delay--;
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(KPGIEHHDIDA), "GHGPDLAMLFL")]
    [HarmonyPostfix]
    public static void KPGIEHHDIDA_GHGPDLAMLFL()
    {
        if (KPGIEHHDIDA.LHOICDLLMID == 1001)
        {
            int rows;
            int columns;
            float scale;
            int startX;
            int startY;
            FindBestFit(Prefixes.Count, -450, -200, 450, 250, out rows, out columns, out scale, out startX, out startY);
            int index = 0;
            foreach (var prefix in Prefixes)
            {
                KPGIEHHDIDA.IHCMEHJGDGH();
                var x = startX + (index % columns * 210 * scale);
                var y = startY - (index / columns * 50 * scale);
                KPGIEHHDIDA.KFGOFBAGLDH[KPGIEHHDIDA.FPCGMGGJBKD].GHGPDLAMLFL(6, "#" + (index + 1) + (index == 0 ? " (highest)" : index == Prefixes.Count - 1 ? " (lowest)" : ""), x, y, scale, scale);
                KPGIEHHDIDA.KFGOFBAGLDH[KPGIEHHDIDA.FPCGMGGJBKD].JDAHAFDKBJG.transform.Find("Value").gameObject.GetComponent<Text>().text = prefix;
                BNNAONOIMBL.AJEPIKJNHMP(KPGIEHHDIDA.KFGOFBAGLDH[KPGIEHHDIDA.FPCGMGGJBKD].ENKGCMNBJMP, 0.8f, 0.8f, 0.8f);
                index++;
            }
            
            KPGIEHHDIDA.IHCMEHJGDGH();
            KPGIEHHDIDA.KFGOFBAGLDH[KPGIEHHDIDA.FPCGMGGJBKD].GHGPDLAMLFL(1, "Proceed", -150f, -280, 1.25f, 1.25f);
            KPGIEHHDIDA.IHCMEHJGDGH();
            KPGIEHHDIDA.KFGOFBAGLDH[KPGIEHHDIDA.FPCGMGGJBKD].GHGPDLAMLFL(1, "Proceed & Hide", 150f, -280, 1.25f, 1.25f);
        }
    }
}