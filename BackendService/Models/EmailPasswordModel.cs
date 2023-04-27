namespace BackendService.Models;

public class EmailPasswordModel
{
    public string email { get; set; }
    public string password { get; set; }
    public string salt { get; set; }
}