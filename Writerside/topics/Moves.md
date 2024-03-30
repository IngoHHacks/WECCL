# 🤼 Moves

<show-structure for="chapter" depth="2"/>

<link-summary>
How to add custom moves to the game.
</link-summary>

<warning>
This feature is experimental and may cause issues.
</warning>

## Adding Moves
You can add moves by inserting files into <include from="snippets.md" element-id="apath"/>.  
The move files must be in a subfolder named `animation`. The following files are used:
`(move_name)` is an asset bundle containing a single AnimationClip for the move.
`(move_name).meta` is a meta file for the move.  
`(move_name).receive` (for grappling moves) is an asset bundle containing a single AnimationClip for the opponent's animation.
All files must be in the same folder.

## Important Notes
- Moves use interpolation between frames. Using keyframes or other animation features may not work as expected. Blame Mat Dickie for this.
- All moves must have a `StopAnimation` event at the end.
- All grapple moves must have an `EndGrapple` event at the end, unless transitioning to another grapple move.
- Some events only work for strike moves, and some only work for grapple moves.
- Frame ranges are inclusive. Animations always run at 30 FPS, regardless of the actual FPS of the game.
- Strike moves usually have a `WindUp` event at the start, which makes the actual animation range start at frame 100. It is not required to have a `WindUp` event for strike moves.

## Animation File
Asset bundles can be created through Unity. For more information on how to create asset bundles, see [Creating Asset Bundles](AssetBundles.md).

## Move Meta File
The move meta file should contain the metadata of the move and a list of events.  
Example strike move meta file:
```
name: Big Punch
types: BigAttack
forwardspeedmultiplier: 1.5
0-99 SetAnimation * 1 3
0-99 WindUp -10 5 0.5
101- SetAnimation * 2 3
110-129 EnableHitbox 8 1000 R_Hand 10 1
110 PlayAudio -1
130 HitConnected?
 130 StrengthCheck?
  130 SetAnimationId 9|10
140 StopAnimation
```
Example grapple move meta file:
```
Name: Neck Stretch
Types: FrontGrapple
1-5 SetAnimation * 0 5
6-10 SetAnimation * 1 5
11-20 SetAnimation * 2 5
21-30 SetAnimation * 3 5
21 DealDamage 200
21 DealStun -150
21 StretchSound -0.2 0.5
21 OppPainSound 0.5
0- Sync 0 5 0 180
31 StopAnimation
31 EndGrapple
```

### Move Metadata
Move metadata should be in the format `key: value`.
The following keys are supported:
<deflist>
<def title="name">
The name of the move.
</def>
<def title="types">
The types of the move. Valid types are <code>StrikeHigh</code>, <code>StrikeLow</code>, <code>BigAttack</code>, <code>RunningAttack</code> (unused), <code>FrontGrapple</code>, <code>BackGrapple</code>.
</def>
</deflist>
<def title="forwardspeedmultiplier">
The forwardspeedmultiplier of the move. This value is multiplied by the animation speed to determine the forward speed of the move. Default is 4.
</def>

### Move Events
Move events should be in the format `[start(-(end))] command arg1 arg2 arg3...`.
Examples:  
```
// Play animation frame 1 from frames 1 to 10.
1-10 SetAnimation * 1
// Play audio at frame 15.
15 PlayAudio -1
// Synchronize the animation starting at frame 0.
0- Sync 0 5 0 180
```

<note>
    All commands and conditions are case-insensitive.
</note>

**The following events are supported:**
### General Commands

<chapter title="Animation Commands" id="animation-commands" collapsible="true" default-state="expanded">

<chapter title="SetAnimation" id="setanimation">
<p></p>

