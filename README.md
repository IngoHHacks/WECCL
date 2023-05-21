# WECCL

Wrestling Empire Custom Content Loader

Loads custom content for Wrestling Empire.

## Current Features

It is currently possible to add:

- Custom music (themes)
- Custom costume textures
- Custom costume meshes
- Custom promos

It is currently possible to override:

- All music (themes)
- All costume textures
- All sound effects
- All world textures

Additional features:

- Automatic remapping of custom content
- Importing and exporting characters

Experimental features:

- Custom arenas

## Getting Started

### Installation (Automatic)

This is the recommended way to install the mods.

- Download and install [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager) or [r2modman](https://github.com/ebkr/r2modmanPlus/releases)
- Click the `Install with Mod Manager` button on top of the [page](https://thunderstore.io/c/wrestling-empire/p/IngoH/WECCL/) (or install it through the mod manager directly)
- Run the game via the mod manager

### Installation (Manual)

To install this mod manually, you first need to install BepInEx 5.4.21 as a mod loader for Wrestling Empire. A guide for this can be found [here](https://docs.bepinex.dev/articles/user_guide/installation/index.html#where-to-download-bepinex).
It is recommended to use download BepInEx from [here](https://thunderstore.io/c/wrestling-empire/p/BepInEx/BepInExPack/) to ensure you get the correct version.

To install WECCL, you simply need to copy `WECCL.dll` from releases to `./BepInEx/plugins`.

## Important Notes

When using a mod manager, `./BepInEx` can be found with `Browse profile folder` in the mod manager's settings.
When using BepInEx manually, `./BepInEx` can be found in the game's root folder.
`Assets`, `Overrides`, etc. can be anywhere in the `plugins` folder, e.g. `./BepInEx/plugins/MyMod/Assets`.
If you're stuck, you can always ask for help in the [modding Discord](https://discord.gg/mH56AhUwPR).

## Adding Content

You can add content by inserting images and audio files into `./BepInEx/plugins/Assets`. Any audio file
here will be considered a theme. Image files must either be in a subfolder or prefixed by the costume texture name.  
Example: `body_material_abc.png` or `body_material/abc.png` (`_` before 01 is not required; `abc` can be anything)

| Supported Texture Names  |
|--------------------------|
| arms_elbow_pad           |
| arms_flesh               |
| arms_glove               |
| arms_material            |
| arms_wristband           |
| body_collar              |
| body_flesh_female        |
| body_flesh_male          |
| body_material            |
| body_pattern             |
| face_beard               |
| face_female              |
| face_male                |
| face_mask                |
| hair_shave               |
| hair_texture_solid       |
| hair_texture_transparent |
| legs_flesh               |
| legs_footwear            |
| legs_kneepad             |
| legs_laces               |
| legs_material            |
| legs_pattern             |

### Metadata

Metadata files can be used to add additional information to assets, e.g. skin tone. Metadata files must be named the same as the asset, but with the `.meta` extension.
Example: `abc.png` and `abc.meta`.
Meta files must be a newline-separated list of key-value pairs in the format `key: value` (space is optional).
The following keys are supported:

| Key | Description                                                                                                                                                                                                                                                                       |
|-----|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| skin_tone | The skin tone of the character. Must be formatted as `r,g,b` (e.g. `1.0, 0.0, 0.0`) or an HTML string (e.g. `#FF0000` or `red`). `1.0, 1.0, 1.0` is the default skin tone (white). `0.75, 0.75, 0.75` and `0.5, 0.45, 0.3` are the two other skin tones used in the vanilla game. |

## Meshes

As with textures, meshes can be added by placing them in `./BepInEx/plugins/Assets`. Meshes must be in a subfolder by the mesh name.
Example: `body_mesh/abc`. The extension must be `.mesh` or no extension at all.
Meshes should be inside an asset bundle with it being the only mesh in the bundle. The first submesh will be the one affected by the game's mesh color setting. Others can be manually set in the metadata file.

| Supported Mesh Names       |
|----------------------------|
| arms_shape                 |
| body_shape                 |
| face_headwear              |
| face_shape                 |
| hair_extension             |
| hair_hairstyle_solid       |
| hair_hairstyle_transparent |
| legs_shape                 |

### Metadata

Metadata files can be used to add additional information to meshes, e.g. mesh color. Metadata files must be named the same as the mesh, but with the `.meta` extension.
Example: `abc` and `abc.meta`. 
Meta files must be a newline-separated list of key-value pairs in the format `key: value` (space is optional).
The following keys are supported:

| Key           | Description                                                                                                                                                                                                         |
|---------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| scale         | The scale of the mesh. Must be formatted as `x,y,z` (e.g. `1.0, 1.0, 1.0`). `1.0, 1.0, 1.0` is the default scale.                                                                                                   |
| position      | The position of the mesh. Must be formatted as `x,y,z` (e.g. `0.0, 0.0, 0.0`). `0.0, 0.0, 0.0` is the default position.                                                                                             |
| rotation      | The rotation of the mesh. Must be formatted as `x,y,z` (e.g. `0.0, 0.0, 0.0`). `0.0, 0.0, 0.0` is the default rotation.                                                                                             |
| submeshXcolor | The color of submesh X. Must be formatted as `r,g,b` (e.g. `1,0, 0.0, 0.0`) or an HTML string (e.g. `#FF0000` or `red`). Overriding submesh 0 is not supported, as it is affected by the game's mesh color setting. |

### Custom Arenas

**Custom arenas are very experimental and may not work as intended. Use at your own risk.**  
Custom arenas work the same as meshes, but require a GameObject as the root object and should be placed in a subfolder named `arena`.  
Example: `arena/abc`. The extension must be `.mesh` or no extension at all (Please note that it is still not actually a mesh, but a GameObject. This will be changed in the future).  
There is functionality in place to automatically assign collision to the arena, but this doesn't work for diagonal walls, i.e. walls that are not aligned with the X or Z axis. It may also not work as expected with some wall shapes.

## Custom Promos

You can add custom promos by placing a .promo file inside `./BepInEx/plugins/Assets`. The file must contain metadata in the format `key: value` (space is optional), and newline-separated dialog lines in the format `"line1","line2",speaker,target(,taunt,demeanor,commands)`.
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
`title` must be a string.  
`description` must be a string. [P1] and [P2] will be replaced with the names of the characters.  
`characters` must be an comma-separated list of integers, or an integer prefixed by `:`. When using a prefixed integer, an array from 1 to that integer will be created. When using a list, the list will be used as the array.  
1 and 2 are the default characters as selected by the user, 3 is another character, -1 is the referee, and 11 and 22 are the tag team partners of 1 and 2 respectively. Other values are not supported.  
`"line1"` and `"line2"` must be strings. The quotes are required. For quotes inside the string, use `\"`.  
`$name#` will be replaced with the name of the character with the corresponding id.  
`@him/his/etc#` will be replaced with the pronoun of the character with the corresponding id, e.g. `@his1 friend` -> `his friend` or `her friend` depending on wrestler #'s gender. Supported pronouns are `He, he, His, his, Male, male, Man, man, Guy, guy, Boy, boy`.  
`$promotion#`, `$$belt(#1)_(#2)`, `$$champ(#1)_(#2)` will be replaced with the name of promotion #, the name of promotion #1's belt #2, and the name of promotion #1's champion of belt #2 respectively.
`speaker` must be an integer.
`target` must be an integer.
`taunt` must be a string or integer. A list of taunts can be found in `TauntAnims.md`.
`demeanor` must be an integer. A positive value will make the character happy for the given number of frames, and a negative value will make the character angry for the given number of frames.
`commands` must be a list in the format `command:arg1:arg2:arg3...`. Commands are separated by a semicolon. Example: `SetFace:1;SetRealFriend:1:2`.
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

## Overriding Content

You can override content by placing any image or audio file inside `./BepInEx/plugins/Overrides`
referenced by the internal name. A zip file containing all the overridable files can be found in
the [Modding Discord](https://discord.gg/mH56AhUwPR)

## Exporting Characters

Characters are automatically exported to `./BepInEx/plugins/Export` when the game is saved.

## Importing Characters

Characters can be imported by placing a JSON file inside `./BepInEx/plugins/Import`. Make sure to set
the `overrideMode` property to the desired mode.  
`append` will add the imported character.  
`override` will override the character with the same `id`.  
`merge` will replace all non-default values of the character referenced by the `id`.
Imported JSON files will be automatically deleted after the game is saved (unless the config
option `DeleteImportedCharacters` is set to `false`).

## Uploading mods

You can upload your mods on [Thunderstore](https://thunderstore.io/c/wrestling-empire/create/). In order to do this, you first need to connect either your Discord or GitHub account to Thunderstore and create a team in your account settings (if you haven't already).
Thunderstore uses the following format:
```
📁ModName.zip
 ┣📂plugins (required if you want your mod to actually do something)
 ┃  ┣📂Assets
 ┃  ┃ ┣📂face_male
 ┃  ┃ ┃ ┗🖼️face.png
 ┃  ┃ ┗🔊theme.mp3
 ┃  ┗📂Overrides
 ┃    ┣🖼️Fed01.jpg
 ┃    ┗🔊Theme00.ogg
 ┣🖼️icon.png (required, 256x256)
 ┣📃manifest.json (required)
 ┣📃README.md (required)
 ┗📃CHANGELOG.md (optional)
 
 * The files in Assets and Overrides are just examples. You can have any number of files with any name and valid extension in these folders. Neither of them is required.
```
`plugins` is a folder which contains all the files that your mod needs to function.
`icon.png` is a 256x256 PNG file which will be used as the icon for your mod on Thunderstore.
`manifest.json` is a JSON file which must contain the following:  
```json
{
  "namespace": "IngoH",
  "name": "NameOfMod",
  "description": "This is a description of the mod.",
  "version_number": "1.0.0",
  "dependencies": [
    "BepInEx-BepInExPack-5.4.2100",
    "IngoH-WECCL-(LatestVersion)"
  ],
  "website_url": "(Add GitHub link here, or leave empty)"
}
```
`namespace` must be your team name on Thunderstore.
`name` may only contain alphanumeric characters and underscores (A-Z, a-z, 0-9, _).
`version_number` must be in [semver](https://semver.org/) (semantic versioning) format.
`dependencies` is a list of dependencies. You can find the dependencies of mod on the Thunderstore page of the mod, listed under "Dependency string".
`website_url` should be empty, or a link to the GitHub page of the mod or any other page where users can find more information about the mod.

`README.md` is a [Markdown](https://www.markdownguide.org/cheat-sheet/) file that will be displayed on the mod page. It should contain the description of the mod, and any additional information that might be useful.  
`CHANGELOG.md` is also a Markdown file that will be displayed on the mod page if it exists. It should contain a list of changes for each version of the mod.

## Modding Discord

Join the [modding Discord](https://discord.gg/mH56AhUwPR) to receive support and talk with other modders and content
creators!

## Donations

Donations are always appreciated. You can donate on my [Ko-fi page](https://ko-fi.com/IngoH).  

## Contributors
**IngoH**
**GamingMaster**
**Street**

## Special Thanks
**Mat Dickie** for generously donating €1,000  
**All the Discord Testers**  
**Everyone who published mods using WECCL**
