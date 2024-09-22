# Changelog

### Release 1.11.0
- Promos can now be added through code using the API.
- Meshes should work correctly again.
- Better support for doors in custom arenas.

### Release 1.10.8
- Looks like custom characters with custom textures have been broken for a long time. Why didn't anyone tell me? Well, it's fixed now. I'm sorry for the inconvenience.
- Mat Dickie, if you're reading this, not to point fingers, but if only you didn't make special textures have negative values in the texture index, this wouldn't have happened.
- Duplicate acceptable config values are now removed to prevent issues with changing settings.

### Release 1.10.7
- Improved handling changing settings with large values in the mod settings tab.
- Fixed some issues with adding/removing characters.

### Release 1.10.6
- Fixed the new Court location being inaccessible.

### Release 1.10.5
- The number of loaded content and code mods is now shown in the main menu.
- Fixed an issue with certain custom costume parts not being loaded correctly.
- Fixed issues with loading old save files.

### Release 1.10.4
- Fixed an issue where caches are still being created even if caching is disabled.

### Release 1.10.3
- Added fast travel to the pause menu.
- Added promo variable: `$team#1`.
- fence_climbables now allow you to control your movement while climbing.
- Fixed some issues with animations.

### Release 1.10.2
- Fixed some issues with the new backup system.

### Release 1.10.1
- Fixed character remapping not working correctly.
- Fixed an issue with saving.
- Backups now contain mappings and meta files.
- Backups are now created whenever the game is saved (before writing the save file), instead of only when the game is launched.
- The saving issue was caused by Mat. I'm sure of it. He's a great guy, but he's always up to something. I'm watching you, Mat.

### Release 1.10.0
- UPDATE TO GAME VERSION 1.65

### Release 1.9.4
- Added support for new furniture and weapon types that were added in newer versions of the game.
- Renamed `Bannana` weapon type to `Banana`. The old name will no longer work! (I'm assuming nobody used it yet anyway.)

### Release 1.9.3
- Added promo variables: `$location`, `$prop#1`, `$stat#1_#2`, `$match`, and `$date(#1)`.
- Promo variables with 2 arguments no longer require two '$' symbols (e.g. `$stat#1_#2` instead of `$$stat#1_#2`). The old format is still supported, but may be removed in the future.

### Release 1.9.2
- Negative move speed (third argument of `SetAnimation`) now copies the default speed (set by `TransitionFrames`) instead.
- Renamed `StartAnimation` to `WindUp` in custom moves (since it's not actually the start of the animation).

### Release 1.9.1
- Added support for custom height limits for custom arenas.
- Barriers and fences without MeshColliders will now give a warning instead of breaking the game.

### Release 1.9.0
- Fixed the new TV Studio arena not showing up.
- Fixed crowd sign scaling with crowd size.

### Release 1.8.6
- Fixed career mode character not resetting correctly when restoring default save file.  
- Fixed custom laces.
- Content is now loaded alphabetically (folders first, then files) to make remapping more predictable and less likely to break.

### Release 1.8.5
- Fixed distant crowd not appearing.

### Release 1.8.4
- Haha, you thought I fixed the case sensitivity issue in the previous release? Well, WRONG! Turns out I only fixed it for 4 out of 6 cases. Now the issue is fixed for real, I hope. 
- Mat Dickie, if you're reading this, remember that it's not 2003 anymore. We have lists now. You don't need to use damn arrays for everything. I'm sure you're a great guy, but please, for the love of god, use lists. I'm begging you.

### Release 1.8.3
- Fixed an issue with restoring the default save file if the roster size is smaller than the default roster size.
- It is no longer possible to delete the last character in all rosters.
- Fixed an issue from the previous release where overrides would not be loaded due to case sensitivity, for real this time. The last fix only fixed the issue for ¼ of the cases.

### Release 1.8.2
- Fixed an issue from the previous release where overrides would not be loaded due to case sensitivity.

### Release 1.8.1
- Files will no longer be cached if less than 10 GB of free space is available on the drive.
- The cache folder is no longer hidden (that was a bad idea).
- The persistent data path (used for storing the cache and mappings) can now be changed in the config.
- Overrides are case-insensitive now.

### Release 1.8.0
- Fixed the new arena introduced in GV 1.62 being broken.

### Release 1.7.14
- Fixed an IndexOutOfRangeException when exporting characters.

### Release 1.7.13
- Fix for custom buttons.

### Release 1.7.12
- Fixed fed limits not being applied correctly.

### Release 1.7.11
- Fixed an error when deleting a character from the character search screen.
- Incorrect costume indexes will now only be fixed if out of bounds; mismatching indexes will not be fixed, as they are
  likely intentional.

### Release 1.7.10
- Fixed character importing issues.
- Added support for custom buttons in the mod config menu.
- Added a "reset imported characters" button to the mod config menu to clear the imported characters list.

### Release 1.7.9
- Added WECCL events to the mod API for other mods to listen to.
- Currently, the only events are CharacterEvents when a character is added or removed.
- Renamed Wrestling Empire Custom Content Loader to WECCL in logs.
- Log messages now show the source of the log message.
- The loading bar now has different colors per content type.
- Fixed the prefix priority screen.
- The prefix priority screen and warning screen now support controllers properly.
- Added a warning screen whenever an exception occurs.
- Fixed a bug where the contents of the first menu in the config editor would be copied to the previously selected menu.
- Question of today: How do you pronounce WECCL?
- A) Weckle
- B) Weasel
- C) W E C C L
- D) Maybelline
- Answer: I don't care. I'm just happy you're using my mod. <3

