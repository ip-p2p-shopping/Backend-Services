using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.Models;

public class ShoppingInstance
{
    public ShoppingInstance() {
        Id = Guid.NewGuid().ToString();
    }
    [Key]
    public string Id {get;set;}
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public bool Bought { get; set; }
}