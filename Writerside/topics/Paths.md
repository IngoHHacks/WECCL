# 📂 Paths

<show-structure for="chapter" depth="2"/>

<link-summary>
Paths used by WECCL.
</link-summary>

## Root Path
The root path (referred to by `.` in the documentation) depends on the context.
When installing content manually, the path is:  
`[Wrestling Empire]\BepInEx\plugins`.  
When installing content via Steam Workshop or Thunderstore, the path is:  
`[Wrestling Empire]\BepInEx\plugins\Manual`.  
When uploading content to Steam Workshop or Thunderstore, the path is:  
`[Mod Root]\plugins`.  

`[Wrestling Empire]` is the folder where Wrestling Empire is installed, found by right-clicking the game in Steam and selecting "Manage" -> "Browse local files".  
`[Mod Root]` is the folder where the mod is packaged to. It is the folder where the `manifest.txt` or `manifest.json` file is located.

## Content Paths
All content folders are searched recursively, so you can place them anywhere inside the root path.  
Note that the `Manual` folder isn't necessarily required. However, since Steam Workshop may modify the content, it is recommended to use the `Manual` folder to avoid potential issues.
Content inside folders also is searched recursively unless stated otherwise.

### Mod Paths
`./Assets` is the content folder.

`./Overrides` is the overrides folder.

`./Import` is used to import wrestlers.

`./Libraries` can be used to load DLLs used by custom arenas.

### Data Paths
`./Export` stores all wrestlers currently in the save file.

`./Purgatory` stores deleted wrestlers.

`./Debug` is a folder used for debugging. The default configuration will not use this folder.

`./Data` stores data used by WECCL itself. Leave this folder alone.