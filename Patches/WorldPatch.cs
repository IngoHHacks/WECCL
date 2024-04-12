using TMPro;
using WECCL.Content;
using Object = UnityEngine.Object;

namespace WECCL.Patches;

[HarmonyPatch]
public class WorldPatch
{
    private static int _tempLocation = -999;
    private static readonly LimitedDictionary<string, float> RaycastCache = new(1000);
    
    /*
     * Patch:
     * - Loads custom arenas if the player is in a custom location.
     */
    [HarmonyPatch(typeof(World), nameof(World.ICGNAJFLAHL))]
    [HarmonyPrefix]
    public static bool World_ICGNAJFLAHL(int EJDHFNIJFHI)
    {
        try
        {
            if (World.location > VanillaCounts.Data.NoLocations)
            {
                Debug.Log("Loading location " + World.location);
                World.waterDefault = 0f;
                World.no_baskets = 0;
                if (EJDHFNIJFHI != 0 && World.gArena != null)
                {
                    Object.Destroy(World.gArena);
                }

                MappedWorld.GetArenaShape();
                if (World.location >= 999999)
                {
                    World.gArena = Object.Instantiate(HubLocationPrefab);
                }
                else
                {
                    World.gArena =
                        Object.Instantiate(CustomArenaPrefabs[World.location - VanillaCounts.Data.NoLocations - 1]);
                }

                if (MappedMenus.screen == 60)
                {
                    World.gArena.transform.eulerAngles = new Vector3(0f, 170f, 0f);
                }

                if (Mathf.Abs(World.waterOffset) <= 1f)
                {
                    World.waterOffset = 0f;
                }

                if (UnmappedGlobals.CBMHGKFFHJE == 1)
                {
                    World.waterOffset = World.floodLevel;
                }
                else
                {
                    World.floodLevel = World.waterOffset;
                }

                MappedWorld.waterLevel = World.waterDefault + World.waterOffset;
                MappedWorld.LoadWater();
                if (UnmappedMenus.FAKHAFKOBPB == 60)
                {
                    return false;
                }

                World.JOLFKJKNBLP(World.location);
                if (UnmappedMenus.FAKHAFKOBPB != 14)
                {
                    UnmappedSound.LCKFJMHBGAH();
                }

                if (EJDHFNIJFHI == 0)
                {
                    if (World.ringShape > 0)
                    {
                        World.DBKOAJKLBIF();
                    }

                    World.DOGCAIADJOP();
                }

                UnmappedBlocks.NALPMNNGKAE();
                return false;
            }
        }
        catch (Exception e)
        {
            LogError("Error loading location " + World.location + ": " + e);
        }

        return true;
    }

    /*
     * Patch:
     * - Clears the 'blocks' (collision boxes) when loading a custom arena.
     */
    [HarmonyPatch(typeof(UnmappedBlocks), nameof(UnmappedBlocks.NALPMNNGKAE))]
    [HarmonyPrefix]
    public static void Blocks_NALPMNNGKAE_Pre()
    {
        if (World.location > VanillaCounts.Data.NoLocations)
        {
            UnmappedBlocks.FACCLLDILBH = 0;
            UnmappedBlocks.CEIEEMCIOMD = 0;
            UnmappedBlocks.PGEMKGMPGON = 0;
            UnmappedBlocks.LCJFMEAFLBH = new KBNFBEFCJGO[UnmappedBlocks.CEIEEMCIOMD + 1];
            UnmappedBlocks.LCJFMEAFLBH[0] = new KBNFBEFCJGO();
            UnmappedBlocks.MKAIGHEJEBJ = 0;
            UnmappedBlocks.BAOOLJCLBIH = 0;
            UnmappedBlocks.FBEMAEDLBLN = new OGAJMOPCPLJ[UnmappedBlocks.BAOOLJCLBIH + 1];
            UnmappedBlocks.FBEMAEDLBLN[0] = new OGAJMOPCPLJ();
            UnmappedBlocks.DKCGNNPEHAI = 0;
            UnmappedBlocks.NKHOABLELKA = new JBGEBIDPBOK[UnmappedBlocks.DKCGNNPEHAI + 1];
            UnmappedBlocks.NKHOABLELKA[0] = new JBGEBIDPBOK();
            _tempLocation = World.location;
            World.location = 999;
            //Sets arenaShape to 0 here to stop spawning of default collisions while setting it to other shapes based on object in custom arena
            World.arenaShape = 0;
        }
    }

