namespace BackendService.Models;

public class StoreRegisterModel : EmailPasswordModel
{
    public string name { get; set; }
    public double latitudine { get; set; }
    public double longitudine { get; set; }
}