using WECCL.Patches;

namespace WECCL.Utils;

public static class MenuUtils
{
    internal static float PrevScale = 1.0f;
    
    internal static readonly Dictionary<int, Tuple<int, int, float, int, int>> _optimalLayouts = new();
    
    public static void FindBestFit(int size, int minX, int minY, int maxX, int maxY, out int rows, out int columns,
        out float scale, out int startX,
        out int startY)
    {
        if (_optimalLayouts.TryGetValue(size, out Tuple<int, int, float, int, int> tuple))
        {
            rows = tuple.Item1;
            columns = tuple.Item2;
            scale = tuple.Item3;
            startX = tuple.Item4;
            startY = tuple.Item5;
            PrevScale = scale;
            return;
        }

        int itemWidth = size > 35 ? 210 : 245;
        int itemHeight = size > 48 ? 50 : 60;
        int totalWidth = maxX - minX;
        int totalHeight = maxY - minY;
        float curScale = 1.25f;
        if (size > 48)
        {
            curScale = 1.0f;
        }
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
                startX = minX + ((scaledTotalWidth - curTotalWidth) / 2);
                int curTotalHeight = curRows * curHeight;
                startY = maxY - ((scaledTotalHeight - curTotalHeight) / 2);
                LogDebug(
                    $"Found best fit for {size} items: {rows} rows, {columns} columns, {scale} scale, {startX} startX, {startY} startY");
                _optimalLayouts.Add(size,
                    new Tuple<int, int, float, int, int>(rows, columns, scale, startX, startY));
                PrevScale = curScale;
                return;
            }

            curScale /= 1.05f;
        }
    }
    
    public static float Scale(float size)
    {
        return size * PrevScale;
    }
}