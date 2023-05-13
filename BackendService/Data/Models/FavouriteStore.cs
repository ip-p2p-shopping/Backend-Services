using System.ComponentModel.DataAnnotations;

namespace BackendService.Data.Models;

public class FavouriteStore
{
    [Key]
    public string UserId { get; set; }
    public string StoreName { get; set; }
}