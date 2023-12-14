namespace WECCL.Animation;

public class AnimationCondition
{
    public string Name { get; set; }
    
    List<AnimationArgument<object>> Arguments { get; set; } = new();
    
    public Func<MappedPlayer, bool> Condition { get; set; }
    
    public AnimationCondition(string name, Func<MappedPlayer, bool> condition)
    {
        Name = name;
        Condition = condition;
    }
}