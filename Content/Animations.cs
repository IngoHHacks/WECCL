using Random = UnityEngine.Random;

namespace WECCL.Content;

public static class Animations
{
    public static void DoCustomAnimation(GamePlayer player, int animationId, float fwdSpeedMultiplier = 4f)
    {
        if (animationId < 1000000) return;
        var anim = animationId - 1000000;
        AnimationData.DoCustomAnimation(anim, player, player.CLNAALEKPCG);
        if (player.CLNAALEKPCG >= 101f || player.EMINEEGHAPE > 0f)
        {
            player.ANONONPEEHD(player.MPPIDPBCCEM, player.HPCDKFLHCFH * fwdSpeedMultiplier);
        }
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

    public static void PerformTestAnimation(GMIKIMHFABP player, int animationId, float forwardSpeedMultiplier)
    {
        if (player.NFFKFPPBPID.EMINEEGHAPE > 0f && player.CLNAALEKPCG >= 26f && player.CLNAALEKPCG < 33f && player.JNAIOECFIIA == 0f)
		{
            player.CLNAALEKPCG = 33f;
		}
		if (player.CLNAALEKPCG == 33f && player.JNAIOECFIIA == 0f)
		{
			JKPIHABGBGP.BDFEDMELBOL(player.BJANPGIFKHD, JKPIHABGBGP.KIIHJEICAAB[JGKBBDPDIBC.OCMIPAODMHH(1, 3)], -0.2f, -0.5f);
		}
		if (player.CLNAALEKPCG >= 19f && player.CLNAALEKPCG <= 59f)
		{
            player.CBMEOIGPDAE(180f, 0.05f);
		}
		if (player.CLNAALEKPCG >= 33f && player.CLNAALEKPCG <= 59f)
		{
            player.IFEOLAOAIDC(player.ONPIHJLEBOI + 180f, 0.2f, 1f);
		}
		if (player.NFFKFPPBPID.EMINEEGHAPE != 0f)
		{
            player.IFEOLAOAIDC(player.ONPIHJLEBOI + 180f, 0.1f, 1f);
		}
		if (player.CLNAALEKPCG >= 33f && player.CLNAALEKPCG <= 39f)
		{
            player.CMMMEGLFLNJ = 27;
            player.OJHKNFHFOCO = 45f;
            player.AEBLPLJCLMD = 4f;
            player.DGLNEFGGPEN(player.NFFKFPPBPID, 25f, 4.6f, 315f, 200f, player.AEBLPLJCLMD);
		}
		if (player.CLNAALEKPCG >= 40f && player.CLNAALEKPCG <= 44f)
		{
            player.CMMMEGLFLNJ = 27;
            player.OJHKNFHFOCO = 46f;
            player.AEBLPLJCLMD = 3f;
            player.DGLNEFGGPEN(player.NFFKFPPBPID, 315f, 2.1f, 250f, 200f, player.AEBLPLJCLMD);
		}
		if (player.CLNAALEKPCG >= 45f && player.CLNAALEKPCG <= 49f)
		{
            player.CMMMEGLFLNJ = 27;
            player.OJHKNFHFOCO = 47f;
            player.AEBLPLJCLMD = 3f;
            player.DGLNEFGGPEN(player.NFFKFPPBPID, 350f, 4.2f, 180f, 170f, player.AEBLPLJCLMD);
		}
		if (player.CLNAALEKPCG >= 33f && player.CLNAALEKPCG <= 49f)
		{
            player.NOPKNLGDEBG(player.NFFKFPPBPID, 1, 281, 31f, player.MPPIDPBCCEM + 180f);
		}
		if (player.CLNAALEKPCG >= 33f && player.CLNAALEKPCG <= 49f)
		{
            player.GDDODFHDDOH(2, 2);
		}
		if (player.CLNAALEKPCG >= 50f && player.CLNAALEKPCG <= 54f)
		{
            player.CMMMEGLFLNJ = 27;
            player.OJHKNFHFOCO = 48f;
            player.AEBLPLJCLMD = 3f;
            player.DGLNEFGGPEN(player.NFFKFPPBPID, 15f, 6.3f, 135f, 175f, player.AEBLPLJCLMD);
		}
		if (player.CLNAALEKPCG >= 33f && player.CLNAALEKPCG <= 54f)
		{
            player.NFFKFPPBPID.MIPHFHABEHK += (player.MHDPPBHFPPK - player.NFFKFPPBPID.MHDPPBHFPPK) * 10f;
		}
		if (player.CLNAALEKPCG >= 38f && player.CLNAALEKPCG <= 62f && player.EPBLHHBCDEA > 0)
		{
            player.LPMJNLEKEKB = 1;
            player.NFFKFPPBPID.LPMJNLEKEKB = 1;
            player.NFFKFPPBPID.HDDGHJGCEHL(player.NFFKFPPBPID.CPKMGEBANCB(1), 1f, 1f, 0, 1f);
		}
		if (player.CLNAALEKPCG >= 55f && player.CLNAALEKPCG <= 59f)
		{
            player.CMMMEGLFLNJ = 27;
            player.OJHKNFHFOCO = 49f;
            player.AEBLPLJCLMD = 3f;
            player.DGLNEFGGPEN(player.NFFKFPPBPID, 20f, 5f, 90f, 180f, player.AEBLPLJCLMD);
		}
		if (player.CLNAALEKPCG == 59f)
		{
            player.JFNHONJLFEO(1, 7f, 2f);
            player.DNOAEFLJMJH(-0.4f);
            player.NFFKFPPBPID.DNOAEFLJMJH(-0.4f);
		}
		if (player.CLNAALEKPCG >= 60f)
		{
            player.CMMMEGLFLNJ = 27;
            player.OJHKNFHFOCO = 50f;
            player.AEBLPLJCLMD = 3.5f;
            player.IFEOLAOAIDC(player.ONPIHJLEBOI + 180f, 0.1f, 1f);
		}
		if (player.CLNAALEKPCG >= 65f)
		{
            player.CNJJKFKLFEK(994);
            player.NFFKFPPBPID.EBLJNJNOGDO();
			player.IJCPFBLMBJA();
		}
    }

    public static void PerformPostGrappleCode(GMIKIMHFABP player)
    {
        if (player.OMBLNOJKAFN > 0 && player.JNAIOECFIIA != 0f)
        {
            player.CLNAALEKPCG -= 2f * NBPIEPNKBDG.GCFFPILNJCB;
            player.AEBLPLJCLMD += 2f;
            if (player.CLNAALEKPCG <= player.JNAIOECFIIA || (player.JNAIOECFIIA < 0f && player.CLNAALEKPCG <= 0f))
            {
                player.CLNAALEKPCG = Mathf.Abs(player.JNAIOECFIIA);
                if (player.JNAIOECFIIA <= 0f)
                {
                    if (player.NFFKFPPBPID.IKDCADDELIH(player.PPFFBIPHOEE, player.OIHBMKLFEBJ) < 90f)
                    {
                        player.NFFKFPPBPID.IBHLENAAGNJ(player, 201, -5f);
                    }
                    else
                    {
                        player.IBHLENAAGNJ(player.NFFKFPPBPID, 301, -5f);
                    }
                }
                player.JNAIOECFIIA = 0f;
            }
        }
        if ((float)player.CMMMEGLFLNJ + player.OJHKNFHFOCO > 0f)
        {
            player.NGLANBOGABN[0] = player.CMMMEGLFLNJ;
            player.LLKOAMBPPMJ[0] = player.OJHKNFHFOCO;
            player.GMIKKEHDFOM[0] = player.AEBLPLJCLMD;
            if (player.MPNLGGMEJOC == 0 && player.CLGGNCLADPG == 0f)
            {
                player.MPNLGGMEJOC = player.CMMMEGLFLNJ + 1;
                player.CLGGNCLADPG = player.OJHKNFHFOCO;
            }
            if (player.OMBLNOJKAFN > 0 && player.NFFKFPPBPID != null)
            {
                player.NFFKFPPBPID.NGLANBOGABN[0] = player.MPNLGGMEJOC;
                player.NFFKFPPBPID.LLKOAMBPPMJ[0] = player.CLGGNCLADPG;
                player.NFFKFPPBPID.GMIKKEHDFOM[0] = player.AEBLPLJCLMD;
            }
            if (player.MDOCJJELCBG >= 550 && player.MDOCJJELCBG < 600 && player.FPBAFENCCHN > 0)
            {
                player.PDIDJMBAMPJ.NGLANBOGABN[0] = player.CMMMEGLFLNJ + 2;
                player.PDIDJMBAMPJ.LLKOAMBPPMJ[0] = player.OJHKNFHFOCO;
                player.PDIDJMBAMPJ.GMIKKEHDFOM[0] = player.AEBLPLJCLMD;
            }
        }
        player.IPNKFGHIDJP.KAEBPNEJNMC(JGKBBDPDIBC.JOGCPNBFFNJ(4, JGKBBDPDIBC.OCMIPAODMHH(3, 5)), 0.15f);
    }
}