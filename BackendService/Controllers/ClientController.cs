using BackendService.Data;
using BackendService.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BackendService.Utils;
using Microsoft.AspNetCore.Authorization;

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


}