    /*
     * Patch:
     * - Creates the 'blocks' (collision boxes) when loading a custom arena.
     * - Renders the debug collision boxes if the user has enabled them in the config.
     * - Replaces two Airport doors to lead to the 'hub' location.
     */
    [HarmonyPatch(typeof(UnmappedBlocks), nameof(UnmappedBlocks.NALPMNNGKAE))]
    [HarmonyPostfix]
    public static void Blocks_NALPMNNGKAE_Post()
    {
        if (_tempLocation != -999)
        {
            ArenaPatch.SetCustomArenaShape();

            World.location = _tempLocation;
            _tempLocation = -999;

            int last = UnmappedBlocks.CEIEEMCIOMD;
            UnmappedBlocks.FACCLLDILBH = last;

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
                UnmappedBlocks.FACCLLDILBH++;
                int FACCLLDILBH = UnmappedBlocks.FACCLLDILBH;
                UnmappedBlocks.DFLLBNMHHIH();
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].AMJOGFHEBKI = 0f;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].FNNBCDPJBIO = yBottom;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].NALEIJHPOHN = yTop - yBottom;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].GMFOALGKLJK = 0;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].EONCNOGEOFC[1] = topRight.x;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].MKOCPPCIKEM[1] = topRight.z;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].EONCNOGEOFC[4] = bottomRight.x;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].MKOCPPCIKEM[4] = bottomRight.z;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].EONCNOGEOFC[3] = bottomLeft.x;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].MKOCPPCIKEM[3] = bottomLeft.z;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].EONCNOGEOFC[2] = topLeft.x;
                UnmappedBlocks.LCJFMEAFLBH[FACCLLDILBH].MKOCPPCIKEM[2] = topLeft.z;
            }

            foreach (GameObject gameObject in (from t in World.gArena.GetComponentsInChildren<Transform>()
                         where t.gameObject != null && t.gameObject.name.StartsWith("Barrier_Climbables")
                         select t.gameObject).ToArray())
            {
                MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();

                if (meshCollider != null)
                {
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

                    UnmappedBlocks.FACCLLDILBH++;
                    int peifijckaoc = UnmappedBlocks.FACCLLDILBH;
                    UnmappedBlocks.DFLLBNMHHIH();
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].AMJOGFHEBKI = 0f;
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].FNNBCDPJBIO = worldCorners[0].y;
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].NALEIJHPOHN = worldCorners[1].y - worldCorners[0].y;
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].GMFOALGKLJK = 1;
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].HHFIFJPHINJ = "Barrier";
                    GameObject arenaObject = GetTopLevelParent(gameObject);
                    if (arenaObject.transform.rotation == Quaternion.Euler(0f, 180f, 0f))
                    {
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[1] = worldCorners[4].x + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[1] = worldCorners[4].z + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[4] = worldCorners[7].x - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[4] = worldCorners[7].z + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[3] = worldCorners[3].x - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[3] = worldCorners[3].z - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[2] = worldCorners[0].x + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[2] = worldCorners[0].z - 2.5f;
                    }
                    else
                    {
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[1] = worldCorners[3].x + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[1] = worldCorners[3].z + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[4] = worldCorners[0].x - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[4] = worldCorners[0].z + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[3] = worldCorners[4].x - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[3] = worldCorners[4].z - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[2] = worldCorners[7].x + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[2] = worldCorners[7].z - 2.5f;
                    }
                }
                else
                {
                    string warning = "Barrier_Climbables with name '" + gameObject.name + "' is missing a meshCollider and won't work as expected.";
                    LogWarning(warning);
                }
            }

            foreach (GameObject gameObject in (from t in World.gArena.GetComponentsInChildren<Transform>()
                         where t.gameObject != null && t.gameObject.name.StartsWith("Fence_Climbables")
                         select t.gameObject).ToArray())
            {
                MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
                if (meshCollider != null)
                {
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

                    UnmappedBlocks.FACCLLDILBH++;
                    int peifijckaoc = UnmappedBlocks.FACCLLDILBH;
                    UnmappedBlocks.DFLLBNMHHIH();
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].AMJOGFHEBKI = 0f;
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].FNNBCDPJBIO = worldCorners[0].y;
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].NALEIJHPOHN = worldCorners[1].y - worldCorners[0].y;
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].GMFOALGKLJK = 1;
                    UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].HHFIFJPHINJ = "Cage";
                    GameObject arenaObject = GetTopLevelParent(gameObject);
                    if (arenaObject.transform.rotation == Quaternion.Euler(0f, 180f, 0f))
                    {
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[1] = worldCorners[4].x + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[1] = worldCorners[4].z + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[4] = worldCorners[7].x - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[4] = worldCorners[7].z + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[3] = worldCorners[3].x - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[3] = worldCorners[3].z - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[2] = worldCorners[0].x + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[2] = worldCorners[0].z - 2.5f;
                    }
                    else
                    {
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[1] = worldCorners[3].x + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[1] = worldCorners[3].z + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[4] = worldCorners[0].x - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[4] = worldCorners[0].z + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[3] = worldCorners[4].x - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[3] = worldCorners[4].z - 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].EONCNOGEOFC[2] = worldCorners[7].x + 2.5f;
                        UnmappedBlocks.LCJFMEAFLBH[peifijckaoc].MKOCPPCIKEM[2] = worldCorners[7].z - 2.5f;
                    }
                }
                else
                {
                    string warning = "Fence_Climbables with name '" + gameObject.name + "' is missing a meshCollider and won't work as expected.";
                    LogWarning(warning);
                }
            }

            for (UnmappedBlocks.FACCLLDILBH = last + 1;
                 UnmappedBlocks.FACCLLDILBH <= UnmappedBlocks.CEIEEMCIOMD;
                 UnmappedBlocks.FACCLLDILBH++)
            {
                if (UnmappedBlocks.LCJFMEAFLBH[UnmappedBlocks.FACCLLDILBH].GMBNIJFILBA != null)
                {
                    UnmappedBlocks.LCJFMEAFLBH[UnmappedBlocks.FACCLLDILBH].AECHPPEKCKO = UnmappedBlocks
                        .LCJFMEAFLBH[UnmappedBlocks.FACCLLDILBH].GMBNIJFILBA.transform.localEulerAngles;
                    UnmappedBlocks.MKAIGHEJEBJ = 1;
                }
            }

            GameObject[] doors = World.gArena.GetComponentsInChildren<Transform>()
                .Where(t => t.gameObject != null && t.gameObject.name.StartsWith("Exit")).Select(t => t.gameObject)
                .ToArray();
            foreach (GameObject door in doors)
            {
                CreateDoor(door);
            }

            if (World.location >= 999999)
            {
                // Hub world stuff
                var prevDoor = World.gArena
                    .GetComponentsInChildren<Transform>().FirstOrDefault(t => t.gameObject != null && t.gameObject.name.Equals("Prev")).gameObject;
                var prev = CreateDoor(prevDoor);
                if (World.location == 999999)
                {
                    prev.destination = 15;
                    prev.destinationX = -15;
                    prev.destinationZ = -85;
                }
                else
                {
                    prev.destination = World.location - 1;
                    prev.destinationX = 0;
                    prev.destinationZ = -100;
                }
                var nextDoor = World.gArena
                    .GetComponentsInChildren<Transform>().FirstOrDefault(t => t.gameObject != null && t.gameObject.name.Equals("Next")).gameObject;
                var next = CreateDoor(nextDoor);
                if (World.location - 999999 >= (CustomArenaPrefabs.Count - 1) / 10)
                {
                    next.destination = 15;
                    next.destinationX = 15;
                    next.destinationZ = -85;
                }
                else
                {
                    next.destination = World.location + 1;
                    next.destinationX = 0;
                    next.destinationZ = 100;
                }
                var locDoors = World.gArena
                    .GetComponentsInChildren<Transform>().Where(t => t.gameObject != null && t.gameObject.name.StartsWith("Loc") && !t.gameObject.name.StartsWith("LocName")).Select(t => t.gameObject).ToArray();
                
                var offset = (World.location - 999999) * 10;
                foreach (GameObject locDoor in locDoors)
                {
                    var loc = CreateDoor(locDoor);
                    if (offset + int.Parse(locDoor.name.Substring(3)) >= CustomArenaPrefabs.Count)
                    {
                        var locName = World.gArena
                            .GetComponentsInChildren<Transform>().FirstOrDefault(t => t.gameObject != null && t.gameObject.name.Equals("LocName" + locDoor.name.Substring(3))).gameObject;
                        locName.SetActive(false);
                        loc.destination = 15;
                        loc.destinationX = 15;
                        loc.destinationZ = -85;
                    }
                    else
                    {
                        var locName = World.gArena
                            .GetComponentsInChildren<Transform>().FirstOrDefault(t => t.gameObject != null && t.gameObject.name.Equals("LocName" + locDoor.name.Substring(3))).gameObject;
                        locName.SetActive(true);
                        locName.GetComponent<TextMeshPro>().text = CustomArenaPrefabs[offset + int.Parse(locDoor.name.Substring(3)) - 1].name;
                        loc.destination = VanillaCounts.Data.NoLocations + offset +
                            int.Parse(locDoor.name.Substring(3));
                        loc.destinationX = 0;
                        loc.destinationZ = 0;
                        var firstExit = CustomArenaPrefabs[offset + int.Parse(locDoor.name.Substring(3)) - 1].GetComponentsInChildren<Transform>().FirstOrDefault(t => t.gameObject != null && t.gameObject.name.StartsWith("Exit"))?.gameObject;
                        if (firstExit != null)
                        {
                            var pos = firstExit.transform.position + (firstExit.transform.forward *
                                                                      Mathf.Max(20, Mathf.Max(firstExit.transform.localScale.x,
                                                                          firstExit.transform.localScale.z)));
                            loc.destinationX = pos.x;
                            loc.destinationZ = pos.z;
                        }
                    }
                }
            }
        }

        if (Plugin.DebugRender.Value)
        {
            KBNFBEFCJGO[] arr = UnmappedBlocks.LCJFMEAFLBH;
            for (int i = 1; i < arr.Length; i++)
            {
                try
                {
                    GameObject scene = World.gArena;
                    float[] x4 = arr[i].EONCNOGEOFC; // float[5], x4[0] is always 0
                    float[] z4 = arr[i].MKOCPPCIKEM; // float[5], z4[0] is always 0
                    float yLow = arr[i].FNNBCDPJBIO; // float
                    float yHigh = arr[i].NALEIJHPOHN; // float
                    int type = arr[i].GMFOALGKLJK; // int
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

                    if (arr[i].BPDFBLFOPNL != null)
                    {
                        IEnumerable<float> xl = arr[i].BPDFBLFOPNL.Skip(1);
                        IEnumerable<float> zl = arr[i].NEHKMCJAHCG.Skip(1);
                        Color color2 = new Color(1f, 1f, 1f);
                        DrawLines(scene.transform, xl.ToArray(), yLow, yHigh, zl.ToArray(), color2, "Seating");
                    }
                }
                catch (Exception e)
                {
                    LogWarning(e);
                }
            }

            JBGEBIDPBOK[] arr2 = UnmappedBlocks.NKHOABLELKA;
            for (int i = 1; i < arr2.Length; i++)
            {
                try
                {
                    GameObject scene = World.gArena;
                    float[] x4 = arr2[i].EONCNOGEOFC; // float[5], x4[0] is always 0
                    float[] z4 = arr2[i].MKOCPPCIKEM; // float[5], z4[0] is always 0
                    int yLow = 0;
                    int yHigh = 0;
                    Color color = new Color(1f, 0.5f, 0f);
                    DrawCube(scene.transform, x4, yLow, yHigh, z4, color);
                }
                catch (Exception e)
                {
                    LogWarning(e);
                }
            }

            OGAJMOPCPLJ[] arr3 = UnmappedBlocks.FBEMAEDLBLN;
            for (int i = 1; i < arr3.Length; i++)
            {
                try
                {
                    GameObject scene = World.gArena;
                    float[] x4 = arr3[i].EONCNOGEOFC; // float[5], x4[0] is always 0
                    float[] z4 = arr3[i].MKOCPPCIKEM; // float[5], z4[0] is always 0
                    float yLow = World.ground - 5f;
                    float yHigh = arr3[i].NALEIJHPOHN; // float

                    Color color = new Color(0.5f, 1f, 0f);
                    DrawCube(scene.transform, x4, yLow, yHigh, z4, color);
                }
                catch (Exception e)
                {
                    LogWarning(e);
                }
            }
        }

        if (World.location == 15)
        {
            ((MappedDoor)MappedBlocks.door[1]).destination = 999999;
            ((MappedDoor)MappedBlocks.door[1]).destinationX = 0;
            ((MappedDoor)MappedBlocks.door[1]).destinationZ = 100;
            ((MappedDoor)MappedBlocks.door[2]).destination = 999999;
            ((MappedDoor)MappedBlocks.door[2]).destinationX = 0;
            ((MappedDoor)MappedBlocks.door[2]).destinationZ = 100;
        }
    }

    private static MappedDoor CreateDoor(GameObject door)
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
        
        Vector3 topRight = corners[0];
        Vector3 bottomRight = corners[0];
        Vector3 bottomLeft = corners[0];
        Vector3 topLeft = corners[0];
        
        float yTop = center.y + up;
        float yBottom = center.y;
        
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

        return PlaceDoor(topRight, bottomRight, bottomLeft, topLeft, yTop, yBottom, door);
    }
    
    private static MappedDoor PlaceDoor(Vector3 topRight, Vector3 bottomRight, Vector3 bottomLeft, Vector3 topLeft,
        float yTop, float yBottom, GameObject door)
    {
        MappedBlocks.AddDoor();
        MappedBlocks.door[0] = MappedBlocks.door[MappedBlocks.no_doors];
        var i = ((MappedDoor)MappedBlocks.door[0]);
        i.pointX[1] = topRight.x;
        i.pointZ[1] = topRight.z;
        i.pointX[4] = bottomRight.x;
        i.pointZ[4] = bottomRight.z;
        i.pointX[3] = bottomLeft.x;
        i.pointZ[3] = bottomLeft.z;
        i.pointX[2] = topLeft.x;
        i.pointZ[2] = topLeft.z;
        i.height = yTop - yBottom;
        i.angle = door.transform.rotation.eulerAngles.y;
        i.friction = 0f;
        i.sound = MappedSound.door[1];
        i.destination = GetNumSuffix(door.name);
        i.destinationX = 0;
        i.destinationZ = 0;
        i.destinationAngle = door.transform.rotation.eulerAngles.y + 180f;
        
        if (i.height < 10)
        {
            i.height = 10;
        }
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        for (int j = 1; j <= 4; j++)
        {
            minX = Mathf.Min(minX, i.pointX[j]);
            maxX = Mathf.Max(maxX, i.pointX[j]);
            minZ = Mathf.Min(minZ, i.pointZ[j]);
            maxZ = Mathf.Max(maxZ, i.pointZ[j]);
        }
        float midX = (minX + maxX) / 2f;
        float midZ = (minZ + maxZ) / 2f;
        if (maxX - minX < 20)
        {
            for (int k = 1; k <= 4; k++)
            {
                if (i.pointX[k] < midX)
                {
                    i.pointX[k] = midX - 10f;
                }
                else
                {
                    i.pointX[k] = midX + 10f;
                }
            }
        }
        if (maxZ - minZ < 20)
        {
            for (int l = 1; l <= 4; l++)
            {
                if (i.pointZ[l] < midZ)
                {
                    i.pointZ[l] = midZ - 10f;
                }
                else
                {
                    i.pointZ[l] = midZ + 10f;
                }
            }
        }
        return MappedBlocks.door[0];
    }
    
    private static int GetNumSuffix(string name)
    {
        if (!char.IsDigit(name[name.Length - 1]))
        {
            return 2;
        }
        var i = name.Length - 1;
        while (char.IsDigit(name[i]))
        {
            i--;
        }
        return int.Parse(name.Substring(i + 1));
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

    /*
     * Patch:
     * - Disables default boundaries for custom arenas
     */
    [HarmonyPatch(typeof(World), nameof(World.JOLFKJKNBLP))]
    [HarmonyPrefix]
    public static bool World_JOLFKJKNBLP(int HJANGKEJCJE)
    {
        if (World.location > VanillaCounts.Data.NoLocations)
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

    /*
     * Patch:
     * - Sets custom arenas as 'available' for the game to load.
     */
    [HarmonyPatch(typeof(World), nameof(World.COMEDPJDBKM))]
    [HarmonyPrefix]
    public static bool World_COMEDPJDBKM(ref int __result, int HJANGKEJCJE)
    {
        __result = 1;
        if (HJANGKEJCJE >= 999999)
        {
            return false;
        }
        if (World.mapVersion < 2f)
        {
            if ((HJANGKEJCJE >= 17 && HJANGKEJCJE <= VanillaCounts.Data.NoLocations && HJANGKEJCJE != 21 && HJANGKEJCJE != 27) ||
                HJANGKEJCJE - VanillaCounts.Data.NoLocations - 1 >= CustomArenaPrefabs.Count)
            {
                __result = 0;
            }
        }

        return false;
    }

    /*
     * Patch:
     * - Determines the floor height for custom arenas.
     */
    [HarmonyPatch(typeof(World), nameof(World.KJOEBADBOME))]
    [HarmonyPostfix]
    public static void World_KJOEBADBOME(ref float __result, float MMBJPONJJGM, float EJOKLBHLEEJ, float FNFJENPGCHM)
    {
        if (World.location > VanillaCounts.Data.NoLocations)
        {
            if (World.ringShape != 0 && MMBJPONJJGM > -40f && MMBJPONJJGM < 40f && FNFJENPGCHM > -40f &&
                FNFJENPGCHM < 40f)
            {
                return;
            }

            Vector3 coords = new Vector3(MMBJPONJJGM, EJOKLBHLEEJ, FNFJENPGCHM).Round(2);
            string cstr = coords.ToString();
            if (RaycastCache.TryGetValue(cstr, out float cached))
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

            if (!RaycastCache.ContainsKey(cstr))
            {
                RaycastCache.Add(cstr, __result);
            }
        }
    }
}