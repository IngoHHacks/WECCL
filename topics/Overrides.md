# 🔄️ Overrides

<show-structure for="chapter" depth="2"/>

<link-summary>
How to change existing content in the game.
</link-summary>

Overrides are a way to replace existing content in the game with your own. This is useful for replacing textures, sounds, and music.  
Overrides can be added by placing files into <include from="snippets.md" element-id="opath"/>.
For adding new content, see [Adding Content](Content.md).

## Default Files

A zip file containing all the default files can be found [here](https://ingoh.net/weccl/files/WrestlingEmpire_EditableAssets.zip).  
This folder shows the structure of where to place your overrides and can be used as a reference.  
Note that files from asset bundles should be placed in a folder corresponding to the asset bundle's name. Any other files should be placed in the `resources` folder.

## Supported Formats

The same formats as new content are supported. See [Supported Image Formats](Costumes.md#supported-formats) and [Supported Audio Formats](Themes.md#supported-formats) for more information.

## Music (Themes) {id=music}

All themes are stored in the `music` folder.
Themes are named `Theme00`, `Theme01`, etc.
`Theme00` is the background music for the main menu, all other themes are wrestler themes.

## Costumes

Costumes are stored in the `costumes` folder.

## Audio (Sound Effects) {id=audio}

Audio files are stored in the `audio` folder.

## Textures

UI textures and similar are stored in the `sprites` and `resources` folders.  
World textures are stored in the `world` and `resources` folders.  
All other textures are stored in the `resources` folder.