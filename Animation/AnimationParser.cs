using WECCL.Content;
using Random = UnityEngine.Random;

namespace WECCL.Animation;

public static class AnimationParser
{
    private static int _numAnimations = 0;
    
    public static readonly Func<string, (string Value, bool Result)> ParseString = value => (value, true);

    public static readonly Func<string, (int? Value, bool Result)> ParseBasicInt = value =>
    {
        return int.TryParse(value, out var result) ? (result, true) : (null, false);
    };
    
    public static readonly Func<string, (int? Value, bool Result)> ParseInt = value =>
    {
        string[] orSplit = value.Split('|');
        int[] values = new int[orSplit.Length];
        for (int i = 0; i < orSplit.Length; i++)
        {
            if (orSplit[i].Contains('-') && !orSplit[i].StartsWith("-"))
            {
                string[] split = orSplit[i].Split('-');
                if (split.Length == 2)
                {
                    if (int.TryParse(split[0], out var min) && int.TryParse(split[1], out var max))
                    {
                        values[i] = Random.Range(min, max + 1);
                    }
                    else
                    {
                        return (null, false);
                    }
                }
                else
                {
                    return (null, false);
                }
            }
            else
            {
                if (int.TryParse(orSplit[i], out var result))
                {
                    values[i] = result;
                }
                else
                {
                    return (null, false);
                }
            }
        }
        return (values[Random.Range(0, values.Length)], true);
    };
    
    public static readonly Func<string, (int? Value, bool Result)> ParseAnimationId = value =>
    {
        if (value == "*") return (100, true);
        return ParseInt(value);
    };
    
    public static readonly Func<string, (float? Value, bool Result)> ParseBasicFloat = value =>
    {
        return float.TryParse(value, out var result) ? (result, true) : (null, false);
    };
    
    public static readonly Func<string, (float? Value, bool Result)> ParseFloat = value =>
    {
        string[] orSplit = value.Split('|');
        float[] values = new float[orSplit.Length];
        for (int i = 0; i < orSplit.Length; i++)
        {
            if (orSplit[i].Contains('-') && !orSplit[i].StartsWith("-"))
            {
                string[] split = orSplit[i].Split('-');
                if (split.Length == 2)
                {
                    if (float.TryParse(split[0], out var min) && float.TryParse(split[1], out var max))
                    {
                        values[i] = Random.Range(min, max);
                    }
                    else
                    {
                        return (null, false);
                    }
                }
                else
                {
                    return (null, false);
                }
            }
            else
            {
                if (float.TryParse(orSplit[i], out var result))
                {
                    values[i] = result;
                }
                else
                {
                    return (null, false);
                }
            }
        }
        return (values[Random.Range(0, values.Length)], true);
    };
    
    public static readonly Func<string, (bool? Value, bool Result)> ParseBool = value =>
    {
        value = value.ToLower();
        if (value == "true" || value == "1" || value == "t" || value == "y" || value == "yes" || value == "on") return (true, true);
        if (value == "false" || value == "0" || value == "f" || value == "n" || value == "no" || value == "off") return (false, true);
        return (null, false);
    };

    public static readonly Func<string, (Limb? Value, bool Result)> ParseLimb = value =>
    {
        return Enum.TryParse(value, true, out Limb limb) ? (limb, true) : (null, false);
    };

    public static AnimationData ReadText(string text)
    {
        AnimationData data = new();
        var lines = text.Replace("\t", "    ").Replace("\r", "").Split('\n');
        int i = 0;
        int indent = 0;
        bool inCondition = false;
        foreach (var _line in lines)
        {
            var line = _line;
            i++;
            try
            {
                if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#") ||
                    line.Trim().StartsWith("//")) continue;
                if (line.Contains(":"))
                {
                    var split2 = line.Split(new[] { ':' }, 2);
                    bool valid = false;
                    switch (split2[0].ToLower())
                    {
                        case "name":
                            data.Name = split2[1].Trim();
                            valid = true;
                            break;
                        case "types":
                            data.Types = split2[1].Trim().Split(' ')
                                .Select(x => (AnimationData.MoveType)Enum.Parse(typeof(AnimationData.MoveType), x, true)).ToList();
                            valid = true;
                            break;
                        case "forwardspeedmultiplier":
                            data.ForwardSpeedMultiplier = float.Parse(split2[1].Trim());
                            valid = true;
                            break;
                        default:
                            LogWarning($"Unknown animation data property: {split2[0]} in animation data {data.Name ?? "Unnamed Animation" ?? "Unnamed Animation"}");
                            break;
                    }

                    if (valid) continue;
                }

                if (line.Trim().EndsWith("?"))
                {
                    var condition = TimedCondition.FromString(line);
                    if (!inCondition && condition.Indent > indent)
                    {
                        LogWarning($"Condition at line {i}: {line} in animation data {data.Name ?? "Unnamed Animation"} is indented, but not inside another condition. Indentation will be ignored.");
                    }
                    else if (inCondition && condition.Indent <= indent)
                    {
                        LogWarning($"Condition at line  {i}: {line} in animation data {data.Name ?? "Unnamed Animation"} is not indented, but is inside another condition. The previous condition will be ignored.");
                    }
                    indent = condition.Indent;
                    inCondition = true;
                    data.Timeline.Add(condition);
                }
                else
                {
                    var action = TimedAction.FromString(line);
                    if (!inCondition && action.Indent > indent)
                    {
                        LogWarning($"Action at line {i}: {line} in animation data {data.Name ?? "Unnamed Animation"} is indented, but not inside a condition. Indentation will be ignored.");
                    }
                    else if (inCondition && action.Indent <= indent)
                    {
                        LogWarning($"Action at line {i}: {line} in animation data {data.Name ?? "Unnamed Animation"} is not indented, but is inside a condition. The previous condition will be ignored.");
                    }
                    indent = action.Indent;
                    inCondition = false;
                    data.Timeline.Add(action);
                }
            }
            catch (Exception e)
            {
                LogError($"Failed to parse line {i}: {line} in animation data {data.Name ?? "Unnamed Animation"}:\n{e}");
            }
        }
        
        foreach (var type in data.Types)
        {
            var id = _numAnimations + 1000000;
            switch (type)
            {
                case AnimationData.MoveType.StrikeHigh:
                    MappedAnims.moveUpper = MappedAnims.moveUpper.Concat(new[] {id}).ToArray();
                    break;
                case AnimationData.MoveType.StrikeLow:
                    MappedAnims.moveLower = MappedAnims.moveLower.Concat(new[] {id}).ToArray();
                    break;
                case AnimationData.MoveType.BigAttack:
                    MappedAnims.moveBigAttack = MappedAnims.moveBigAttack.Concat(new[] {id}).ToArray();
                    break;
                case AnimationData.MoveType.RunningAttack:
                    MappedAnims.moveRunning = MappedAnims.moveRunning.Concat(new[] {id}).ToArray();
                    break;
                case AnimationData.MoveType.FrontGrapple:
                    MappedAnims.moveFront = MappedAnims.moveFront.Concat(new[] {id}).ToArray();
                    break;
                case AnimationData.MoveType.BackGrapple:
                    MappedAnims.moveBack = MappedAnims.moveBack.Concat(new[] {id}).ToArray();
                    break;
            }
        }
        _numAnimations++;
        return data;
    }
    
    
    public static AnimationData ReadFile(string path)
    {
        return ReadText(File.ReadAllText(path));
    }
}