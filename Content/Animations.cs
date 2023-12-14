using WECCL.Animation;
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
        if (playerM.animTim >= 101f || playerM.runUp > 0f && playerM.grappling == 0)
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
    
    public static void SetGrappleAnimation(this MappedPlayer player, int file, float frame, float speed = -1)
    {
        player.fileA = file;
        player.frameA = frame;
        player.fileB = file + 1;
        player.frameB = frame;
        player.t = speed == -1 ? player.s : speed == -2 ? 3f + player.animTim * 3f : speed;
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

    public static void SetAnimationId(this MappedPlayer player, int animId, float startTim = 0f)
    {
        player.ChangeAnim(animId, startTim);
    }
    
    public static bool HitConnected(this MappedPlayer player)
    {
        return player.itemSting[0] != 0;
    }

    public static bool StrengthCheck(this MappedPlayer player)
    {
        return Random.Range(24f, player.stat[2]) < 25f;
    }

    public static bool ExecuteCommand(MappedPlayer player, string command, string[] args, float currentFrame, int startFrame, int endFrame, bool grapple)
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
                    if (args[0] == "*")
                    {
                        args[0] = "100";
                    }
                    if (grapple)
                    {
                        player.SetGrappleAnimation(int.Parse(args[0]), float.Parse(args[1]),
                            args.Length > 2 ? float.Parse(args[2]) : -1f);
                    }
                    else
                    {
                        player.SetAnimation(int.Parse(args[0]), float.Parse(args[1]),
                            args.Length > 2 ? float.Parse(args[2]) : -1f);
                    }
                    return false;
                
                case "setoppanimation":
                    player.pV.SetAnimation(int.Parse(args[0]), float.Parse(args[1]), args.Length > 2 ? float.Parse(args[2]) : -1f);
                    return false;
                
                case "enablehitbox":
                    player.EnableHitbox(float.Parse(args[0]), float.Parse(args[1]), (Limb)Enum.Parse(typeof(Limb), args[2]), args.Length > 3 ? float.Parse(args[3]) : 0f, args.Length > 4 ? float.Parse(args[4]) : 0f, startFrame, endFrame);
                    return false;
                
                case "moveattack":
                    player.MoveAttack(player.pV, int.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
                    return false;
                
                case "dealdamage":
                    float pain = float.Parse(args[0]);
                    pain *= player.stat[2] / 100f;
                    if (player.special > 0f)
                    {
                        pain *= 1.25f;
                    }
                    if (pain > 300f && player.special >= 5f)
                    {
                        player.special /= 5f;
                    }
                    player.pV.health -= pain;
                    MappedSound.Pop(player.id, 0);
                    MappedMatch.score += pain / 2f * player.ScoreFilter(player.pV.id);
                    player.spirit += pain / 10f;
                    player.pV.spirit -= pain / 15f;
                    return false;
                
                case "dealstun":
                    player.pV.blind = float.Parse(args[0]);
                    return false;
                
                case "setreleaseanim":
                    player.releaseAnim = int.Parse(args[0]);
                    return false;
                
                case "setoppreleaseanim":
                    player.pV.releaseAnim = int.Parse(args[0]);
                    return false;
                
                case "stepsound":
                    StepSound(player, float.Parse(args[0]));
                    return false;
                
                case "swingsound":
                    MappedSound.Emit(player.audio, MappedSound.swing[MappedGlobals.Rnd(1, 3)], float.Parse(args[1]), float.Parse(args[0]));
                    return false;
                
                case "heavyswingsound":
                    MappedSound.Emit(player.audio, MappedSound.swing[MappedGlobals.Rnd(1, 4)], float.Parse(args[1]), float.Parse(args[0]));
                    return false;
                
                case "vheavyswingsound":
                    MappedSound.Emit(player.audio, MappedSound.swing[MappedGlobals.Rnd(3, 5)], float.Parse(args[1]), float.Parse(args[0]));
                    return false;
                
                case "stretchsound":
                    MappedSound.Emit(player.audio, MappedSound.stretch[MappedGlobals.Rnd(1, 3)], float.Parse(args[1]), float.Parse(args[0]));
                    return false;
                
                case "impacthighsound":
                    MappedSound.Emit(player.audio, MappedSound.impactHigh[MappedGlobals.Rnd(1, 2)], float.Parse(args[1]), float.Parse(args[0]));
                    return false;
                
                case "heavyimpacthighsound":
                    MappedSound.Emit(player.audio, MappedSound.impactHigh[MappedGlobals.Rnd(3, 4)], float.Parse(args[1]), float.Parse(args[0]));
                    return false;       
                
                case "vheavyimpacthighsound":
                    MappedSound.Emit(player.audio, MappedSound.impactHigh[5], float.Parse(args[1]), float.Parse(args[0]));
                    return false;
                
                case "impactlowsound":
                    MappedSound.Emit(player.audio, MappedSound.impactLow[MappedGlobals.Rnd(1, 2)], float.Parse(args[1]), float.Parse(args[0]));
                    return false;
                
                case "heavyimpactlowsound":
                    MappedSound.Emit(player.audio, MappedSound.impactLow[MappedGlobals.Rnd(3, 4)], float.Parse(args[1]), float.Parse(args[0]));
                    return false;
                
                case "vheavyimpactlowsound":
                    MappedSound.Emit(player.audio, MappedSound.impactLow[5], float.Parse(args[1]), float.Parse(args[0]));
                    return false;
                
                case "painsound":
                    player.PainSound(args.Length > 0 ? float.Parse(args[0]) : 0.5f);
                    return false;
                
                case "opppainsound":
                    player.pV.PainSound(args.Length > 0 ? float.Parse(args[0]) : 0.5f);
                    return false;
                
                case "setanimationid":
                    player.SetAnimationId(int.Parse(args[0]), args.Length > 1 ? float.Parse(args[1]) : 0f);
                    return false;
                
                case "setoppanimationid":
                    player.pV.SetAnimationId(int.Parse(args[0]), args.Length > 1 ? float.Parse(args[1]) : 0f);
                    return false;
                
                case "stopanimation":
                    player.SetAnimationId(0);
                    return false;

                case "hitconnected?":
                    return player.HitConnected();
                
                case "strengthcheck?":
                    return player.StrengthCheck();
                
                case "sync":
                    player.SyncMove(player.pV, float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]), player.t);
                    return false;
                
                case "endgrapple":
                    player.LoseGrapple();
                    return false;
                
                case "directionalinput":
                    player.DirectMove(float.Parse(args[0]), float.Parse(args[1]));
                    return false;
                
                case "transitionframes":
                    var speed = float.Parse(args[0]);
                    if (grapple)
                    {   
                        player.t = speed == -1 ? player.s : speed == -2 ? 3f + player.animTim * 3f : speed;
                    }
                    else
                    {
                        player.transition[0] = speed == -1 ? player.s : speed == -2 ? 3f + player.animTim * 3f : speed;
                    }
                    return false;
                
                case "riskbreak":
                    player.BreakMove(int.Parse(args[0]), int.Parse(args[1]));
                    return false;
                
                case "riskreversal":
                    player.RiskReversal(player.pV, int.Parse(args[0]), args.Length > 1 ? int.Parse(args[1]) : -1f);
                    return false;
                
                case "riskcounter":
                    player.RiskCounter(player.pV, int.Parse(args[0]), int.Parse(args[1]), float.Parse(args[2]), args.Length > 3 ? float.Parse(args[3]) : 0f);
                    return false;
                
                case "moveimpact":
                    player.MoveImpact(int.Parse(args[0]), int.Parse(args[1]), float.Parse(args[2]));
                    return false;
                
                case "applypin":
                    player.ApplyMovePin(args.Length > 0 ? int.Parse(args[0]) : 2);
                    return false;
                
                case "bounce":
                    player.Bounce(args.Length > 0 ? int.Parse(args[0]) : -0.2f);
                    return false;
                
                case "oppbounce":
                    player.pV.Bounce(args.Length > 0 ? int.Parse(args[0]) : -0.2f);
                    return false;
                
                case "pinning?":
                    return player.pinning > 0;
                
                case "travel":
                    player.MoveTravel(args.Length > 0 ? int.Parse(args[0]) : player.travelA, args.Length > 1 ? float.Parse(args[1]) : player.travel, args.Length > 2 ? float.Parse(args[2]) : 0f);
                    return false;
                
                case "advance":
                    player.Advance(args.Length > 0 ? int.Parse(args[0]) : player.travelA, args.Length > 1 ? float.Parse(args[1]) : player.travel);
                    return false;
                    
                case "sellbackfall":
                    player.pV.SellBackFall();
                    return false;
                
                case "sellfrontfall":
                    player.pV.SellFrontFall();
                    return false;
                
                case "findsmashes":
                    player.pV.FindSmashes(player.pV.LimbY(1), float.Parse(args[0]), float.Parse(args[1]), int.Parse(args[2]), float.Parse(args[3]));
                    return false;
                
                default:
                    throw new Exception("Unknown command " + command);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error executing command " + command + " with args " + string.Join(",", args), e);
        }
    }

    public static void PerformPostGrappleCode(MappedPlayer player)
    {
        if (player.grappling > 0 && player.reverse != 0f)
        {
            player.animTim -= 2f * MappedAnims.speed;
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

    public static bool IsGrappleMove(int id)
    {
        if (id >= 200 && id < 800)
        {
            return true;
        }
        if (id >= 1000000)
        {
            return CustomAnimations[id - 1000000].IsGrapple;
        }
        return false;
    }

    public static bool IsRegularMove(int id)
    {
        if ((id >= 0 && id < 800) || id >= 1000000)
        {
            return true;
        }
        return false;
    }
}