<list>
<li><control>Parameters:</control> <code>[file: int|*] [frame: int] [frames: float = -1]</code></li>
<li>Plays an animation.</li>
<li><control>Note:</control> If <code>file</code> is <code>*</code>, the animation from the move file is played. Otherwise, the vanilla animation file is used.</li>
<li><control>Note:</control> <code>frames</code> determines the speed of the animation. Lower values make the animation faster. Negative values are ignored and use the default speed (set by <code>TransitionFrames</code>). Typical values are 3-10. Can be a float.</li>
</list>
</chapter>
<chapter title="SetAnimationId" id="setanimationid">
<list>
<li><control>Parameters:</control> <code>[animationId: int] [startTim: float = 0]</code></li>
<li>Plays an animation by ID.</li>
</list>
</chapter>
<chapter title="StopAnimation" id="stopanimation">
<list>
<li>No parameters.</li>
<li>Stops the animation.</li>
<li><control>Note:</control> Required.</li>
</list>
</chapter>
<chapter title="TransitionFrames" id="transitionframes">
<list>
<li><control>Parameters:</control> <code>[speed: float]</code></li>
<li>Sets the transition frames for the move (essentially the animation speed).</li>
</list>
</chapter>

</chapter>

<chapter title="Movement Commands" id="movement-commands" collapsible="true" default-state="expanded">

<chapter title="Advance" id="advance">
<p></p>
<list>
<li><control>Parameters:</control> <code>[angle: float] [travel: float]</code></li>
<li>Moves the player forwards.</li>
</list>
</chapter>
</chapter>

<chapter title="Audio Commands" id="audio-commands" collapsible="true" default-state="expanded">

<chapter title="StepSound" id="stepsound">
<p></p>
<list>
<li><control>Parameters:</control> <code>[volume: float]</code></li>
<li>Plays a step sound.</li>
</list>
</chapter>
<chapter title="SwingSound" id="swingsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a swing sound.</li>
</list>
</chapter>
<chapter title="HeavySwingSound" id="heavyswingsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a heavy swing sound.</li>
</list>
</chapter>
<chapter title="VHeavySwingSound" id="vheavyswingsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a very heavy swing sound.</li>
</list>
</chapter>
<chapter title="StretchSound" id="stretchsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a stretch sound.</li>
</list>
</chapter>
<chapter title="ImpactHighSound" id="impacthighsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a high impact sound.</li>
</list>
</chapter>
<chapter title="HeavyImpactHighSound" id="heavyimpacthighsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a heavy high impact sound.</li>
</list>
</chapter>
<chapter title="VHeavyImpactHighSound" id="vheavyimpacthighsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a very heavy high impact sound.</li>
</list>
</chapter>
<chapter title="ImpactLowSound" id="impactlowsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a low impact sound.</li>
</list>
</chapter>
<chapter title="HeavyImpactLowSound" id="heavyimpactlowsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a heavy low impact sound.</li>
</list>
</chapter>
<chapter title="VHeavyImpactLowSound" id="vheavyimpactlowsound">
<list>
<li><control>Parameters:</control> <code>[variation: float = 0] [volume: float = 1]</code></li>
<li>Plays a very heavy low impact sound.</li>
</list>
</chapter>
<chapter title="PainSound" id="painsound">
<list>
<li><control>Parameters:</control> <code>[volume: float = 0.5]</code></li>
<li>Plays a pain sound.</li>
</list>
</chapter>

</chapter>

<chapter title="Miscellaneous Commands" id="miscellaneous-commands" collapsible="true" default-state="expanded">

<chapter title="Bounce" id="bounce">
<p></p>
<list>
<li><control>Parameters:</control> <code>[level: float = -0.2]</code></li>
<li>Bounces the player.</li>
</list>
</chapter>

</chapter>

### Strike-Specific Commands

<chapter title="Animation Commands" id="strike-animation-commands" collapsible="true" default-state="expanded">

<chapter title="WindUp" id="windup">
<p></p>
<list>
<li><control>Parameters:</control> <code>[speed: float] [buildupFrames: int] [forwardMomentum: float = 0]</code></li>
<li>Performs the windup animation of the move.</li>
<li><control>Note:</control> <code>speed</code> is the minimum speed of the animation. Some randomization is applied. Set it to a negative value to disable randomization (the absolute value is used).</li>
</list>
</chapter>

</chapter>

<chapter title="Damage Commands" id="strike-damage-commands" collapsible="true" default-state="expanded">

