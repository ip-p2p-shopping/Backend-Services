using BackendService.Data;
using BackendService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BackendService.Utils;
using System.Text;

namespace BackendService;

[ApiController]
[Route("api/[controller]")]
public class ProductController : IdentityController
{

    private readonly ShoppingDbContext _context;
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger, ShoppingDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "products")]
    public async Task<ActionResult<List<Product>>> Get()
    {
        string storeId = await GetStoreId(UserId);
        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.StoreId == storeId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(int id)
    {
        string storeId = await GetStoreId(UserId);
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return BadRequest("Product not found.");

        if (product.StoreId != storeId)
            return BadRequest("You're not allowed to view the product with id " + product.Id + ".");

        return Ok(product);
    }

    [HttpPost("new")]
    public async Task<ActionResult<List<Product>>> AddProduct(Product product)
    {
        product.StoreId = await GetStoreId(UserId);;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.StoreId == product.StoreId));
    }

    [HttpPut("edit")]
    public async Task<ActionResult<List<Product>>> UpdateProduct(Product request)
    {
        string storeId = await GetStoreId(UserId);
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null)
            return BadRequest("Product not found.");

        if (product.StoreId != storeId)
            return BadRequest("You're not allowed to edit the product with id " + product.Id + ".");


        product.Name = request.Name;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        product.Description = request.Description;
        product.MeasureUnit = request.MeasureUnit;
        product.StoreId = await GetStoreId(UserId);
        product.ImageURL = request.ImageURL;
        product.ImageURLs = request.ImageURLs; //pentru a modifica TOATE pozele unui produs
        
        await _context.SaveChangesAsync();

        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.StoreId == product.StoreId));
    }    

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<List<Product>>> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return BadRequest("Product not found.");
        
        if (product.StoreId != await GetStoreId(UserId))
            return BadRequest("You're not allowed to delete the product with id " + id + ".");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.StoreId == product.StoreId));
    }

    [HttpDelete("delete/")]
    public async Task<ActionResult<List<Product>>> DeleteProducts(List<int> Ids)
    {
        for(int i = 0; i < Ids.Count; ++i){
        var product = await _context.Products.FindAsync(Ids[i]);
        if (product == null)
            return BadRequest("Product with id " + Ids[i] + " not found.");

        if(product.StoreId != await GetStoreId(UserId))
            return BadRequest("You're not allowed to remove the product with id " + Ids[i] + ".");

        _context.Products.Remove(product);
        }
        await _context.SaveChangesAsync();

        string storeId = await GetStoreId(UserId);
        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.StoreId == storeId));
    }

    [HttpPost("batch-upload")]
    public async Task<ActionResult<List<Product>>> BatchUpload(List<IFormFile> file)
    {
        var result = new StringBuilder();
        using (var reader = new StreamReader(file[0].OpenReadStream()))
        {
            reader.ReadLine();
            while (reader.Peek() >= 0) {
                string line = reader.ReadLine();
                string[] elements = line.Split(",");
                Product product = new Product(){
                    Name = elements[0],
                    Price = Int32.Parse(elements[1]),
                    Quantity = Int32.Parse(elements[2]),
                    MeasureUnit = elements[3],
                    ImageURL = elements[4],
                    StoreId = await GetStoreId(UserId)
                };
                _context.Products.Add(product);
            }
                await _context.SaveChangesAsync();
                
        }

        var productList = await _context.Products.ToListAsync();
        string storeId = await GetStoreId(UserId);
        return Ok(productList.Where(productR => productR.StoreId == storeId));
    }

    private async Task<string> GetStoreId(string userId) => (await _context.Users.FindAsync(userId)).StoreId;
}
