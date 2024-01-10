# 🎛️ Options Buttons

<show-structure for="chapter" depth="2"/>

<link-summary>
API reference for adding custom buttons to the options menu.
</link-summary>

## Namespace
`WECCL.API`

## Methods
`void Buttons.RegisterCustomButton(this BaseUnityPlugin plugin, string text, Func<string> action, bool extraConfirmation = false)` registers a custom button with the given text and action.

## Example
```C#
using WECCL.API;

public class Plugin : BaseUnityPlugin
{
    void Awake()
    {
        Buttons.RegisterCustomButton("My Button", () =>
        {
            // Do something
            return "Button clicked!";
        });
    }
}
```

## Notes
- The button will be added to the 'Mods' tab in the options menu, after all the config options.
- The return value of the action will be the text of the button after it is clicked. This can't be null.
- Setting `extraConfirmation` to `true` will make the button require two clicks to activate, showing "Are you sure?" first.