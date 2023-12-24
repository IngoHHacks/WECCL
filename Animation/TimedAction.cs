namespace WECCL.Animation;

internal class TimedAction : AnimationEvent
{
    public AnimationAction Action;
    public ArgumentMap Arguments;
    
    public TimedAction(int startFrame, int endFrame, AnimationAction action, ArgumentMap arguments, int indent) : base(startFrame, endFrame, indent)
    {
        Action = action;
        Arguments = arguments;
    }
    
    public static TimedAction FromString(string line)
    {
        int startFrame, endFrame;
        int indent = 0;
        while (line[indent] == ' ')
        {
            indent++;
        }
        var split = line.Trim().Split(' ');
        try
        {
            startFrame = split[0].Contains("-") ? int.Parse(split[0].Split('-')[0]) : int.Parse(split[0]);
            if (split[0].EndsWith("-"))
            {
                endFrame = int.MaxValue;
            }
            else
            {
                endFrame = split[0].Contains("-") ? int.Parse(split[0].Split('-')[1]) : int.Parse(split[0]);
            }
        }
        catch (Exception)
        {
            throw new Exception($"Failed to parse start/end frames from line: {line}: {split[0]} is not a valid frame range");
        }

        if (split.Length < 2)
        {
            throw new Exception($"Failed to parse animation action from line: {line}: no action specified");
        }

        string command = split[1].Trim().ToLower();
        if (!AnimationActions.Actions.TryGetValue(command, out var action))
        {
            throw new Exception($"Unknown animation action: {command}");
        }
        
        List<string> args = split.Length > 2 ? split.Skip(2).ToList() : new List<string>();
        if (args.Count > action.GetArgumentCount() || args.Count < action.GetRequiredArgumentCount())
        {
            throw new Exception($"Invalid number of arguments for action {action.Name}: expected {(action.GetRequiredArgumentCount() == action.GetArgumentCount() ? action.GetArgumentCount() : action.GetRequiredArgumentCount() + "-" + action.GetArgumentCount())} but got {args.Count}");
        }
        if (!action.ParseArguments(args, out var arguments, out var error))
        {
            throw new Exception($"Failed to parse arguments for action {action.Name}: {error}");
        }
        
        TimedAction timedAction = new(startFrame, endFrame, action, arguments, indent);
        return timedAction;
    }
}