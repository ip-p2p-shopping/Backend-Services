namespace BackendService.Data.Models;

public class Product {

  public int Id { get; set; }
  public String Name { get; set; } = string.Empty;
  public String Category { get; set; } = string.Empty;
  public int Price { get; set; }
  public int MaxPrice { get; set; }
  public int Quantity { get; set; }
  public String Description { get; set; } = string.Empty;
  public String MeasureUnit { get; set; } = string.Empty;
  public String SellerId { get; set; } = string.Empty;
  public String ImageURL { get; set; } = string.Empty;  
  public List<string> ImageURLs { get; set; } = new List<string>();
  public List<LocationProduct> LocationProducts { get; set; } = new List<LocationProduct>();
}