# 📲 Events

<show-structure for="chapter" depth="2"/>

<link-summary>
API reference for adding custom events.
</link-summary>

## Namespace
`WECCL.API.Events`

## Methods
`void CharacterEvents.RegisterBeforeCharacterRemovedAction(Action<CharacterRemovedEvent> action)` registers an action to be called before a character is removed.  
`void CharacterEvents.RegisterAfterCharacterRemovedAction(Action<CharacterRemovedEvent> action)` registers an action to be called after a character is removed.  
`void CharacterEvents.RegisterBeforeCharacterAddedAction(Action<CharacterAddedEvent> action)` registers an action to be called before a character is added.  
`void CharacterEvents.RegisterAfterCharacterAddedAction(Action<CharacterAddedEvent> action)` registers an action to be called after a character is added.  
`void CharacterEvents.UnregisterBeforeCharacterRemovedAction(Action<CharacterRemovedEvent> action)` unregisters an action to be called before a character is removed.  
`void CharacterEvents.UnregisterAfterCharacterRemovedAction(Action<CharacterRemovedEvent> action)` unregisters an action to be called after a character is removed.  
`void CharacterEvents.UnregisterBeforeCharacterAddedAction(Action<CharacterAddedEvent> action)` unregisters an action to be called before a character is added.  
`void CharacterEvents.UnregisterAfterCharacterAddedAction(Action<CharacterAddedEvent> action)` unregisters an action to be called after a character is added.

## Example
```C#
using WECCL.API.Events;

public class Plugin : BaseUnityPlugin
{
    void Awake()
    {
        CharacterEvents.RegisterBeforeCharacterRemovedAction((CharacterRemovedEvent e) =>
        {
            // Do something, e.g.
            Logger.LogInfo($"Character {e.Character.name} is about to be removed!");
        });
    }
}
```

## Event Types

### Event (Base for all events)
`EventState State`: The state of the event. Can be `Before`, `AfterSuccess` or `AfterFailure`.

### CharacterAddedEvent and CharacterRemovedEvent
`int CharId`: The ID of the character that was added or removed.  
`Character Character`: The character that was added or removed.  
`Source SourceType`: The source of the event. Always `Delete` for `CharacterRemovedEvent`. Can be `Create` or `Import` for `CharacterAddedEvent`.