<chapter title="EnableHitbox" id="enablehitbox">
<p></p>
<list>
<li><control>Parameters:</control> <code>[distance: float] [damage: float] [limb: Limb] [angle: float = 0] [particle: float = 0]</code></li>
<li>Enables a hitbox for the move.</li>
</list>
</chapter>

</chapter>

### Grapple-Specific Commands

<chapter title="Animation Commands" id="grapple-animation-commands" collapsible="true" default-state="expanded">

<chapter title="SetOppAnimation" id="setoppanimation">
<p></p>
<list>
<li><control>Parameters:</control> <code>[file: int] [frame: int] [speed: float = -1]</code></li>
<li>Plays an animation for the opponent.</li>
</list>
</chapter>
<chapter title="SetOppAnimationId" id="setoppanimationid">
<list>
<li><control>Parameters:</control> <code>[animationId: int] [startTim: float = 0]</code></li>
<li>Plays an animation for the opponent by ID.</li>
</list>
</chapter>
<chapter title="StopOppAnimation" id="stopoppanimation">
<list>
<li>No parameters.</li>
<li>Stops the opponent's animation.</li>
</list>
</chapter>
<chapter title="SetReleaseAnim" id="setreleaseanim">
<list>
<li><control>Parameters:</control> <code>[releaseAnim: int]</code></li>
<li>Sets the release animation for the move.</li>
</list>
</chapter>
<chapter title="SetOppReleaseAnim" id="setoppreleaseanim">
<list>
<li><control>Parameters:</control> <code>[releaseAnim: int]</code></li>
<li>Sets the release animation for the opponent.</li>
</list>
</chapter>
<chapter title="EndGrapple" id="endgrapple">
<list>
<li>No parameters.</li>
<li>Ends the grapple.</li>
<li><control>Note:</control> Required unless transitioning to another grapple move.</li>
</list>
</chapter>

</chapter>

<chapter title="Damage Commands" id="grapple-damage-commands" collapsible="true" default-state="expanded">

<chapter title="MoveAttack" id="moveattack">
<p></p>
<list>
<li><control>Parameters:</control> <code>[contact: int] [pain: float] [weapon: float]</code></li>
<li>Performs the attack part of the move, dealing damage and playing a sound.</li>
</list>
</chapter>
<chapter title="MoveImpact" id="moveimpact">
<list>
<li><control>Parameters:</control> <code>[style: int] [level: float] [weapon: float]</code></li>
<li>Performs the move's impact, dealing damage and playing a sound.</li>
</list>
</chapter>
<chapter title="DealDamage" id="dealdamage">
<list>
<li><control>Parameters:</control> <code>[pain: float]</code></li>
<li>Deals damage to the opponent without playing a sound.</li>
</list>
</chapter>
<chapter title="DealStun" id="dealstun">
<list>
<li><control>Parameters:</control> <code>[blind: float]</code></li>
<li>Applies stun (blind) effect to the opponent.</li>
</list>
</chapter>

</chapter>

<chapter title="Sell Commands" id="grapple-sell-commands" collapsible="true" default-state="expanded">

<chapter title="SellBackFall" id="sellbackfall">
<p></p>
<list>
<li>No parameters.</li>
<li>Makes the opponent sell a back fall.</li>
</list>
</chapter>
<chapter title="SellFrontFall" id="sellfrontfall">
<list>
<li>No parameters.</li>
<li>Makes the opponent sell a front fall.</li>
</list>
</chapter>

</chapter>

<chapter title="Movement Commands" id="grapple-movement-commands" collapsible="true" default-state="expanded">

<chapter title="Sync" id="sync">
<p></p>
<list>
<li><control>Parameters:</control> <code>[projectA: float] [range: float] [angleA: float] [angleB: float]</code></li>
<li>Synchronizes the opponent's position and rotation.</li>
<li></li>
</list>
</chapter>
<chapter title="Travel" id="travel">
<list>
<li><control>Parameters:</control> <code>[angle: float] [travel: float] [inherit: float = 0]</code></li>
<li>Moves the player forwards (similar to <code>Advance</code> but specific to grapples).</li>
</list>
</chapter>
<chapter title="DirectionalInput" id="directionalinput">
<list>
<li><control>Parameters:</control> <code>[offsetA: float] [turnSpeed: float]</code></li>
<li>Applies directional input.</li>
<li><control>Note:</control> Currently unknown if strike moves can use this.</li>
</list>
</chapter>

