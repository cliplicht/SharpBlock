using SharpBlock.API.Events;

namespace SharpBlock.API.Core;

public interface IServer
{
    void RegisterEvent<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
    void SendMessageToAll(string message);
}