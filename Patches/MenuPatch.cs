using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WECCL.Patches;

[HarmonyPatch]
internal class MenuPatch
{
    private static int _lastFed = 1;
    private static Dictionary<int, Tuple<int, int, float, int, int>> _optimalLayouts = new();
    
    [HarmonyPatch(typeof(Characters), "NBBPIGNONKO")]
    [HarmonyPrefix]
    public static void Characters_NBBPIGNONKO(int FODHPJLILOD)
    {
        _lastFed = FODHPJLILOD;
    }
    
    [HarmonyPatch(typeof(OMNHIAMJHKF), "GHGPDLAMLFL")]
    [HarmonyPrefix]
    public static void OMNHIAMJHKF_GHGPDLAMLFL(int PPPEMNOKLLL, string DOCHPFFDDHL, ref float PLPIOEGOEOP, ref float FFMFHEJFJHO, ref float GJKLLIOBLBN, ref float LALIOOHGONN)
    {
        try
        {
            if (PPPEMNOKLLL != 5)
            {
                return;
            }

            var fedSize = Characters.fedData[_lastFed].size;
            if (fedSize > 48)
            {
                int actualIndex = (((int)PLPIOEGOEOP + 525) / 210) + ((-(int)FFMFHEJFJHO + 110) / 60 * 6);
                
                int rows;
                int columns;
                float scale;
                int startX;
                int startY;
                FindBestFit(fedSize, out rows, out columns, out scale, out startX, out startY);

                GJKLLIOBLBN = scale;
                LALIOOHGONN = scale;
                PLPIOEGOEOP = startX + (actualIndex % columns) * 210 * scale;
                FFMFHEJFJHO = startY - (actualIndex / columns) * 50 * scale;

            }
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }
    
    private static void FindBestFit(int size, out int rows, out int columns, out float scale, out int startX, out int startY)
    {
        if (_optimalLayouts.TryGetValue(size, out var tuple))
        {
            rows = tuple.Item1;
            columns = tuple.Item2;
            scale = tuple.Item3;
            startX = tuple.Item4;
            startY = tuple.Item5;
            return;
        }
        int minX = -525;
        int maxX = 525;
        int minY = -310;
        int maxY = 110;
        int itemWidth = 210;
        int itemHeight = 50;
        int totalWidth = maxX - minX;
        int totalHeight = maxY - minY;
        float curScale = 1f;
        while (true)
        {
            int scaledTotalWidth = totalWidth + (int)(itemWidth * curScale);
            int scaledTotalHeight = totalHeight + (int)(itemHeight * curScale);
            int curWidth = (int)(itemWidth * curScale);
            int curHeight = (int)(itemHeight * curScale);
            int curColumns = scaledTotalWidth / curWidth;
            int curRows = scaledTotalHeight / curHeight;
            int curItems = curColumns * curRows;
            if (curItems >= size)
            {
                rows = curRows;
                columns = curColumns;
                scale = curScale;
                int curTotalWidth = curColumns * curWidth;
                startX = minX + (scaledTotalWidth - curTotalWidth) / 2;
                int curTotalHeight = curRows * curHeight;
                startY = maxY - (scaledTotalHeight - curTotalHeight) / 2;
                Plugin.Log.LogDebug($"Found best fit for {size} items: {rows} rows, {columns} columns, {scale} scale, {startX} startX, {startY} startY");
                _optimalLayouts.Add(size, new Tuple<int, int, float, int, int>(rows, columns, scale, startX, startY));
                return;
            }
            curScale /= 1.05f;
        }
    }
}