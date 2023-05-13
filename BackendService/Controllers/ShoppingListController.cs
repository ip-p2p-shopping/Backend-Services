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

        return shoppingInstances.Select(si => new {
            quantity = si.Quantity,
            product = _context.Products.Find(si.ProductId)
        }).ToArray();
    }
    

    [HttpPost("new")]
    public async Task<bool> AddProduct([FromBody]ShoppingProduct shoppingProduct)
    {
        try
        {
            var existingInstance = await _context.ShoppingInstances
                .Where(x => x.UserId == UserId && x.ProductId == shoppingProduct.ProductId).FirstOrDefaultAsync();
            if (existingInstance == null)
            {
                var shoppingInstance = new ShoppingInstance
                {
                    UserId = UserId,
                    ProductId = shoppingProduct.ProductId,
                    Quantity = shoppingProduct.Quantity,
                    Bought = false
                };

                _context.ShoppingInstances.Add(shoppingInstance);   
            }
            else
            {
                existingInstance.Quantity += shoppingProduct.Quantity;
            }
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    [HttpPost("edit")]
    public async Task<bool> UpdateQuantity([FromBody]ShoppingProduct shoppingProduct)
    {
        try
        {
            var shoppingInstance = await _context.ShoppingInstances
                .FirstOrDefaultAsync(si => si.UserId == UserId && si.ProductId == shoppingProduct.ProductId);

            shoppingInstance.Quantity = shoppingProduct.Quantity;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    [HttpPost("markAsFound")]
    public async Task<bool> UpdateBought([FromBody]ShoppingProduct shoppingProduct)
    {
        try
        {
            var shoppingInstance = await _context.ShoppingInstances
                .FirstOrDefaultAsync(si => si.UserId == UserId && si.ProductId == shoppingProduct.ProductId);

            shoppingInstance.Bought = true;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;   
        }
    }
    

    [HttpDelete("delete/{id}")]
    public async Task<bool> Delete(int id)
    {
        try
        {
            var shoppingInstance = await _context.ShoppingInstances
                .FirstOrDefaultAsync(si => si.UserId == UserId && si.ProductId == id);

            if (shoppingInstance == null)
                return false;

            if (shoppingInstance.UserId != UserId)
                return false;

            _context.ShoppingInstances.Remove(shoppingInstance);
            await _context.SaveChangesAsync();

            var result = await _context.ShoppingInstances.ToListAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
