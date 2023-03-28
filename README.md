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

This is the recommended way to install the mods.

- Download and install [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager) or [r2modman](https://timberborn.thunderstore.io/package/ebkr/r2modman/)
- Click the `Install with Mod Manager` button on top of the [page](https://thunderstore.io/c/wrestling-empire/p/IngoH/WECCL/) (or install it through the mod manager directly)
- Run the game via the mod manager

### Installation (Manual)

To install this mod manually, you first need to install BepInEx 5.4.21 as a mod loader for Wrestling Empire. A guide for this
can be found [here](https://docs.bepinex.dev/articles/user_guide/installation/index.html#where-to-download-bepinex).
Wrestling Empire needs the x64 Mono version.

To install WECCL, you simply need to copy `WECCL.dll` from releases to `Wrestling Empire/BepInEx/plugins`.

## Adding Content

You can add content by inserting images and audio files into `Wrestling Empire/BepInEx/plugins/Assets`. Any audio file
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

| Key | Description                                                                                                                                                                                                                           |
|-----|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| skin_tone | The skin tone of the character. Must be formatted as `r,g,b` (e.g. `0.5, 0.5, 0.5`). `1.0, 1.0, 1.0` is the default skin tone (white). `0.75, 0.75, 0.75` and `0.5, 0.45, 0.3` are the two other skin tones used in the vanilla game. |

## Overriding content

You can override content by placing any image or audio file inside `Wrestling Empire/BepInEx/plugins/Overrides`
referenced by the internal name. A zip file containing all the overridable files can be found in
the [Modding Discord](https://discord.gg/mH56AhUwPR)

## Exporting Characters

Characters are automatically exported to `Wrestling Empire/BepInEx/plugins/Export` when the game is saved.

## Importing Characters

Characters can be imported by placing a JSON file inside `Wrestling Empire/BepInEx/plugins/Import`. Make sure to set
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

## Special Thanks
**Mat Dickie** for generously donating €1,000  
**All the Discord Testers**  
**Everyone who published mods using WECCL**
