namespace WECCL.Patches;

[HarmonyPatch]
internal class MenuPatch
{
    private static int _lastFed = 1;
    internal static readonly Dictionary<int, Tuple<int, int, float, int, int>> _optimalLayouts = new();

    private static int _expectedNextId = -1;

    [HarmonyPatch(typeof(Characters), nameof(Characters.PBNPILLGGLH))]
    [HarmonyPrefix]
    public static void Characters_PBNPILLGGLH(int HCKDAHDFLIF) // Second argument
    {
        _lastFed = HCKDAHDFLIF;
        _expectedNextId = 0;
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

                FindBestFit(fedSize, -525, -310, 525, 110, out rows, out columns, out scale, out startX, out startY);

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
}