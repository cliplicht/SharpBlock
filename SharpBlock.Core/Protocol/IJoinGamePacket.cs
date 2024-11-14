namespace SharpBlock.Core.Protocol;

public interface IJoinGamePacket : IPacket
{
    public int EntityId { get; set; } // Unique player entity ID
    public byte GameMode { get; set; } // Game mode (0 = Survival, 1 = Creative, etc.)
    public int Dimension { get; set; } // Dimension (0 = Overworld, -1 = Nether, 1 = End)
    public long HashedSeed { get; set; } // World seed hash (for consistency across clients)
    public int MaxPlayers { get; set; } // Max players in the world (this is informational for clients)
    public string LevelType { get; set; } // Level type, such as "default" or "flat"
    public int ViewDistance { get; set; } // Client's view distance (affects chunk loading)
    public bool ReducedDebugInfo { get; set; } // Hide debug information from the client (F3 screen)
}