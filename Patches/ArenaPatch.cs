using System.Text.RegularExpressions;
using WECCL.Content;
using Debug = UnityEngine.Debug;

namespace WECCL.Patches;

[HarmonyPatch]
public class ArenaPatch
{
    public static Dictionary<string, int> objectMappings;
    public static float yOverride;
    public static int NoOriginalLocationsValue;

    private void Awake()
    {
        CreateObjectMapping();
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

    [HarmonyPatch(typeof(IFNKAOLDPGM))]
    public static class IFNKAOLDPGMPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OJMNGOHBIJH")]
        public static void OJMNGOHBIJHPrePatch()
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
        [HarmonyPatch("JACOFMPNHGH")]
        public static void JACOFMPNHGHPatch()
        {
            SetCustomArenaShape();
        }

        [HarmonyPostfix]
        [HarmonyPatch("DMGJOHGEOKF")]
        public static void DMGJOHGEOKFPatch()
        {
            SetCustomArenaShape();
            if (World.location > VanillaCounts.NoLocations)
            {
                World.JCDNJODBBMF();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("OFCGMNIICAM")]
        public static void OFCGMNIICAMPatch(ref int ICOMOCOJCLP, ref string __result, string IDLAKOEHIEF = "")
        {
            string originalResult = __result;
            string text = "Location " + ICOMOCOJCLP.ToString();

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
            GameObject[] gameObjects = GameObject.FindObjectsOfType<GameObject>();

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
        [HarmonyPatch("OGCEKDEJBBF")]
        public static void OGCEKDEJBBFPatch(int JIBNLDBGFLN, ref Vector3 __result, int HKLOLOLBMDM = 0)
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
                    if (itemMarkerEast != null && itemMarkerNorth != null && itemMarkerSouth != null && itemMarkerWest != null)
                    {

                        if (itemMarkerNorth != null)
                        {
                            float northDistance = Vector3.Distance(itemMarkerNorth.transform.position, new Vector3(0.0f, -0.4f, 0.0f));
                            furthestNorthDistance = northDistance;
                        }

                        if (itemMarkerEast != null)
                        {
                            float eastDistance = Vector3.Distance(itemMarkerEast.transform.position, new Vector3(0.0f, -0.4f, 0.0f));
                            furthestEastDistance = eastDistance;
                        }

                        if (itemMarkerSouth != null)
                        {
                            float southDistance = Vector3.Distance(itemMarkerSouth.transform.position, new Vector3(0.0f, -0.4f, 0.0f));
                            furthestSouthDistance = southDistance;
                        }

                        if (itemMarkerWest != null)
                        {
                            float westDistance = Vector3.Distance(itemMarkerWest.transform.position, new Vector3(0.0f, -0.4f, 0.0f));
                            furthestWestDistance = westDistance;
                        }

                        // The furthest distances from the center coordinates
                        float itemBorderNorth = furthestNorthDistance;
                        float itemBorderEast = furthestEastDistance;
                        float itemBorderSouth = furthestSouthDistance;
                        float itemBorderWest = furthestWestDistance;

                        float newX = UnityEngine.Random.Range(-itemBorderWest, itemBorderEast);
                        float newZ = UnityEngine.Random.Range(-itemBorderSouth, itemBorderNorth);
                        float newY = World.ground;

                        __result = new Vector3(newX, newY, newZ);
                    }
                }
            }
        }
        

        [HarmonyPostfix]
        [HarmonyPatch("OGAHGABKJFO")]
        public static void OGAHGABKJFOPatch(ref int JIBNLDBGFLN)
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
                float camDistanceFloat = new float();

