using WECCL.API;

namespace WECCL.Managers;

internal static class CodePromoManager {

    internal static List<PromoData> PromosData = new();
    internal static List<Func<int, PromoLine>> PageHandlers = new();

    internal static PromoLine HandlePage(int id)
    {
        if (id < PageHandlers.Count) {
            if (PageHandlers[id] != null) {
                return PageHandlers[id].Invoke(MappedPromo.page);
            }
            else
            {
                var pd = PromosData[id];
                var page = MappedPromo.page;
                if (page >= pd.PromoLines.Count) {
                    return null;
                }
                return pd.PromoLines[page];
            }
        }
        return null;
    }


    internal static PromoData GetPromoData(int id)
    {
        if (id < PageHandlers.Count) {
            return PromosData[id];
        }
        return null;
    }
}