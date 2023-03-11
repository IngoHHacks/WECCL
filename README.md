# WECCL 
Wrestling Empire Custom Content Loader

Loads custom content for Wrestling Empire.

## Current Features
It is currently possible to add:
- Custom music (themes)
- Custom costume textures

It is currently possible to override:
- All music (themes)
- All costume textures
- Most sound effects
- Most world textures

Additional features:
- Automatic remapping of custom content
- Exporting characters as JSON files

Experimental features:
- Importing characters from JSON files (very experimental, likely to cause issues)

## Getting Started

### Installation (Automatic)
Automatic installation is not supported until the game is added to Thunderstore. This will happen in the upcoming weeks.

### Installation (Manual)
To install this mod manually, you first need to install BepInEx as a mod loader for Wrestling Empire. A guide for this can be found [here](https://docs.bepinex.dev/articles/user_guide/installation/index.html#where-to-download-bepinex). Wrestling Empire needs the x64 Mono version.

To install WECCL, you simply need to copy `WECCL.dll` from releases to `Wrestling Empire/BepInEx/plugins`.

## Adding Content
You can add content by inserting images and audio files into `Wrestling Empire/BepInEx/plugins/Assets`. Any audio file here will be considered a theme. Image files must either be in a subfolder or prefixed by the costume texture name.  
Example: `body_material_abc.png` or `body_material/abc.png` (`_` before 01 is required; `abc` can be anything)

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

## Overriding content
You can override content by placing any image or audio file inside `Wrestling Empire/BepInEx/plugins/Overrides` referenced by the internal name. A zip file containing all the overridable files can be found in the [Modding Discord](https://discord.gg/mH56AhUwPR)

## Exporting Characters
Characters are automatically exported to `Wrestling Empire/BepInEx/plugins/Exports` when the game is saved.

## Importing Characters
Characters can be imported by placing a JSON file inside `Wrestling Empire/BepInEx/plugins/Imports`. Make sure to set the `overrideMode` property to the desired mode.  
`append` will add the imported character.  
`override` will override the character with the same `id`.  
`merge` will replace all non-default values of the character referenced by the `id`.
Imported JSON files will be automatically deleted after the game is saved (unless the config option `DeleteImportedCharacters` is set to `false`).

## Modding Discord
Join the [Modding Discord](https://discord.gg/mH56AhUwPR) to receive support and talk with other modders and content creators!

## Donations
Donations are always appreciated. You can donate on my [Ko-fi page](https://ko-fi.com/IngoH).
