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
            product = GetProductDetails(si.ProductId)
        }).ToArray();
    }
    
    private object GetProductDetails(int id) {
        var product =  _context.Products.Find(id);

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

        var store = _context.Stores.Find(product.StoreId);
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

    [HttpGet("history")]
    public async Task<object[]> GetShoppingHistory()
    {
        var shoppingInstances = await _context.ShoppingInstances.Where(si => si.UserId == UserId && si.Bought == true).ToListAsync();

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
                .Where(x => x.UserId == UserId && x.ProductId == shoppingProduct.ProductId && x.Bought == false).FirstOrDefaultAsync();
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
