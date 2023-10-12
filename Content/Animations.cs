using Random = UnityEngine.Random;

namespace WECCL.Content;

public static class Animations
{
    public static void DoCustomAnimation(GamePlayer player, int animationId, float fwdSpeedMultiplier = 4f)
    {
        if (animationId < 1000000) return;
        var anim = animationId - 1000000;
        AnimationData.DoCustomAnimation(anim, player, player.LPACFDIEDJO);
        if (player.LPACFDIEDJO >= 101f || player.NHNHHJCCJKF > 0f)
        {
            player.LJIOJPKMCDI(player.PDEGHHLLFDG, player.JJKOMLILIEF * fwdSpeedMultiplier);
        }
    }

    public static void StartAnimation(this GamePlayer player, float speed, int buildupFrames, float forwardMomentum = 0f)
    {
        player.MMDGPJPPHBF(forwardMomentum, speed, buildupFrames); // negative speed is absolute, positive adds randomness
    }
    public static void SetAnimation(this GamePlayer player, int file, float frame, float speed = -1)
    {
        player.EBICMNLGAKI[0] = file;
        player.PDBHIFCJDDG[0] = frame;
        player.HDPHBCBFHPK[0] = speed == -1 ? player.JJKOMLILIEF : speed == -2 ? 3f + player.LPACFDIEDJO * 3f : speed;
    }

    public static void EnableHitbox(this GamePlayer player, float distance, float damage, Limb limb, float angle = 0f,
        float particle = 0f, float startFrame = 101f, float endFrame = -1)
    {
        if (endFrame == -1)
        {
            endFrame = 100f + player.JJKOMLILIEF * 2f;
        }
        if (player.DPAKIEHNCML(startFrame, endFrame, -1) > 0f && player.HDCLENBFPJK(9) > 0)
        {
            player.OEAGPIPAMKL(angle, distance, player.CODHLHPJMGJ((int)limb), damage, particle);
        }
    }

    public static void PlayAudio(this GamePlayer player, float audio)
    {
        player.OLBEMKGPKFO(audio);
    }

    public static void SetAnimation(this GamePlayer player, params int[] animIds)
    {
        var anim = animIds.Length == 1 ? animIds[0] : animIds[Random.Range(0, animIds.Length)];
        player.DNEBIMMJJIC(anim);
    }
    
    public static bool HitConnected(this GamePlayer player)
    {
        return player.JIGBJIIOMJF[0] != 0;
    }

    public static bool StrengthCheck(this GamePlayer player)
    {
        return Random.Range(24f, player.IDLOAACGLKA[2]) < 25f;
    }

    public static bool ExecuteCommand(DJEKCMMMFJM player, string command, string[] args, float currentFrame, int startFrame, int endFrame)
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

    public static void PerformTestAnimation(GamePlayer player, int animationId, float forwardSpeedMultiplier)
    {
        if (player.IOIOGPEJHKC.NHNHHJCCJKF > 0f && player.LPACFDIEDJO >= 26f && player.LPACFDIEDJO < 33f && player.IKBLMKACKEI == 0f)
		{
            player.LPACFDIEDJO = 33f;
		}
		if (player.LPACFDIEDJO == 33f && player.IKBLMKACKEI == 0f)
		{
			IKPECOJMCAB.OBLNONIKENE(player.MLDLMDCFHOM, IKPECOJMCAB.LDMFFNNHNEE[LFNJDEGJLLJ.NBNFJOFFMHO(1, 3)], -0.2f, -0.5f);
		}
		if (player.LPACFDIEDJO >= 19f && player.LPACFDIEDJO <= 59f)
		{
            player.EELOGOGIKHB(180f, 0.05f);
		}
		if (player.LPACFDIEDJO >= 33f && player.LPACFDIEDJO <= 59f)
		{
            player.BBNBEEFPCAN(player.OPCNCMKFPAN + 180f, 0.2f, 1f);
		}
		if (player.IOIOGPEJHKC.NHNHHJCCJKF != 0f)
		{
            player.BBNBEEFPCAN(player.OPCNCMKFPAN + 180f, 0.1f, 1f);
		}
		if (player.LPACFDIEDJO >= 33f && player.LPACFDIEDJO <= 39f)
		{
            player.AKLPBFADAMB = 27;
            player.NMJNOMIGHPF = 45f;
            player.FFJICLMPCBB = 4f;
            player.HEFPEOHGLDG(player.IOIOGPEJHKC, 25f, 4.6f, 315f, 200f, player.FFJICLMPCBB);
		}
		if (player.LPACFDIEDJO >= 40f && player.LPACFDIEDJO <= 44f)
		{
            player.AKLPBFADAMB = 27;
            player.NMJNOMIGHPF = 46f;
            player.FFJICLMPCBB = 3f;
            player.HEFPEOHGLDG(player.IOIOGPEJHKC, 315f, 2.1f, 250f, 200f, player.FFJICLMPCBB);
		}
		if (player.LPACFDIEDJO >= 45f && player.LPACFDIEDJO <= 49f)
		{
            player.AKLPBFADAMB = 27;
            player.NMJNOMIGHPF = 47f;
            player.FFJICLMPCBB = 3f;
            player.HEFPEOHGLDG(player.IOIOGPEJHKC, 350f, 4.2f, 180f, 170f, player.FFJICLMPCBB);
		}
		if (player.LPACFDIEDJO >= 33f && player.LPACFDIEDJO <= 49f)
		{
            player.LCCEECNNBDN(player.IOIOGPEJHKC, 1, 281, 31f, player.PDEGHHLLFDG + 180f);
		}
		if (player.LPACFDIEDJO >= 33f && player.LPACFDIEDJO <= 49f)
		{
            player.PHAHIJLKDOP(2, 2);
		}
		if (player.LPACFDIEDJO >= 50f && player.LPACFDIEDJO <= 54f)
		{
            player.AKLPBFADAMB = 27;
            player.NMJNOMIGHPF = 48f;
            player.FFJICLMPCBB = 3f;
            player.HEFPEOHGLDG(player.IOIOGPEJHKC, 15f, 6.3f, 135f, 175f, player.FFJICLMPCBB);
		}
		if (player.LPACFDIEDJO >= 33f && player.LPACFDIEDJO <= 54f)
		{
            player.IOIOGPEJHKC.AMEBGMAOFGG += (player.IIMBIPKKEKP - player.IOIOGPEJHKC.IIMBIPKKEKP) * 10f;
		}
		if (player.LPACFDIEDJO >= 38f && player.LPACFDIEDJO <= 62f && player.JCEJPDHEDFD > 0)
		{
            player.HBLBDHPNOGH = 1;
            player.IOIOGPEJHKC.HBLBDHPNOGH = 1;
            player.IOIOGPEJHKC.CBBMPOCIBHK(player.IOIOGPEJHKC.CODHLHPJMGJ(1), 1f, 1f, 0, 1f);
		}
		if (player.LPACFDIEDJO >= 55f && player.LPACFDIEDJO <= 59f)
		{
            player.AKLPBFADAMB = 27;
            player.NMJNOMIGHPF = 49f;
            player.FFJICLMPCBB = 3f;
            player.HEFPEOHGLDG(player.IOIOGPEJHKC, 20f, 5f, 90f, 180f, player.FFJICLMPCBB);
		}
		if (player.LPACFDIEDJO == 59f)
		{
            player.PNLALIIPIDJ(1, 7f, 2f);
            player.IIEBONLLGAF(-0.4f);
            player.IOIOGPEJHKC.IIEBONLLGAF(-0.4f);
		}
		if (player.LPACFDIEDJO >= 60f)
		{
            player.AKLPBFADAMB = 27;
            player.NMJNOMIGHPF = 50f;
            player.FFJICLMPCBB = 3.5f;
            player.BBNBEEFPCAN(player.OPCNCMKFPAN + 180f, 0.1f, 1f);
		}
		if (player.LPACFDIEDJO >= 65f)
		{
            player.DNEBIMMJJIC(994);
            player.IOIOGPEJHKC.CCAHBNAHHOC();
			player.HLLPKADOMEJ();
		}
    }

