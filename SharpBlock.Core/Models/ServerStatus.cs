namespace SharpBlock.Core.Models;

public class Status
{
    public Version version { get; set; }
    public Players Players { get; set; }
    public Description Description { get; set; }
    public string Favicon { get; set; }
    public bool EnforcesSecureChat { get; set; }
}

public class Version
{
    public string Name { get; set; }
    public int Protocol { get; set; }
}

public class Players
{
    public int Max { get; set; }
    public int Online { get; set; }
    public object[] Sample { get; set; }
}

public class Description
{
    public string Text { get; set; }
}