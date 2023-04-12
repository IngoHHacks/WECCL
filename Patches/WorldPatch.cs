using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
public class WorldPatch
{
    [HarmonyPatch(typeof(World), "GHGPDLAMLFL")]
    [HarmonyPrefix]
    public static bool World_GHGPDLAMLFL(int DHLGJAHFOAM)
    {
        if (World.location == 3)
        {
            Debug.Log("Loading location " + World.location);
            World.waterDefault = 0f;
            World.no_baskets = 0;
            if (DHLGJAHFOAM != 0 && World.gArena != null)
            {
                Object.Destroy(World.gArena);
            }
            World.COJKOCKHJNI();

            World.gArena = Object.Instantiate(GetPrefab());
            
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
            
            var temp = World.location;
            
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
            
            World.location = 23;
            KONPHAKHJPP.KDPBDDEJPLC();
            World.location = temp;
            return false;
        }
        return true;
    }

    private static GameObject _prefab;
    
    private static GameObject GetPrefab()
    {
        if (_prefab == null)
        {
            var bundleLocation =
                @"C:\Program Files (x86)\Steam\steamapps\common\Wrestling Empire\BepInEx\plugins\Prefabs\template";
            _prefab = AssetBundle.LoadFromFile(bundleLocation).LoadAsset<GameObject>("Main");
        }
        return _prefab;
    }
}