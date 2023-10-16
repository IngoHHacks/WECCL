using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using WECCL.Content;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace WECCL.Patches;

[HarmonyPatch]
public class ArenaPatch
{
    public static Dictionary<string, int> objectMappings;
    public static Dictionary<string, int> weaponMappings;
    public static List<string> weaponList;
    public static float yOverride;
    public static int NoOriginalLocationsValue;
    public static bool freezeAnnouncers;

    private void Awake()
    {
        CreateObjectMapping();
        CreateWeaponMapping();
    }

    private static void CreateObjectMapping()
    {
        objectMappings = new Dictionary<string, int>
        {
            { "Table", 1 },
            { "Office Chair", 2 },
            { "Announce Desk", 3 },
            { "Steps", 4 },
            { "Desk", 5 },
            { "Bench", 6 },
            { "Ladder", 7 },
            { "Wooden Crate", 8 },
            { "Cardboard Box", 9 },
            { "Trashcan", 10 },
            { "Toilet", 11 },
            { "Bed", 12 },
            { "Snooker Table", 13 },
            { "Stool", 14 },
            { "Round Table", 15 },
            { "Barrel", 16 },
            { "Coffin", 17 },
            { "Wheelchair", 18 },
            { "Folding Chair", 19 },
            { "Motorcycle", 20 },
            { "Bicycle", 21 },
            { "Car", 22 },
            { "Van", 23 },
            { "Vending Machine", 24 },
            { "Computer Desk", 25 },
            { "Piano", 26 }
            // Note, left Motorcycle, Bicycle, Car and Van here even though from testing they do not work.
            // If they ever have the models added they would become valid instantly but not included in documentation as to avoid confusion
        };
    }

    private static void CreateWeaponMapping()
    {
        weaponMappings = new Dictionary<string, int>
        {
            { "Belt", 0 },
            { "Microphone", 1 },
            { "Camera", 2 },
            { "Bell", 3 },
            { "Explosive", 4 },
            { "Baseball Bat", 5 },
            { "Chair", 6 },
            { "Cage Piece", 7 },
            { "Wooden Board", 8 },
            { "Table Piece", 9 },
            { "Table Leg", 10 },
            { "Barbed Bat", 11 },
            { "Cardboard", 12 },
            { "Ladder Piece", 13 },
            { "Plank", 14 },
            { "Pipe", 15 },
            { "Nightstick", 16 },
            { "Cane", 17 },
            { "Step", 18 },
            { "Dumbbell", 19 },
            { "Weight", 20 },
            { "Trashcan Lid", 21 },
            { "Skateboard", 22 },
            { "Water Bottle", 23 },
            { "Milk Bottle", 24 },
            { "Beer Bottle", 25 },
            { "Light Tube", 26 },
            { "Hammer", 27 },
            { "Console", 28 },
            { "Briefcase", 29 },
            { "Brass Knuckles", 30 },
            { "Extinguisher", 31 },
            { "Trophy", 32 },
            { "Gun", 33 },
            { "Broom", 34 },
            { "Sign", 35 },
            { "Picture", 36 },
            { "Glass Pane", 37 },
            { "Guitar", 38 },
            { "Tennis Racket", 39 },
            { "Phone", 40 },
            { "Cue", 41 },
            { "Tombstone", 42 },
            { "Cash", 43 },
            { "Burger", 44 },
            { "Pizza", 45 },
            { "Hotdog", 46 },
            { "Apple", 47 },
            { "Orange", 48 },
            { "Bannana", 49 },
            { "Crutch", 50 },
            { "Backpack", 51 },
            { "Shovel", 52 },
            { "Book", 53 },
            { "Magazine", 54 },
            { "Tablet", 55 },
            { "Thumbtacks", 56 },
            { "Football", 57 },
            { "Basketball", 58 },
            { "American Football", 59 },
            { "Baseball", 60 },
            { "Tennis Ball", 61 },
            { "Beach Ball", 62 },
            { "Tyre", 63 },
            { "Large Gift", 64 },
            { "Gift", 65 },
            { "Chainsaw", 66 },
            { "Handcuffs", 67 },
            { "Rubber Chicken", 68 }
            //Not all items the game has values for work, EG chainsaw and American Football are ones I tested that did not appear ingame.
        };
    }

    public static void SetCustomArenaShape()
    {
        GameObject arenaShape = GameObject.Find("arenaShape4");
        if (arenaShape != null)
        {
            World.arenaShape = 4;
        }

        arenaShape = GameObject.Find("arenaShape3");
        if (arenaShape != null)
        {
            World.arenaShape = 3;
        }

        arenaShape = GameObject.Find("arenaShape2");
        if (arenaShape != null)
        {
            World.arenaShape = 2;
        }

        arenaShape = GameObject.Find("arenaShape1");
        if (arenaShape != null)
        {
            World.arenaShape = 1;
        }
    }

