using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.Models;

public class ShoppingInstance
{
    [Key]
    public string UserId { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; }
    public bool Bought { get; set; }
}