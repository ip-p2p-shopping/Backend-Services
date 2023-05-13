using BackendService.Data.Models;

namespace BackendService.Models;

public class ProductWithLocations : Product
{
    public List<Location> Locations {get;set;} = new List<Location>();
}

public class Location {
    public double latitude {get;set;}
    public double longitude {get;set;}
}