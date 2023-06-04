using Random = UnityEngine.Random;

namespace WECCL.Content;

public static class Animations
{
    public static void DoCustomAnimation(GamePlayer player, int animationId)
    {
        if (animationId < 1000000) return;
        var anim = animationId - 1000000;
        AnimationData.DoCustomAnimation(anim, player, player.CLNAALEKPCG);
    }

    public static void StartAnimation(this GamePlayer player, float speed, int buildupFrames, float forwardMomentum = 0f)
    {
        player.LANNIOEGOPD(forwardMomentum, speed, buildupFrames); // negative speed is absolute, positive adds randomness
    }
    public static void SetAnimation(this GamePlayer player, int file, float frame, float speed = -1)
    {
        player.NGLANBOGABN[0] = file;
        player.LLKOAMBPPMJ[0] = frame;
        player.GMIKKEHDFOM[0] = speed == -1 ? player.HPCDKFLHCFH : speed == -2 ? 3f + player.CLNAALEKPCG * 3f : speed;
    }

    public static void EnableHitbox(this GamePlayer player, float distance, float damage, Limb limb, float angle = 0f,
        float particle = 0f, float startFrame = 101f, float endFrame = -1)
    {
        if (endFrame == -1)
        {
            endFrame = 100f + player.HPCDKFLHCFH * 2f;
        }
        if (player.BDOOOPNCGPM(startFrame, endFrame, -1) > 0f && player.AJFLDDJKKFB(9) > 0)
        {
            player.AEPGNFJPAFD(angle, distance, player.CPKMGEBANCB((int)limb), damage, particle);
        }
    }

    public static void PlayAudio(this GamePlayer player, float audio)
    {
        player.PLPKKFIPGCK(audio);
    }

    public static void SetAnimation(this GamePlayer player, params int[] animIds)
    {
        var anim = animIds.Length == 1 ? animIds[0] : animIds[Random.Range(0, animIds.Length)];
        player.CNJJKFKLFEK(anim);
    }
    
    public static bool HitConnected(this GamePlayer player)
    {
        return player.GNMKBNPIOIM[0] != 0;
    }

    public static bool StrengthCheck(this GamePlayer player)
    {
        return Random.Range(24f, player.MGHOMFFCEGK[2]) < 25f;
    }

    public static void ResetAngle(this GamePlayer player, float frame)
    {
        if (player.CLNAALEKPCG >= 101f || player.EMINEEGHAPE > 0f)
        {
            player.ANONONPEEHD(player.MPPIDPBCCEM, frame);
        }
    }

    public static bool ExecuteCommand(GMIKIMHFABP player, string command, string[] args, float currentFrame, int startFrame, int endFrame)
    {
        try
        {
            Plugin.Log.LogDebug($"Executing command {command} with args {string.Join(", ", args)}");
            switch (command.ToLower().Replace("_", ""))
            {
                case "startanimation":
                    player.StartAnimation(float.Parse(args[0]), int.Parse(args[1]), args.Length > 2 ? float.Parse(args[2]) : 0f);
                    return false;
                
                case "setanimation":
                    player.SetAnimation(int.Parse(args[0]), float.Parse(args[1]), args.Length > 2 ? float.Parse(args[2]) : -1f);
                    return false;
                
                case "enablehitbox":
                    player.EnableHitbox(float.Parse(args[0]), float.Parse(args[1]), (Limb)Enum.Parse(typeof(Limb), args[2]), args.Length > 3 ? float.Parse(args[3]) : 0f, args.Length > 4 ? float.Parse(args[4]) : 0f, startFrame, endFrame);
                    return false;
                
                case "playaudio":
                    player.PlayAudio(float.Parse(args[0]));
                    return false;
                
                case "setanimationid":
                    player.SetAnimation(args.Select(int.Parse).ToArray());
                    return false;
                
                case "stopanimation":
                    player.SetAnimation(0);
                    player.ResetAngle(currentFrame - 100);
                    return false;

                case "hitconnected?":
                    return player.HitConnected();
                
                case "strengthcheck?":
                    return player.StrengthCheck();
                
                default:
                    throw new Exception("Unknown command " + command);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error executing command " + command + " with args " + string.Join(",", args), e);
        }
    }
}