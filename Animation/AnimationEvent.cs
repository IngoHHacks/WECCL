namespace WECCL.Animation;

public abstract class AnimationEvent
{
    public int StartFrame;
    public int EndFrame;
    public int Indent = 0;
    
    public AnimationEvent(int startFrame, int endFrame, int indent)
    {
        StartFrame = startFrame;
        EndFrame = endFrame;
        Indent = indent;
    }
    
    public bool IsInRange(int frame)
    {
        return frame >= StartFrame && frame <= EndFrame;
    }
}