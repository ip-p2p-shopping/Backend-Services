using BackendService.Data.Models;

namespace BackendService.Models;

public class ProductWithLocations : Product
{
    public List<Location> Locations {get;set;} = new List<Location>();
}

public class Location {
    public double latitudine {get;set;}
    public double longitudine {get;set;}
}