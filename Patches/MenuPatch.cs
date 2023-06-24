using UnityEngine.UI;
using WECCL.Content;

namespace WECCL.Patches;

[HarmonyPatch]
internal class MenuPatch
{
    private static int _lastFed = 1;
    internal static readonly Dictionary<int, Tuple<int, int, float, int, int>> _optimalLayouts = new();

    private static int _expectedNextId = -1;

    [HarmonyPatch(typeof(Characters), nameof(Characters.PBNPILLGGLH))]
    [HarmonyPrefix]
    public static bool Characters_PBNPILLGGLH(int LOIILHLDKKE, int HCKDAHDFLIF, int IDNKMKFMPOG) // Second argument
    {
        _lastFed = HCKDAHDFLIF;
        _expectedNextId = 0;
        if (HCKDAHDFLIF == VanillaCounts.NoFeds + 1)
        {
            DNDIEGNJOKN.MNJEEKGMEEC = Characters.no_chars;
            DNDIEGNJOKN.FDJHLFJLILM = Characters.c.Skip(1).SortBy(LOIILHLDKKE).Select(x => x.id).ToArray().Prepend(0).ToArray();
            DNDIEGNJOKN.GJNFOKIHEON = new int[Characters.no_chars + 1];
            for (int i = 0; i < DNDIEGNJOKN.FDJHLFJLILM.Length; i++)
            {
                DNDIEGNJOKN.GJNFOKIHEON[DNDIEGNJOKN.FDJHLFJLILM[i]] = i;
            }
            return false;
        }
        return true;
    }

    /*
     * GameMenus.ICKGKDOKJEN is called when the player opens the editor (including the fed editor)
     * This patch is used to resize the character editor to fit the roster size if it is larger than 48 (vanilla max)
     */
    [HarmonyPatch(typeof(GameMenus), nameof(GameMenus.ICKGKDOKJEN))]
    [HarmonyPrefix]
    public static void GameMenus_ICKGKDOKJEN(int IJLDPEFGOOL, string NPDFJAEJIND, ref float GBKANPHAPIG,
        ref float AHMKMFPJFJA, ref float GLMFADFPECG, ref float BKBCELICBON)
    {
        try
        {
            if (IJLDPEFGOOL != 5)
            {
                return;
            }

            int fedSize = Characters.fedData[_lastFed].size;
            if (fedSize > 48)
            {
                int actualIndex = (((int)GBKANPHAPIG + 525) / 210) + ((-(int)AHMKMFPJFJA + 110) / 60 * 6);

                if (actualIndex != _expectedNextId)
                {
                    return;
                }

                _expectedNextId++;

                int rows;
                int columns;
                float scale;
                int startX;
                int startY;

                var y = 110;
                
                if (_lastFed == VanillaCounts.NoFeds + 1)
                {
                    y = 70;
                }
                
                FindBestFit(fedSize, -525, -310, 525, y, out rows, out columns, out scale, out startX, out startY);

                GLMFADFPECG = scale;
                BKBCELICBON = scale;
                GBKANPHAPIG = startX + (actualIndex % columns * 210 * scale);
                AHMKMFPJFJA = startY - (actualIndex / columns * 50 * scale);
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    [HarmonyPatch(typeof(Scene_Select_Char), nameof(Scene_Select_Char.Update))]
    [HarmonyPrefix]
    public static void Scene_Select_Char_Update()
    {
        Characters.no_feds = VanillaCounts.NoFeds + 1;
        if (Characters.fedData.Length <= Characters.no_feds)
        {
            Array.Resize(ref Characters.fedData, Characters.no_feds + 1);
            Characters.fedData[Characters.no_feds] = new Roster();
            Characters.fedData[Characters.no_feds].size = Characters.no_chars;
        }
    }

    [HarmonyPatch(typeof(Scene_Select_Char), nameof(Scene_Select_Char.Update))]
    [HarmonyPostfix]
    public static void Scene_Select_Char_Update_Postfix()
    {
        Characters.no_feds = VanillaCounts.NoFeds;
    }
    
    /*
    [HarmonyPatch(typeof(DNDIEGNJOKN), nameof(DNDIEGNJOKN.ICKGKDOKJEN))]
    [HarmonyPostfix]
    public static void DNDIEGNJOKN_ICKGKDOKJEN()
    {
        if (DNDIEGNJOKN.OBNLIIMODBI == 11 && Characters.fed == VanillaCounts.NoFeds + 1)
        {
            DNDIEGNJOKN.LKMAEOFENHG();
            DNDIEGNJOKN.FPLAGLKCKII[DNDIEGNJOKN.CFPJFAKOKMD].ICKGKDOKJEN(2, "Search", 0, 110, 1, 1);
        }
    }
    */
}