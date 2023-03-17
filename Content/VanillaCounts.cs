namespace WECCL.Content;

public class VanillaCounts
{
    public static bool IsInitialized { get; internal set; }
    
    public static List<int> MaterialCounts { get; internal set; } = new();
    
    public static List<int> FleshCounts { get; internal set; } = new();
    
    public static List<int> ShapeCounts { get; internal set; } = new();
    
    public static int BodyFemaleCount { get; internal set; } // Negative Flesh[2]
    
    public static int FaceFemaleCount { get; internal set; } // Negative Material[3]
    
    public static int SpecialFootwearCount { get; internal set; } // Negative Material[14] and [15]
    
    public static int TransparentHairMaterialCount { get; internal set; } // Negative Material[17]
    
    public static int TransparentHairHairstyleCount { get; internal set; } // Negative Shape[17]
    
    public static int KneepadCount { get; internal set; } // Negative Material[24] and [25]
    
    public static int MusicCount { get; internal set; }
}