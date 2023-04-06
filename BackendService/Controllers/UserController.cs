using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BackendService.Data;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BackendService.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ShoppingDbContext _context;
    private readonly IUserService _userService;

    public UserController(ShoppingDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _userService.RegisterAsync(request);
        return Ok();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _userService.LoginAsync(request);
        return Ok(response);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userService.GetByIdAsync(userId);
        return Ok(user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }
}