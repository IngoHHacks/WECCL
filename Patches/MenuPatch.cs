using WECCL.Utils;

namespace WECCL.Patches;

[HarmonyPatch]
internal class MenuPatch
{
    private static int _lastFed = 1;
    internal static readonly Dictionary<int, Tuple<int, int, float, int, int>> _optimalLayouts = new();

    private static int _expectedNextId = -1;

    [HarmonyPatch(typeof(Characters), nameof(Characters.DBAFDIJHDAO))]
    [HarmonyPrefix]
    public static void Characters_DBAFDIJHDAO(int GDMOLNLLKAK)
    {
        _lastFed = GDMOLNLLKAK;
        _expectedNextId = 0;
    }

    /*
     * GameMenus.DMGJOHGEOKF is called when the player opens the editor (including the fed editor)
     * This patch is used to resize the character editor to fit the roster size if it is larger than 48 (vanilla max)
     */
    [HarmonyPatch(typeof(GameMenus), nameof(GameMenus.DMGJOHGEOKF))]
    [HarmonyPrefix]
    public static void GameMenus_DMGJOHGEOKF(int MBEAKNKLKOE, string DHPDOHFLOII, ref float EPGAPPEOJKI,
        ref float POKNPEKLCCE, ref float DOCNIBNJKOL, ref float MJPLHDGLHDF)
    {
        try
        {
            if (MBEAKNKLKOE != 5)
            {
                return;
            }

            int fedSize = Characters.fedData[_lastFed].size;
            if (fedSize > 48)
            {
                int actualIndex = (((int)EPGAPPEOJKI + 525) / 210) + ((-(int)POKNPEKLCCE + 110) / 60 * 6);

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

                DOCNIBNJKOL = scale;
                MJPLHDGLHDF = scale;
                EPGAPPEOJKI = startX + (actualIndex % columns * 210 * scale);
                POKNPEKLCCE = startY - (actualIndex / columns * 50 * scale);
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }
}