namespace BackendService.Data.Models;

public class User
{
    public User()
    {
        Id = Guid.NewGuid().ToString();

        FirstName = string.Empty;
        LastName = String.Empty;
        Address = String.Empty;

        StoreId = String.Empty;
    }
    
    public string Id { get; set; }
    
    public string Email { get; set; }
    
    // TODO Should be hashed
    public string Password { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string Address { get; set; }
    
    public string StoreId { get; set; }
}