    public static void PerformPostGrappleCode(DJEKCMMMFJM player)
    {
        if (player.GPGFNAKDOBC > 0 && player.IKBLMKACKEI != 0f)
        {
            player.LPACFDIEDJO -= 2f * BCKHHMIMAEN.PGMACJDMLPP;
            player.FFJICLMPCBB += 2f;
            if (player.LPACFDIEDJO <= player.IKBLMKACKEI || (player.IKBLMKACKEI < 0f && player.LPACFDIEDJO <= 0f))
            {
                player.LPACFDIEDJO = Mathf.Abs(player.IKBLMKACKEI);
                if (player.IKBLMKACKEI <= 0f)
                {
                    if (player.IOIOGPEJHKC.NEICJKCJPJO(player.DCLLKPILCBP, player.FFEONFCEHDF) < 90f)
                    {
                        player.IOIOGPEJHKC.JGIKHFDDBOP(player, 201, -5f);
                    }
                    else
                    {
                        player.JGIKHFDDBOP(player.IOIOGPEJHKC, 301, -5f);
                    }
                }
                player.IKBLMKACKEI = 0f;
            }
        }
        if ((float)player.AKLPBFADAMB + player.NMJNOMIGHPF > 0f)
        {
            player.EBICMNLGAKI[0] = player.AKLPBFADAMB;
            player.PDBHIFCJDDG[0] = player.NMJNOMIGHPF;
            player.HDPHBCBFHPK[0] = player.FFJICLMPCBB;
            if (player.JGKCHHHIDFE == 0 && player.DPIMBMHANLC == 0f)
            {
                player.JGKCHHHIDFE = player.AKLPBFADAMB + 1;
                player.DPIMBMHANLC = player.NMJNOMIGHPF;
            }
            if (player.GPGFNAKDOBC > 0 && player.IOIOGPEJHKC != null)
            {
                player.IOIOGPEJHKC.EBICMNLGAKI[0] = player.JGKCHHHIDFE;
                player.IOIOGPEJHKC.PDBHIFCJDDG[0] = player.DPIMBMHANLC;
                player.IOIOGPEJHKC.HDPHBCBFHPK[0] = player.FFJICLMPCBB;
            }
            if (player.JGPFJIBNLFC >= 550 && player.JGPFJIBNLFC < 600 && player.FPIEAAKPCND > 0)
            {
                player.GKGCGLMLHLL.EBICMNLGAKI[0] = player.AKLPBFADAMB + 2;
                player.GKGCGLMLHLL.PDBHIFCJDDG[0] = player.NMJNOMIGHPF;
                player.GKGCGLMLHLL.HDPHBCBFHPK[0] = player.FFJICLMPCBB;
            }
        }
        player.LLEGGMCIALJ.GKEJDFNGEDG(LFNJDEGJLLJ.ICJAEBKBCNH(4, LFNJDEGJLLJ.NBNFJOFFMHO(3, 5)), 0.15f);
    }
}