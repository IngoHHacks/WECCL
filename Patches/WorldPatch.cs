using WECCL.Content;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
public class WorldPatch
{
    private static int _tempLocation = -999;
    private static bool checkedArenaRotation;
    private static readonly LimitedDictionary<string, float> _raycastCache = new(1000);

    [HarmonyPatch(typeof(World), nameof(World.GBDGLHHCLCI))]
    [HarmonyPrefix]
    public static bool World_GBDGLHHCLCI(int PDILMINEGMA)
    {
        try
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                Debug.Log("Loading location " + World.location);
                World.waterDefault = 0f;
                World.no_baskets = 0;
                if (PDILMINEGMA != 0 && World.gArena != null)
                {
                    Object.Destroy(World.gArena);
                }

                MappedWorld.GetArenaShape();
                World.gArena = Object.Instantiate(CustomArenaPrefabs[World.location - VanillaCounts.NoLocations - 1]);

                if (MappedMenus.screen == 60)
                {
                    World.gArena.transform.eulerAngles = new Vector3(0f, 170f, 0f);
                }

                if (Mathf.Abs(World.waterOffset) <= 1f)
                {
                    World.waterOffset = 0f;
                }

                if (UnmappedGlobals.NHDABIOCLFH == 1)
                {
                    World.waterOffset = World.floodLevel;
                }
                else
                {
                    World.floodLevel = World.waterOffset;
                }

                MappedWorld.waterLevel = World.waterDefault + World.waterOffset;
                MappedWorld.MNFPDCKOJCC();
                if (UnmappedMenus.AAAIDOOHBCM == 60)
                {
                    return false;
                }

                World.LJMEMIODMEO(World.location);
                if (UnmappedMenus.AAAIDOOHBCM != 14)
                {
                    UnmappedSound.ABAEEOPALNG();
                }

                if (PDILMINEGMA == 0)
                {
                    if (World.ringShape > 0)
                    {
                        World.PAHHLMJINCM();
                    }

                    World.NCKKKPKJMCE();
                }

                UnmappedBlocks.AOKBJAAKFKD();
                return false;
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError("Error loading location " + World.location + ": " + e);
        }

