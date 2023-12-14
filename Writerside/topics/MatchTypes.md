# ⚙️ Match Types

## Methods
`CustomMatch.RegisterCustomPreset(string Name, bool PositiveValue)` will return you a match preset ID. Adding match preset to the positive end will also make them appear in career matches.
`Content.CustomMatch.RegisterCustomCage(string Name, bool PositiveValue)` will return you a cage ID.

## Example
```C#
using WECCL.API;

public class Plugin : BaseUnityPlugin
{
    static int MyMatchPresetID;
    static int MyCageID;

    void Awake()
    {
        MyMatchPresetID = CustomMatch.RegisterCustomPreset("My Match Preset", true);
        MyCageID = CustomMatch.RegisterCustomCage("My Cage", true);   
    }
}
```

## Notes
The functions are meant to return a unique ID so mods don't conflict with each other.  
You'll still actually need to create the match type and cage in the game.