namespace WECCL.Content;

public class AnimationData
{
    private static int numAnimations = 0;
    
    public List<AnimationCommand> AnimationCommands = new();

    public AnimationClip ReceiveAnim = null;

    public class AnimationCommand
    {
        public int Indent;
        
        public int StartFrame;
        public int EndFrame;
        public string Command;
        public string[] Args;
    }
    
    public string Name;
    public MoveType[] Types = Array.Empty<MoveType>();
    public float ForwardSpeedMultiplier = 4f;
    public bool IsGrapple => ReceiveAnim != null;

    public static AnimationData ParseString(string animationData)
    {
        var data = new AnimationData();
        var lines = animationData.Split('\n');
        int i = 0;
        foreach (var line in lines)
        {
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
                                .Select(x => (MoveType)Enum.Parse(typeof(MoveType), x, true)).ToArray();
                            valid = true;
                            break;
                        case "forwardspeedmultiplier":
                            data.ForwardSpeedMultiplier = float.Parse(split2[1].Trim());
                            valid = true;
                            break;
                    }

                    if (valid) continue;
                }

                var command = new AnimationCommand();
                var indent = 0;
                while (line[indent] == ' ') indent++;
                command.Indent = indent;
                var split = line.Trim().Split(' ');
                command.StartFrame = split[0].Contains("-") ? int.Parse(split[0].Split('-')[0]) : int.Parse(split[0]);
                if (split[0].EndsWith("-"))
                {
                    command.EndFrame = int.MaxValue;
                }
                else
                {
                    command.EndFrame = split[0].Contains("-") ? int.Parse(split[0].Split('-')[1]) : int.Parse(split[0]);
                }

                command.Command = split[1];
                command.Args = split.Length > 2 ? split.Skip(2).ToArray() : Array.Empty<string>();
                data.AnimationCommands.Add(command);
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Failed to parse line {i} of animation data {data.Name}: {e}");
                Plugin.Log.LogError($"Line: {line}");
            }
        }
        
        foreach (var type in data.Types)
        {
            var id = numAnimations + 1000000;
            switch (type)
            {
                case MoveType.StrikeHigh:
                    MappedAnims.moveUpper = MappedAnims.moveUpper.Concat(new[] {id}).ToArray();
                    break;
                case MoveType.StrikeLow:
                    MappedAnims.moveLower = MappedAnims.moveLower.Concat(new[] {id}).ToArray();
                    break;
                case MoveType.BigAttack:
                    MappedAnims.moveBigAttack = MappedAnims.moveBigAttack.Concat(new[] {id}).ToArray();
                    break;
                case MoveType.RunningAttack:
                    MappedAnims.moveRunning = MappedAnims.moveRunning.Concat(new[] {id}).ToArray();
                    break;
                case MoveType.FrontGrapple:
                    MappedAnims.moveFront = MappedAnims.moveFront.Concat(new[] {id}).ToArray();
                    break;
                case MoveType.BackGrapple:
                    MappedAnims.moveBack = MappedAnims.moveBack.Concat(new[] {id}).ToArray();
                    break;
            }
        }
        numAnimations++;
        return data;
    }
    
    public static AnimationData ParseFile(string path)
    {
        return ParseString(File.ReadAllText(path));
    }

    public void Play(UnmappedPlayer player, float frame, bool grapple)
    {
        int indent = 0;
        foreach (var command in AnimationCommands)
        {
            if (command.Indent <= indent)
            {
                indent = command.Indent;
                if (frame >= command.StartFrame && frame <= command.EndFrame)
                {
                    var result = ExecuteCommand(player, command.Command, command.Args, frame, command.StartFrame, command.EndFrame, grapple);
                    if (result)
                    {
                        indent++;
                    }
                }
            }
        }
    }

    public bool ExecuteCommand(MappedPlayer player, string command, string[] args, float currentFrame, int startFrame, int endFrame, bool grapple)
    {
        return Animations.ExecuteCommand(player, command, args, currentFrame, startFrame, endFrame, grapple);
    }

    public static void DoCustomAnimation(int anim, MappedPlayer player, float frame, bool grapple)
    {
        CustomAnimationClips[anim].Item2.Play(player, (int)frame, grapple);
    }

    public enum MoveType
    {
        StrikeHigh,
        StrikeLow,
        BigAttack,
        RunningAttack,
        FrontGrapple,
        BackGrapple,
    }
}