    [HarmonyPatch(typeof(UnmappedBlocks))]
    public static class UnmappedBlocksPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AOKBJAAKFKD")]
        public static void AOKBJAAKFKDPrePatch()
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                World.arenaShape = 0;
            }
        }
    }

    [HarmonyPatch(typeof(World))]
    public static class WorldPatch
    {
        internal static int _tempLocation;

        [HarmonyPostfix]
        [HarmonyPatch("IBKGGNIDBEG")]
        public static void IBKGGNIDBEGPatch()
        {
            SetCustomArenaShape();
        }

        [HarmonyPostfix]
        [HarmonyPatch("GBDGLHHCLCI")]
        public static void GBDGLHHCLCIPatch()
        {
            SetCustomArenaShape();
            if (World.location > VanillaCounts.NoLocations)
            {
                World.AIDICBALEKM();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("ANEBLMBCGGF")]
        public static void ANEBLMBCGGFPatch(ref string __result, ref int CKKJILCHIGF, string AAJMFOHFOOK)
        {
            string originalResult = __result;
            string text = "Location " + CKKJILCHIGF;

            GameObject arenaName = FindGameObjectWithNameStartingWith("Arena Name:");
            if (arenaName != null)
            {
                text = arenaName.name.Substring("Arena Name:".Length);
            }
            else
            {
                text = originalResult;
            }

            __result = text;
        }

        private static GameObject FindGameObjectWithNameStartingWith(string name)
        {
            GameObject[] gameObjects = Object.FindObjectsOfType<GameObject>();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.name.StartsWith(name))
                {
                    return gameObject;
                }
            }

            return null;
        }

        [HarmonyPostfix]
        [HarmonyPatch("FPKIMMEMMHM")]
        public static void FPKIMMEMMHMPatch(ref Vector3 __result, int KNEDMBFJJAA, int OKBNFDJOAKH)
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                if (World.gArena != null)
                {
                    GameObject itemMarkerNorth = GameObject.Find("Itemborder (North)");
                    GameObject itemMarkerEast = GameObject.Find("Itemborder (East)");
                    GameObject itemMarkerSouth = GameObject.Find("Itemborder (South)");
                    GameObject itemMarkerWest = GameObject.Find("Itemborder (West)");

                    float furthestNorthDistance = float.MinValue;
                    float furthestEastDistance = float.MinValue;
                    float furthestSouthDistance = float.MaxValue;
                    float furthestWestDistance = float.MaxValue;
                    if (itemMarkerEast != null && itemMarkerNorth != null && itemMarkerSouth != null &&
                        itemMarkerWest != null)
                    {
                        if (itemMarkerNorth != null)
                        {
                            float northDistance = Vector3.Distance(itemMarkerNorth.transform.position,
                                new Vector3(0.0f, -0.4f, 0.0f));
                            furthestNorthDistance = northDistance;
                        }

                        if (itemMarkerEast != null)
                        {
                            float eastDistance = Vector3.Distance(itemMarkerEast.transform.position,
                                new Vector3(0.0f, -0.4f, 0.0f));
                            furthestEastDistance = eastDistance;
                        }

                        if (itemMarkerSouth != null)
                        {
                            float southDistance = Vector3.Distance(itemMarkerSouth.transform.position,
                                new Vector3(0.0f, -0.4f, 0.0f));
                            furthestSouthDistance = southDistance;
                        }

                        if (itemMarkerWest != null)
                        {
                            float westDistance = Vector3.Distance(itemMarkerWest.transform.position,
                                new Vector3(0.0f, -0.4f, 0.0f));
                            furthestWestDistance = westDistance;
                        }

                        // The furthest distances from the center coordinates
                        float itemBorderNorth = furthestNorthDistance;
                        float itemBorderEast = furthestEastDistance;
                        float itemBorderSouth = furthestSouthDistance;
                        float itemBorderWest = furthestWestDistance;

                        float newX = Random.Range(-itemBorderWest, itemBorderEast);
                        float newZ = Random.Range(-itemBorderSouth, itemBorderNorth);
                        float newY = World.ground;

                        __result = new Vector3(newX, newY, newZ);
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("LJMEMIODMEO")]
        public static void LJMEMIODMEOPatch(ref int KNEDMBFJJAA)
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                GameObject[] freezeObj = Object.FindObjectsOfType<GameObject>();
                GameObject[] announcerFreezeObj =
                    freezeObj.Where(obj => obj.name.StartsWith("AnnouncerFreeze")).ToArray();
                if (announcerFreezeObj.Length > 0)
                {
                    freezeAnnouncers = true;
                }
                else
                {
                    freezeAnnouncers = false;
                }

                GameObject[] objects = Object.FindObjectsOfType<GameObject>();
                float camDistanceFloat = new();

                string desiredName = "camDistance";
                GameObject[] camDistanceObj = objects.Where(obj => obj.name.StartsWith(desiredName)).ToArray();
                if (camDistanceObj.Length > 0)
                {
                    string[] camDistance =
                        camDistanceObj.Select(obj => obj.name.Substring(desiredName.Length)).ToArray();

                    foreach (string distance in camDistance)
                    {
                        float parsedDistance;
                        if (float.TryParse(distance, out parsedDistance))
                        {
                            camDistanceFloat = parsedDistance;
                        }
                        else
                        {
                            UnityEngine.Debug.LogError("Failed to parse camDistance: " + distance);
                        }
                    }
                }

                World.ground = 0f;
                World.ceiling = 100f;
                if (camDistanceFloat != 0)
                {
                    World.camNorth = camDistanceFloat;
                    World.camSouth = -camDistanceFloat;
                    World.camEast = camDistanceFloat;
                    World.camWest = -camDistanceFloat;
                }
                else
                {
                    //Default to original arena values
                    World.camNorth = 135f;
                    World.camSouth = -135f;
                    World.camEast = 135f;
                    World.camWest = -135f;
                }

                if (World.gArena != null)
                {
                    GameObject markerNorth = GameObject.Find("Marker (North)");
                    GameObject markerEast = GameObject.Find("Marker (East)");
                    GameObject markerSouth = GameObject.Find("Marker (South)");
                    GameObject markerWest = GameObject.Find("Marker (West)");

                    float furthestNorthDistance = float.MinValue;
                    float furthestEastDistance = float.MinValue;
                    float furthestSouthDistance = float.MaxValue;
                    float furthestWestDistance = float.MaxValue;
                    if (markerEast != null && markerNorth != null && markerSouth != null && markerWest != null)
                    {
                        if (markerNorth != null)
                        {
                            float northDistance = Vector3.Distance(markerNorth.transform.position,
                                new Vector3(0.0f, -0.4f, 0.0f));
                            furthestNorthDistance = northDistance;
                        }

                        if (markerEast != null)
                        {
                            float eastDistance = Vector3.Distance(markerEast.transform.position,
                                new Vector3(0.0f, -0.4f, 0.0f));
                            furthestEastDistance = eastDistance;
                        }

                        if (markerSouth != null)
                        {
                            float southDistance = Vector3.Distance(markerSouth.transform.position,
                                new Vector3(0.0f, -0.4f, 0.0f));
                            furthestSouthDistance = southDistance;
                        }

                        if (markerWest != null)
                        {
                            float westDistance = Vector3.Distance(markerWest.transform.position,
                                new Vector3(0.0f, -0.4f, 0.0f));
                            furthestWestDistance = westDistance;
                        }

                        // The furthest distances from the center coordinates
                        float furthestNorth = furthestNorthDistance;
                        float furthestEast = furthestEastDistance;
                        float furthestSouth = furthestSouthDistance;
                        float furthestWest = furthestWestDistance;

                        World.farNorth = furthestNorth;
                        World.farSouth = -furthestEast;
                        World.farEast = furthestSouth;
                        World.farWest = -furthestWest;
                    }
                }

                SetCustomArenaShape();
            }
        }

        //Get signs to randomise on custom arenas
        [HarmonyPostfix]
        [HarmonyPatch("AIDICBALEKM")]
        public static void AIDICBALEKMPatch(int EALGGMEGJNL = 0)
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                int num4 = UnmappedGlobals.NBNFJOFFMHO(1, 6);
                int num5;
                Transform[] signTransforms = World.gArena.transform.GetComponentsInChildren<Transform>(true);
                int count = 0;
                for (int i = 0; i < signTransforms.Length; i++)
                {
                    if (signTransforms[i].name.StartsWith("Sign"))
                    {
                        count++;
                    }
                }

                num5 = count;

                for (int i = 1; i <= num5; i++)
                {
                    Transform transform4 = World.gArena.transform.Find("arena/Signs/Sign" + i.ToString("00"));
                    if (!(transform4 != null))
                    {
                        continue;
                    }

                    int num6 = 0;
                    if (UnmappedGlobals.OOGBIHFJOIH > 0 && World.crowdSize > 0f && World.crowdSize <= 1f)
                    {
                        if ((i <= 18 && World.crowdSize >= 0.25f) || World.crowdSize >= 0.6f)
                        {
                            num6 = 1;
                        }

                        if (World.crowdSize < 0.7f && (i == 21 || i == 31 || i == 35))
                        {
                            num6 = 0;
                        }

                        if (UnmappedMenus.AAAIDOOHBCM == 50 && UnmappedGlobals.OOGBIHFJOIH == 1 &&
                            UnmappedGlobals.NBNFJOFFMHO(0, 1) == 0)
                        {
                            num6 = 0;
                        }
                    }

                    if (num6 > 0)
                    {
                        transform4.gameObject.SetActive(true);
                        if (UnmappedMenus.AAAIDOOHBCM == 50 && EALGGMEGJNL == 0)
                        {
                            if (UnmappedGlobals.OOGBIHFJOIH >= 2)
                            {
                                num4 = UnmappedGlobals.NBNFJOFFMHO(1, 6);
                            }

                            transform4.gameObject.GetComponent<Renderer>().sharedMaterial =
                                UnmappedTextures.IHAPGLGAILI[num4];
                        }
                    }
                    else if (UnmappedMenus.AAAIDOOHBCM == 50)
                    {
                        Object.Destroy(transform4.gameObject);
                    }
                    else
                    {
                        transform4.gameObject.SetActive(false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(UnmappedPlayer))]
        public static class UnmappedPlayerPatch
        {
            public static int storedValue;

            [HarmonyPrefix]
            [HarmonyPatch("PAOEHLEJKIJ")]
            public static void PAOEHLEJKIJPatch(UnmappedPlayer __instance)
            {
                if (freezeAnnouncers)
                {
                    if (__instance.FOPIBFHEBHM == 0)
                    {
                        storedValue = __instance.MKBFLJJAFPE;
                        __instance.MKBFLJJAFPE = 0;
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("PAOEHLEJKIJ")]
            public static void PAOEHLEJKIJPostPatch(UnmappedPlayer __instance)
            {
                if (freezeAnnouncers)
                {
                    if (__instance.FOPIBFHEBHM == 0 && storedValue != __instance.MKBFLJJAFPE)
                    {
                        __instance.MKBFLJJAFPE = storedValue;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(JOCFFDIGAGD))]
    public static class JOCFFDIGAGDPatch
    {
        public static bool furnitureAdded;
        public static List<string> furnitureList;

        [HarmonyPostfix]
        [HarmonyPatch("OCHAKPEPNOC")]
        public static void OCHAKPEPNOCPatch(ref int __result, int CLIEPAGIKOH, int KNHFAEABEPK, int BKLMEHCJBLK = 0)
        {
            int num = __result;
            furnitureAdded = false;
            if (KNHFAEABEPK == 1)
            {
                //Code is making new list of arena items so set our list back to empty here
                furnitureList = new List<string>();
            }

            if (CLIEPAGIKOH > VanillaCounts.NoLocations)
            {
                if (JOCFFDIGAGD.CNNFMKADJAH == null)
                {
                    JOCFFDIGAGD.CNNFMKADJAH = new Stock[1];
                }

                JOCFFDIGAGD.CNNFMKADJAH[0] = new Stock();
                {
                    //Maybe consider making this dynamic with map objects too for spawning stairs at any of the 4 (Or 6) corners
                    if (World.ringShape == 1)
                    {
                        if (KNHFAEABEPK == 1)
                        {
                            furnitureAdded = true;
                            furnitureList.Add("Steps1");
                            yOverride = 0f;
                            JOCFFDIGAGD.CNNFMKADJAH[0].OKIALPNMFHA(4, CLIEPAGIKOH, -35f * World.ringSize,
                                35f * World.ringSize, 315f);
                        }

                        if (KNHFAEABEPK == 2)
                        {
                            furnitureList.Add("Steps2");
                            furnitureAdded = true;
                            yOverride = 0f;
                            JOCFFDIGAGD.CNNFMKADJAH[0].OKIALPNMFHA(4, CLIEPAGIKOH, 35f * World.ringSize,
                                -35f * World.ringSize, 135f);
                        }
                    }

                    if (World.ringShape == 2)
                    {
                        if (KNHFAEABEPK == 1)
                        {
                            furnitureList.Add("Steps1");
                            furnitureAdded = true;
                            yOverride = 0f;
                            JOCFFDIGAGD.CNNFMKADJAH[0].OKIALPNMFHA(4, CLIEPAGIKOH, -21f * World.ringSize,
                                35f * World.ringSize, 330f);
                        }

                        if (KNHFAEABEPK == 2)
                        {
                            furnitureList.Add("Steps2");
                            furnitureAdded = true;
                            yOverride = 0f;
                            JOCFFDIGAGD.CNNFMKADJAH[0].OKIALPNMFHA(4, CLIEPAGIKOH, 21f * World.ringSize,
                                -35f * World.ringSize, 150f);
                        }
                    }
                }

                GameObject[] announcerObjects = Object.FindObjectsOfType<GameObject>()
                    .Where(obj => obj.name.StartsWith("AnnouncerDeskBundle")).ToArray();

                foreach (GameObject announcerObject in announcerObjects)
                {
                    ProcessAnnouncerDesk(announcerObject);
                }

                GameObject[] customGameObjects = Object.FindObjectsOfType<GameObject>()
                    .Where(obj => obj.name.StartsWith("GameObject:")).ToArray();

                foreach (GameObject customGameObject in customGameObjects)
                {
                    CustomGameObjectSpawner(customGameObject);
                }


                if (JOCFFDIGAGD.MAPEACFCDGA(JOCFFDIGAGD.CNNFMKADJAH[0].type) == 0 ||
                    JOCFFDIGAGD.CNNFMKADJAH[0].type > JOCFFDIGAGD.CAFMCHDHHEA)
                {
                    JOCFFDIGAGD.CNNFMKADJAH[0].type = 0;
                }

                if (BKLMEHCJBLK != 0 && JOCFFDIGAGD.CNNFMKADJAH[0].type != 0)
                {
                    if (BKLMEHCJBLK > 0)
                    {
                        num = JOCFFDIGAGD.HANCPGHNHOI();
                        JOCFFDIGAGD.CNNFMKADJAH[num].OKIALPNMFHA(JOCFFDIGAGD.CNNFMKADJAH[0].type,
                            JOCFFDIGAGD.CNNFMKADJAH[0].location, JOCFFDIGAGD.CNNFMKADJAH[0].x,
                            JOCFFDIGAGD.CNNFMKADJAH[0].z, JOCFFDIGAGD.CNNFMKADJAH[0].angle);
                    }
                    else
                    {
                        num = JOCFFDIGAGD.MFDCLFKDDFB(JOCFFDIGAGD.CNNFMKADJAH[0].type);
                        if (JOCFFDIGAGD.CNNFMKADJAH[0].scale != 1f)
                        {
                            JOCFFDIGAGD.GOCCMDGLIIA[num].IIMBIPKKEKP = JOCFFDIGAGD.CNNFMKADJAH[0].scale;
                            JOCFFDIGAGD.GOCCMDGLIIA[num].PPEDAJCCNPK(JOCFFDIGAGD.CNNFMKADJAH[0].type);
                            JOCFFDIGAGD.GOCCMDGLIIA[num].OOPKPKCHBEN.transform.localScale = new Vector3(
                                JOCFFDIGAGD.GOCCMDGLIIA[num].IIMBIPKKEKP, JOCFFDIGAGD.GOCCMDGLIIA[num].IIMBIPKKEKP,
                                JOCFFDIGAGD.GOCCMDGLIIA[num].IIMBIPKKEKP);
                        }

                        JOCFFDIGAGD.GOCCMDGLIIA[num].ODDCEOJJDFH(JOCFFDIGAGD.CNNFMKADJAH[0].x, World.ground,
                            JOCFFDIGAGD.CNNFMKADJAH[0].z, JOCFFDIGAGD.CNNFMKADJAH[0].angle);
                    }
                }

                if (BKLMEHCJBLK == 0 && JOCFFDIGAGD.CNNFMKADJAH[0].type != 0)
                {
                    num = KNHFAEABEPK;
                }
            }

            __result = num;

            void CustomGameObjectSpawner(GameObject customObject)
            {
                string customObjectName = customObject.name.Substring("GameObject:".Length);
                //Remove numbers from end of the name
                customObjectName = Regex.Replace(customObjectName, @"\d+$", string.Empty);
                Vector3 newObjectPosition = customObject.transform.position;
                Quaternion newObjectRotation = customObject.transform.rotation;

                if (!furnitureList.Contains(customObject.name) && !furnitureAdded)
                {
                    //Always add to list even if a valid customObjectId isn't returned to save going in everytime
                    furnitureList.Add(customObject.name);
                    int customObjectId = GetMapping(customObjectName);
                    if (customObjectId > 0)
                    {
                        furnitureAdded = true;
                        yOverride = newObjectPosition.y;
                        JOCFFDIGAGD.CNNFMKADJAH[0].OKIALPNMFHA(customObjectId, CLIEPAGIKOH, newObjectPosition.x,
                            newObjectPosition.z, newObjectRotation.eulerAngles.y);
                    }
                }
            }

            void ProcessAnnouncerDesk(GameObject deskObject)
            {
                // Use Original table and chair positions to make custom location of them stay together
                Vector3 originalTablePosition = new(44f, 0f, 43f);
                Quaternion originalTableRotation = Quaternion.Euler(0f, 180f, 0f);

                Vector3 originalChair1Position = new(39.5f, 0f, 50.5f);
                Vector3 originalChair2Position = new(48.5f, 0f, 50.5f);

                Vector3 originalChair1RelativePosition = Quaternion.Euler(0f, originalTableRotation.eulerAngles.y, 0f) *
                                                         (originalChair1Position - originalTablePosition);
                Vector3 originalChair2RelativePosition = Quaternion.Euler(0f, originalTableRotation.eulerAngles.y, 0f) *
                                                         (originalChair2Position - originalTablePosition);

                // Retrieve the position (x, y, z) and rotation of the object
                Vector3 newDeskPosition = deskObject.transform.position;
                Quaternion newDeskRotation = deskObject.transform.rotation;

                Quaternion relativeRotation = Quaternion.Inverse(originalTableRotation) * newDeskRotation;

                // Adjust the chair positions based on the relative rotation of the desk object
                Vector3 updatedChair1Position =
                    newDeskPosition + (relativeRotation * (originalChair1Position - originalTablePosition));
                Vector3 updatedChair2Position =
                    newDeskPosition + (relativeRotation * (originalChair2Position - originalTablePosition));

                // Add the furniture to the list and perform other actions
                if (!furnitureList.Contains(deskObject.name) && !furnitureAdded)
                {
                    furnitureList.Add(deskObject.name);
                    furnitureAdded = true;
                    yOverride = newDeskPosition.y;
                    JOCFFDIGAGD.CNNFMKADJAH[0].OKIALPNMFHA(3, CLIEPAGIKOH, newDeskPosition.x, newDeskPosition.z,
                        newDeskRotation.eulerAngles.y);
                }

                if (!furnitureList.Contains(deskObject.name + "ChairA") && !furnitureAdded)
                {
                    furnitureList.Add(deskObject.name + "ChairA");
                    furnitureAdded = true;
                    yOverride = newDeskPosition.y;
                    JOCFFDIGAGD.CNNFMKADJAH[0].OKIALPNMFHA(2, CLIEPAGIKOH, updatedChair1Position.x,
                        updatedChair1Position.z, newDeskRotation.eulerAngles.y);
                }

                if (!furnitureList.Contains(deskObject.name + "ChairB") && !furnitureAdded)
                {
                    furnitureList.Add(deskObject.name + "ChairB");
                    furnitureAdded = true;
                    yOverride = newDeskPosition.y;
                    JOCFFDIGAGD.CNNFMKADJAH[0].OKIALPNMFHA(2, CLIEPAGIKOH, updatedChair2Position.x,
                        updatedChair2Position.z, newDeskRotation.eulerAngles.y);
                }
            }

            int GetMapping(string input)
            {
                if (objectMappings == null)
                {
                    //Make sure objectMappings is populated before it is used.
                    CreateObjectMapping();
                }

                if (objectMappings.ContainsKey(input))
                {
                    return objectMappings[input];
                }

                return 0;
            }
        }
    }

    [HarmonyPatch(typeof(BGLBLDMOHEO))]
    public static class BGLBLDMOHEOPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ODDCEOJJDFH")]
        public static void ODDCEOJJDFHPostPatch(BGLBLDMOHEO __instance)
        {
            if (yOverride != 0f)
            {
                //This overrides the height for placement of furniture so it can be above ground level.
                JOCFFDIGAGD.CNNFMKADJAH[__instance.FAFNGINDBMM].y = yOverride;
                __instance.JNBIGPECCOB = yOverride;
                __instance.DOOCGGBPAFM = yOverride;
                __instance.BEHMHIINOGM = yOverride;

                //Set yOverride back to 0 afterwards so going to another map doesn't spawn all furniture in the air...
                yOverride = 0f;
            }
        }
    }

    [HarmonyPatch(typeof(Scene_Match_Setup))]
    public static class Scene_Match_SetupPatch
    {
        //oldArenaFurnitureCount used to make sure furniture number on custom arena is correct when swapping between multiple custom arenas
        private static int oldArenaFurnitureCount;

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static void UpdatePrePatch(Scene_Match_Setup __instance)
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                if (World.location != __instance.oldArena)
                {
                    if (World.location <= 1 && __instance.oldArena <= 1)
                    {
                        World.IBKGGNIDBEG(1);
                    }
                    else
                    {
                        World.GBDGLHHCLCI(1);
                    }

                    World.MFACINIEEPI();
                    World.AOECADAKFHI();
                    if (UnmappedGlobals.NHDABIOCLFH > 0)
                    {
                        Progress.arenaFog = World.fog;
                    }

                    //Calc furniture we should have on custom arena
                    GameObject[] announcerObjects = Object.FindObjectsOfType<GameObject>()
                        .Where(obj => obj.name.StartsWith("AnnouncerDeskBundle")).ToArray();
                    GameObject[] gameObjects = Object.FindObjectsOfType<GameObject>()
                        .Where(obj => obj.name.StartsWith("GameObject:")).ToArray();
                    int steps = 0;
                    if (World.ringShape > 0)
                    {
                        steps = 2;
                    }

                    JOCFFDIGAGD.LMPJNAODHHL = gameObjects.Length + (announcerObjects.Length * 3) + steps -
                                              oldArenaFurnitureCount;
                    oldArenaFurnitureCount = JOCFFDIGAGD.LMPJNAODHHL - steps;
                    JOCFFDIGAGD.IBLGHPIJAGA = 0;
                    int num = IDHJEMEKEMM.CMKPEAEKHGN(World.location);
                    if (IDHJEMEKEMM.LMPJNAODHHL < num)
                    {
                        IDHJEMEKEMM.LMPJNAODHHL = num;
                    }
                }

                __instance.oldArena = World.location;
            }
            else
            {
                oldArenaFurnitureCount = 0;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        public static void UpdatePostPatch()
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                //Force barriers to 0, can't work out how to disable option since its controlled by arena shape
                //So just forcing it to always be none does the job well enough
                World.arenaBarriers = 0;
            }
        }
    }
    [HarmonyPatch(typeof(ILPOGGNCJEN))]
    public static class ILPOGGNCJENPrePatch
    {
        public static bool HBFKOOEPFLHReduced = false;

        [HarmonyPrefix]
        [HarmonyPatch("DLADNAFPGPJ")]
        public static void LDLADNAFPGPJPrePatch(ILPOGGNCJEN __instance, float DKOBDIJJOGO, float LDEAEOHOCPO, float MFCNEPBJODD = 0f)
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                //Force set these to 20 / -20 to match original arena so it can trigger (Think its related to size of the door in original as custom arenas seem to have these a values of around 1.5 instead).
                __instance.LABFAOEKOBM[1] = 20;
                __instance.LABFAOEKOBM[2] = 20;
                __instance.LABFAOEKOBM[3] = -20;
                __instance.LABFAOEKOBM[4] = -20;
                if (!HBFKOOEPFLHReduced)
                {
                    //Also once only per map load, set this value to 5 less as otherwise the wrestlers needed to stand on a near exact spot to exit which the AI would almost never do.
                    __instance.HBFKOOEPFLH[3] -= 5;
                    HBFKOOEPFLHReduced = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(UnmappedPlayer))]
    public static class UnmappedPlayerPrePatch
    {
        private static int stored_FJPJNBKADDJ;
        private static bool ifStatementOnePassed;
        private static bool ifStatementTwoPassed;
        
        [HarmonyPrefix]
        [HarmonyPatch("LDFLNBABOOK")]
        public static void LDFLNBABOOKPrePatch(UnmappedPlayer __instance)
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                GameObject[] pyroObjects = Object.FindObjectsOfType<GameObject>()
                    .Where(obj => obj.name.StartsWith("PyroSpawn")).ToArray();

                if (pyroObjects.Length > 0)
                {
                    if (__instance.FKPIGOJCEAK == 1f && __instance.JGPFJIBNLFC != 54 &&
                        __instance.DCLLKPILCBP > World.camWest && __instance.DCLLKPILCBP < World.camEast &&
                        __instance.FFEONFCEHDF > World.camSouth &&
                        __instance.FFEONFCEHDF < World.camNorth && __instance.DCLLKPILCBP > World.farWest &&
                        __instance.DCLLKPILCBP < World.farEast && __instance.FFEONFCEHDF > World.farSouth &&
                        __instance.FFEONFCEHDF < World.farNorth &&
                        __instance.HIMOPKGMFKO(__instance.DCLLKPILCBP, __instance.BEHMHIINOGM, __instance.FFEONFCEHDF) >
                        0 && (UnmappedBlocks.JDJPKOBBNLG(__instance.DCLLKPILCBP, __instance.FFEONFCEHDF) > 0 ||
                              World.arenaShape * World.arenaBarriers == 0))
                    {
                        ifStatementOnePassed = true;
                    }
                    else
                    {
                        ifStatementOnePassed = false;
                    }

                    if (PHECEOMIMND.IPAFPBPKIKP == 1 && PHECEOMIMND.KGFJGDMFNLL == __instance.DHBIELODIAN &&
                        World.arenaShape > 0 && UnmappedGlobals.FJPJNBKADDJ > 0)
                    {
                        ifStatementTwoPassed = true;
                    }
                    else
                    {
                        ifStatementTwoPassed = false;
                    }

                    //Set this to zero to stop original pyro from going off
                    stored_FJPJNBKADDJ = UnmappedGlobals.FJPJNBKADDJ;
                    
                    UnmappedGlobals.FJPJNBKADDJ = 0;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("LDFLNBABOOK")]
        public static void LDFLNBABOOKPostPatch(UnmappedPlayer __instance)
        {
            //Set GFEDPBPDALB.AFJDBIAFGKE back at postfix
            if (stored_FJPJNBKADDJ != 0)
            {
                UnmappedGlobals.FJPJNBKADDJ = stored_FJPJNBKADDJ;
            }

            if (ifStatementOnePassed)
            {
                if (ifStatementTwoPassed)
                {
                    GameObject[] pyroObjects = Object.FindObjectsOfType<GameObject>()
                        .Where(obj => obj.name.StartsWith("PyroSpawn")).ToArray();

                    foreach (GameObject pyroObject in pyroObjects)
                    {
                        Vector3 newPyroPosition = pyroObject.transform.position;

                        if (__instance.LLEGGMCIALJ.pyro == 1 || __instance.LLEGGMCIALJ.pyro < 0)
                        {
                            CLJNCLLMLAO.GDIEKCLACCI(11, Color.white, 10f, null, 0f, newPyroPosition.y + 25f,
                                newPyroPosition.z, 0f, 0f, 0.1f);
                        }

                        if (__instance.LLEGGMCIALJ.pyro == 2 || __instance.LLEGGMCIALJ.pyro < 0)
                        {
                            CLJNCLLMLAO.GDIEKCLACCI(10, Color.white, 10f, null, -7f, newPyroPosition.y,
                                newPyroPosition.z);
                            CLJNCLLMLAO.GDIEKCLACCI(10, Color.white, 10f, null, 7f, newPyroPosition.y,
                                newPyroPosition.z);
                        }

                        if (__instance.LLEGGMCIALJ.pyro == 3 || __instance.LLEGGMCIALJ.pyro < 0)
                        {
                            UnmappedSound.OBLNONIKENE(__instance.MLDLMDCFHOM, UnmappedSound.BBLGKPMJOBA, -0.1f);
                            CLJNCLLMLAO.GDIEKCLACCI(91, Color.white, 8f, null, 0f, newPyroPosition.y + 7f,
                                newPyroPosition.z, 180f, 0.2f);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(IDHJEMEKEMM))]
    public static class IDHJEMEKEMMPrePatch
    {
        internal static Vector3? newWeaponPosition;
        internal static Vector3? newWeaponRotation;
        internal static int customWeaponId;
        internal static string customWeaponName;

        public static string CustomWeaponName
        {
            get => customWeaponName;
            set => customWeaponName = value;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ICEOBEPGFNC")]
        public static void ICEOBEPGFNCPrePatch()
        {
            ILPOGGNCJENPrePatch.HBFKOOEPFLHReduced = false;
            //Reset these to null so loading custom map second time onwards doesn't force all outside ring weapons to a weapon spawn point
            newWeaponPosition = null;
            newWeaponRotation = null;
        }

        [HarmonyPostfix]
        [HarmonyPatch("ICEOBEPGFNC")]
        public static void ICEOBEPGFNCPostPatch()
        {
            newWeaponPosition = null;
            newWeaponRotation = null;
            weaponList = new List<string>();
            System.Random random = new();

            //Loops through here to add weapons, ACBEHIAKAPB = weapon ID
            GameObject[] customWeaponObjects = Object.FindObjectsOfType<GameObject>()
                .Where(obj => obj.name.StartsWith("WeaponObject:")).ToArray();
            foreach (GameObject customWeaponObject in customWeaponObjects)
            {
                string customWeaponName = customWeaponObject.name.Substring("WeaponObject:".Length);
                //Remove numbers from end of the name
                customWeaponName = Regex.Replace(customWeaponName, @"\d+$", string.Empty);
                CustomWeaponName = customWeaponName;
                newWeaponPosition = customWeaponObject.transform.position;
                newWeaponRotation = customWeaponObject.transform.eulerAngles;

                if (!weaponList.Contains(customWeaponObject.name))
                {
                    if (customWeaponName == "Random")
                    {
                        customWeaponId = random.Next(1, 68 + 1);
                    }
                    else
                    {
                        customWeaponId = GetWeaponMapping(customWeaponName);
                    }

                    weaponList.Add(customWeaponObject.name);
                    if (customWeaponId != 0)
                    {
                        IDHJEMEKEMM.MFDCLFKDDFB(customWeaponId);
                    }
                }
            }
        }

        private static int GetWeaponMapping(string input)
        {
            if (weaponMappings == null)
            {
                //Make sure weaponMappings is populated before it is used.
                CreateWeaponMapping();
            }

            if (weaponMappings.ContainsKey(input))
            {
                return weaponMappings[input];
            }

            return 0;
        }
    }

    [HarmonyPatch(typeof(IAFGPLGNLKO))]
    public static class IAFGPLGNLKOPrePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GBDGLHHCLCI")]
        public static void GBDGLHHCLCIPostPatch(int ECLDPCGPPPL, int IMCLDENKCOE, IAFGPLGNLKO __instance,
            int ACBEHIAKAPB = 0)
        {
            if (IDHJEMEKEMMPrePatch.newWeaponPosition != null && IDHJEMEKEMMPrePatch.newWeaponRotation != null)
            {
                __instance.DCLLKPILCBP = IDHJEMEKEMMPrePatch.newWeaponPosition.Value.x;
                __instance.DOOCGGBPAFM = IDHJEMEKEMMPrePatch.newWeaponPosition.Value.y;
                __instance.FFEONFCEHDF = IDHJEMEKEMMPrePatch.newWeaponPosition.Value.z;
                __instance.NAMDOACBNED = IDHJEMEKEMMPrePatch.newWeaponRotation.Value.y;
                float rotationX = IDHJEMEKEMMPrePatch.newWeaponRotation.Value.x;
                float rotationZ = IDHJEMEKEMMPrePatch.newWeaponRotation.Value.z;
                string weaponName = IDHJEMEKEMMPrePatch.CustomWeaponName;
                if (weaponName == "Random")
                {
                    rotationX = 0f;
                    __instance.NAMDOACBNED = UnmappedGlobals.NBNFJOFFMHO(0, 359);
                    rotationZ = 0f;
                }

                //Need to update these for weapons to allow pickup
                __instance.BEHMHIINOGM = __instance.DOOCGGBPAFM;
                __instance.LOCLGGLOMNK = __instance.DCLLKPILCBP;
                __instance.JNBIGPECCOB = __instance.BEHMHIINOGM;
                __instance.OMHKIDHMBFL = __instance.FFEONFCEHDF;
                __instance.GDJANGFJJNM = __instance.NAMDOACBNED;

                __instance.LJMCHNKEPJP.transform.position = new Vector3(__instance.DCLLKPILCBP, __instance.BEHMHIINOGM,
                    __instance.FFEONFCEHDF);
                __instance.LJMCHNKEPJP.transform.eulerAngles =
                    new Vector3(rotationX, __instance.NAMDOACBNED, rotationZ);
            }
        }
    }

    [HarmonyPatch(typeof(DJLJEFBLPKG))]
    public static class DJLJEFBLPKG_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GBDGLHHCLCI")]
        public static void GBDGLHHCLCI_Patch()
        {
            GameObject titanCamera = GameObject.Find("TitantronCamera");
            if (titanCamera)
            {
                titanCamera.AddComponent<CameraTracking>();
            }
        }
    }

    public class CameraTracking : MonoBehaviour
    {
        public GameObject CameraFocalPoint;

        private void Start()
        {
            this.CameraFocalPoint = GameObject.Find("Camera Focal Point");
        }

        private void Update()
        {
            if (this.CameraFocalPoint)
            {
                this.transform.LookAt(this.CameraFocalPoint.transform);
            }
        }
    }
}