namespace WECCL.Animation;

internal class AnimationData
{
    public string Name { get; set; }
    public List<MoveType> Types { get; set; } = new();
    public float ForwardSpeedMultiplier { get; set; } = 4f;
    public AnimationTimeline Timeline { get; set; } = new();
    
    public AnimationClip Anim = null;
    public AnimationClip ReceiveAnim = null;
    
    public bool IsGrapple => ReceiveAnim != null;
    
    
    public enum MoveType
    {
        StrikeHigh,
        StrikeLow,
        BigAttack,
        RunningAttack,
        FrontGrapple,
        BackGrapple,
    }
    
    public static void AddAnimation(AnimationData anim)
    {
        CustomAnimations.Add(anim);
    }

    public static void DoCustomAnimation(int anim, MappedPlayer p, float frame)
    {
        if (anim < 0 || anim >= CustomAnimations.Count)
        {
            throw new ArgumentException($"Animation #{anim} does not exist.");
        }
        CustomAnimations[anim].Timeline.Step(p, (int)frame);
    }
}