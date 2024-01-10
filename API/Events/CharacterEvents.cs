namespace WECCL.API.Events;

public static class CharacterEvents
{
    internal static Action<CharacterAddedEvent> OnBeforeCharacterAdded;
    internal static Action<CharacterRemovedEvent> OnBeforeCharacterRemoved;
    internal static Action<CharacterAddedEvent> OnAfterCharacterAdded;
    internal static Action<CharacterRemovedEvent> OnAfterCharacterRemoved;
    
    internal static void InvokeBeforeCharacterAdded(int charId, Character character, CharacterAddedEvent.Source source = CharacterAddedEvent.Source.Create)
    {
        OnBeforeCharacterAdded?.Invoke(new CharacterAddedEvent(charId, character, EventState.Before, source));
    }
    internal static void InvokeBeforeCharacterRemoved(int charId, Character character, CharacterRemovedEvent.Source source = CharacterRemovedEvent.Source.Delete)
    {
        OnBeforeCharacterRemoved?.Invoke(new CharacterRemovedEvent(charId, character, EventState.Before, source));
    }
    
    internal static void InvokeAfterCharacterAdded(int charId, Character character, CharacterAddedEvent.Source source = CharacterAddedEvent.Source.Create)
    {
        OnAfterCharacterAdded?.Invoke(new CharacterAddedEvent(charId, character, EventState.AfterSuccess, source));
    }
    
    internal static void InvokeAfterCharacterRemoved(int charId, Character character, CharacterRemovedEvent.Source source = CharacterRemovedEvent.Source.Delete)
    {
        OnAfterCharacterRemoved?.Invoke(new CharacterRemovedEvent(charId, character, EventState.AfterSuccess, source));
    }
    
    internal static void InvokeAfterCharacterAddedFailure(int charId, Character character, CharacterAddedEvent.Source source = CharacterAddedEvent.Source.Create)
    {
        OnAfterCharacterAdded?.Invoke(new CharacterAddedEvent(charId, character, EventState.AfterFailure, source));
    }
    
    internal static void InvokeAfterCharacterRemovedFailure(int charId, Character character, CharacterRemovedEvent.Source source = CharacterRemovedEvent.Source.Delete)
    {
        OnAfterCharacterRemoved?.Invoke(new CharacterRemovedEvent(charId, character, EventState.AfterFailure, source));
    }
    
    public static void RegisterBeforeCharacterAddedAction(Action<CharacterAddedEvent> action)
    {
        OnBeforeCharacterAdded += action;
    }
    
    public static void RegisterBeforeCharacterRemovedAction(Action<CharacterRemovedEvent> action)
    {
        OnBeforeCharacterRemoved += action;
    }
    
    public static void RegisterAfterCharacterAddedAction(Action<CharacterAddedEvent> action)
    {
        OnAfterCharacterAdded += action;
    }
    
    public static void RegisterAfterCharacterRemovedAction(Action<CharacterRemovedEvent> action)
    {
        OnAfterCharacterRemoved += action;
    }
    
    public static void UnregisterBeforeCharacterAddedAction(Action<CharacterAddedEvent> action)
    {
        OnBeforeCharacterAdded -= action;
    }
    
    public static void UnregisterBeforeCharacterRemovedAction(Action<CharacterRemovedEvent> action)
    {
        OnBeforeCharacterRemoved -= action;
    }
    
    public static void UnregisterAfterCharacterAddedAction(Action<CharacterAddedEvent> action)
    {
        OnAfterCharacterAdded -= action;
    }
    
    public static void UnregisterAfterCharacterRemovedAction(Action<CharacterRemovedEvent> action)
    {
        OnAfterCharacterRemoved -= action;
    }
}

public abstract class CharacterEvent : Event
{
    public int CharId { get; }
    public Character Character { get; }

    public CharacterEvent(int charId, Character character, EventState state) : base(state)
    {
        this.CharId = charId;
        this.Character = character;
    }
}

public class CharacterAddedEvent : CharacterEvent
{
    public enum Source
    {
        Create,
        Import,
    }
    
    public Source SourceType { get; }
    
    public CharacterAddedEvent(int charId, Character character, EventState state, Source source) : base(charId, character, state)
    {
        this.SourceType = source;
    }
}

public class CharacterRemovedEvent : CharacterEvent
{
    public enum Source
    {
        Delete,
    }
    
    public Source SourceType { get; }
    
    public CharacterRemovedEvent(int charId, Character character, EventState state, Source source) : base(charId, character, state)
    {
        this.SourceType = source;
    }
}