</chapter>

<chapter title="Audio Commands" id="grapple-audio-commands" collapsible="true" default-state="expanded">

<chapter title="OppPainSound" id="opppainsound">
<p></p>
<list>
<li><control>Parameters:</control> <code>[volume: float = 0.5]</code></li>
<li>Plays a pain sound for the opponent.</li>
</list>
</chapter>

</chapter>

<chapter title="Interaction Commands" id="grapple-interaction-commands" collapsible="true" default-state="expanded">

<chapter title="RiskBreak" id="riskbreak">
<p></p>
<list>
<li><control>Parameters:</control> <code>[risk: int] [damage: int]</code></li>
<li>Risks breaking up the grapple.</li>
</list>
</chapter>
<chapter title="RiskReversal" id="riskreversal">
<list>
<li><control>Parameters:</control> <code>[risk: int] [rewind: float = -1]</code></li>
<li>Risks the opponent reversing the grapple.</li>
</list>
</chapter>
<chapter title="RiskCounter" id="riskcounter">
<list>
<li><control>Parameters:</control> <code>[risk: int] [newAnim: int] [newTim: float] [newAngle: float]</code></li>
<li>Risks the opponent countering the grapple.</li>
</list>
</chapter>

</chapter>

<chapter title="Miscellaneous Commands" id="grapple-miscellaneous-commands" collapsible="true" default-state="expanded">

<chapter title="ApplyPin" id="applypin">
<p></p>
<list>
<li><control>Parameters:</control> <code>[chance: int = 2]</code></li>
<li>Pins the opponent with a chance.</li>
</list>
</chapter>
<chapter title="OppBounce" id="oppbounce">
<list>
<li><control>Parameters:</control> <code>[level: float = -0.2]</code></li>
<li>Bounces the opponent.</li>
</list>
</chapter>
<chapter title="FindSmashes" id="findsmashes">
<list>
<li><control>Parameters:</control> <code>[rangeOffset: float] [level: float] [landing: int] [includeHumans: float = 0]</code></li>
<li>Finds smashes.</li>
</list>
</chapter>

</chapter>

### Conditions
Conditions are used to determine whether an event should be executed. If the condition is not met, any events after the condition with an indentation level greater than the condition will not be executed.  
Conditions have no parameters.

<chapter title="Conditions" id="conditions-chapter" collapsible="true" default-state="expanded">

<chapter title="HitConnected?" id="hitconnected">
<p></p>
<list>
<li>Checks if the hit connected.</li>
<li><control>Note:</control> This condition is only valid for strike moves.</li>
</list>
</chapter>
<chapter title="StrengthCheck?" id="strengthcheck">
<list>
<li>Performs a strength check.</li>
<li>The formula for the strength check is <code>Random(24, STRENGTH) &lt; 25</code>. Higher strength decreases the chance of this condition being met.</li>
</list>
</chapter>
<chapter title="Pinning?" id="pinning">
<list>
<li>Checks if the player is pinning the opponent.</li>
</list>
</chapter>

</chapter>

### Parameters
`int` is an integer (whole number). Supports `|` and `-` operators.
`float` is a float (decimal number). Supports `|` and `-` operators.
`*` is a literal `*`.
`Limb` is a limb name, see [Limb Names](Limbs.md).
### Operators
`|` is the OR operator. For example, `1|2` means `1` or `2`, and `10|20|30` means `10` or `20` or `30`.  
`-` is the range operator. For example, `1-10` means `1` to `10`. Can be used with `|`. For example, `1-10|20-30` means `1` to `10` or `20` to `30`.
<warning>
    <p>
        Operators can only be used for integer and float parameters.<br/>
        Frame ranges do not support the `|` operator.
    </p>
</warning>