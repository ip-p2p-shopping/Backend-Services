using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.Models;

public class FavouriteProduct
{
    [Key]
    public string UserId { get; set; }
    public int ProductId { get; set; }
}