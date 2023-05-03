namespace BackendService.Data.Models;

public class Product{

  public int Id { get; set; }
  public String Name { get; set; } = string.Empty;
  public String Category { get; set; } = string.Empty;
  public int Price { get; set; }
  public int MaxPrice { get; set; }
  public int Quantity { get; set; }
  public String MeasureUnit { get; set; } = string.Empty;
  public String SellerId { get; set; } = string.Empty;
  public String ImageURL { get; set; } = string.Empty;  

}