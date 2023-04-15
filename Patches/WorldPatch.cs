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
        try
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
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("Error loading location " + World.location + ": " + e);
        }

        return true;
    }
    
    private static int _tempLocation = -999;

    [HarmonyPatch(typeof(KONPHAKHJPP), "KDPBDDEJPLC")]
    [HarmonyPrefix]
    public static void KONPHAKHJPP_KDPBDDEJPLC()
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            KONPHAKHJPP.GEHOOPJCAKD = 0;
            KONPHAKHJPP.NDMKCOCBPAD = 0;
            KONPHAKHJPP.IMLGDANNNJE = 0;
            KONPHAKHJPP.GNHIAENOPLP = new HDLGBEEGFLH[KONPHAKHJPP.NDMKCOCBPAD + 1];
            KONPHAKHJPP.GNHIAENOPLP[0] = new HDLGBEEGFLH();
            KONPHAKHJPP.DNHJBMMJOCM = 0;
            KONPHAKHJPP.NMKBGBEIKOD = 0;
            KONPHAKHJPP.KAEKIEBHBEN = new DFJEEHIJCBN[KONPHAKHJPP.NMKBGBEIKOD + 1];
            KONPHAKHJPP.KAEKIEBHBEN[0] = new DFJEEHIJCBN();
            KONPHAKHJPP.LCDOOCIJIDJ = 0;
            KONPHAKHJPP.PKKDOKFBMDF = new NDIILPMNNIO[KONPHAKHJPP.LCDOOCIJIDJ + 1];
            KONPHAKHJPP.PKKDOKFBMDF[0] = new NDIILPMNNIO();
            _tempLocation = World.location;
            World.location = 999;
        }
    }

    [HarmonyPatch(typeof(KONPHAKHJPP), "KDPBDDEJPLC")]
    [HarmonyPostfix]
    public static void KONPHAKHJPP_KDPBDDEJPLC_Postfix()
    {

        if (_tempLocation != -999)
        {
            World.location = _tempLocation;
            _tempLocation = -999;

            var last = KONPHAKHJPP.NDMKCOCBPAD;
            KONPHAKHJPP.GEHOOPJCAKD = last;
            
            var colliders = World.gArena.GetComponentsInChildren<MeshCollider>();
            foreach (var collider in colliders)
            {
                if (Math.Abs(collider.transform.rotation.eulerAngles.x - 90) > 1)
                {
                    continue;
                }
                Matrix4x4 matrix = collider.transform.localToWorldMatrix;
                var corners = new Vector3[4];
                var up = collider.transform.up * 5;
                corners[0] = matrix.MultiplyPoint3x4(new Vector3(5, 0, 5));
                corners[1] = matrix.MultiplyPoint3x4(new Vector3(5, 0, -5));
                corners[2] = matrix.MultiplyPoint3x4(new Vector3(-5, 0, 5));
                corners[3] = matrix.MultiplyPoint3x4(new Vector3(-5, 0, -5));
                
                Array.Sort(corners, (a, b) =>
                {
                    return a.y.CompareTo(b.y);
                });
                var yBottom = corners[0].y;
                var yTop = corners[3].y;
                
                corners[0] -= up;
                corners[1] -= up;
                corners[2] = corners[1] + up * 2;
                corners[3] = corners[0] + up * 2;

                var xSorted = new Vector3[4];
                var zSorted = new Vector3[4];
                Array.Copy(corners, xSorted, 4);
                Array.Copy(corners, zSorted, 4);
                Array.Sort(xSorted, (a, b) =>
                {
                    return a.x.CompareTo(b.x);
                });
                Array.Sort(zSorted, (a, b) =>
                {
                    return a.z.CompareTo(b.z);
                });
                
                Vector3 topRight = corners[0];
                Vector3 bottomRight = corners[0];
                Vector3 bottomLeft = corners[0];
                Vector3 topLeft = corners[0];

                if (zSorted[3].x > zSorted[2].x)
                {
                    topRight = zSorted[3];
                }
                else
                {
                    topRight = zSorted[2];
                }
                if (zSorted[1].x > zSorted[0].x)
                {
                    bottomRight = zSorted[1];
                }
                else
                {
                    bottomRight = zSorted[0];
                }
                if (zSorted[1].x < zSorted[0].x)
                {
                    bottomLeft = zSorted[1];
                }
                else
                {
                    bottomLeft = zSorted[0];
                }
                if (zSorted[3].x < zSorted[2].x)
                {
                    topLeft = zSorted[3];
                }
                else
                {
                    topLeft = zSorted[2];
                }


                foreach (var corner in corners)
                {
                    Plugin.Log.LogInfo("Corner: " + corner);
                }
                
                Plugin.Log.LogInfo("Top right: " + topRight);
                Plugin.Log.LogInfo("Right Bottom right: " + bottomRight);
                Plugin.Log.LogInfo("Bottom Bottom left: " + bottomLeft);
                Plugin.Log.LogInfo("Top left: " + topLeft);
                

                // Create block
                KONPHAKHJPP.GEHOOPJCAKD++;
                var GEHOOPJCAKD = KONPHAKHJPP.GEHOOPJCAKD;
                KONPHAKHJPP.IHCMEHJGDGH();
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].AAAHCCBLLMN = 0f;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].AHOBHCOPAIN = yBottom;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].KHDIGLGDCDG = yTop;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].PADJKHFHPMP = 0;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].DPDGDOOCONK[1] = topRight.x;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].IAJJHLBOCMI[1] = topRight.z;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].DPDGDOOCONK[2] = bottomRight.x;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].IAJJHLBOCMI[2] = bottomRight.z;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].DPDGDOOCONK[3] = bottomLeft.x;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].IAJJHLBOCMI[3] = bottomLeft.z;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].DPDGDOOCONK[4] = topLeft.x;
                KONPHAKHJPP.GNHIAENOPLP[GEHOOPJCAKD].IAJJHLBOCMI[4] = topLeft.z;
            }
            for (KONPHAKHJPP.GEHOOPJCAKD = last + 1; KONPHAKHJPP.GEHOOPJCAKD <= KONPHAKHJPP.NDMKCOCBPAD; KONPHAKHJPP.GEHOOPJCAKD++)
            {
                if (KONPHAKHJPP.GNHIAENOPLP[KONPHAKHJPP.GEHOOPJCAKD].IIBNKAGIFHK != null)
                {
                    KONPHAKHJPP.GNHIAENOPLP[KONPHAKHJPP.GEHOOPJCAKD].LMGGHIOODKC = KONPHAKHJPP.GNHIAENOPLP[KONPHAKHJPP.GEHOOPJCAKD].IIBNKAGIFHK.transform.localEulerAngles;
                    KONPHAKHJPP.DNHJBMMJOCM = 1;
                }
            }
            
        }

        if (Plugin.DebugRender.Value)
        {
            var arr = KONPHAKHJPP.GNHIAENOPLP;
            for (int i = 1; i < arr.Length; i++)
            {
                try
                {
                    var scene = World.gArena;
                    var x4 = arr[i].DPDGDOOCONK; // float[5], x4[0] is always 0
                    var z4 = arr[i].IAJJHLBOCMI; // float[5], z4[0] is always 0
                    var yLow = arr[i].AHOBHCOPAIN; // float
                    var yHigh = arr[i].KHDIGLGDCDG; // float
                    var type = arr[i].PADJKHFHPMP; // int
                    var color = Color.red;
                    switch (type)
                    {
                        case -6:
                            color = new Color(0f, 1f, 0f);
                            break;
                        case -5:
                            color = new Color(0f, 1f, 0.5f);
                            break;
                        case -4:
                            color = new Color(0f, 1f, 1f);
                            break;
                        case -3:
                            color = new Color(0f, 0.5f, 1f);
                            break;
                        case -2:
                            color = new Color(0f, 0f, 1f);
                            break;
                        case -1:
                            color = new Color(0.5f, 0f, 1f);
                            break;
                        case 1:
                            color = new Color(1f, 0f, 1f);
                            break;
                        case 11:
                            color = new Color(1f, 0f, 0f);
                            break;
                        case 12:
                            color = new Color(1f, 1f, 0f);
                            break;
                    }

                    DrawCube(scene.transform, x4, yLow, yHigh, z4, color);

                    if (arr[i].LHGLJFOBMMC != null)
                    {
                        var xl = arr[i].LHGLJFOBMMC.Skip(1);
                        var zl = arr[i].CGNINMDBMFH.Skip(1);
                        var color2 = new Color(1f, 1f, 1f);
                        DrawLines(scene.transform, xl.ToArray(), yLow, yHigh, zl.ToArray(), color2, "Seating");
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.LogWarning(e);
                }
            }

            var arr2 = KONPHAKHJPP.PKKDOKFBMDF;
            for (int i = 1; i < arr2.Length; i++)
            {
                try
                {
                    var scene = World.gArena;
                    var x4 = arr2[i].DPDGDOOCONK; // float[5], x4[0] is always 0
                    var z4 = arr2[i].IAJJHLBOCMI; // float[5], z4[0] is always 0
                    var yLow = 0;
                    var yHigh = 0;
                    var color = new Color(1f, 0.5f, 0f);
                    DrawCube(scene.transform, x4, yLow, yHigh, z4, color);
                }
                catch (Exception e)
                {
                    Plugin.Log.LogWarning(e);
                }
            }

            var arr3 = KONPHAKHJPP.KAEKIEBHBEN;
            for (int i = 1; i < arr3.Length; i++)
            {
                try
                {
                    var scene = World.gArena;
                    var x4 = arr3[i].DPDGDOOCONK; // float[5], x4[0] is always 0
                    var z4 = arr3[i].IAJJHLBOCMI; // float[5], z4[0] is always 0
                    var yLow = 0;
                    var yHigh = arr3[i].KHDIGLGDCDG; // float

                    var color = new Color(0.5f, 1f, 0f);
                    DrawCube(scene.transform, x4, yLow, yHigh, z4, color);
                }
                catch (Exception e)
                {
                    Plugin.Log.LogWarning(e);
                }
            }
        }
    }

    private static void DrawCube(Transform parent, float[] x4, float yLow, float yHigh, float[] z4, Color color)
    {
        // Bottom
        var child = new GameObject("Bottom");
        child.transform.parent = parent;
        var lineRenderer = child.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, new Vector3(x4[1], yLow, z4[1]));
        lineRenderer.SetPosition(1, new Vector3(x4[2], yLow, z4[2]));
        lineRenderer.SetPosition(2, new Vector3(x4[3], yLow, z4[3]));
        lineRenderer.SetPosition(3, new Vector3(x4[4], yLow, z4[4]));
        lineRenderer.sortingOrder = 999;
        lineRenderer.loop = true;

        // Top
        child = new GameObject("Top");
        child.transform.parent = parent;
        lineRenderer = child.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 4;
        lineRenderer.SetPosition(0, new Vector3(x4[1], yHigh, z4[1]));
        lineRenderer.SetPosition(1, new Vector3(x4[2], yHigh, z4[2]));
        lineRenderer.SetPosition(2, new Vector3(x4[3], yHigh, z4[3]));
        lineRenderer.SetPosition(3, new Vector3(x4[4], yHigh, z4[4]));
        lineRenderer.sortingOrder = 999;
        lineRenderer.loop = true;

        // Sides
        child = new GameObject("Side1");
        child.transform.parent = parent;
        lineRenderer = child.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(x4[1], yLow, z4[1]));
        lineRenderer.SetPosition(1, new Vector3(x4[1], yHigh, z4[1]));
        lineRenderer.sortingOrder = 999;

        child = new GameObject("Side2");
        child.transform.parent = parent;
        lineRenderer = child.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(x4[2], yLow, z4[2]));
        lineRenderer.SetPosition(1, new Vector3(x4[2], yHigh, z4[2]));
        lineRenderer.sortingOrder = 999;

        child = new GameObject("Side3");
        child.transform.parent = parent;
        lineRenderer = child.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(x4[3], yLow, z4[3]));
        lineRenderer.SetPosition(1, new Vector3(x4[3], yHigh, z4[3]));
        lineRenderer.sortingOrder = 999;

        child = new GameObject("Side4");
        child.transform.parent = parent;
        lineRenderer = child.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(x4[4], yLow, z4[4]));
        lineRenderer.SetPosition(1, new Vector3(x4[4], yHigh, z4[4]));
        lineRenderer.sortingOrder = 999;
    }

    private static void DrawLines(Transform parent, float[] xl, float yLow, float yHigh, float[] zl, Color color,
        string name = "Lines")
    {
        var count = Math.Min(xl.Length, zl.Length);
        var child = new GameObject(name);
        child.transform.parent = parent;
        var lineRenderer = child.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = count;
        for (var i = 0; i < count; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(xl[i], yLow, zl[i]));
        }

        lineRenderer.sortingOrder = 999;
        lineRenderer.loop = true;
    }

    [HarmonyPatch(typeof(World), "EKDPJJFKIEJ")]
    [HarmonyPrefix]
    public static bool World_EKDPJJFKIEJ(int FPJOPENMOGN)
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            World.ground = 0f;
            World.ceiling = 100f;
            World.farNorth = 100f;
            World.farSouth = -100f;
            World.farEast = 100f;
            World.farWest = -100f;
            World.camNorth = 60f;
            World.camSouth = -60f;
            World.camEast = 60;
            World.camWest = -60f;
            return false;
        }

        return true;
    }


    [HarmonyPatch(typeof(World), "KEPCOHBMNDK")]
    [HarmonyPrefix]
    public static bool World_KEPCOHBMNDK(ref int __result, int FPJOPENMOGN)
    {
        __result = 1;
        if (World.mapVersion < 2f)
        {
            if (FPJOPENMOGN is >= 17 and < 21 ||
                FPJOPENMOGN - VanillaCounts.NoLocations - 1 >= CustomArenaPrefabs.Count)
            {
                __result = 0;
            }
        }

        return false;
    }

    [HarmonyPatch(typeof(World), "MEKHOHAMAOJ")]
    [HarmonyPostfix]
    public static void World_MEKHOHAMAOJ(ref float __result, float GHGHLNECCPN, float KHNBEHPOGMD, float GEKJOOHPPCB)
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            if (World.ringShape != 0 && GHGHLNECCPN > -40f && GHGHLNECCPN < 40f && GEKJOOHPPCB > -40f && GEKJOOHPPCB < 40f)
            {
                return;
            }

            var coords = new Vector3(GHGHLNECCPN, KHNBEHPOGMD, GEKJOOHPPCB);
            // Raycast down to find the ground
            var ray = new Ray(coords + new Vector3(0, 5f, 0f), Vector3.down);
            if (Physics.Raycast(ray, out var hit, 105f, 1 << 0))
            {
                __result = hit.point.y;
            }
            else
            {
                __result = World.ground;
            }   
        }
    }
}