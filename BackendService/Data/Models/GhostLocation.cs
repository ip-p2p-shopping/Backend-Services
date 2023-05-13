namespace BackendService.Data.Models;

public class GhostLocation
{
    public GhostLocation()
    {
        Id = Guid.NewGuid().ToString();
    }
    public string Id { get; set; }
    public int ProductId { get; set; }
    public double Lat { get; set; }
    public double Long { get; set; }
}