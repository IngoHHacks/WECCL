using Random = UnityEngine.Random;

namespace WECCL.Content;

public static class Animations
{
    public static void DoCustomAnimation(UnmappedPlayer player, int animationId, float fwdSpeedMultiplier = 4f)
    {
        var playerM = (MappedPlayer) player;
        if (animationId < 1000000) return;
        var anim = animationId - 1000000;
        AnimationData.DoCustomAnimation(anim, playerM, playerM.animTim);
        if (playerM.animTim >= 101f || playerM.runUp > 0f)
        {
            playerM.AttackTravel(playerM.a, playerM.runUp * fwdSpeedMultiplier);
        }
    }

    public static void StartAnimation(this MappedPlayer player, float speed, int buildupFrames, float forwardMomentum = 0f)
    {
        player.CockAttack(forwardMomentum, speed, buildupFrames); // negative speed is absolute, positive adds randomness
    }
    public static void SetAnimation(this MappedPlayer player, int file, float frame, float speed = -1)
    {
        player.animFile[0] = file;
        player.frame[0] = frame;
        player.transition[0] = speed == -1 ? player.s : speed == -2 ? 3f + player.animTim * 3f : speed;
    }

    public static void EnableHitbox(this MappedPlayer player, float distance, float damage, Limb limb, float angle = 0f,
        float particle = 0f, float startFrame = 101f, float endFrame = -1)
    {
        if (endFrame == -1)
        {
            endFrame = 100f + player.s * 2f;
        }
        if (player.RangeCurve(startFrame, endFrame, -1) > 0f && player.LimbIntact(9) > 0)
        {
            player.FindImpacts(angle, distance, player.LimbY((int)limb), damage, particle);
        }
    }

    public static void StepSound(this MappedPlayer player, float vol)
    {
        player.StepSound(vol);
    }

    public static void SetAnimation(this MappedPlayer player, params int[] animIds)
    {
        var anim = animIds.Length == 1 ? animIds[0] : animIds[Random.Range(0, animIds.Length)];
        player.ChangeAnim(anim);
    }
    
    public static bool HitConnected(this MappedPlayer player)
    {
        return player.itemSting[0] != 0;
    }

    public static bool StrengthCheck(this MappedPlayer player)
    {
        return Random.Range(24f, player.stat[2]) < 25f;
    }

    public static bool ExecuteCommand(MappedPlayer player, string command, string[] args, float currentFrame, int startFrame, int endFrame)
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
                    StepSound(player, float.Parse(args[0]));
                    return false;
                
                case "setanimationid":
                    player.SetAnimation(args.Select(int.Parse).ToArray());
                    return false;
                
                case "stopanimation":
                    player.SetAnimation(0);
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

