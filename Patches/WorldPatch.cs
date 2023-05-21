using WECCL.Content;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
public class WorldPatch
{
    [HarmonyPatch(typeof(World), nameof(World.DMGJOHGEOKF))]
    [HarmonyPrefix]
    public static bool World_DMGJOHGEOKF(int BPPIGPFGLLG)
    {
        try
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                Debug.Log("Loading location " + World.location);
                World.waterDefault = 0f;
                World.no_baskets = 0;
                if (BPPIGPFGLLG != 0 && World.gArena != null)
                {
                    Object.Destroy(World.gArena);
                }

                World.HOOLOHJJIFF();
                World.gArena = Object.Instantiate(CustomArenaPrefabs[World.location - VanillaCounts.NoLocations - 1]);

                if (GameScreens.GLDIFJOEOIO == 60)
                {
                    World.gArena.transform.eulerAngles = new Vector3(0f, 170f, 0f);
                }

                if (Mathf.Abs(World.waterOffset) <= 1f)
                {
                    World.waterOffset = 0f;
                }

                if (GameGlobals.NNJIFJPMBGJ == 1)
                {
                    World.waterOffset = World.floodLevel;
                }
                else
                {
                    World.floodLevel = World.waterOffset;
                }

                World.waterLevel = World.waterDefault + World.waterOffset;
                World.BBLKECFGPOE();
                if (GameScreens.GLDIFJOEOIO == 60)
                {
                    return false;
                }

                World.OGAHGABKJFO(World.location);
                if (GameScreens.GLDIFJOEOIO != 14)
                {
                    GameAudio.CMCGCNHGLHN();
                }

                if (BPPIGPFGLLG == 0)
                {
                    if (World.ringShape > 0)
                    {
                        World.COELFJDMFAA();
                    }

                    World.AHKIEJMJPHF();
                }

                GameCollision.OJMNGOHBIJH();
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

    [HarmonyPatch(typeof(GameCollision), nameof(GameCollision.OJMNGOHBIJH))]
    [HarmonyPrefix]
    public static void GameCollision_OJMNGOHBIJH()
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            GameCollision.PEIFIJCKAOC = 0;
            GameCollision.NJCPFHDPBOJ = 0;
            GameCollision.FKKOPAMLAIE = 0;
            GameCollision.NNIOLGAKPGG = new AJILJNLIOOH[GameCollision.NJCPFHDPBOJ + 1];
            GameCollision.NNIOLGAKPGG[0] = new AJILJNLIOOH();
            GameCollision.HGKKJGCBEPH = 0;
            GameCollision.BKAPMFNPOIB = 0;
            GameCollision.BAJJBMHMGBH = new MCBHJKMPEMD[GameCollision.BKAPMFNPOIB + 1];
            GameCollision.BAJJBMHMGBH[0] = new MCBHJKMPEMD();
            GameCollision.DOFJDPIHJPG = 0;
            GameCollision.IKGAAJIDDEJ = new CILCKMOJGJG[GameCollision.DOFJDPIHJPG + 1];
            GameCollision.IKGAAJIDDEJ[0] = new CILCKMOJGJG();
            _tempLocation = World.location;
            World.location = 999;
            //Sets arenaShape to 0 here to stop spawning of defaut collisions while setting it to other shapes based on object in custom arena
            World.arenaShape = 0;
        }
    }

    [HarmonyPatch(typeof(GameCollision), nameof(GameCollision.OJMNGOHBIJH))]
    [HarmonyPostfix]
    public static void GameCollision_OJMNGOHBIJH_Postfix()
    {

        if (_tempLocation != -999)
        {
            ArenaPatch.SetCustomArenaShape();

            World.location = _tempLocation;
            _tempLocation = -999;

            var last = GameCollision.NJCPFHDPBOJ;
            GameCollision.PEIFIJCKAOC = last;

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
                    bottomRight = zSorted[2];
                }
                else
                {
                    topRight = zSorted[2];
                    bottomRight = zSorted[3];
                }
                if (zSorted[1].x < zSorted[0].x)
                {
                    bottomLeft = zSorted[1];
                    topLeft = zSorted[0];
                }
                else
                {
                    bottomLeft = zSorted[0];
                    topLeft = zSorted[1];
                }

                // Create block
                GameCollision.PEIFIJCKAOC++;
                var PEIFIJCKAOC = GameCollision.PEIFIJCKAOC;
                GameCollision.OHBONDHEDEC();
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].EBPHCBADBLI = 0f;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].IHBEDOLDPJC = yBottom;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].JKHPFKDCNJL = yTop;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].FPHEOHBIAFN = 0;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].BOBFOLNHBCL[1] = topRight.x;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].FDMAHBOILBC[1] = topRight.z;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].BOBFOLNHBCL[4] = bottomRight.x;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].FDMAHBOILBC[4] = bottomRight.z;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].BOBFOLNHBCL[3] = bottomLeft.x;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].FDMAHBOILBC[3] = bottomLeft.z;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].BOBFOLNHBCL[2] = topLeft.x;
                GameCollision.NNIOLGAKPGG[PEIFIJCKAOC].FDMAHBOILBC[2] = topLeft.z;
            }

            foreach (GameObject gameObject in (from t in World.gArena.GetComponentsInChildren<Transform>()
                                               where t.gameObject != null && t.gameObject.name.StartsWith("Barrier_Climbables")
                                               select t.gameObject).ToArray<GameObject>())
            {
                MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();

                Bounds bounds = meshCollider.sharedMesh.bounds;

                Vector3 center = bounds.center;
                Vector3 extents = bounds.extents;

                // Calculate the 8 corners of the bounding box
                Vector3[] corners = new Vector3[8];
                corners[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
                corners[1] = center + new Vector3(-extents.x, -extents.y, extents.z);
                corners[2] = center + new Vector3(extents.x, -extents.y, extents.z);
                corners[3] = center + new Vector3(extents.x, -extents.y, -extents.z);
                corners[4] = center + new Vector3(-extents.x, extents.y, -extents.z);
                corners[5] = center + new Vector3(-extents.x, extents.y, extents.z);
                corners[6] = center + new Vector3(extents.x, extents.y, extents.z);
                corners[7] = center + new Vector3(extents.x, extents.y, -extents.z);

                // Get the 8 corners of the bounding box as world position
                Vector3[] worldCorners = new Vector3[corners.Length];
                for (int i = 0; i < corners.Length; i++)
                {
                    worldCorners[i] = meshCollider.transform.TransformPoint(corners[i]);
                }

                GameCollision.PEIFIJCKAOC++;
                int peifijckaoc = GameCollision.PEIFIJCKAOC;
                GameCollision.OHBONDHEDEC();
                GameCollision.NNIOLGAKPGG[peifijckaoc].EBPHCBADBLI = 0f;
                GameCollision.NNIOLGAKPGG[peifijckaoc].IHBEDOLDPJC = worldCorners[0].y;
                GameCollision.NNIOLGAKPGG[peifijckaoc].JKHPFKDCNJL = worldCorners[1].y;
                GameCollision.NNIOLGAKPGG[peifijckaoc].FPHEOHBIAFN = 1;
                //Below controls the type of collision, tempted to replicate this foreach loop for other object names for other collision types like "Cage"
                GameCollision.NNIOLGAKPGG[peifijckaoc].HDFOLIBBAFE = "Barrier";
                GameCollision.NNIOLGAKPGG[peifijckaoc].BOBFOLNHBCL[1] = worldCorners[4].x + 2.5f;
                GameCollision.NNIOLGAKPGG[peifijckaoc].FDMAHBOILBC[1] = worldCorners[4].z + 2.5f;
                GameCollision.NNIOLGAKPGG[peifijckaoc].BOBFOLNHBCL[4] = worldCorners[7].x - 2.5f;
                GameCollision.NNIOLGAKPGG[peifijckaoc].FDMAHBOILBC[4] = worldCorners[7].z + 2.5f;
                GameCollision.NNIOLGAKPGG[peifijckaoc].BOBFOLNHBCL[3] = worldCorners[3].x - 2.5f;
                GameCollision.NNIOLGAKPGG[peifijckaoc].FDMAHBOILBC[3] = worldCorners[3].z - 2.5f;
                GameCollision.NNIOLGAKPGG[peifijckaoc].BOBFOLNHBCL[2] = worldCorners[0].x + 2.5f;
                GameCollision.NNIOLGAKPGG[peifijckaoc].FDMAHBOILBC[2] = worldCorners[0].z - 2.5f;
            }
            for (GameCollision.PEIFIJCKAOC = last + 1; GameCollision.PEIFIJCKAOC <= GameCollision.NJCPFHDPBOJ; GameCollision.PEIFIJCKAOC++)
            {
                if (GameCollision.NNIOLGAKPGG[GameCollision.PEIFIJCKAOC].HJGHHNAKAPD != null)
                {
                    GameCollision.NNIOLGAKPGG[GameCollision.PEIFIJCKAOC].HHEADFAGOMH = GameCollision.NNIOLGAKPGG[GameCollision.PEIFIJCKAOC].HJGHHNAKAPD.transform.localEulerAngles;
                    GameCollision.HGKKJGCBEPH = 1;
                }
            }
            var doors = World.gArena.GetComponentsInChildren<Transform>().Where(t => t.gameObject != null && t.gameObject.name.StartsWith("Exit")).Select(t => t.gameObject).ToArray();
            foreach (var door in doors)
            {
                var corners = new Vector3[4];
                var center = door.transform.position;
                var localScale = door.transform.localScale;
                var up = localScale.y;
                var right = localScale.x;
                var forward = localScale.z;
                corners[0] = center + new Vector3(right, 0, forward);
                corners[1] = center + new Vector3(right, 0, -forward);
                corners[2] = center + new Vector3(-right, 0, forward);
                corners[3] = center + new Vector3(-right, 0, -forward);

                var yTop = center.y + up;
                var yBottom = center.y - up;

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
                    bottomRight = zSorted[2];
                }
                else
                {
                    topRight = zSorted[2];
                    bottomRight = zSorted[3];
                }
                if (zSorted[1].x < zSorted[0].x)
                {
                    bottomLeft = zSorted[1];
                    topLeft = zSorted[0];
                }
                else
                {
                    bottomLeft = zSorted[0];
                    topLeft = zSorted[1];
                }

                // Create door
                GameCollision.AHKNDAPINCE();
                GameCollision.BAJJBMHMGBH[0] = GameCollision.BAJJBMHMGBH[GameCollision.BKAPMFNPOIB];
                GameCollision.BAJJBMHMGBH[0].BOBFOLNHBCL[1] = topRight.x;
                GameCollision.BAJJBMHMGBH[0].FDMAHBOILBC[1] = topRight.z;
                GameCollision.BAJJBMHMGBH[0].BOBFOLNHBCL[4] = bottomRight.x;
                GameCollision.BAJJBMHMGBH[0].FDMAHBOILBC[4] = bottomRight.z;
                GameCollision.BAJJBMHMGBH[0].BOBFOLNHBCL[3] = bottomLeft.x;
                GameCollision.BAJJBMHMGBH[0].FDMAHBOILBC[3] = bottomLeft.z;
                GameCollision.BAJJBMHMGBH[0].BOBFOLNHBCL[2] = topLeft.x;
                GameCollision.BAJJBMHMGBH[0].FDMAHBOILBC[2] = topLeft.z;
                GameCollision.BAJJBMHMGBH[0].JKHPFKDCNJL = yTop;
                GameCollision.BAJJBMHMGBH[0].PIOOGAJMCNL = door.transform.rotation.eulerAngles.y;
                GameCollision.BAJJBMHMGBH[0].OCELBBFKDMN = 1f;
                GameCollision.BAJJBMHMGBH[0].FLCIPPNLONE = EHIOFPLJLKH.BAJJBMHMGBH[1];
                GameCollision.BAJJBMHMGBH[0].PBPPKPEFLCG = int.Parse(door.name.Substring(4));
                GameCollision.BAJJBMHMGBH[0].KJOCNBPHILL = door.transform.rotation.eulerAngles.y + 180f;
            }

        }

        if (Plugin.DebugRender.Value)
        {
            var arr = GameCollision.NNIOLGAKPGG;
            for (int i = 1; i < arr.Length; i++)
            {
                try
                {
                    var scene = World.gArena;
                    var x4 = arr[i].BOBFOLNHBCL; // float[5], x4[0] is always 0
                    var z4 = arr[i].FDMAHBOILBC; // float[5], z4[0] is always 0
                    var yLow = arr[i].IHBEDOLDPJC; // float
                    var yHigh = arr[i].JKHPFKDCNJL; // float
                    var type = arr[i].FPHEOHBIAFN; // int
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

                    if (arr[i].JBALCHFODDL != null)
                    {
                        var xl = arr[i].JBALCHFODDL.Skip(1);
                        var zl = arr[i].HEFNOMCGPFP.Skip(1);
                        var color2 = new Color(1f, 1f, 1f);
                        DrawLines(scene.transform, xl.ToArray(), yLow, yHigh, zl.ToArray(), color2, "Seating");
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.LogWarning(e);
                }
            }

            var arr2 = GameCollision.IKGAAJIDDEJ;
            for (int i = 1; i < arr2.Length; i++)
            {
                try
                {
                    var scene = World.gArena;
                    var x4 = arr2[i].BOBFOLNHBCL; // float[5], x4[0] is always 0
                    var z4 = arr2[i].FDMAHBOILBC; // float[5], z4[0] is always 0
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

            var arr3 = GameCollision.BAJJBMHMGBH;
            for (int i = 1; i < arr3.Length; i++)
            {
                try
                {
                    var scene = World.gArena;
                    var x4 = arr3[i].BOBFOLNHBCL; // float[5], x4[0] is always 0
                    var z4 = arr3[i].FDMAHBOILBC; // float[5], z4[0] is always 0
                    var yLow = World.ground - 5f;
                    var yHigh = arr3[i].JKHPFKDCNJL; // float

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

    [HarmonyPatch(typeof(World), nameof(World.OGAHGABKJFO))]
    [HarmonyPrefix]
    public static bool World_OGAHGABKJFO(int JIBNLDBGFLN)
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            World.ground = 0f;
            World.ceiling = 100f;
            World.farNorth = 9999f;
            World.farSouth = -9999f;
            World.farEast = 9999f;
            World.farWest = -9999f;
            World.camNorth = 60f;
            World.camSouth = -60f;
            World.camEast = 60;
            World.camWest = -60f;
            return false;
        }

        return true;
    }


    [HarmonyPatch(typeof(World), nameof(World.KFIOOMEFCBG))]
    [HarmonyPrefix]
    public static bool World_KFIOOMEFCBG(ref int __result, int JIBNLDBGFLN)
    {
        __result = 1;
        if (World.mapVersion < 2f)
        {
            if ((JIBNLDBGFLN >= 17 && JIBNLDBGFLN <= VanillaCounts.NoLocations && JIBNLDBGFLN != 21) ||
                JIBNLDBGFLN - VanillaCounts.NoLocations - 1 >= CustomArenaPrefabs.Count)
            {
                __result = 0;
            }
        }

        return false;
    }

    [HarmonyPatch(typeof(World), nameof(World.LGKCNJMDODE))]
    [HarmonyPostfix]
    public static void World_LGKCNJMDODE(ref float __result, float CPKGAGIGBGC, float GMBDDOADNKI, float MPEGIJJDMNF)
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            if (World.ringShape != 0 && CPKGAGIGBGC > -40f && CPKGAGIGBGC < 40f && MPEGIJJDMNF > -40f && MPEGIJJDMNF < 40f)
            {
                return;
            }

            var coords = new Vector3(CPKGAGIGBGC, GMBDDOADNKI, MPEGIJJDMNF);
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