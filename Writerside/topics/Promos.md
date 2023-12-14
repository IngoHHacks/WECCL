# 🎤 Promos

<show-structure for="chapter" depth="2"/>

<link-summary>
How to add custom promos to the game.
</link-summary>

## Adding Promos

You can add custom promos by placing a `.promo` file inside <include from="snippets.md" element-id="apath"/>.
The file must contain metadata in
the format `key: value` (space is optional), and newline-separated dialog lines in the
format `"line1","line2",speaker,target(,taunt,demeanor,commands)`.  
Example:

```
title: Test Promo
description: Promo between [P1] and [P2]
characters: 1,2
"Well, well, well, if it isn't $name2.","What brings you here, brother?",1,2
"I came to put an end to your reign of terror, $name1.","And I heard you're touting some newfangled gadget. What is it?",2,1,Point At Ground,-50
"Oh, it's no gadget, brother.","It's the future of wrestling: custom promos.",1,2,Shake Finger,50
"Custom promos? Sounds like a cheap ploy to me.","I'm not falling for it.",2,1,Thumbs Down,-50
"Cheap? Hardly, brother. Custom promos are the real deal. And speaking", "of deals, how about we settle things once and for all in the ring?",1,2
"You're on, $name1. And you'd better believe", "I'll be bringing my A-game.",2,1,Thumb Stampede,0,PlayAudio:Cheer
```
### Required Metadata
<deflist type="narrow">
<def title="title">
The title of the promo. Must be a string.
</def>
<def title="description">
The description of the promo. Must be a string. [P1] and [P2] will be replaced with the names of the characters.
</def>
<def title="characters">
The characters in the promo. Must be a comma-separated list of integers, or an integer prefixed by `:`. When using a prefixed integer,
an array from 1 to that integer will be created. When using a list, the list will be used as the array.
1 and 2 are the default characters as selected by the user, 3 is another character, -1 is the referee, and 11 and 22 are
the tag team partners of 1 and 2 respectively. Other values are not supported.
</def>
</deflist>

### Optional Metadata
<deflist type="narrow">
<def title="use_names">
Whether to use character names in `surprise_entrants`, `speaker`, `target` and commands instead of their ids. Their names can be used even if they are not selected in the `characters` field.
Must be <code>True</code> or <code>False</code>. Defaults to <code>False</code>.
</def>
<def title="surprise_entrants">
Sets the given wrestlers (and their managers) as surprise entrants, who will only come out through the curtain once the promo ends. Must be a comma-separated list of match character ids.
</def>
<def title="next_promo">
Sets another custom promo to happen once this one ends. Must be a title of another custom promo. This allows you to have surprise entrants have voice lines once they enter the ring.
</def>
</deflist>

### Dialog Lines
<deflist type="narrow">
<def title="line1/line2">
The dialog line. Must be a string. The quotes are required. For quotes inside the string, you need to escape them with <code>\"</code>.
</def>
<def title="speaker">
The id of the character speaking the line. Must be an integer.
</def>
<def title="target">
The id of the character being spoken to. Must be an integer.
</def>
<def title="taunt">
The taunt to use. Must be a string or integer. A list of taunts can be found in <a href="Taunts.md">Taunts</a>.
</def>
<def title="demeanor">
The demeanor of the character. Must be an integer. A positive value will make the character happy for the given number of frames, and a
negative value will make the character angry for the given number of frames.
</def>
<def title="commands">
A list of commands to execute. Must be a list in the format `command:arg1:arg2:arg3...`. Commands are separated by a semicolon.
Example: <code>SetFace:1;SetRealFriend:1:2</code>
</def>
</deflist>

The following commands are supported:

