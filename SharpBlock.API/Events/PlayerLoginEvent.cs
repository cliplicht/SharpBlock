namespace SharpBlock.API.Events;

public class PlayerLoggedInEvent : IEvent
{
    public string PlayerName { get; set; }
}