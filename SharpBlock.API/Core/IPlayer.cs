namespace SharpBlock.API.Core;

public interface IPlayer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}