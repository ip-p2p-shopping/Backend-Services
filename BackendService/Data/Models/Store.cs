namespace BackendService.Data.Models;

public class Store
{
    public Store()
    {
        Id = Guid.NewGuid().ToString();
    }
    public string Id { get; set; }
    public string StoreName { get; set; }
    public double Lat { get; set; }
    public double Long { get; set; }
}