### Release 1.7.8
- Fixed an infinite loop in the mod config menu.

### Release 1.7.7
- You can now right click the music menu in the editor to show a list to select from.
- Editor lists (moves, music) can now be scrolled with the mouse wheel.

### Release 1.7.6
- Some tweaks to the save file repairer.
- Mod options are now sorted alphabetically.
- The mod options menu now remembers the last selected tab.
- Fixed some issues with mod option menu.

### Release 1.7.5
- Fixed an issue with custom themes not being stored in the save file correctly.
- Custom themes are now sorted alphabetically in the editor.
- Fixed aliases not working.
- `Assets`, `Overrides`, and `Import` folders no longer get created in the WECCL folder since this is redundant.

### Release 1.7.4
- Generated characters from the character search screen now work correctly.
- The main menu now shows the WECCL version.

### Release 1.7.3
- Promos can now be assigned to categories, including vanilla categories.
- Promos can now be set to play during wrestler career mode.

### Release 1.7.2
- Fixed some issues with the save file repairer and importing characters.

### Release 1.7.1
- Fixed a bug where moving to the roster editor from the character search screen would cause the game to softlock.

### Release 1.7.0
- Added support for custom strike and grapple moves.
- Added support for adding and removing characters from the search screen.
- Deleted characters are exported to the `Purgatory` folder after saving.
- Separated the modded save file from the vanilla save file (can be changed in the config).
- FindAndProcessCrowdObjects now work on any custom arena.
- Fixed some issues with using controllers in the character search screen.
- Stack traces are now enabled through Unity instead of a patch.
- Improved broken save file detection and repair.
- Fixed a bug where the Legends roster size wouldn't be expanded correctly.

### Release 1.6.2
- Fixed mappings for custom costumes. The system has been reworked to automatically detect future changes to the game's
  vanilla costume count.
- Restore default save file will no longer break if custom characters are in the high score list (any custom characters
  in the high score list will be replaced by the first default character).
- Note that Ingo will come to your house and steal your socks if you don't properly back up your save file. You have been
  warned.
- P.S. Ingo is very nice and would never do such a thing. But still, back up your save file. Or else...

### Release 1.6.1
- Broken save files will now be attempted to be repaired on launch.
- Added support for mentioning moves in custom promos.
- Added support for additional crowd and signs in custom arenas.
- Various improvements to custom arena support.

### Release 1.6.0
- UPDATE TO GAME VERSION 1.60
- Added door logic to custom arenas.
- Improved fence/barrier logic for custom arenas.

## GV 1.59:

### Release 1.5.0
- Fixed a lot of character importing issues.

### Release 1.4.5
- Some fixes for Steam Workshop support.

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
