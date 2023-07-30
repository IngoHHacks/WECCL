namespace WECCL.Updates;

internal class VersionDiffGroup : VersionDiff
{
    public VersionDiffGroup(params VersionDiff[] diffs)
    {
        foreach (VersionDiff diff in diffs)
        {
            this.MaterialCountsDiff = this.MaterialCountsDiff.Zip(diff.MaterialCountsDiff, (a, b) => a + b).ToList();
            this.FleshCountsDiff = this.FleshCountsDiff.Zip(diff.FleshCountsDiff, (a, b) => a + b).ToList();
            this.ShapeCountsDiff = this.ShapeCountsDiff.Zip(diff.ShapeCountsDiff, (a, b) => a + b).ToList();
            this.BodyFemaleCountDiff += diff.BodyFemaleCountDiff;
            this.FaceFemaleCountDiff += diff.FaceFemaleCountDiff;
            this.SpecialFootwearCountDiff += diff.SpecialFootwearCountDiff;
            this.TransparentHairMaterialCountDiff += diff.TransparentHairMaterialCountDiff;
            this.TransparentHairHairstyleCountDiff += diff.TransparentHairHairstyleCountDiff;
            this.KneepadCountDiff += diff.KneepadCountDiff;
            this.MusicCountDiff += diff.MusicCountDiff;
        }
    }
}