    public static void PerformTestAnimation(MappedPlayer player, int animationId, float forwardSpeedMultiplier)
    {
        if (player.pV.runUp > 0f && player.animTim >= 26f && player.animTim < 33f && player.reverse == 0f)
		{
            player.animTim = 33f;
		}
		if (player.animTim == 33f && player.reverse == 0f)
		{
			MappedSound.Emit(player.audio, MappedSound.swing[MappedGlobals.Rnd(1, 3)], -0.2f, -0.5f);
		}
		if (player.animTim >= 19f && player.animTim <= 59f)
		{
            player.DirectMove(180f, 0.05f);
		}
		if (player.animTim >= 33f && player.animTim <= 59f)
		{
            player.MoveTravel(player.moveA + 180f, 0.2f, 1f);
		}
		if (player.pV.runUp != 0f)
		{
            player.MoveTravel(player.moveA + 180f, 0.1f, 1f);
		}
		if (player.animTim >= 33f && player.animTim <= 39f)
		{
            player.fileA = 27;
            player.frameA = 45f;
            player.t = 4f;
            player.SyncMove(player.pV, 25f, 4.6f, 315f, 200f, player.t);
		}
		if (player.animTim >= 40f && player.animTim <= 44f)
		{
            player.fileA = 27;
            player.frameA = 46f;
            player.t = 3f;
            player.SyncMove(player.pV, 315f, 2.1f, 250f, 200f, player.t);
		}
		if (player.animTim >= 45f && player.animTim <= 49f)
		{
            player.fileA = 27;
            player.frameA = 47f;
            player.t = 3f;
            player.SyncMove(player.pV, 350f, 4.2f, 180f, 170f, player.t);
		}
		if (player.animTim >= 33f && player.animTim <= 49f)
		{
            player.RiskCounter(player.pV, 1, 281, 31f, player.a + 180f);
		}
		if (player.animTim >= 33f && player.animTim <= 49f)
		{
            player.BreakMove(2, 2);
		}
		if (player.animTim >= 50f && player.animTim <= 54f)
		{
            player.fileA = 27;
            player.frameA = 48f;
            player.t = 3f;
            player.SyncMove(player.pV, 15f, 6.3f, 135f, 175f, player.t);
		}
		if (player.animTim >= 33f && player.animTim <= 54f)
		{
            player.pV.offsetTY += (player.scale - player.pV.scale) * 10f;
		}
		if (player.animTim >= 38f && player.animTim <= 62f && player.v > 0)
		{
            player.pinning = 1;
            player.pV.pinning = 1;
            player.pV.FindSmashes(player.pV.LimbY(1), 1f, 1f, 0, 1f);
		}
		if (player.animTim >= 55f && player.animTim <= 59f)
		{
            player.fileA = 27;
            player.frameA = 49f;
            player.t = 3f;
            player.SyncMove(player.pV, 20f, 5f, 90f, 180f, player.t);
		}
		if (player.animTim == 59f)
		{
            player.MoveImpact(1, 7f, 2f);
            player.Bounce(-0.4f);
            player.pV.Bounce(-0.4f);
		}
		if (player.animTim >= 60f)
		{
            player.fileA = 27;
            player.frameA = 50f;
            player.t = 3.5f;
            player.MoveTravel(player.moveA + 180f, 0.1f, 1f);
		}
		if (player.animTim >= 65f)
		{
            player.ChangeAnim(994);
            player.pV.SellBackFall();
			player.LoseGrapple();
		}
    }

    public static void PerformPostGrappleCode(MappedPlayer player)
    {
        if (player.grappling > 0 && player.reverse != 0f)
        {
            player.animTim -= 2f * UnmappedAnims.PGMACJDMLPP;
            player.t += 2f;
            if (player.animTim <= player.reverse || (player.reverse < 0f && player.animTim <= 0f))
            {
                player.animTim = Mathf.Abs(player.reverse);
                if (player.reverse <= 0f)
                {
                    if (player.pV.Facing(player.x, player.z) < 90f)
                    {
                        player.pV.ExecuteMove(player, 201, -5f);
                    }
                    else
                    {
                        player.ExecuteMove(player.pV, 301, -5f);
                    }
                }
                player.reverse = 0f;
            }
        }
        if ((float)player.fileA + player.frameA > 0f)
        {
            player.animFile[0] = player.fileA;
            player.frame[0] = player.frameA;
            player.transition[0] = player.t;
            if (player.fileB == 0 && player.frameB == 0f)
            {
                player.fileB = player.fileA + 1;
                player.frameB = player.frameA;
            }
            if (player.grappling > 0 && player.pV != null)
            {
                player.pV.animFile[0] = player.fileB;
                player.pV.frame[0] = player.frameB;
                player.pV.transition[0] = player.t;
            }
            if (player.anim >= 550 && player.anim < 600 && player.assist > 0)
            {
                player.pAssist.animFile[0] = player.fileA + 2;
                player.pAssist.frame[0] = player.frameA;
                player.pAssist.transition[0] = player.t;
            }
        }
        player.charData.TrainStat(MappedGlobals.RndOr(4, MappedGlobals.Rnd(3, 5)), 0.15f);
    }
}