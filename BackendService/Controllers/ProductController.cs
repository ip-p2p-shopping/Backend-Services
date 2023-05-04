using BackendService.Data;
using BackendService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BackendService.Utils;

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
        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.SellerId == UserId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return BadRequest("Product not found.");

        if (product.SellerId != UserId)
            return BadRequest("You're not allowed to view the product with id " + product.Id + ".");

        return Ok(product);
    }

    [HttpPost("new")]
    public async Task<ActionResult<List<Product>>> AddProduct(Product product)
    {
        product.SellerId = UserId;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.SellerId == UserId));
    }

    [HttpPut("edit")]
    public async Task<ActionResult<List<Product>>> UpdateProduct(Product request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null)
            return BadRequest("Product not found.");

        if (product.SellerId != UserId)
            return BadRequest("You're not allowed to edit the product with id " + product.Id + ".");


        product.Name = request.Name;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        product.Description = request.Description;
        product.MeasureUnit = request.MeasureUnit;
        product.SellerId = UserId;
        product.ImageURL = request.ImageURL;
        product.ImageURLs = request.ImageURLs; //pentru a modifica TOATE pozele unui produs
        
        await _context.SaveChangesAsync();

        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.SellerId == UserId));
    }    

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<List<Product>>> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return BadRequest("Product not found.");
        
        if (product.SellerId != UserId)
            return BadRequest("You're not allowed to delete the product with id " + id + ".");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.SellerId == UserId));
    }

    [HttpDelete("delete/")]
    public async Task<ActionResult<List<Product>>> DeleteProducts(List<int> Ids)
    {
        for(int i = 0; i < Ids.Count; ++i){
        var product = await _context.Products.FindAsync(Ids[i]);
        if (product == null)
            return BadRequest("Product with id " + Ids[i] + " not found.");

        if(product.SellerId != UserId)
            return BadRequest("You're not allowed to remove the product with id " + Ids[i] + ".");

        _context.Products.Remove(product);
        }
        await _context.SaveChangesAsync();

        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.SellerId == UserId));
    }

    [HttpPost("newFromFile")]
    public async Task<ActionResult<List<Product>>> AddProducts(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var result = await _context.Products.ToListAsync();
        return Ok(result.Where(productR => productR.SellerId == UserId));
    }
}
