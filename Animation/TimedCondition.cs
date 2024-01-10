namespace WECCL.Animation;

internal class TimedCondition : AnimationEvent
{
    public AnimationCondition Condition { get; set; }
    
    public TimedCondition(int startFrame, int endFrame, AnimationCondition condition, int indent) : base(startFrame, endFrame, indent)
    {
        Condition = condition;
    }
    
    public static TimedCondition FromString(string line)
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
        if (command.EndsWith("?"))
        {
            command = command.Substring(0, command.Length - 1);
        }
        if (!AnimationActions.Conditions.TryGetValue(command, out var condition))
        {
            throw new Exception($"Unknown animation condition: {command}");
        }
        
        TimedCondition timedCondition = new(startFrame, endFrame, condition, indent);
        return timedCondition;
    }
}