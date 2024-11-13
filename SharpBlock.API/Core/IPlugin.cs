namespace SharpBlock.API.Core;

public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    void OnEnable();
    void OnDisable();
}