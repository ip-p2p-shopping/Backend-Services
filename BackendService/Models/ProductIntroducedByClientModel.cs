namespace BackendService.Models;

public class ProductIntroducedByClientModel
{
    public string Name { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public string ImgURL { get; set; }
    public double Lat { get; set; }
    public double Long { get; set; }

}