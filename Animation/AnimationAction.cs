namespace WECCL.Animation;

public class AnimationAction
{
    public string Name { get; set; }
    
    List<IAnimationArgument> Arguments { get; set; } = new();
    
    public Action<MappedPlayer, ArgumentMap> Action { get; set; }
    
    public AnimationAction(string name, Action<MappedPlayer, ArgumentMap> action, params IAnimationArgument[] arguments)
    {
        Name = name;
        Action = action;
        Arguments.AddRange(arguments);
    }
    
    public void Execute(MappedPlayer player, ArgumentMap arguments)
    {
        Action(player, arguments);
    }
    
    public int GetArgumentCount()
    {
        return Arguments.Count;
    }
    
    public int GetRequiredArgumentCount()
    {
        int count = 0;
        foreach (IAnimationArgument arg in Arguments)
        {
            if (arg.Required)
            {
                count++;
            }
        }
        return count;
    }

    public bool ParseArguments(List<string> args, out ArgumentMap o, out string error)
    {
        o = new ArgumentMap();
        error = "";
        if (args.Count > GetArgumentCount() || args.Count < GetRequiredArgumentCount())
        {
            error = $"Invalid number of arguments for action {Name}: expected {(GetRequiredArgumentCount() == GetArgumentCount() ? GetArgumentCount() : GetRequiredArgumentCount() + "-" + GetArgumentCount())} but got {args.Count}";
            return false;
        }
        for (int i = 0; i < args.Count; i++)
        {
            if (!Arguments[i].Validate(args[i].Trim()))
            {
                error = $"Argument {i + 1} ({Arguments[i].Name}) cannot be parsed from {args[i].Trim()}";
                return false;
            }
            o.Add(Arguments[i].Name, args[i].Trim());
        }
        for (int i = args.Count; i < Arguments.Count; i++)
        {
            if (Arguments[i].Required)
            {
                error = $"Argument {i + 1} ({Arguments[i].Name}) is required but was not provided";
                return false;
            }
            o.Add(Arguments[i].Name, Arguments[i].DefaultValue);
        }
        return true;
    }
}