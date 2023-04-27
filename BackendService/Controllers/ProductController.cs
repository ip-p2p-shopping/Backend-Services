using BackendService.Data;
using BackendService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BackendService;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
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
        return Ok(await _context.Products.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return BadRequest("Product not found.");
        return Ok(product);
    }

    [HttpPost("new")]
    public async Task<ActionResult<List<Product>>> AddProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(await _context.Products.ToListAsync());
    }

    [HttpPut("edit")]
    public async Task<ActionResult<List<Product>>> UpdateProduct(Product request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null)
            return BadRequest("Product not found.");

        product.Name = request.Name;
        product.Price = request.Price;

        await _context.SaveChangesAsync();

        return Ok(await _context.Products.ToListAsync());
    }    

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<List<Product>>> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return BadRequest("Product not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(await _context.Products.ToListAsync());
    }

    [HttpDelete("delete/")]
    public async Task<ActionResult<List<Product>>> DeleteProducts(List<int> Ids)
    {
        for(int i = 0; i < Ids.Count; ++i){
        var product = await _context.Products.FindAsync(Ids[i]);
        if (product == null)
            return BadRequest("Product with id + " + Ids[i] + " not found.");

        _context.Products.Remove(product);
        }
        await _context.SaveChangesAsync();

        return Ok(await _context.Products.ToListAsync());
    }

    [HttpPost("newFromFile")]
    public async Task<ActionResult<List<Product>>> AddProducts(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(await _context.Products.ToListAsync());
    }
}
