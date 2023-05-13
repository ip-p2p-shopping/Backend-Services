using BackendService.Data;
using BackendService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BackendService.Utils;
using Microsoft.AspNetCore.Authorization;
using BackendService.Models;

namespace BackendService;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientController : IdentityController
{

    private readonly ShoppingDbContext _context;
    private readonly ILogger<ClientController> _logger;

    public ClientController(ILogger<ClientController> logger, ShoppingDbContext context)
    {
        _logger = logger;
        _context = context;
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
    public async Task<ActionResult> GetProductDetails(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return BadRequest("Product not found.");

        return Ok(product);
    }

    [HttpPost("newFavouriteProduct")]
    public async Task<bool> AddFavouriteProduct(ShoppingProduct shoppingProduct)
    {
        try
        {
            var favouriteProduct = new FavouriteProduct
            {
                UserId = UserId,
                ProductId = shoppingProduct.ProductId
            };

            _context.FavouriteProducts.Add(favouriteProduct);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    [HttpGet("products")]
    public async Task<ActionResult<object>> GetShoppingList()
    {
        var shoppingInstances = await _context.ShoppingInstances.Where(si => si.UserId == UserId && si.Bought == false).ToListAsync();

        return Ok(shoppingInstances.Select(async si => new {
            quantity = si.Quantity,
            product = await _context.Products.FindAsync(si.ProductId)
        }));
    }

    [HttpGet("favouriteProducts")]
    public async Task<ActionResult<object>> GetFavouriteProducts()
    {
        var favouriteProducts = await _context.FavouriteProducts.Where(si => si.UserId == UserId).ToListAsync();

        return Ok(favouriteProducts.Select(async si => new {
            product = await _context.Products.FindAsync(si.ProductId)
        }));
    }

    [HttpPost("newFavouriteStore")]
    public async Task<bool> AddFavouriteStore(StoreRegisterModel storeRegisterModel)
    {
        try
        {
            var favouriteStore = new FavouriteStore
            {
                UserId = UserId,
                StoreName = storeRegisterModel.name
            };

            _context.FavouriteStores.Add(favouriteStore);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    [HttpGet("favouriteStores")]
    public async Task<ActionResult<object>> GetFavouriteStores()
    {
        var favouriteStores = await _context.FavouriteStores.Where(si => si.UserId == UserId).ToListAsync();

        return Ok(favouriteStores.Select(async si => new {
            store = await _context.Stores.FindAsync(si.StoreName)
        }));
    }


}