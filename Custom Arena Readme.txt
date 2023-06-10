This readme is if you want to use unity to create a custom arena and has some guideance on what you can do to get certain things in your custom arenas using WECCL.


-Customise the arena name so instead of displaying as "Location X" it can be whatever YOU the maker wants. Simply make an object called "Arena Name:Insert name you want here", make sure there is no space after the : and all the text after the : will be the arena name ingame.


-Define arena shape, if you want your arena to be treated like the main arena where wrestlers go to the ring and generally try to stay inside the ring, make an object called "arenaShape4" There is also functionality to do arenaShape3, arenaShape2 and arenaShape1. Though I am unsure on how the AI acts on these so feel free to test it.


-Make barriers that can be climbed on like the original arena barriers that can be climbed on / over. Simply make sure your barriers start with the name "Barrier_Climbables" (If you put them as a child of an object to group them, don't have "Barrier_Climbables" in the parents name) The mod will then render a climbable collision box around these objects for you as long as they have an appropriate meshCollider set.

-Also can make "Cage" like climbable terrain, eg for chainlink fences. Just name it "Fence_Climbables" and the rest of the logic will work like the barriers but with cage climbing logic.

-Set custom camera distance, make an object called camDistance200, will set the camera distance to 200 (Default arena camera distance is 135 which this mod will set it to if you don't make this object.) If you are wondering why you want this, I found this setting impacts when the name tag will appear and fireworks / smoke etc from entrances happens, the lower the camera distance, the closer to the ring the entrance effect seems to trigger. Also is just useful if you want to make small indoor maps like new backstage rooms.


-Set Pyro spawn point for smoke etc. Useful if you have made a map with a ramp or otherwise want it to trigger in a particular location. Simply make an object called "PyroSpawn" and place it where you want the pyro to appear.


-Now you can accurately set the map bounds. Following the method of making map bounds via Mesh Plane filters as described in the arena making tutorial, simply locate the most outter Planes, then name them as "Marker (North)", "Marker (East)" "Marker (South)" and "Marker (West)". North should be the marker in the direction the camera faces when setting up a match in exihbition (So the way wrestlers will traditionally enter from). The mod will then get the position of these 4 markers and use it to set the outer map bounds rather than using the default 9999 in all directions and thus this allows weapons to spawn as expected.


-You also can set Itemborder bounds, by making empty ojbects called Itemborder (North), Itemborder (East), Itemborder (South) and Itemborder (West) around the ring, this can set the outer bounds weapons will spawn around ring, nice if your map has issues with weapons spawning on or otherside of arena barriers on a more traditional stadium type of arena.


-You can define fixed furniture to spawn in your map. By default if you have a ring it will spawn 2 stairs at opposite corners like normal. What you can define yourself includes:

By making Objects in your map called "AnnouncerDeskBundle" (Can do "AnnouncerDeskBundle2" etc if you want multiple) this will spawn an announcer desk centered at the point the object is placed and also spawn the 2 office chairs at the same rotation and distance as they normally would be. Make sure to leave space for the chairs when placing this.

By making Objects in your map that start with "GameObject:" followed by the furtintue name, you can spawn that furinture in at that position and rotation too, this includes doing announcer desk without office chairs or manual placement of steel steps etc. (Example is "GameObject:Stool" and "GameObject:Stool2" if you want multiple of them) full list of valid names that spawn a furniture is below:

Table, Office Chair, Announce Desk, Steps, Desk, Bench, Ladder, Wooden Crate, Cardboard Box, Trashcan, Toilet, Bed, Snooker Table, Stool, Round Table, Barrel, Coffin, Wheelchair, Folding Chair, Vending Machine, Computer Desk, Piano


-You can also define fixed weapon spawns for your map in a very similar system to the furniture, make Objects starting with "WeaponObject:" instead of game object, otherwise the instructions are the same as for spawning furniture. The list of valid weapons is below (Please note some of this list are weapons the game has code for, but no model. So if you try to spawn a chainsaw it won't do anything, the same is likely true for a lot of the later weapons as I also tested the american football):

Belt, Microphone, Camera, Bell, Explosive, Baseball Bat, Chair, Cage Piece, Wooden Board, Table Piece, Table Leg, Barbed Bat, Cardboard, Ladder Piece, Plank, Pipe, Nightstick, Cane, Step, Dumbbell, Weight, Trashcan Lid, Skateboard, Water Bottle, Milk Bottle, Beer Bottle, Light Tube, Hammer, Console, Briefcase, Brass Knuckles, Extinguisher, Trophy, Gun, Broom, Sign, Picture, Glass Pane, Guitar, Tennis Racket, Phone, Cue, Tombstone, Cash, Burger, Pizza, Hotdog, Apple, Orange, Bannana, Crutch, Backpack, Shovel, Book, Magazine, Tablet, Thumbtacks, Football, Basketball, American Football, Baseball, Tennis Ball, Beach Ball, Tyre, Large Gift, Gift, Chainsaw, Handcuffs, Rubber Chicken

In addition, you can use "WeaponObject:Random", "WeaponObject:Random2" etc to spawn a random weapon at a fixed location (This will also sometimes spawn nothing if it randomly hits a weapon that doesn't work)

-If you have announce desk further from ring and find your announcers leave their seats to go stand at ringside, simply make an object called "AnnouncerFreeze" and the mod will disable the announcer AI so they don't leave their chairs.

Any questions try the modding discord here: https://discord.gg/mH56AhUwPR
