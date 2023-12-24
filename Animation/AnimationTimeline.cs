namespace WECCL.Animation;

internal class AnimationTimeline
{
    public List<AnimationEvent> Timeline { get; set; } = new();
    
    public AnimationTimeline()
    {
        
    }
    
    public void Add(AnimationEvent animationEvent)
    {
        Timeline.Add(animationEvent);
    }
    
    public void Step(MappedPlayer player, int frame)
    {
        int cndIndent = 0;
        bool cnd = true;
        for (int i = 0; i < Timeline.Count; i++)
        {
            AnimationEvent e = Timeline[i];
            if (e is TimedCondition condition)
            {
                if (condition.IsInRange(frame))
                {
                    cnd = condition.Condition.Condition(player);
                    cndIndent = condition.Indent;
                }
            }
            else if (e is TimedAction action)
            {
                if (!cnd)
                {
                    if (action.Indent > cndIndent)
                    {
                        continue;
                    }
                    cnd = true;
                }
                if (action.IsInRange(frame))
                {
                    action.Action.Execute(player, action.Arguments);
                }
            }
        }
    }
}