        return true;
    }

    [HarmonyPatch(typeof(UnmappedBlocks), nameof(UnmappedBlocks.AOKBJAAKFKD))]
    [HarmonyPrefix]
    public static void GameCollision_AOKBJAAKFKD()
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            UnmappedBlocks.JLJKOBEBPAO = 0;
            UnmappedBlocks.MFLAEIPNEIA = 0;
            UnmappedBlocks.NKBPGCFONJL = 0;
            UnmappedBlocks.BBAOMBAGGBC = new PICGPEKCOHA[UnmappedBlocks.MFLAEIPNEIA + 1];
            UnmappedBlocks.BBAOMBAGGBC[0] = new PICGPEKCOHA();
            UnmappedBlocks.HBBOGBPIMCN = 0;
            UnmappedBlocks.IGLHKCAMDKO = 0;
            UnmappedBlocks.JNBNDGANKDE = new ILPOGGNCJEN[UnmappedBlocks.IGLHKCAMDKO + 1];
            UnmappedBlocks.JNBNDGANKDE[0] = new ILPOGGNCJEN();
            UnmappedBlocks.NIGMNOKBFDN = 0;
            UnmappedBlocks.KJNNFDCGCMC = new OALMLCHDNLI[UnmappedBlocks.NIGMNOKBFDN + 1];
            UnmappedBlocks.KJNNFDCGCMC[0] = new OALMLCHDNLI();
            _tempLocation = World.location;
            World.location = 999;
            //Sets arenaShape to 0 here to stop spawning of default collisions while setting it to other shapes based on object in custom arena
            World.arenaShape = 0;
        }
    }

    [HarmonyPatch(typeof(UnmappedBlocks), nameof(UnmappedBlocks.AOKBJAAKFKD))]
    [HarmonyPostfix]
    public static void GameCollision_AOKBJAAKFKD_Postfix()
    {
        if (_tempLocation != -999)
        {
            ArenaPatch.SetCustomArenaShape();

            World.location = _tempLocation;
            _tempLocation = -999;

            int last = UnmappedBlocks.MFLAEIPNEIA;
            UnmappedBlocks.JLJKOBEBPAO = last;

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
                UnmappedBlocks.JLJKOBEBPAO++;
                int JLJKOBEBPAO = UnmappedBlocks.JLJKOBEBPAO;
                UnmappedBlocks.MFDCLFKDDFB();
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].NBAJKICHHFK = 0f;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].BEHMHIINOGM = yBottom;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].PKAAALALAKD = yTop - yBottom;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].ACPDNMBGGLI = 0;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].LABFAOEKOBM[1] = topRight.x;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].HBFKOOEPFLH[1] = topRight.z;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].LABFAOEKOBM[4] = bottomRight.x;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].HBFKOOEPFLH[4] = bottomRight.z;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].LABFAOEKOBM[3] = bottomLeft.x;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].HBFKOOEPFLH[3] = bottomLeft.z;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].LABFAOEKOBM[2] = topLeft.x;
                UnmappedBlocks.BBAOMBAGGBC[JLJKOBEBPAO].HBFKOOEPFLH[2] = topLeft.z;
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

                UnmappedBlocks.JLJKOBEBPAO++;
                int peifijckaoc = UnmappedBlocks.JLJKOBEBPAO;
                UnmappedBlocks.MFDCLFKDDFB();
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].NBAJKICHHFK = 0f;
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].BEHMHIINOGM = worldCorners[0].y;
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].PKAAALALAKD = worldCorners[1].y - worldCorners[0].y;
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].ACPDNMBGGLI = 1;
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].JOKKOGKMHLN = "Barrier";
                GameObject arenaObject = GetTopLevelParent(gameObject);
                if (arenaObject.transform.rotation == Quaternion.Euler(0f, 180f, 0f))
                {
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[1] = worldCorners[4].x + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[1] = worldCorners[4].z + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[4] = worldCorners[7].x - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[4] = worldCorners[7].z + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[3] = worldCorners[3].x - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[3] = worldCorners[3].z - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[2] = worldCorners[0].x + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[2] = worldCorners[0].z - 2.5f;
                }
                else
                {
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[1] = worldCorners[3].x + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[1] = worldCorners[3].z + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[4] = worldCorners[0].x - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[4] = worldCorners[0].z + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[3] = worldCorners[4].x - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[3] = worldCorners[4].z - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[2] = worldCorners[7].x + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[2] = worldCorners[7].z - 2.5f;
                }
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
              
                UnmappedBlocks.JLJKOBEBPAO++;
                int peifijckaoc = UnmappedBlocks.JLJKOBEBPAO;
                UnmappedBlocks.MFDCLFKDDFB();
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].NBAJKICHHFK = 0f;
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].BEHMHIINOGM = worldCorners[0].y;
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].PKAAALALAKD = worldCorners[1].y - worldCorners[0].y;
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].ACPDNMBGGLI = 1;
                UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].JOKKOGKMHLN = "Cage";
                GameObject arenaObject = GetTopLevelParent(gameObject);
                if (arenaObject.transform.rotation == Quaternion.Euler(0f, 180f, 0f))
                {
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[1] = worldCorners[4].x + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[1] = worldCorners[4].z + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[4] = worldCorners[7].x - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[4] = worldCorners[7].z + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[3] = worldCorners[3].x - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[3] = worldCorners[3].z - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[2] = worldCorners[0].x + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[2] = worldCorners[0].z - 2.5f;
                }
                else
                {
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[1] = worldCorners[3].x + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[1] = worldCorners[3].z + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[4] = worldCorners[0].x - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[4] = worldCorners[0].z + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[3] = worldCorners[4].x - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[3] = worldCorners[4].z - 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].LABFAOEKOBM[2] = worldCorners[7].x + 2.5f;
                    UnmappedBlocks.BBAOMBAGGBC[peifijckaoc].HBFKOOEPFLH[2] = worldCorners[7].z - 2.5f;
                }
            }

            for (UnmappedBlocks.JLJKOBEBPAO = last + 1;
                 UnmappedBlocks.JLJKOBEBPAO <= UnmappedBlocks.MFLAEIPNEIA;
                 UnmappedBlocks.JLJKOBEBPAO++)
            {
                if (UnmappedBlocks.BBAOMBAGGBC[UnmappedBlocks.JLJKOBEBPAO].IDMHPBFAHDN != null)
                {
                    UnmappedBlocks.BBAOMBAGGBC[UnmappedBlocks.JLJKOBEBPAO].BKADHEGHCEA = UnmappedBlocks
                        .BBAOMBAGGBC[UnmappedBlocks.JLJKOBEBPAO].IDMHPBFAHDN.transform.localEulerAngles;
                    UnmappedBlocks.HBBOGBPIMCN = 1;
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
                UnmappedBlocks.ALMEAGODEBL();
                UnmappedBlocks.JNBNDGANKDE[0] = UnmappedBlocks.JNBNDGANKDE[UnmappedBlocks.IGLHKCAMDKO];
                UnmappedBlocks.JNBNDGANKDE[0].LABFAOEKOBM[1] = topRight.x;
                UnmappedBlocks.JNBNDGANKDE[0].HBFKOOEPFLH[1] = topRight.z;
                UnmappedBlocks.JNBNDGANKDE[0].LABFAOEKOBM[4] = bottomRight.x;
                UnmappedBlocks.JNBNDGANKDE[0].HBFKOOEPFLH[4] = bottomRight.z;
                UnmappedBlocks.JNBNDGANKDE[0].LABFAOEKOBM[3] = bottomLeft.x;
                UnmappedBlocks.JNBNDGANKDE[0].HBFKOOEPFLH[3] = bottomLeft.z;
                UnmappedBlocks.JNBNDGANKDE[0].LABFAOEKOBM[2] = topLeft.x;
                UnmappedBlocks.JNBNDGANKDE[0].HBFKOOEPFLH[2] = topLeft.z;
                UnmappedBlocks.JNBNDGANKDE[0].PKAAALALAKD = yTop - yBottom;
                UnmappedBlocks.JNBNDGANKDE[0].NAMDOACBNED = door.transform.rotation.eulerAngles.y;
                UnmappedBlocks.JNBNDGANKDE[0].CEJAAAKBJKD = 1f;
                UnmappedBlocks.JNBNDGANKDE[0].NHDHKCNOCBB = UnmappedSound.JNBNDGANKDE[1];
                UnmappedBlocks.JNBNDGANKDE[0].HLGEDBPIMPK = int.Parse(door.name.Substring(4));
                UnmappedBlocks.JNBNDGANKDE[0].JPENFFBCAMN = door.transform.rotation.eulerAngles.y + 180f;
            }
        }

        if (Plugin.DebugRender.Value)
        {
            PICGPEKCOHA[] arr = UnmappedBlocks.BBAOMBAGGBC;
            for (int i = 1; i < arr.Length; i++)
            {
                try
                {
                    GameObject scene = World.gArena;
                    float[] x4 = arr[i].LABFAOEKOBM; // float[5], x4[0] is always 0
                    float[] z4 = arr[i].HBFKOOEPFLH; // float[5], z4[0] is always 0
                    float yLow = arr[i].BEHMHIINOGM; // float
                    float yHigh = arr[i].PKAAALALAKD; // float
                    int type = arr[i].ACPDNMBGGLI; // int
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

                    if (arr[i].LBPNEBAJAND != null)
                    {
                        IEnumerable<float> xl = arr[i].LBPNEBAJAND.Skip(1);
                        IEnumerable<float> zl = arr[i].MGEMCDDDMPM.Skip(1);
                        Color color2 = new Color(1f, 1f, 1f);
                        DrawLines(scene.transform, xl.ToArray(), yLow, yHigh, zl.ToArray(), color2, "Seating");
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.LogWarning(e);
                }
            }

            OALMLCHDNLI[] arr2 = UnmappedBlocks.KJNNFDCGCMC;
            for (int i = 1; i < arr2.Length; i++)
            {
                try
                {
                    GameObject scene = World.gArena;
                    float[] x4 = arr2[i].LABFAOEKOBM; // float[5], x4[0] is always 0
                    float[] z4 = arr2[i].HBFKOOEPFLH; // float[5], z4[0] is always 0
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

            ILPOGGNCJEN[] arr3 = UnmappedBlocks.JNBNDGANKDE;
            for (int i = 1; i < arr3.Length; i++)
            {
                try
                {
                    GameObject scene = World.gArena;
                    float[] x4 = arr3[i].LABFAOEKOBM; // float[5], x4[0] is always 0
                    float[] z4 = arr3[i].HBFKOOEPFLH; // float[5], z4[0] is always 0
                    float yLow = World.ground - 5f;
                    float yHigh = arr3[i].PKAAALALAKD; // float

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

    public static GameObject GetTopLevelParent(GameObject childObject)
    {
        Transform currentTransform = childObject.transform;

        // Traverse up the hierarchy until there's no parent
        while (currentTransform.parent != null)
        {
            currentTransform = currentTransform.parent;
        }

        // Return the top-level parent GameObject
        return currentTransform.gameObject;
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

    [HarmonyPatch(typeof(World), nameof(World.LJMEMIODMEO))]
    [HarmonyPrefix]
    public static bool World_LJMEMIODMEO(int KNEDMBFJJAA)
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


    [HarmonyPatch(typeof(World), nameof(World.MAPEACFCDGA))]
    [HarmonyPrefix]
    public static bool World_MAPEACFCDGA(ref int __result, int KNEDMBFJJAA)
    {
        __result = 1;
        if (World.mapVersion < 2f)
        {
            if ((KNEDMBFJJAA >= 17 && KNEDMBFJJAA <= VanillaCounts.NoLocations && KNEDMBFJJAA != 21) ||
                KNEDMBFJJAA - VanillaCounts.NoLocations - 1 >= CustomArenaPrefabs.Count)
            {
                __result = 0;
            }
        }

        return false;
    }

    [HarmonyPatch(typeof(World), nameof(World.JMFAKOLINLF))]
    [HarmonyPostfix]
    public static void World_JMFAKOLINLF(ref float __result, float DKOBDIJJOGO, float IBDKLAELPND, float LDEAEOHOCPO)
    {
        if (World.location > VanillaCounts.NoLocations)
        {
            if (World.ringShape != 0 && DKOBDIJJOGO > -40f && DKOBDIJJOGO < 40f && LDEAEOHOCPO > -40f &&
                LDEAEOHOCPO < 40f)
            {
                return;
            }

            Vector3 coords = new Vector3(DKOBDIJJOGO, IBDKLAELPND, LDEAEOHOCPO).Round(2);
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