| Command         | Arguments                | Description                                              | Example               |
|-----------------|--------------------------|----------------------------------------------------------|-----------------------|
| SetFace         | wrestlerId               | Sets the given wrestler to 'face' alignment.             | `SetFace:1`           |
| SetHeel         | wrestlerId               | Sets the given wrestler to 'heel' alignment.             | `SetHeel:1`           |
| SetRealEnemy    | wrestlerId1, wrestlerId2 | Sets the given wrestlers to 'real enemy' relationship.   | `SetRealEnemy:1:2`    |
| SetStoryEnemy   | wrestlerId1, wrestlerId2 | Sets the given wrestlers to 'story enemy' relationship.  | `SetStoryEnemy:1:2`   |
| SetRealFriend   | wrestlerId1, wrestlerId2 | Sets the given wrestlers to 'real friend' relationship.  | `SetRealFriend:1:2`   |
| SetStoryFriend  | wrestlerId1, wrestlerId2 | Sets the given wrestlers to 'story friend' relationship. | `SetStoryFriend:1:2`  |
| SetRealNeutral  | wrestlerId1, wrestlerId2 | Removes the relationship between the given wrestlers.    | `SetRealNeutral:1:2`  |
| SetStoryNeutral | wrestlerId1, wrestlerId2 | Removes the relationship between the given wrestlers.    | `SetStoryNeutral:1:2` |
| PlayAudio       | audioName/Id             | Plays the given crowd audio.                             | `PlayAudio:Cheer`     |

Commands and names are case-insensitive.

### Special Arguments
`$name#`, `@him/his/etc#`, `$promotion#`, `$$belt(#1)_(#2)`, `$$champ(#1)_(#2)`, `$$moveFront(#1)_(#2)`, `$$moveBack(#1)_(#2)`, `$$moveGround(#1)_(#2)`, `$$moveAttack(#1)_(#2)`, `$$moveCrush(#1)_(#2)`, `$$taunt(#1)_(#2)` are special arguments that will be replaced with the corresponding value.

<deflist type="medium">
<def title="$name#">
The name of the character with the corresponding id.
</def>
<def title="@him/his/etc#">
The pronoun  id, e.g. <code>@his1 friend</code> -> <code>his friend</code> or <code>her friend</code> depending on wrestler #'s gender.
Supported pronouns are <code>He, he, His, his, Male, male, Man, man, Guy, guy, Boy, boy</code>
</def>
<def title="$promotion#">
The name of promotion #.
</def>
<def title="$$belt(#1)_(#2)">
The name of promotion #1's belt #2.
</def>
<def title="$$champ(#1)_(#2)">
The name of promotion #1's champion of belt #2.
</def>
<def title="$$moveFront(#1)_(#2)">
The name of character #1's front move number #2.
</def>
<def title="$$moveBack(#1)_(#2)">
The name of character #1's back move number #2.
</def>
<def title="$$moveGround(#1)_(#2)">
The name of character #1's ground move number #2.
</def>
<def title="$$moveAttack(#1)_(#2)">
The name of character #1's attack move number #2.
</def>
<def title="$$moveCrush(#1)_(#2)">
The name of character #1's crush move number #2.
</def>
<def title="$$taunt(#1)_(#2)">
The name of character #1's taunt number #2.
</def>
</deflist>

The following moves are supported:

| Move Type | 0       | 1               | 2                  | 3             | 4               | 5             | 6                  | 7                  | 8           | 9         | 10            | 11          | 12          | 13       | 14      | 15     | 16  |
|-----------|---------|-----------------|--------------------|---------------|-----------------|---------------|--------------------|--------------------|-------------|-----------|---------------|-------------|-------------|----------|---------|--------|-----|
| Front     | Special | Attack + Up     | Attack + Centre    | Attack + Side | Attack + Down   | Run + Up      | Run + Centre       | Run + Side         | Run + Down  | Pick + Up | Pick + Centre | Pick + Side | Pick + Down | Momentum | Running | Flying | ??? |
| Back      | Special | Attack + Centre | Attack + Direction | Run + Centre  | Run + Direction | Pick + Centre | Pick + Direction   | Rear Running       | Rear Flying |           |               |             |             |          |         |        |     |
| Ground    |         | Head x Attack   | Head x Run         | Head x Pick   | Legs x Attack   | Legs x Run    | Legs x Pick        |                    |             |           |               |             |             |          |         |        |     |
| Attack    |         | Upper Attack    | Lower Attack       | Big Attack    | Running Attack  | Flying Attack | Middle Rope Attack | Springboard Attack | Tope Attack |           |               |             |             |          |         |        |     |
| Crush     |         | ???             | Stomp              | Big Crush     | Running Crush   | Flying Crush  | Middle Rope Crush  | Springboard Crush  | Tope Crush  |           |               |             |             |          |         |        |     |
| Taunt     |         | Entrance        | Taunt              | Special       | Celebration     |               |                    |                    |             |           |               |             |             |          |         |        |     |
