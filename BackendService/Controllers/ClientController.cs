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
    public async Task<ActionResult<List<Dictionary<string, object>>>> GetProducts()
    {
        var products = await _context.Products.ToListAsync();

        var categories = products.GroupBy(p => p.Category);

        var response = new List<Dictionary<string, object>>();

        foreach (var category in categories)
        {
            var categoryDict = new Dictionary<string, object>();
            categoryDict["id"] = category.Key;
            categoryDict["title"] = "Titlu categorie";
            categoryDict["subtitle"] = "Subtitlu categorie";

            var categoryProducts = category.Take(4).ToList();
            categoryDict["products"] = categoryProducts;

            categoryDict["showMore"] = category.Count() > 4;

            response.Add(categoryDict);
        }

        return Ok(response);
    }


    [HttpGet("products")]
    public async Task<ActionResult<List<Dictionary<string, object>>>> GetProductsByCategory(string category = null, string search = null)
    {
        var productsQuery = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(category))
            productsQuery = productsQuery.Where(p => p.Category == category);

        if (!string.IsNullOrEmpty(search))
            productsQuery = productsQuery.Where(p => p.Name.Contains(search));

        if (string.IsNullOrEmpty(category) && string.IsNullOrEmpty(search))
            return BadRequest("Please provide a valid category and/or search parameter.");

        var products = await productsQuery.ToListAsync();

        var response = new List<Dictionary<string, object>>();

        foreach (var product in products)
        {
            var productDict = new Dictionary<string, object>();
            productDict["id"] = product.Id;
            productDict["title"] = product.Name;
            productDict["image"] = product.ImageURL;
            productDict["priceRange"] = "$" + product.Price.ToString("0.00");

            response.Add(productDict);
        }

        return Ok(response);
    }


    [HttpGet("products/{id}")]
    public async Task<ActionResult<Dictionary<string, object>>> GetProductDetails(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return BadRequest("Product not found.");


        var response = new Dictionary<string, object>();
        response["id"] = product.Id;
        response["title"] = product.Name;
        response["description"] = "Descrierea produsului va fi aici.";
        response["image"] = product.ImageURL;
        response["price"] = "$" + product.Price.ToString("0.00");
        response["measureUnit"] = product.MeasureUnit;
        response["sellerId"] = product.SellerId;

        return Ok(response);
    }


}