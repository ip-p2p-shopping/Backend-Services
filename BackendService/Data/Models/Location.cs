namespace BackendService.Data.Models
{
    public class Location
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<LocationProduct> LocationProducts { get; set; } = new List<LocationProduct>();
    }
}
