namespace Dotnet9.WebApi.ResultPattern.Demo.Data.Models;

public class PenguinModel
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public string Name { get; set; } = null!;
    
    public string Species { get; set; } = null!;
    
    public int Age { get; set; }
}