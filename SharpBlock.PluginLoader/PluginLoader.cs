using SharpBlock.API.Core;

namespace PluginLoader;

public class PluginLoader
{
    private readonly List<IPlugin> _plugins = new();

    public void LoadPlugins(string pluginsDirectory)
    {
        
    }

    public void EnablePlugins()
    {
        foreach (var plugin in _plugins)
        {
            plugin.OnEnable();
        }
    }

    public void DisablePlugins()
    {
        foreach (var plugin in _plugins)
        {
            plugin.OnDisable();
        }
    }
}