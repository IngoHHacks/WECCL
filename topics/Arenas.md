# 🏟️ Arenas

<show-structure for="chapter" depth="2"/>

<link-summary>
How to add custom arenas to the game.
</link-summary>

## Adding Arenas
Custom arenas require an asset bundle with a GameObject as the root object and should be placed in a subfolder named `arena`.  
Asset bundles can be created through Unity. For more information on how to create asset bundles, see [Creating Asset Bundles](AssetBundles.md).
There is functionality in place to automatically assign collision to the arena, but this doesn't work for diagonal walls, i.e. walls that are not aligned with the X or Z axis. It may also not work as expected with some other wall shapes.

## Features

### Arena Name Customization
- **Customize the arena name:** Create an object named `Arena Name:YourArenaName`. Ensure no space follows the colon. The text after the colon becomes the arena name in-game.

### Arena Shape Definition
- **Define arena shape:** To emulate the main arena's behavior, create an object named `arenaShape4`. There are options for `arenaShape3`, `arenaShape2`, and `arenaShape1`, though their AI behavior is less certain and open for experimentation.

### Barrier and Cage Creation
- **Climbable barriers:** Barriers named `Barrier_Climbables` will be treated as climbable. If grouped under a parent object, avoid using `Barrier_Climbables` in the parent's name. The mod generates climbable collision boxes for these barriers with a proper meshCollider.
- **Cage-like terrain:** For chainlink fences or similar structures, use the name `Fence_Climbables`. The mod applies cage climbing logic to these objects.

### Camera and Effects
- **Custom camera distance:** Create an object named `camDistance200` to set the camera distance to 200 (default is 135). This affects the trigger distance for name tags and entrance effects.

### Pyrotechnics
- **Pyro spawn points:** Mark pyrotechnic spawn locations with an object named `PyroSpawn`.

### Map Bounds
- **Set map bounds:** Following the Mesh Plane filters method, name the outermost Planes as `Marker (North)`, `Marker (East)`, `Marker (South)`, and `Marker (West)`. The mod uses these markers to define map boundaries.

### Item Borders
- **Item spawn boundaries:** Create empty objects named `Itemborder (North)`, `Itemborder (East)`, `Itemborder (South)`, and `Itemborder (West)` to define where weapons spawn around the ring.

### Fixed Furniture
- **Announcer Desk and Chairs:** Use `AnnouncerDeskBundle` (and `AnnouncerDeskBundle2` for multiples) to spawn a desk with office chairs at the specified location.
- **Other Furniture:** Create objects named `GameObject:FurnitureName` to spawn specific furniture items at that location. See [Furniture](Furniture.md) for a list of furniture names.

### Weapon Spawns
- **Fixed weapon spawns:** Use `WeaponObject:WeaponName` to spawn specific weapons. See [Weapons](Weapons.md) for a list of weapon names.
- Use `WeaponObject:Random` for random weapon spawns.

### Additional Options
- **Announcer AI:** To prevent announcers from leaving their seats, create an object named `AnnouncerFreeze`.
- **Titantron Camera:** For dedicated titantron cameras, name the object `TitantronCamera` for automatic rotation towards the action.