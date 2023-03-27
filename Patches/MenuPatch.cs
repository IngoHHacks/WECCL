using WECCL.Utils;

namespace WECCL.Patches;

[HarmonyPatch]
internal class MenuPatch
{
    private static int _lastFed = 1;
    internal static readonly Dictionary<int, Tuple<int, int, float, int, int>> _optimalLayouts = new();

    private static int _expectedNextId = -1;

    [HarmonyPatch(typeof(Characters), "NBBPIGNONKO")]
    [HarmonyPrefix]
    public static void Characters_NBBPIGNONKO(int FODHPJLILOD)
    {
        _lastFed = FODHPJLILOD;
        _expectedNextId = 0;
    }

    /*
     * GameMenus.GHGPDLAMLFL is called when the player opens the editor (including the fed editor)
     * This patch is used to resize the character editor to fit the roster size if it is larger than 48 (vanilla max)
     */
    [HarmonyPatch(typeof(GameMenus), "GHGPDLAMLFL")]
    [HarmonyPrefix]
    public static void GameMenus_GHGPDLAMLFL(int PPPEMNOKLLL, string DOCHPFFDDHL, ref float PLPIOEGOEOP,
        ref float FFMFHEJFJHO, ref float GJKLLIOBLBN, ref float LALIOOHGONN)
    {
        try
        {
            if (PPPEMNOKLLL != 5)
            {
                return;
            }

            int fedSize = Characters.fedData[_lastFed].size;
            if (fedSize > 48)
            {
                int actualIndex = (((int)PLPIOEGOEOP + 525) / 210) + ((-(int)FFMFHEJFJHO + 110) / 60 * 6);

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

                GJKLLIOBLBN = scale;
                LALIOOHGONN = scale;
                PLPIOEGOEOP = startX + (actualIndex % columns * 210 * scale);
                FFMFHEJFJHO = startY - (actualIndex / columns * 50 * scale);
            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }
}