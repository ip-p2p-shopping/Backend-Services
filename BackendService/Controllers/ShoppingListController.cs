using BackendService.Data;
using BackendService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BackendService.Utils;
using System.Text;
using BackendService.Models;

namespace BackendService;

[ApiController]
[Route("api/[controller]")]
public class ShoppingListController : IdentityController
{

    private readonly ShoppingDbContext _context;
    private readonly ILogger<ProductController> _logger;

    public ShoppingListController(ILogger<ProductController> logger, ShoppingDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("products")]
    public async Task<object[]> GetShoppingList()
    {
        var shoppingInstances = await _context.ShoppingInstances.Where(si => si.UserId == UserId && si.Bought == false).ToListAsync();

        return shoppingInstances.Select(async si => new {
            quantity = si.Quantity,
            product = await _context.Products.FindAsync(si.ProductId)
        }).ToArray();
    }
    

    [HttpPost("new")]
    public async Task<bool> AddProduct([FromBody]ShoppingProduct shoppingProduct)
    {
        try
        {
            var shoppingInstance = new ShoppingInstance
            {
                UserId = UserId,
                ProductId = shoppingProduct.ProductId,
                Quantity = shoppingProduct.Quantity,
                Bought = false
            };

            _context.ShoppingInstances.Add(shoppingInstance);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    [HttpPut("edit")]
    public async Task<ActionResult<List<Product>>> UpdateQuantity(ShoppingProduct shoppingProduct, int newQuantity)
    {
        var shoppingInstance = await _context.ShoppingInstances
        .FirstOrDefaultAsync(si => si.UserId == UserId && si.ProductId == shoppingProduct.ProductId);

        if (shoppingInstance != null)
        {
            shoppingInstance.Quantity = newQuantity;
            await _context.SaveChangesAsync();
        }

        var result = await _context.ShoppingInstances.ToListAsync();
        return Ok(result.Where(shoppingInstanceR => shoppingInstanceR.UserId == shoppingInstance.UserId));
    }
    

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult<List<Product>>> Delete(ShoppingProduct shoppingProduct)
    {
         var shoppingInstance = await _context.ShoppingInstances
        .FirstOrDefaultAsync(si => si.UserId == UserId && si.ProductId == shoppingProduct.ProductId);

        if (shoppingInstance == null)
            return BadRequest("Shopping Instance not found.");
        
        if (shoppingInstance.UserId != UserId)
            return BadRequest("You're not allowed to delete the product with id " + shoppingInstance.ProductId + ".");

        _context.ShoppingInstances.Remove(shoppingInstance);
        await _context.SaveChangesAsync();

        var result = await _context.ShoppingInstances.ToListAsync();
        return Ok(result.Where(shoppingInstanceR => shoppingInstanceR.UserId == shoppingInstance.UserId));
    }
}
