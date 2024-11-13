namespace SharpBlock.Options;

public class WorldsOptions
{
    public const string Worlds = "Worlds";
    public string Name { get; set; } = "world";
    public bool IsHardcore { get; set; }
    public byte GameMode { get; set; }
    public int ViewDistance { get; set; }
    public int SimulationDistance { get; set; }
    public bool ReduceDebugInfo { get; set; }
    public bool EnableRespawnScreen { get; set; }
    public bool IsDebug { get; set; }
    public bool IsFlat { get; set; }
    public Dimension[] Dimensions { get; set; } = null!;
}
