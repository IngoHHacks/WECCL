using Newtonsoft.Json;

namespace WECCL.Content;

public class VanillaCounts
{
    public static bool IsInitialized { get; internal set; }

    [JsonProperty]
    public static List<int> MaterialCounts { get; internal set; } = new();

    [JsonProperty]
    public static List<int> FleshCounts { get; internal set; } = new();

    [JsonProperty]
    public static List<int> ShapeCounts { get; internal set; } = new();

    [JsonProperty]
    public static int BodyFemaleCount { get; internal set; } // Negative Flesh[2]

    [JsonProperty]
    public static int FaceFemaleCount { get; internal set; } // Negative Material[3]

    [JsonProperty]
    public static int SpecialFootwearCount { get; internal set; } // Negative Material[14] and [15]

    [JsonProperty]
    public static int TransparentHairMaterialCount { get; internal set; } // Negative Material[17]

    [JsonProperty]
    public static int TransparentHairHairstyleCount { get; internal set; } // Negative Shape[17]

    [JsonProperty]
    public static int KneepadCount { get; internal set; } // Negative Material[24] and [25]

    [JsonProperty]
    public static int MusicCount { get; internal set; }
}