                string desiredName = "camDistance";
                GameObject[] camDistanceObj = objects.Where(obj => obj.name.StartsWith(desiredName)).ToArray();
                if (camDistanceObj != null)
                {
                    string[] camDistance = camDistanceObj.Select(obj => obj.name.Substring(desiredName.Length)).ToArray();

                    foreach (string distance in camDistance)
                    {
                        float parsedDistance;
                        if (float.TryParse(distance, out parsedDistance))
                        {
                            camDistanceFloat = parsedDistance;
                        }
                        else
                        {
                            Debug.LogError("Failed to parse camDistance: " + distance);
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
                            float northDistance = Vector3.Distance(markerNorth.transform.position, new Vector3(0.0f, -0.4f, 0.0f));
                            furthestNorthDistance = northDistance;
                        }

                        if (markerEast != null)
                        {
                            float eastDistance = Vector3.Distance(markerEast.transform.position, new Vector3(0.0f, -0.4f, 0.0f));
                            furthestEastDistance = eastDistance;
                        }

                        if (markerSouth != null)
                        {
                            float southDistance = Vector3.Distance(markerSouth.transform.position, new Vector3(0.0f, -0.4f, 0.0f));
                            furthestSouthDistance = southDistance;
                        }

                        if (markerWest != null)
                        {
                            float westDistance = Vector3.Distance(markerWest.transform.position, new Vector3(0.0f, -0.4f, 0.0f));
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
        [HarmonyPatch("JCDNJODBBMF")]
        public static void JCDNJODBBMFPatch(int CLMNJCOMHJG = 0)
        {
            if (World.location > VanillaCounts.NoLocations)
            {
                int num4 = GFEDPBPDALB.GLHCKJPAOPG(1, 6);
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
                    if (GFEDPBPDALB.KFGGEOKGFDL > 0 && World.crowdSize > 0f && World.crowdSize <= 1f)
                    {
                        if ((i <= 18 && World.crowdSize >= 0.25f) || World.crowdSize >= 0.6f)
                        {
                            num6 = 1;
                        }
                        if (World.crowdSize < 0.7f && (i == 21 || i == 31 || i == 35))
                        {
                            num6 = 0;
                        }
                        if (LAHGBLEJCEO.GLDIFJOEOIO == 50 && GFEDPBPDALB.KFGGEOKGFDL == 1 && GFEDPBPDALB.GLHCKJPAOPG(0, 1) == 0)
                        {
                            num6 = 0;
                        }
                    }
                    if (num6 > 0)
                    {
                        transform4.gameObject.SetActive(value: true);
                        if (LAHGBLEJCEO.GLDIFJOEOIO == 50 && CLMNJCOMHJG == 0)
                        {
                            if (GFEDPBPDALB.KFGGEOKGFDL >= 2)
                            {
                                num4 = GFEDPBPDALB.GLHCKJPAOPG(1, 6);
                            }
                            transform4.gameObject.GetComponent<Renderer>().sharedMaterial = JBAFDOKGDIJ.DEFAPOLNFOO[num4];
                        }
                    }
                    else if (LAHGBLEJCEO.GLDIFJOEOIO == 50)
                    {
                        UnityEngine.Object.Destroy(transform4.gameObject);
                    }
                    else
                    {
                        transform4.gameObject.SetActive(value: false);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(KCEBMOOKIPH))]
    public static class KCEBMOOKIPHPatch
    {
        public static bool furnitureAdded = false;
        public static List<string> furnitureList;
        [HarmonyPostfix]
        [HarmonyPatch("OHOCDJPLPPB")]
        public static void OHOCDJPLPPBPatch(int ODKMKAAPGEM, int HNCNGGMCOAF, ref int __result, int DPJHDBMLFPB = 0)
        {
            int num = __result;
            furnitureAdded = false;
            if (HNCNGGMCOAF == 1)
            {
                //Code is making new list of arena items so set our list back to empty here
                furnitureList = new List<string>();
            }
            if (ODKMKAAPGEM > VanillaCounts.NoLocations)
            {
                if (KCEBMOOKIPH.GPIHKBFAOMK == null)
                {
                    KCEBMOOKIPH.GPIHKBFAOMK = new Stock[1];
                }
                KCEBMOOKIPH.GPIHKBFAOMK[0] = new Stock();
                {
                    //Maybe consider making this dynamic with map objects too for spawning stairs at any of the 4 (Or 6) corners
                    if (World.ringShape == 1)
                    {
                        if (HNCNGGMCOAF == 1)
                        {
                            furnitureAdded = true;
                            furnitureList.Add("Steps1");
                            yOverride = 0f;
                            KCEBMOOKIPH.GPIHKBFAOMK[0].KIIIOPKHPHI(4, ODKMKAAPGEM, -35f * World.ringSize, 35f * World.ringSize, 315f);
                        }
                        if (HNCNGGMCOAF == 2)
                        {
                            furnitureList.Add("Steps2");
                            furnitureAdded = true;
                            yOverride = 0f;
                            KCEBMOOKIPH.GPIHKBFAOMK[0].KIIIOPKHPHI(4, ODKMKAAPGEM, 35f * World.ringSize, -35f * World.ringSize, 135f);
                        }
                    }
                    if (World.ringShape == 2)
                    {
                        if (HNCNGGMCOAF == 1)
                        {
                            furnitureList.Add("Steps1");
                            furnitureAdded = true;
                            yOverride = 0f;
                            KCEBMOOKIPH.GPIHKBFAOMK[0].KIIIOPKHPHI(4, ODKMKAAPGEM, -21f * World.ringSize, 35f * World.ringSize, 330f);
                        }
                        if (HNCNGGMCOAF == 2)
                        {
                            furnitureList.Add("Steps2");
                            furnitureAdded = true;
                            yOverride = 0f;
                            KCEBMOOKIPH.GPIHKBFAOMK[0].KIIIOPKHPHI(4, ODKMKAAPGEM, 21f * World.ringSize, -35f * World.ringSize, 150f);
                        }
                    }
                }

                GameObject[] announcerObjects = GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.StartsWith("AnnouncerDeskBundle")).ToArray();

                foreach (GameObject announcerObject in announcerObjects)
                {
                    ProcessAnnouncerDesk(announcerObject);
                }

                GameObject[] customGameObjects = GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.StartsWith("GameObject:")).ToArray();

                foreach (GameObject customGameObject in customGameObjects)
                {
                    CustomGameObjectSpawner(customGameObject);
                }


                if (KCEBMOOKIPH.KFIOOMEFCBG(KCEBMOOKIPH.GPIHKBFAOMK[0].type) == 0 || KCEBMOOKIPH.GPIHKBFAOMK[0].type > KCEBMOOKIPH.JMKPIDIDDFK)
                {
                    KCEBMOOKIPH.GPIHKBFAOMK[0].type = 0;
                }
                if (DPJHDBMLFPB != 0 && KCEBMOOKIPH.GPIHKBFAOMK[0].type != 0)
                {
                    if (DPJHDBMLFPB > 0)
                    {
                        num = KCEBMOOKIPH.ONNDIMGHIPP();
                        KCEBMOOKIPH.GPIHKBFAOMK[num].KIIIOPKHPHI(KCEBMOOKIPH.GPIHKBFAOMK[0].type, KCEBMOOKIPH.GPIHKBFAOMK[0].location, KCEBMOOKIPH.GPIHKBFAOMK[0].x, KCEBMOOKIPH.GPIHKBFAOMK[0].z, KCEBMOOKIPH.GPIHKBFAOMK[0].angle);
                    }
                    else
                    {
                        num = KCEBMOOKIPH.OHBONDHEDEC(KCEBMOOKIPH.GPIHKBFAOMK[0].type);
                        if (KCEBMOOKIPH.GPIHKBFAOMK[0].scale != 1f)
                        {
                            KCEBMOOKIPH.JMENOMCNCGF[num].PNPLJGMLGGB = KCEBMOOKIPH.GPIHKBFAOMK[0].scale;
                            KCEBMOOKIPH.JMENOMCNCGF[num].ICHKOACHPMJ(KCEBMOOKIPH.GPIHKBFAOMK[0].type);
                            KCEBMOOKIPH.JMENOMCNCGF[num].PNINKKAAPBD.transform.localScale = new Vector3(KCEBMOOKIPH.JMENOMCNCGF[num].PNPLJGMLGGB, KCEBMOOKIPH.JMENOMCNCGF[num].PNPLJGMLGGB, KCEBMOOKIPH.JMENOMCNCGF[num].PNPLJGMLGGB);
                        }
                        KCEBMOOKIPH.JMENOMCNCGF[num].OGAIPIGBDOD(KCEBMOOKIPH.GPIHKBFAOMK[0].x, World.ground, KCEBMOOKIPH.GPIHKBFAOMK[0].z, KCEBMOOKIPH.GPIHKBFAOMK[0].angle);
                    }
                }
                if (DPJHDBMLFPB == 0 && KCEBMOOKIPH.GPIHKBFAOMK[0].type != 0)
                {
                    num = HNCNGGMCOAF;
                }
            }
            //Reset customY to 0 before leaving
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
                        KCEBMOOKIPH.GPIHKBFAOMK[0].KIIIOPKHPHI(customObjectId, ODKMKAAPGEM, newObjectPosition.x, newObjectPosition.z, newObjectRotation.eulerAngles.y);
                    }
                }
            }

            void ProcessAnnouncerDesk(GameObject deskObject)
            {
                // Use Original table and chair positions to make custom location of them stay together
                Vector3 originalTablePosition = new Vector3(44f, 0f, 43f);
                Quaternion originalTableRotation = Quaternion.Euler(0f, 180f, 0f);

                Vector3 originalChair1Position = new Vector3(39.5f, 0f, 50.5f);
                Vector3 originalChair2Position = new Vector3(48.5f, 0f, 50.5f);

                Vector3 originalChair1RelativePosition = Quaternion.Euler(0f, originalTableRotation.eulerAngles.y, 0f) * (originalChair1Position - originalTablePosition);
                Vector3 originalChair2RelativePosition = Quaternion.Euler(0f, originalTableRotation.eulerAngles.y, 0f) * (originalChair2Position - originalTablePosition);

                // Retrieve the position (x, y, z) and rotation of the object
                Vector3 newDeskPosition = deskObject.transform.position;
                Quaternion newDeskRotation = deskObject.transform.rotation;

                Quaternion relativeRotation = Quaternion.Inverse(originalTableRotation) * newDeskRotation;

                // Adjust the chair positions based on the relative rotation of the desk object
                Vector3 updatedChair1Position = newDeskPosition + (relativeRotation * (originalChair1Position - originalTablePosition));
                Vector3 updatedChair2Position = newDeskPosition + (relativeRotation * (originalChair2Position - originalTablePosition));

                // Add the furniture to the list and perform other actions
                if (!furnitureList.Contains(deskObject.name) && !furnitureAdded)
                {
                    furnitureList.Add(deskObject.name);
                    furnitureAdded = true;
                    yOverride = newDeskPosition.y;
                    KCEBMOOKIPH.GPIHKBFAOMK[0].KIIIOPKHPHI(3, ODKMKAAPGEM, newDeskPosition.x, newDeskPosition.z, newDeskRotation.eulerAngles.y);
                }
                if (!furnitureList.Contains(deskObject.name + "ChairA") && !furnitureAdded)
                {
                    furnitureList.Add(deskObject.name + "ChairA");
                    furnitureAdded = true;
                    yOverride = newDeskPosition.y;
                    KCEBMOOKIPH.GPIHKBFAOMK[0].KIIIOPKHPHI(2, ODKMKAAPGEM, updatedChair1Position.x, updatedChair1Position.z, newDeskRotation.eulerAngles.y);
                }
                if (!furnitureList.Contains(deskObject.name + "ChairB") && !furnitureAdded)
                {
                    furnitureList.Add(deskObject.name + "ChairB");
                    furnitureAdded = true;
                    yOverride = newDeskPosition.y;
                    KCEBMOOKIPH.GPIHKBFAOMK[0].KIIIOPKHPHI(2, ODKMKAAPGEM, updatedChair2Position.x, updatedChair2Position.z, newDeskRotation.eulerAngles.y);
                }
            }

            int GetMapping(string input)
            {
                if (objectMappings == null)
                {
                    //Make sure objectMappings is populated before it is used.
                    ArenaPatch.CreateObjectMapping();
                }
                if (objectMappings.ContainsKey(input))
                {
                    return objectMappings[input];
                }
                else
                {
                    return 0;
                }
            }
        }
    }
    [HarmonyPatch(typeof(COHBONKLIKF))]
    public static class COHBONKLIKFPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("OGAIPIGBDOD")]
        public static void OGAIPIGBDODPostPatch(float AOFCMJDNFGD, float DKIDHBJMKFF, float CLMHDGDKBCH, float MGENCLKAFLH, COHBONKLIKF __instance)
        {
            if (yOverride != 0f)
            {
                //This overrides the height for placement of furniture so it can be above ground level.
                KCEBMOOKIPH.GPIHKBFAOMK[__instance.CEJNFFKHENG].y = yOverride;
                __instance.MIFDMGILIND = yOverride;
                __instance.CPNHGCLBEFH = yOverride;
                __instance.IHBEDOLDPJC = yOverride;
            }
        }
    }

    [HarmonyPatch(typeof(Scene_Match_Setup))]
    public static class Scene_Match_SetupPatch
    {
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
}