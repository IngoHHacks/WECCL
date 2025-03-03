namespace WECCL.API.Types;

public class AdvFeatures
{
    public enum CommandType
    {
        None,
        SetFace,
        SetHeel,
        SetRealEnemy,
        SetStoryEnemy,
        SetRealFriend,
        SetStoryFriend,
        SetRealNeutral,
        SetStoryNeutral,
        PlayAudio
    }

    public CommandType Command { get; set; } = CommandType.None;
    public List<string> Args { get; set; } = new();

    public bool SetCommand(string cmd)
    {
        if (Enum.TryParse(cmd, true, out CommandType command))
        {
            this.Command = command;
            return true;
        }

        return false;
    }

    public bool IsValidArgumentCount(out int expected)
    {
        if (this.Command == CommandType.None)
        {
            expected = -1;
            return true;
        }

        if (this.Command is CommandType.PlayAudio || this.Command is CommandType.SetFace || this.Command is CommandType.SetHeel)
        {
            expected = 1;
            return this.Args.Count == 1;
        }

        expected = 2;
        return this.Args.Count == 2;
    }
}