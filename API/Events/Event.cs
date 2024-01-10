namespace WECCL.API.Events;

public enum EventState
{
    Before,
    AfterSuccess,
    AfterFailure
}

public abstract class Event
{
    public EventState State { get; }
    
    public Event(EventState state)
    {
        State = state;
    }
}