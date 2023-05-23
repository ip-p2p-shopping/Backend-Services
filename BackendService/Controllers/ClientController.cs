using BackendService.Data;
using BackendService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BackendService.Utils;
using Microsoft.AspNetCore.Authorization;
using BackendService.Models;
using BackendService.Services;


namespace BackendService;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientController : IdentityController
{

    private readonly ShoppingDbContext _context;
    private readonly ILogger<ClientController> _logger;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;

    public ClientController(ILogger<ClientController> logger, ShoppingDbContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
    {
        _logger = logger;
        _context = context;
        _hostingEnvironment = hostingEnvironment;
    }


    [HttpGet("productsCategory")]
    public async Task<IActionResult> GetProducts(string search = "", string searchCategory = "")
    {
        var products = await _context.Products.ToListAsync();
        if(!string.IsNullOrEmpty(search)) {
            search = search.ToLower();
            products = products.Where(x => x.Name.ToLower().Contains(search) || x.Category.ToLower().Contains(search)).ToList();
        }
        if(!string.IsNullOrEmpty(searchCategory))
        {
            searchCategory = searchCategory.ToLower();
            products = products.Where(x => x.Category.ToLower().Contains(searchCategory)).ToList();
        }
        var categories = products.GroupBy(p => p.Category);
        var response = new List<Dictionary<string, object>>();

        foreach (var category in categories)
        {
            var categoryDict = new Dictionary<string, object>();
            categoryDict["id"] = category.Key;
            categoryDict["title"] = category.Key;
            categoryDict["subtitle"] = "Subtitlu categorie";

            var categoryProducts = category.Take(4).ToList();
            categoryDict["products"] = categoryProducts;

            categoryDict["showMore"] = category.Count() > 4;

            response.Add(categoryDict);
        }

        return Ok(response);
    }

    [HttpGet("products/{id}")]
    public async Task<ProductWithLocations> GetProductDetails(int id)
    {
        var product = await _context.Products.FindAsync(id);

        ProductWithLocations ploc = new ProductWithLocations();
        ploc.Id = product.Id;
        ploc.Name = product.Name;
        ploc.Category = product.Category;
        ploc.Price = product.Price;
        ploc.MaxPrice = product.Price;
        ploc.Quantity = product.Quantity;
        ploc.Description = product.Description;
        ploc.MeasureUnit = product.MeasureUnit;
        ploc.StoreId = product.StoreId;
        ploc.ImageURL = product.ImageURL;
        ploc.ImageURLs = product.ImageURLs;
        ploc.Locations = new List<Location>();

        var store = await _context.Stores.FindAsync(product.StoreId);
        if(store != null) {
            ploc.Locations.Add(new Location{
                latitude = store.Lat,
                longitude = store.Long
            });
        }
        foreach(var loc in _context.GhostLocations.Where(x => x.ProductId == product.Id))
        {
            ploc.Locations.Add(new Location{ 
                latitude = loc.Lat,
                longitude = loc.Long
            });
        }

        return ploc;
    }
    
    [HttpPost("uploadImage")]
    [AllowAnonymous]
    public async Task<string> UploadImage(IFormFile file)
    {
        try {
            string imgName = $"{Guid.NewGuid().ToString()}.{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", imgName);
            Console.WriteLine($"Inserting into {filePath}");
            if (file.Length > 0)
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return filePath;
        }
        catch(Exception e) {
            Console.WriteLine(e);
            return e.ToString();
        }
    }

    [RequestFormLimits(ValueLengthLimit = 209715200)]
    [HttpPost("newProductByClient")]
    public async Task<object> AddProductByClient([FromBody]ProductIntroducedByClientModel model)
    {
        int productId = -1;
        try
        {
            bool exitentStore = false;
            foreach (var store in _context.Stores.ToList())
            {
                if(LocationHelper.VerifyLocation(store.Lat, store.Long, model.Lat, model.Long)){
                    exitentStore = true;
                    var productIntroducedByClient = new Product {
                        Name = model.Name,
                        Category = model.Category,
                        Price = model.Price,
                        Description = model.Description,
                        StoreId = store.Id,
                        ImageURLs = new List<string>() { model.ImgURL}
                    };
                    _context.Products.Add(productIntroducedByClient);
                    productId = productIntroducedByClient.Id;
                }
            }
            if(!exitentStore){
                var newProduct = new Product{
                    Name = model.Name,
                    Category = model.Category,
                    Price = model.Price,
                    Description = model.Description,
                    ImageURLs = new List<string>() { model.ImgURL}
                };
                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();

                var ghostLocation = new GhostLocation{
                    ProductId = _context.Products.Select(x => x.Id).Max(),
                    Lat = model.Lat,
                    Long = model.Long 
                };
                _context.GhostLocations.Add(ghostLocation);
            }
            await _context.SaveChangesAsync();

            return _context.Products.Select(x => x.Id).Max();
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
}