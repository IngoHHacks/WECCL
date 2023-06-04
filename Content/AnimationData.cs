namespace WECCL.Content;

public class AnimationData
{
    private static int numAnimations = 0;
    
    public List<AnimationCommand> AnimationCommands = new();

    public class AnimationCommand
    {
        public int Indent;
        
        public int StartFrame;
        public int EndFrame;
        public string Command;
        public string[] Args;
    }
    
    public string Name;
    public MoveType[] Types;

    public static AnimationData ParseString(string animationData)
    {
        var data = new AnimationData();
        var lines = animationData.Split('\n');
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#") || line.Trim().StartsWith("//")) continue;
            if (line.Contains(":"))
            {
                var split2 = line.Split(new[]{ ':' }, 2);
                bool valid = false;
                switch (split2[0].ToLower())
                {
                    case "name":
                        data.Name = split2[1].Trim();
                        valid = true;
                        break;
                    case "types":
                        data.Types = split2[1].Trim().Split(' ').Select(x => (MoveType) Enum.Parse(typeof(MoveType), x, true)).ToArray();
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
        
        foreach (var type in data.Types)
        {
            var id = numAnimations + 1000000;
            switch (type)
            {
                case MoveType.StrikeHigh:
                    NBPIEPNKBDG.PDHKBHKKNDC = NBPIEPNKBDG.PDHKBHKKNDC.Concat(new[] {id}).ToArray();
                    break;
                case MoveType.StrikeLow:
                    NBPIEPNKBDG.EMMDKGJFGOD = NBPIEPNKBDG.EMMDKGJFGOD.Concat(new[] {id}).ToArray();
                    break;
                case MoveType.BigAttack:
                case MoveType.RunningAttack:
                    NBPIEPNKBDG.BKLMJELJJGE = NBPIEPNKBDG.BKLMJELJJGE.Concat(new[] {id}).ToArray();
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

    public void Play(GamePlayer player, float frame)
    {
        int indent = 0;
        foreach (var command in AnimationCommands)
        {
            if (command.Indent <= indent)
            {
                indent = command.Indent;
                if (frame >= command.StartFrame && frame <= command.EndFrame)
                {
                    var result = ExecuteCommand(player, command.Command, command.Args, frame, command.StartFrame, command.EndFrame);
                    if (result)
                    {
                        indent++;
                    }
                }
            }
        }
    }

    public bool ExecuteCommand(GamePlayer player, string command, string[] args, float currentFrame, int startFrame, int endFrame)
    {
        return Animations.ExecuteCommand(player, command, args, currentFrame, startFrame, endFrame);
    }

    public static void DoCustomAnimation(int anim, GMIKIMHFABP player, float frame)
    {
        CustomAnimationClips[anim].Item2.Play(player, (int)frame);
    }

    public enum MoveType
    {
        StrikeHigh,
        StrikeLow,
        BigAttack,
        RunningAttack,
    }
}