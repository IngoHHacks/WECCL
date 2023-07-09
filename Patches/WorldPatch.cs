using WECCL.Content;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
public class WorldPatch
{
    private static int _tempLocation = -999;

    private static readonly LimitedDictionary<string, float> _raycastCache = new(1000);

    [HarmonyPatch(typeof(World), nameof(World.ICKGKDOKJEN))]
    [HarmonyPrefix]
    public static bool World_ICKGKDOKJEN(int KDBIJLHICND)
    {
        try
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                Debug.Log("Loading location " + World.location);
                World.waterDefault = 0f;
                World.no_baskets = 0;
                if (KDBIJLHICND != 0 && World.gArena != null)
                {
                    Object.Destroy(World.gArena);
                }

                World.MKJPKDLKFBP();
                World.gArena = Object.Instantiate(CustomArenaPrefabs[World.location - VanillaCounts.NoLocations - 1]);

                if (GameScreens.OBNLIIMODBI == 60)
                {
                    World.gArena.transform.eulerAngles = new Vector3(0f, 170f, 0f);
                }

                if (Mathf.Abs(World.waterOffset) <= 1f)
                {
                    World.waterOffset = 0f;
                }

                if (GameGlobals.BFLIOCNAGDJ == 1)
                {
                    World.waterOffset = World.floodLevel;
                }
                else
                {
                    World.floodLevel = World.waterOffset;
                }

                World.waterLevel = World.waterDefault + World.waterOffset;
                World.MJFCDBJBCGG();
                if (GameScreens.OBNLIIMODBI == 60)
                {
                    return false;
                }

                World.PKHEPCDDIBM(World.location);
                if (GameScreens.OBNLIIMODBI != 14)
                {
                    GameAudio.AGCMPANIFOC();
                }

                if (KDBIJLHICND == 0)
                {
                    if (World.ringShape > 0)
                    {
                        World.IFPGBJHGPLF();
                    }

                    World.KCLJLOGNPGJ();
                }

                GameCollision.BAEIJIILOHL();
                return false;
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("Error loading location " + World.location + ": " + e);
        }

