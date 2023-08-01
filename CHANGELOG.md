# Changelog

## GV 1.59:

### Release 1.4.4
- Fixed invalid flesh not being reset correctly.

### Release 1.4.3
- Fixed custom content remapping.
- Fixed launch count not incrementing on first launch.

### Release 1.4.2
- Support for surprise promo entrants and multiple consecutive promos.
- Data collection fixes and path masking for file paths.
- Proper save remapping for new materials added in GV 1.59.
- Support for remapping saves that are multiple versions behind (up to 1.55).

### Release 1.4.1
- Hotfix for promos not working.

### Release 1.4.0
- UPDATE TO GAME VERSION 1.59

## GV 1.58:

### Release 1.3.11
- Custom music now has names instead of IDs in the editor.
- Added anonymous data collection for exceptions and crashes (this can be disabled in the config).

### Release 1.3.10
- More fixes for search menu and booking career.

### Release 1.3.9
- Fixed the owner of booking career being set to a random wrestler if the original owner is selected as booker.

### Release 1.3.8

- Added manual overrides for custom belt textures which are copied from other belts in vanilla.
- Added experimental config option to allow WECCL to use the full resolution textures without downscaling them; UseFullQualityTextures.
- Added a wrestler search screen to the roster menu.

### Release 1.3.7

- Fixed the 'Special FX' setting being set to 0 when using vanilla arenas.

### Release 1.3.6

- Fixed performance issues for large matches.

### Release 1.3.5

- Fixed announcer AI always being disabled.

### Release 1.3.4

- Added an option to disable announcer AI in custom arenas.
- Added climbable cage support for custom arenas.
- Better error handling for importing characters.
- Fixed some issues with importing characters.
- I blame Mat Dickie for the issues with importing characters, despite the fact that it's clearly my fault. I'm sure he'
  ll understand.
- Sorry, Mat. I love you.
- Furthermore, fixed the changelog formatting. Header sizes are now correct.

### Release 1.3.3

- Improved character importing logic.

### Release 1.3.2

- Added support for pyro and weapon positioning in custom arenas.
- Added titantron camera auto-rotation.

### Release 1.3.1

- Fixed custom furniture positions.
- Fixed warning menu.
- Fixed some other incorrect mappings that may have caused issues.

### Release 1.3.0

- UPDATE TO GAME VERSION 1.58
- Added more arena features.
- Better Game Version checking.
- Added pre-release warnings.

## GV 1.56:

### Release 1.2.5

- Added more promo options.
- Added the option to use names instead of IDs for promo taunts.
- Added the option to use file aliases for custom assets.

### Release 1.2.4

- `characters` in promos is now required to be a list, or an integer prefixed by `:` for a list from 1 to the specified
  number.
- Fixed Thunderstore version removing mappings.
- Fixed female face mappings.
- Added exceptions to the meta and mappings file loaders.

### Release 1.2.3

- Fixed arenas for Thunderstore.
- Audio clips that are too big will no longer be attempted to be cached (due to the 2GB limit of byte arrays).
- Save file game version conversion hairstyle material fix.

### Release 1.2.2

- Added experimental support for doors.
- Increased default location size to 9999 (from 100).
- Fixed some debug rendering issues.
- Fixed cache not correctly updating for overridden assets.

### Release 1.2.1

- Added an exception when using an unsupported game version.
- Added checksums to cached audio clips.

### Release 1.2.0

- UPDATE TO GAME VERSION 1.56
- Added support for costume meshes.
- Added support for custom promos.
- Better character exporting and importing. Note: This will break compatibility with older versions of the mod. Future
  versions will aim to be backwards compatible.
- Exported characters now have the `.character` extension instead of `.json`, and `Meta` and `ContentMappings` now have
  the `.meta` and `.mappings` extensions respectively.
- Game version is now stored in the content mappings to prevent issues with loading content from older versions of the
  game.
- Experimental support for custom arenas.

## GV 1.55:

### Release 1.1.8

- Fixed characters not being able to exported if their name contains characters that are not allowed in file names.

### Release 1.1.7

- Patch to show stack traces.

### Release 1.1.6

- Fix for roster sizes being incorrect.

### Release 1.1.5

- Added in-game loading bar and information about the current content being loaded.
- Fixed fed roster size limit not being applied correctly.
- Fixed assets not being loaded from the root folder if the mod is installed via Thunderstore.

### Release 1.1.4

- Added caching of loaded audio clips to improve performance.

### Release 1.1.3

- Renamed `CustomContentSaveData` to `ContentMappings` and `CustomConfigSaveData` to `MetaFile` to better reflect their
  purpose.
- Added placeholders in code for automatic remapping of custom content after game updates.
- Added last used game version to MetaFile.

### Release 1.1.2

- Fixed arm materials.

### Release 1.1.1

- Added missing audio overrides.

### Release 1.1.0

- Resource overrides for textures with duplicate names can now be prefixed by the GameObject name (
  e.g. `Steps_Texture.png`).
- Fixed pivots not being set correctly for some assets.
- Fixed an exception due to texture rectangle sizes not being rounded.

### Release 1.0.6

- Fix for priority screen proceed button not working.

### Release 1.0.5

- Loaded folders are now logged using their full path.
- Fixed and added back the warning about the game's save file being backed up on first launch.
- Fixed the loading bars not showing up in the log.

### Release 1.0.4

- Fixed prefix priority screen.

### Release 1.0.3

- Temporarily removed the warning about the game's save file being backed up on first launch.

### Release 1.0.2

- Added a GUI to set the priority of override mods in case of conflicts.
- Added the option to include metadata files with assets.
- Added a warning to manually back up the game's save file before running any mods on first launch.

### Release 1.0.1

- Fix for theme count being incorrect when using multiple mods.
- Change CHANGELOG.txt to CHANGELOG.md

### Release 1.0.0

- Initial Thunderstore release.
- Added duplicate import prevention.
- Added base fed limit to the config.

### Beta 1.1.4

- Better support for overriding textures.
- Code cleanup..

### Beta 1.1.3

- Added automatic backups of the game's save file.
- Fixed federation editor menu sizes being affected by the roster size.
- Some code documentation.

### Beta 1.1.2

- Fixed music remapping breaking if no custom music is present.

### Beta 1.1.1

- The game's log is now enabled by default. Info level is disabled by default, but can be enabled in the config.
- Fixed loading order of imported characters with custom assets.

### Beta 1.1.0

- Added support for increasing the character and roster limit.
- Added support for adding and merging custom characters instead of replacing them.

### Beta 1.0.2

- Fixed content map incorrectly loading.
- Removed the extra `_` requirement for asset prefixes.

### Beta 1.0.1

- Added icon.
- Added basic configuration options.
- Better Thunderstore mod support.
- Various improvements and bug fixes.

### Beta 1.0.0

- Initial release.
