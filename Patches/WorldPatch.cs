using WECCL.Content;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
public class WorldPatch
{
    [HarmonyPatch(typeof(World), "GHGPDLAMLFL")]
    [HarmonyPrefix]
    public static bool World_GHGPDLAMLFL(int DHLGJAHFOAM)
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            Debug.Log("Loading location " + World.location);
            World.waterDefault = 0f;
            World.no_baskets = 0;
            if (DHLGJAHFOAM != 0 && World.gArena != null)
            {
                Object.Destroy(World.gArena);
            }

            World.COJKOCKHJNI();

            World.gArena = Object.Instantiate(CustomArenaPrefabs[World.location - VanillaCounts.NoLocations - 1]);

            if (KPGIEHHDIDA.LHOICDLLMID == 60)
            {
                World.gArena.transform.eulerAngles = new Vector3(0f, 170f, 0f);
            }

            if (Mathf.Abs(World.waterOffset) <= 1f)
            {
                World.waterOffset = 0f;
            }

            if (GameGlobals.LPHMMPEBCMM == 1)
            {
                World.waterOffset = World.floodLevel;
            }
            else
            {
                World.floodLevel = World.waterOffset;
            }

            World.waterLevel = World.waterDefault + World.waterOffset;
            World.DPJJFBECFEE();
            if (KPGIEHHDIDA.LHOICDLLMID == 60)
            {
                return false;
            }

            World.EKDPJJFKIEJ(World.location);
            if (KPGIEHHDIDA.LHOICDLLMID != 14)
            {
                CKAMIAJJDBP.PANGKBNCHNP();
            }

            if (DHLGJAHFOAM == 0)
            {
                if (World.ringShape > 0)
                {
                    World.MMPOPGHPAKH();
                }

                World.LOBEPMDGJLD();
            }

            KONPHAKHJPP.KDPBDDEJPLC();
            return false;
        }

        return true;
    }
    
    private static int _tempLoc = -999;

    [HarmonyPatch(typeof(KONPHAKHJPP), "KDPBDDEJPLC")]
    [HarmonyPrefix]
    public static bool KONPHAKHJPP_KDPBDDEJPLC()
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            _tempLoc = World.location;
            World.location = 0;
        }

        return true;
    }

    [HarmonyPatch(typeof(KONPHAKHJPP), "KDPBDDEJPLC")]
    [HarmonyPostfix]
    public static void KONPHAKHJPP_KDPBDDEJPLC_Postfix()
    {
        if (_tempLoc != -999)
        {
            World.location = _tempLoc;
            _tempLoc = -999;
        }
    }
    
    [HarmonyPatch(typeof(World), "KEPCOHBMNDK")]
    [HarmonyPrefix]
    public static bool World_KEPCOHBMNDK(ref int __result, int FPJOPENMOGN)
    {
        __result = 1;
        if (World.mapVersion < 2f && FPJOPENMOGN >= 17 && FPJOPENMOGN < 21)
        {
            __result = 0;
        }
        return false;
    }
}