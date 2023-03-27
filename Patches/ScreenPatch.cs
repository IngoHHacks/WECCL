using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
public class ScreenPatch
{
    [HarmonyPatch(typeof(Scene_Titles), "Start")]
    [HarmonyPostfix]
    public static void SceneTitles_Start()
    {
        KPGIEHHDIDA.CGLNPGPJPJE();
        KPGIEHHDIDA.LHOICDLLMID = 1001;
        KPGIEHHDIDA.LHJLOPGFFPE = 0;
        KPGIEHHDIDA.OKILOINLLAO = 0;
        KPGIEHHDIDA.GHGPDLAMLFL();
    }
    
    [HarmonyPatch(typeof(Scene_Titles), "Update")]
    [HarmonyPrefix]
    public static bool SceneTitles_Update()
    {
        if (KPGIEHHDIDA.LHOICDLLMID == 1001) return false;
        return true;
    }

    [HarmonyPatch(typeof(KPGIEHHDIDA), "GHGPDLAMLFL")]
    [HarmonyPostfix]
    public static void KPGIEHHDIDA_GHGPDLAMLFL()
    {
        if (KPGIEHHDIDA.LHOICDLLMID == 1001)
        {
            var num4 = 1f;
            int y = -50;
            foreach (var prefix in Prefixes)
            {
                KPGIEHHDIDA.IHCMEHJGDGH();
                KPGIEHHDIDA.KFGOFBAGLDH[KPGIEHHDIDA.FPCGMGGJBKD].GHGPDLAMLFL(1, prefix, 0f, y, num4, num4);
                y -= 50;
            }
            
            num4 = 1.5f;
            y -= 50;
            KPGIEHHDIDA.KFGOFBAGLDH[KPGIEHHDIDA.FPCGMGGJBKD].GHGPDLAMLFL(1, "Proceed", 0f, y, num4, num4);
        }
    }
}