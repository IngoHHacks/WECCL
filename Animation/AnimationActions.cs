using System.Diagnostics.CodeAnalysis;
using WECCL.Content;

namespace WECCL.Animation;

internal static class AnimationActions
{
    public static Dictionary<string, AnimationAction> Actions { get; set; } = new();
    public static Dictionary<string, AnimationCondition> Conditions { get; set; } = new();

    [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
    public static void Initialize()
    {
        Actions["windup"] = new AnimationAction("WindUp", (p, args) =>
        {
            float speed = AnimationParser.ParseFloat(args["speed"]).Value.Value;
            int buildupFrames = AnimationParser.ParseInt(args["buildupframes"]).Value.Value;
            float forwardMomemtum = AnimationParser.ParseFloat(args["forwardmomentum"]).Value.Value;
            p.WindUp(speed, buildupFrames, forwardMomemtum);
        },  new AnimationArgument<float?>("speed", AnimationParser.ParseFloat),
            new AnimationArgument<int?>("buildupframes", AnimationParser.ParseInt),
            new AnimationArgument<float?>("forwardmomentum",AnimationParser.ParseFloat, "0"));
        
        Actions["setanimation"] = new AnimationAction("SetAnimation", (p, args) =>
        {
            int file = AnimationParser.ParseAnimationId(args["file"]).Value.Value;
            int frame = AnimationParser.ParseInt(args["frame"]).Value.Value;
            float speed = AnimationParser.ParseFloat(args["speed"]).Value.Value;
            if (p.grappling > 0)
            {
                p.SetGrappleAnimation(file, frame, speed);
            }
            else
            {
                p.SetAnimation(file, frame, speed);
            }
        },  new AnimationArgument<int?>("file", AnimationParser.ParseAnimationId),
            new AnimationArgument<int?>("frame", AnimationParser.ParseInt),
            new AnimationArgument<float?>("speed",AnimationParser.ParseFloat, "-1"));
        
        Actions["setoppanimation"] = new AnimationAction("SetOppAnimation", (p, args) =>
        {
            int file = AnimationParser.ParseAnimationId(args["file"]).Value.Value;
            int frame = AnimationParser.ParseInt(args["frame"]).Value.Value;
            float speed = AnimationParser.ParseFloat(args["speed"]).Value.Value;
            p.pV.SetAnimation(file, frame, speed);
        },  new AnimationArgument<int?>("file", AnimationParser.ParseAnimationId),
            new AnimationArgument<int?>("frame", AnimationParser.ParseInt),
            new AnimationArgument<float?>("speed",AnimationParser.ParseFloat, "-1"));
        
        Actions["enablehitbox"] = new AnimationAction("EnableHitbox", (p, args) =>
        {
            float distance = AnimationParser.ParseFloat(args["distance"]).Value.Value;
            float damage = AnimationParser.ParseFloat(args["damage"]).Value.Value;
            Limb limb = AnimationParser.ParseLimb(args["limb"]).Value.Value;
            float angle = AnimationParser.ParseFloat(args["angle"]).Value.Value;
            float particle = AnimationParser.ParseFloat(args["particle"]).Value.Value;
            p.EnableHitbox(distance, damage, limb, angle, particle);
        },  new AnimationArgument<float?>("distance", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("damage", AnimationParser.ParseFloat),
            new AnimationArgument<Limb?>("limb", AnimationParser.ParseLimb),
            new AnimationArgument<float?>("angle", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("particle", AnimationParser.ParseFloat, "0"));
        
        Actions["moveattack"] = new AnimationAction("MoveAttack", (p, args) =>
        {
            int contact = AnimationParser.ParseInt(args["contact"]).Value.Value;
            float pain = AnimationParser.ParseFloat(args["pain"]).Value.Value;
            float weapon = AnimationParser.ParseFloat(args["weapon"]).Value.Value;
            p.MoveAttack(p.pV, contact, pain, weapon);
        },  new AnimationArgument<int?>("contact", AnimationParser.ParseInt),
            new AnimationArgument<float?>("pain", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("weapon",AnimationParser.ParseFloat));
        
        Actions["dealdamage"] = new AnimationAction("DealDamage", (p, args) =>
        {
            float pain = AnimationParser.ParseFloat(args["pain"]).Value.Value;
            pain *= p.stat[2] / 100f;
            if (p.special > 0f)
            {
                pain *= 1.25f;
            }
            if (pain > 300f && p.special >= 5f)
            {
                p.special /= 5f;
            }
            p.pV.health -= pain;
            MappedSound.Pop(p.id, 0);
            MappedMatch.score += pain / 2f * p.ScoreFilter(p.pV.id);
            p.spirit += pain / 10f;
            p.pV.spirit -= pain / 15f;
        },  new AnimationArgument<float?>("pain", AnimationParser.ParseFloat));
        
        Actions["dealstun"] = new AnimationAction("DealStun", (p, args) =>
        {
            p.pV.blind = AnimationParser.ParseFloat(args["blind"]).Value.Value;
        },  new AnimationArgument<float?>("blind", AnimationParser.ParseFloat));
        
        Actions["setreleaseanim"] = new AnimationAction("SetReleaseAnim", (p, args) =>
        {
            p.releaseAnim = AnimationParser.ParseInt(args["releaseanim"]).Value.Value;
        },  new AnimationArgument<int?>("releaseanim", AnimationParser.ParseInt));
        
        Actions["setoppreleaseanim"] = new AnimationAction("SetOppReleaseAnim", (p, args) =>
        {
            p.pV.releaseAnim = AnimationParser.ParseInt(args["releaseanim"]).Value.Value;
        },  new AnimationArgument<int?>("releaseanim", AnimationParser.ParseInt));
        
        Actions["stepsound"] = new AnimationAction("StepSound", (p, args) =>
        {
            p.StepSound(AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("volume", AnimationParser.ParseFloat));
        
        Actions["swingsound"] = new AnimationAction("SwingSound", (p, args) =>
        {
            MappedSound.Emit(p.audio, MappedSound.swing[MappedGlobals.Rnd(1, 3)], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
        
        Actions["heavyswingsound"] = new AnimationAction("HeavySwingSound", (p, args) =>
        {
            MappedSound.Emit(p.audio, MappedSound.swing[MappedGlobals.Rnd(1, 4)], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
        
        Actions["vheavyswingsound"] = new AnimationAction("VHeavySwingSound", (p, args) =>
        {
            MappedSound.Emit(p.audio, MappedSound.swing[MappedGlobals.Rnd(3, 5)], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
        
        Actions["stretchsound"] = new AnimationAction("StretchSound", (p, args) =>
        {
            MappedSound.Emit(p.audio, MappedSound.stretch[MappedGlobals.Rnd(1, 3)], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
        
        Actions["impacthighsound"] = new AnimationAction("ImpactHighSound", (p, args) =>
            {
                MappedSound.Emit(p.audio, MappedSound.impactHigh[MappedGlobals.Rnd(1, 2)], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
            },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
                new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
        
        Actions["heavyimpacthighsound"] = new AnimationAction("HeavyImpactHighSound", (p, args) =>
        {
            MappedSound.Emit(p.audio, MappedSound.impactHigh[MappedGlobals.Rnd(3, 4)], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
                
        Actions["vheavyimpacthighsound"] = new AnimationAction("VHeavyImpactHighSound", (p, args) =>
        {
            MappedSound.Emit(p.audio, MappedSound.impactHigh[5], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
        
        Actions["impactlowsound"] = new AnimationAction("ImpactLowSound", (p, args) =>
        {
            MappedSound.Emit(p.audio, MappedSound.impactLow[MappedGlobals.Rnd(1, 2)], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
        
        Actions["heavyimpactlowsound"] = new AnimationAction("HeavyImpactLowSound", (p, args) =>
        {
            MappedSound.Emit(p.audio, MappedSound.impactLow[MappedGlobals.Rnd(3, 4)], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
        
        Actions["vheavyimpactlowsound"] = new AnimationAction("VHeavyImpactLowSound", (p, args) =>
        {
            MappedSound.Emit(p.audio, MappedSound.impactLow[5], AnimationParser.ParseFloat(args["variation"]).Value.Value, AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("variation", AnimationParser.ParseFloat, "0"),
            new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "1"));
        
        Actions["painsound"] = new AnimationAction("PainSound", (p, args) =>
        {
            p.PainSound(AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "0.5"));
        
        Actions["opppainsound"] = new AnimationAction("OppPainSound", (p, args) =>
        {
            p.pV.PainSound(AnimationParser.ParseFloat(args["volume"]).Value.Value);
        },  new AnimationArgument<float?>("volume", AnimationParser.ParseFloat, "0.5"));
        
        Actions["setanimationid"] = new AnimationAction("SetAnimationId", (p, args) =>
        {
            int animationId = AnimationParser.ParseInt(args["animationid"]).Value.Value;
            float starttim = AnimationParser.ParseFloat(args["starttim"]).Value.Value;
            p.SetAnimationId(animationId, starttim);
        },  new AnimationArgument<int?>("animationid", AnimationParser.ParseInt),
            new AnimationArgument<float?>("starttim", AnimationParser.ParseFloat, "0"));
        
        Actions["setoppanimationid"] = new AnimationAction("SetOppAnimationId", (p, args) =>
        {
            int animationId = AnimationParser.ParseInt(args["animationid"]).Value.Value;
            float starttim = AnimationParser.ParseFloat(args["starttim"]).Value.Value;
            p.pV.SetAnimationId(animationId, starttim);
        },  new AnimationArgument<int?>("animationid", AnimationParser.ParseInt),
            new AnimationArgument<float?>("starttim", AnimationParser.ParseFloat, "0"));

        Actions["stopanimation"] = new AnimationAction("StopAnimation", (p, args) =>
            p.SetAnimationId(0));
        
        Actions["stopoppanimation"] = new AnimationAction("StopOppAnimation", (p, args) =>
            p.pV.SetAnimationId(0));
        
        Actions["sync"] = new AnimationAction("Sync", (p, args) =>
        {
            float projectA = AnimationParser.ParseFloat(args["projecta"]).Value.Value;
            float range = AnimationParser.ParseFloat(args["range"]).Value.Value;
            float angleA = AnimationParser.ParseFloat(args["anglea"]).Value.Value;
            float angleB = AnimationParser.ParseFloat(args["angleb"]).Value.Value;
            p.SyncMove(p.pV, projectA, range, angleA, angleB, p.t);
        },  new AnimationArgument<float?>("projecta", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("range", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("anglea", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("angleb", AnimationParser.ParseFloat));
        
        Actions["endgrapple"] = new AnimationAction("EndGrapple", (p, args) =>
            p.LoseGrapple());
        
        Actions["directionalinput"] = new AnimationAction("DirectionalInput", (p, args) =>
        {
            float offsetA = AnimationParser.ParseFloat(args["offseta"]).Value.Value;
            float turnSpeed = AnimationParser.ParseFloat(args["turnspeed"]).Value.Value;
            p.DirectMove(offsetA, turnSpeed);
        },  new AnimationArgument<float?>("offseta", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("turnspeed", AnimationParser.ParseFloat));
        
        Actions["transitionframes"] = new AnimationAction("TransitionFrames", (p, args) =>
        {
            float speed = AnimationParser.ParseFloat(args["speed"]).Value.Value;
            if (p.grappling > 0)
            {   
                p.t = speed == -1 ? p.s : speed == -2 ? 3f + p.animTim * 3f : speed;
            }
            else
            {
                p.transition[0] = speed == -1 ? p.s : speed == -2 ? 3f + p.animTim * 3f : speed;
            }
        },  new AnimationArgument<float?>("speed", AnimationParser.ParseFloat));
        
        Actions["riskbreak"] = new AnimationAction("RiskBreak", (p, args) =>
        {
            int risk = AnimationParser.ParseInt(args["risk"]).Value.Value;
            int damage = AnimationParser.ParseInt(args["damage"]).Value.Value;
            p.BreakMove(risk, damage);
        },  new AnimationArgument<int?>("risk", AnimationParser.ParseInt),
            new AnimationArgument<int?>("damage", AnimationParser.ParseInt));
        
        Actions["riskreversal"] = new AnimationAction("RiskReversal", (p, args) =>
        {
            int risk = AnimationParser.ParseInt(args["risk"]).Value.Value;
            float rewind = AnimationParser.ParseFloat(args["rewind"]).Value.Value;
            p.RiskReversal(p.pV, risk, rewind);
        },  new AnimationArgument<int?>("risk", AnimationParser.ParseInt),
            new AnimationArgument<float?>("rewind", AnimationParser.ParseFloat, "-1"));
        
        Actions["riskcounter"] = new AnimationAction("RiskCounter", (p, args) =>
        {
            int risk = AnimationParser.ParseInt(args["risk"]).Value.Value;
            int newAnim = AnimationParser.ParseInt(args["newanim"]).Value.Value;
            float newTim = AnimationParser.ParseFloat(args["newtim"]).Value.Value;
            float newAngle = AnimationParser.ParseFloat(args["newangle"]).Value.Value;
            p.RiskCounter(p.pV, risk, newAnim, newTim, newAngle);
        },  new AnimationArgument<int?>("risk", AnimationParser.ParseInt),
            new AnimationArgument<int?>("newanim", AnimationParser.ParseInt),
            new AnimationArgument<float?>("newtim", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("newangle", AnimationParser.ParseFloat));
        
        Actions["moveimpact"] = new AnimationAction("MoveImpact", (p, args) =>
        {
            int style = AnimationParser.ParseInt(args["style"]).Value.Value;
            float level = AnimationParser.ParseFloat(args["level"]).Value.Value;
            float weapon = AnimationParser.ParseFloat(args["weapon"]).Value.Value;
            p.MoveImpact(style, level, weapon);
        },  new AnimationArgument<int?>("style", AnimationParser.ParseInt),
            new AnimationArgument<float?>("level", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("weapon",AnimationParser.ParseFloat));
        
        Actions["applypin"] = new AnimationAction("ApplyPin", (p, args) =>
        {
            int chance = AnimationParser.ParseInt(args["chance"]).Value.Value;
            p.ApplyMovePin(chance);
        },  new AnimationArgument<int?>("chance", AnimationParser.ParseInt, "2"));
        
        Actions["bounce"] = new AnimationAction("Bounce", (p, args) =>
        {
            float level = AnimationParser.ParseFloat(args["level"]).Value.Value;
            p.Bounce(level);
        },  new AnimationArgument<float?>("level", AnimationParser.ParseFloat, "-0.2"));
        
        Actions["oppbounce"] = new AnimationAction("OppBounce", (p, args) =>
        {
            float level = AnimationParser.ParseFloat(args["level"]).Value.Value;
            p.pV.Bounce(level);
        },  new AnimationArgument<float?>("level", AnimationParser.ParseFloat, "-0.2"));
        
        Actions["travel"] = new AnimationAction("Travel", (p, args) =>
        {
            float angle = AnimationParser.ParseFloat(args["angle"]).Value.Value;
            float speed = AnimationParser.ParseFloat(args["speed"]).Value.Value;
            float inherit = AnimationParser.ParseFloat(args["inherit"]).Value.Value;
            float relativeRotation = (p.a + angle) % 360;

            p.MoveTravel(relativeRotation, speed, inherit);
        },  new AnimationArgument<float?>("angle", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("speed", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("inherit", AnimationParser.ParseFloat, "0"));
        
        Actions["advance"] = new AnimationAction("Advance", (p, args) =>
        {
            float angle = AnimationParser.ParseFloat(args["angle"]).Value.Value;
			float travel = AnimationParser.ParseFloat(args["travel"]).Value.Value;
			p.Advance(angle, travel);
		},  new AnimationArgument<int?>("angle", AnimationParser.ParseInt),
            new AnimationArgument<float?>("travel", AnimationParser.ParseFloat));
        
        Actions["sellbackfall"] = new AnimationAction("SellBackFall", (p, args) =>
            p.SellBackFall());
        
        Actions["sellfrontfall"] = new AnimationAction("SellFrontFall", (p, args) =>
            p.SellFrontFall());

		Actions["oppsellbackfall"] = new AnimationAction("OppSellBackFall", (p, args) =>
			p.pV.SellBackFall());

		Actions["oppsellfrontfall"] = new AnimationAction("OppSellFrontFall", (p, args) =>
			p.pV.SellFrontFall());

		Actions["findsmashes"] = new AnimationAction("FindSmashes", (p, args) =>
        {
            float rangeOffset = AnimationParser.ParseFloat(args["rangeoffset"]).Value.Value;
            float level = AnimationParser.ParseFloat(args["level"]).Value.Value;
            int landing = AnimationParser.ParseInt(args["landing"]).Value.Value;
            float includeHumans = AnimationParser.ParseFloat(args["includehumans"]).Value.Value;
            p.pV.FindSmashes(p.pV.LimbY(1), rangeOffset, level, landing, includeHumans);
        },  new AnimationArgument<float?>("rangeoffset", AnimationParser.ParseFloat),
            new AnimationArgument<float?>("level", AnimationParser.ParseFloat),
            new AnimationArgument<int?>("landing", AnimationParser.ParseInt),
            new AnimationArgument<float?>("includehumans", AnimationParser.ParseFloat, "0"));
        
        Conditions["hitconnected"] = new AnimationCondition("HitConnected", p => p.HitConnected());
        
        Conditions["strengthcheck"] = new AnimationCondition("StrengthCheck", p => p.StrengthCheck());
        
        Conditions["pinning"] = new AnimationCondition("Pinning", p => p.pinning > 0);
        
        Conditions["grappling"] = new AnimationCondition("Grappling", p => p.grappling > 0);
    }
}