namespace WECCL.Updates;

internal abstract class VersionDiff
{
    public List<int> MaterialCountsDiff = new() {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
    
    public List<int> FleshCountsDiff = new() {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
    
    public List<int> ShapeCountsDiff = new() {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};

    public int BodyFemaleCountDiff = 0;
    
    public int FaceFemaleCountDiff = 0;

    public int SpecialFootwearCountDiff = 0;
    
    public int TransparentHairMaterialCountDiff = 0;

    public int TransparentHairHairstyleCountDiff = 0;

    public int KneepadCountDiff = 0;

    public int MusicCountDiff = 0;
}