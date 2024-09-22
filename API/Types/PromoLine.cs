namespace WECCL.API.Types;

public class PromoLine
{
    public string Line1 { get; set; } = "Line 1";
    public string Line2 { get; set; } = "Line 2";

    public int From { get; set; } = 1;
    public int To { get; set; } = 2;

    public string FromName { get; set; } = "";
    public string ToName { get; set; } = "";

    public float Demeanor { get; set; }
    public int TauntAnim { get; set; }

    public List<AdvFeatures> Features { get; set; } = new();
}