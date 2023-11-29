using Newtonsoft.Json;

namespace WECCL.Content;

public class VanillaCounts
{
    private static VanillaCounts _instance;
    
    [JsonIgnore] public static VanillaCounts Data => _instance ??= new VanillaCounts();
    
    [JsonIgnore] public bool IsInitialized { get; internal set; }

    [JsonProperty] public List<int> MaterialCounts { get; internal set; } = new();

    [JsonProperty] public List<int> FleshCounts { get; internal set; } = new();

    [JsonProperty] public List<int> ShapeCounts { get; internal set; } = new();

    [JsonProperty] public int BodyFemaleCount { get; internal set; } // Negative Flesh[2]

    [JsonProperty] public int FaceFemaleCount { get; internal set; } // Negative Material[3]

    [JsonProperty] public int SpecialFootwearCount { get; internal set; } // Negative Material[14] and [15]

    [JsonProperty] public int TransparentHairMaterialCount { get; internal set; } // Negative Material[17]

    [JsonProperty] public int TransparentHairHairstyleCount { get; internal set; } // Negative Shape[17]

    [JsonProperty] public int KneepadCount { get; internal set; } // Negative Material[24] and [25]

    [JsonProperty] public int MusicCount { get; internal set; }

    [JsonIgnore] public int NoLocations { get; internal set; }
    
    [JsonIgnore] public int NoFeds { get; internal set; }
}