        return true;
    }

    [HarmonyPatch(typeof(GameCollision), nameof(GameCollision.BAEIJIILOHL))]
    [HarmonyPrefix]
    public static void GameCollision_BAEIJIILOHL()
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            GameCollision.BELIFAOEFAK = 0;
            GameCollision.DGPJFOABPND = 0;
            GameCollision.LMMGHHPEOKE = 0;
            GameCollision.DKPMHGHNHEH = new MNPJKGBMPFH[GameCollision.DGPJFOABPND + 1];
            GameCollision.DKPMHGHNHEH[0] = new MNPJKGBMPFH();
            GameCollision.IFFPPAPAEEK = 0;
            GameCollision.AOONPPMPJAD = 0;
            GameCollision.LJFPLPEHCPK = new DIAOCEFCCAE[GameCollision.AOONPPMPJAD + 1];
            GameCollision.LJFPLPEHCPK[0] = new DIAOCEFCCAE();
            GameCollision.FJJADKHAJJE = 0;
            GameCollision.MGJIOKKBCJM = new HHKHKMMOLNL[GameCollision.FJJADKHAJJE + 1];
            GameCollision.MGJIOKKBCJM[0] = new HHKHKMMOLNL();
            _tempLocation = World.location;
            World.location = 999;
            //Sets arenaShape to 0 here to stop spawning of default collisions while setting it to other shapes based on object in custom arena
            World.arenaShape = 0;
        }
    }

    [HarmonyPatch(typeof(GameCollision), nameof(GameCollision.BAEIJIILOHL))]
    [HarmonyPostfix]
    public static void GameCollision_BAEIJIILOHL_Postfix()
    {
        if (_tempLocation != -999)
        {
            ArenaPatch.SetCustomArenaShape();

            World.location = _tempLocation;
            _tempLocation = -999;

            int last = GameCollision.DGPJFOABPND;
            GameCollision.BELIFAOEFAK = last;

            MeshCollider[] colliders = World.gArena.GetComponentsInChildren<MeshCollider>();
            foreach (MeshCollider collider in colliders)
            {
                if (Math.Abs(collider.transform.rotation.eulerAngles.x - 90) > 1)
                {
                    continue;
                }

                Matrix4x4 matrix = collider.transform.localToWorldMatrix;
                Vector3[] corners = new Vector3[4];
                Vector3 up = collider.transform.up * 5;
                corners[0] = matrix.MultiplyPoint3x4(new Vector3(5, 0, 5));
                corners[1] = matrix.MultiplyPoint3x4(new Vector3(5, 0, -5));
                corners[2] = matrix.MultiplyPoint3x4(new Vector3(-5, 0, 5));
                corners[3] = matrix.MultiplyPoint3x4(new Vector3(-5, 0, -5));

                Array.Sort(corners, (a, b) =>
                {
                    return a.y.CompareTo(b.y);
                });
                float yBottom = corners[0].y;
                float yTop = corners[3].y;

                corners[0] -= up;
                corners[1] -= up;
                corners[2] = corners[1] + (up * 2);
                corners[3] = corners[0] + (up * 2);

                Vector3[] xSorted = new Vector3[4];
                Vector3[] zSorted = new Vector3[4];
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
                GameCollision.BELIFAOEFAK++;
                int BELIFAOEFAK = GameCollision.BELIFAOEFAK;
                GameCollision.LKMAEOFENHG();
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].EPGJJDJAACP = 0f;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].EDHBIOFAKNL = yBottom;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].HHMMCHPDDPF = yTop - yBottom;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].DIDCENDAHFF = 0;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].MIFAPPFHEPA[1] = topRight.x;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].NGLDIFNHFED[1] = topRight.z;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].MIFAPPFHEPA[4] = bottomRight.x;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].NGLDIFNHFED[4] = bottomRight.z;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].MIFAPPFHEPA[3] = bottomLeft.x;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].NGLDIFNHFED[3] = bottomLeft.z;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].MIFAPPFHEPA[2] = topLeft.x;
                GameCollision.DKPMHGHNHEH[BELIFAOEFAK].NGLDIFNHFED[2] = topLeft.z;
            }

            foreach (GameObject gameObject in (from t in World.gArena.GetComponentsInChildren<Transform>()
                         where t.gameObject != null && t.gameObject.name.StartsWith("Barrier_Climbables")
                         select t.gameObject).ToArray())
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

                GameCollision.BELIFAOEFAK++;
                int peifijckaoc = GameCollision.BELIFAOEFAK;
                GameCollision.LKMAEOFENHG();
                GameCollision.DKPMHGHNHEH[peifijckaoc].EPGJJDJAACP = 0f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].EDHBIOFAKNL = worldCorners[0].y;
                GameCollision.DKPMHGHNHEH[peifijckaoc].HHMMCHPDDPF = worldCorners[1].y - worldCorners[0].y;
                GameCollision.DKPMHGHNHEH[peifijckaoc].DIDCENDAHFF = 1;
                GameCollision.DKPMHGHNHEH[peifijckaoc].BGJDFONPLFK = "Barrier";
                GameCollision.DKPMHGHNHEH[peifijckaoc].MIFAPPFHEPA[1] = worldCorners[4].x + 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].NGLDIFNHFED[1] = worldCorners[4].z + 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].MIFAPPFHEPA[4] = worldCorners[7].x - 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].NGLDIFNHFED[4] = worldCorners[7].z + 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].MIFAPPFHEPA[3] = worldCorners[3].x - 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].NGLDIFNHFED[3] = worldCorners[3].z - 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].MIFAPPFHEPA[2] = worldCorners[0].x + 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].NGLDIFNHFED[2] = worldCorners[0].z - 2.5f;
            }

            foreach (GameObject gameObject in (from t in World.gArena.GetComponentsInChildren<Transform>()
                         where t.gameObject != null && t.gameObject.name.StartsWith("Fence_Climbables")
                         select t.gameObject).ToArray())
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

                GameCollision.BELIFAOEFAK++;
                int peifijckaoc = GameCollision.BELIFAOEFAK;
                GameCollision.LKMAEOFENHG();
                GameCollision.DKPMHGHNHEH[peifijckaoc].EPGJJDJAACP = 0f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].EDHBIOFAKNL = worldCorners[0].y;
                GameCollision.DKPMHGHNHEH[peifijckaoc].HHMMCHPDDPF = worldCorners[1].y - worldCorners[0].y;
                GameCollision.DKPMHGHNHEH[peifijckaoc].DIDCENDAHFF = 1;
                GameCollision.DKPMHGHNHEH[peifijckaoc].BGJDFONPLFK = "Cage";
                GameCollision.DKPMHGHNHEH[peifijckaoc].MIFAPPFHEPA[1] = worldCorners[4].x + 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].NGLDIFNHFED[1] = worldCorners[4].z + 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].MIFAPPFHEPA[4] = worldCorners[7].x - 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].NGLDIFNHFED[4] = worldCorners[7].z + 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].MIFAPPFHEPA[3] = worldCorners[3].x - 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].NGLDIFNHFED[3] = worldCorners[3].z - 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].MIFAPPFHEPA[2] = worldCorners[0].x + 2.5f;
                GameCollision.DKPMHGHNHEH[peifijckaoc].NGLDIFNHFED[2] = worldCorners[0].z - 2.5f;
            }

            for (GameCollision.BELIFAOEFAK = last + 1;
                 GameCollision.BELIFAOEFAK <= GameCollision.DGPJFOABPND;
                 GameCollision.BELIFAOEFAK++)
            {
                if (GameCollision.DKPMHGHNHEH[GameCollision.BELIFAOEFAK].HFNGNNHNFIC != null)
                {
                    GameCollision.DKPMHGHNHEH[GameCollision.BELIFAOEFAK].PMDKOLCNCJA = GameCollision
                        .DKPMHGHNHEH[GameCollision.BELIFAOEFAK].HFNGNNHNFIC.transform.localEulerAngles;
                    GameCollision.IFFPPAPAEEK = 1;
                }
            }

            GameObject[] doors = World.gArena.GetComponentsInChildren<Transform>()
                .Where(t => t.gameObject != null && t.gameObject.name.StartsWith("Exit")).Select(t => t.gameObject)
                .ToArray();
            foreach (GameObject door in doors)
            {
                Vector3[] corners = new Vector3[4];
                Vector3 center = door.transform.position;
                Vector3 localScale = door.transform.localScale;
                float up = localScale.y;
                float right = localScale.x;
                float forward = localScale.z;
                corners[0] = center + new Vector3(right, 0, forward);
                corners[1] = center + new Vector3(right, 0, -forward);
                corners[2] = center + new Vector3(-right, 0, forward);
                corners[3] = center + new Vector3(-right, 0, -forward);

                float yTop = center.y + up;
                float yBottom = center.y - up;

                Vector3[] xSorted = new Vector3[4];
                Vector3[] zSorted = new Vector3[4];

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
                GameCollision.OEPCPOBDAAM();
                GameCollision.LJFPLPEHCPK[0] = GameCollision.LJFPLPEHCPK[GameCollision.AOONPPMPJAD];
                GameCollision.LJFPLPEHCPK[0].MIFAPPFHEPA[1] = topRight.x;
                GameCollision.LJFPLPEHCPK[0].NGLDIFNHFED[1] = topRight.z;
                GameCollision.LJFPLPEHCPK[0].MIFAPPFHEPA[4] = bottomRight.x;
                GameCollision.LJFPLPEHCPK[0].NGLDIFNHFED[4] = bottomRight.z;
                GameCollision.LJFPLPEHCPK[0].MIFAPPFHEPA[3] = bottomLeft.x;
                GameCollision.LJFPLPEHCPK[0].NGLDIFNHFED[3] = bottomLeft.z;
                GameCollision.LJFPLPEHCPK[0].MIFAPPFHEPA[2] = topLeft.x;
                GameCollision.LJFPLPEHCPK[0].NGLDIFNHFED[2] = topLeft.z;
                GameCollision.LJFPLPEHCPK[0].HHMMCHPDDPF = yTop - yBottom;
                GameCollision.LJFPLPEHCPK[0].LHBBEOPJHHD = door.transform.rotation.eulerAngles.y;
                GameCollision.LJFPLPEHCPK[0].AFEEIHIFOIB = 1f;
                GameCollision.LJFPLPEHCPK[0].KDAHMGKJNGA = JKPIHABGBGP.LJFPLPEHCPK[1];
                GameCollision.LJFPLPEHCPK[0].NDHLPIPJBKD = int.Parse(door.name.Substring(4));
                GameCollision.LJFPLPEHCPK[0].MLMDNNNKJDI = door.transform.rotation.eulerAngles.y + 180f;
            }
        }

        if (Plugin.DebugRender.Value)
        {
            MNPJKGBMPFH[] arr = GameCollision.DKPMHGHNHEH;
            for (int i = 1; i < arr.Length; i++)
            {
                try
                {
                    GameObject scene = World.gArena;
                    float[] x4 = arr[i].MIFAPPFHEPA; // float[5], x4[0] is always 0
                    float[] z4 = arr[i].NGLDIFNHFED; // float[5], z4[0] is always 0
                    float yLow = arr[i].EDHBIOFAKNL; // float
                    float yHigh = arr[i].HHMMCHPDDPF; // float
                    int type = arr[i].DIDCENDAHFF; // int
                    Color color = Color.red;
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

                    if (arr[i].DLDGGLMCKMF != null)
                    {
                        IEnumerable<float> xl = arr[i].DLDGGLMCKMF.Skip(1);
                        IEnumerable<float> zl = arr[i].HLODEKCEPPM.Skip(1);
                        Color color2 = new Color(1f, 1f, 1f);
                        DrawLines(scene.transform, xl.ToArray(), yLow, yHigh, zl.ToArray(), color2, "Seating");
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.LogWarning(e);
                }
            }

            HHKHKMMOLNL[] arr2 = GameCollision.MGJIOKKBCJM;
            for (int i = 1; i < arr2.Length; i++)
            {
                try
                {
                    GameObject scene = World.gArena;
                    float[] x4 = arr2[i].MIFAPPFHEPA; // float[5], x4[0] is always 0
                    float[] z4 = arr2[i].NGLDIFNHFED; // float[5], z4[0] is always 0
                    int yLow = 0;
                    int yHigh = 0;
                    Color color = new Color(1f, 0.5f, 0f);
                    DrawCube(scene.transform, x4, yLow, yHigh, z4, color);
                }
                catch (Exception e)
                {
                    Plugin.Log.LogWarning(e);
                }
            }

            DIAOCEFCCAE[] arr3 = GameCollision.LJFPLPEHCPK;
            for (int i = 1; i < arr3.Length; i++)
            {
                try
                {
                    GameObject scene = World.gArena;
                    float[] x4 = arr3[i].MIFAPPFHEPA; // float[5], x4[0] is always 0
                    float[] z4 = arr3[i].NGLDIFNHFED; // float[5], z4[0] is always 0
                    float yLow = World.ground - 5f;
                    float yHigh = arr3[i].HHMMCHPDDPF; // float

                    Color color = new Color(0.5f, 1f, 0f);
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
        GameObject child = new GameObject("Bottom");
        child.transform.parent = parent;
        LineRenderer lineRenderer = child.AddComponent<LineRenderer>();
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
        int count = Math.Min(xl.Length, zl.Length);
        GameObject child = new GameObject(name);
        child.transform.parent = parent;
        LineRenderer lineRenderer = child.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = count;
        for (int i = 0; i < count; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(xl[i], yLow, zl[i]));
        }

        lineRenderer.sortingOrder = 999;
        lineRenderer.loop = true;
    }

    [HarmonyPatch(typeof(World), nameof(World.PKHEPCDDIBM))]
    [HarmonyPrefix]
    public static bool World_PKHEPCDDIBM(int PJNFPIAFBAM)
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


    [HarmonyPatch(typeof(World), nameof(World.GPIENIJBOBC))]
    [HarmonyPrefix]
    public static bool World_GPIENIJBOBC(ref int __result, int PJNFPIAFBAM)
    {
        __result = 1;
        if (World.mapVersion < 2f)
        {
            if ((PJNFPIAFBAM >= 17 && PJNFPIAFBAM <= VanillaCounts.NoLocations && PJNFPIAFBAM != 21) ||
                PJNFPIAFBAM - VanillaCounts.NoLocations - 1 >= CustomArenaPrefabs.Count)
            {
                __result = 0;
            }
        }

        return false;
    }

    [HarmonyPatch(typeof(World), nameof(World.FOOLMKOOCGH))]
    [HarmonyPostfix]
    public static void World_FOOLMKOOCGH(ref float __result, float DPDOBMIPMKE, float HONKBFJOEMG, float MALHMMKLEHG)
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            if (World.ringShape != 0 && DPDOBMIPMKE > -40f && DPDOBMIPMKE < 40f && MALHMMKLEHG > -40f &&
                MALHMMKLEHG < 40f)
            {
                return;
            }

            Vector3 coords = new Vector3(DPDOBMIPMKE, HONKBFJOEMG, MALHMMKLEHG).Round(2);
            string cstr = coords.ToString();
            if (_raycastCache.TryGetValue(cstr, out float cached))
            {
                __result = cached;
                return;
            }

            // Raycast down to find the ground
            Ray ray = new Ray(coords + new Vector3(0, 5f, 0f), Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 105f, 1 << 0))
            {
                __result = hit.point.y;
            }
            else
            {
                __result = World.ground;
            }

            if (!_raycastCache.ContainsKey(cstr))
            {
                _raycastCache.Add(cstr, __result);
            }
        }
    }
}