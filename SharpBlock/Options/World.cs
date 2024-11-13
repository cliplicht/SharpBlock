namespace SharpBlock.Options;

public class Dimension
{
    public string Name { get; set; } = null!;
    public int Id { get; set; }
    public bool PiglinSafe { get; set; }
    public bool Natural { get; set; }
    public float AmbientLight { get; set; }
    public string Infiniburn { get; set; } = null!;
    public bool RespawnAnchorWorks { get; set; }
    public bool HasSkylight { get; set; }
    public bool BedWorks { get; set; }

    public string Effects { get; set; } = null!;

    public int LogicalHeight { get; set; }

    public double CoordinateScale { get; set; }

    public bool Ultrawarm { get; set; }

    public bool HasCeiling { get; set; }
}