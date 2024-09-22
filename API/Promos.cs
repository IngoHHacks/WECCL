namespace HTTCL.API;

public static class Promos
{
    /// <summary>
    /// <para>Use this to register custom promos through code.</para>
    /// </summary>
    /// <param name="data">The Promo Data to register.</param>
    /// <param name="pageHandler">Optional. The page handler to use for this promo for dynamic pages beyond the default feature set.</param>
    /// <returns>The ID of the promo. For non-API functions, add 2000000 to this value.</returns>
    public static int RegisterPromo(PromoData data, Func<int, PromoLine> pageHandler = null)
    {
        CodePromoManager.PromosData.Add(data);
        CodePromoManager.PageHandlers.Add(pageHandler);
        return CodePromoManager.PromosData.Count - 1;
    }

    
    /// <summary>
    /// <para>Assigns a promo to a character.</para>
    /// </summary>
    /// <param name="character">The character to assign the promo to.</param>
    /// <param name="id">The ID of the promo to assign (as returned by RegisterPromo).</param>
    /// <param name="variable">Optional. A variable to pass to the promo.</param>
    /// <param name="risk">The chance of the promo being assigned, determined by generating a random number between 0 and `risk` and checking if it is 0.</param>
    public static void RiskPromo(Character character, int id, int variable = 0, int risk = 0)
    {
        if (id >= PromoData.CODE_PROMO_OFFSET)
        {
            id -= PromoData.CODE_PROMO_OFFSET;
        }
        ((MappedCharacter)character).RiskPromo(id + PromoData.CODE_PROMO_OFFSET, variable, risk);
    }
}