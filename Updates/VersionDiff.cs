using WECCL.Content;

namespace WECCL.Updates;

internal abstract class VersionDiff
{
    public int BodyFemaleCountDiff = 0;

    public int FaceFemaleCountDiff = 0;

    public List<int> FleshCountsDiff = new()
    {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    };

    public int KneepadCountDiff = 0;

    public List<int> MaterialCountsDiff = new()
    {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    };

    public int MusicCountDiff = 0;

    public List<int> ShapeCountsDiff = new()
    {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    };

    public int SpecialFootwearCountDiff = 0;

    public int TransparentHairHairstyleCountDiff = 0;

    public int TransparentHairMaterialCountDiff = 0;

    public static VersionDiff GetVersionDiff(VanillaCounts contentMapVanillaCounts)
    {
        var vd = new GeneratedVersionDiff();
        vd.MaterialCountsDiff = new List<int>();
        vd.FleshCountsDiff = new List<int>();
        vd.ShapeCountsDiff = new List<int>();
        for (int i = 0; i < 40; i++)
        {
            vd.MaterialCountsDiff.Add(VanillaCounts.Data.MaterialCounts[i] - contentMapVanillaCounts.MaterialCounts[i]);
            vd.FleshCountsDiff.Add(VanillaCounts.Data.FleshCounts[i] - contentMapVanillaCounts.FleshCounts[i]);
            vd.ShapeCountsDiff.Add(VanillaCounts.Data.ShapeCounts[i] - contentMapVanillaCounts.ShapeCounts[i]);
        }
        vd.BodyFemaleCountDiff = VanillaCounts.Data.BodyFemaleCount - contentMapVanillaCounts.BodyFemaleCount;
        vd.FaceFemaleCountDiff = VanillaCounts.Data.FaceFemaleCount - contentMapVanillaCounts.FaceFemaleCount;
        vd.SpecialFootwearCountDiff = VanillaCounts.Data.SpecialFootwearCount - contentMapVanillaCounts.SpecialFootwearCount;
        vd.TransparentHairMaterialCountDiff = VanillaCounts.Data.TransparentHairMaterialCount - contentMapVanillaCounts.TransparentHairMaterialCount;
        vd.TransparentHairHairstyleCountDiff = VanillaCounts.Data.TransparentHairHairstyleCount - contentMapVanillaCounts.TransparentHairHairstyleCount;
        vd.KneepadCountDiff = VanillaCounts.Data.KneepadCount - contentMapVanillaCounts.KneepadCount;
        vd.MusicCountDiff = VanillaCounts.Data.MusicCount - contentMapVanillaCounts.MusicCount;
        return vd;
    }
}