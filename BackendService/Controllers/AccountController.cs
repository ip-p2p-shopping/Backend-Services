using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.Data.Models;
using BackendService.Models;
using BackendService.Services;
using BackendService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BackendService
{
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : IdentityController
  {
    private readonly ShoppingDbContext _dbContext;
    private readonly JwtProvider _jwtProvider;
    private readonly HttpClient _httpClient;

    public AccountController(ShoppingDbContext dbContext, JwtProvider jwtProvider)
    {
      _dbContext = dbContext;
      _jwtProvider = jwtProvider;
      _httpClient = new HttpClient();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Login")]
    public async Task<IActionResult> Login(EmailPasswordModel model)
    {
      var user = await _dbContext.Users.Where(x => x.Email == model.email.ToLower().Replace(" ", "")).FirstOrDefaultAsync();
      if (user == null)
      {
        return StatusCode(404);
      }
      if (user.Password != EncryptionHelpers.ComputeHash(model.password))
      {
        return StatusCode(401);
      }
      // TODO check for password
      string token = _jwtProvider.Generate(user);

      return Ok(token);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    public async Task<IActionResult> Register(EmailPasswordModel model)
    {
      if (await _dbContext.Users.Where(x => x.Email == model.email.ToLower().Replace(" ", "")).AnyAsync())
      {
        return StatusCode(409);
      }
      await _dbContext.Users.AddAsync(new User()
      {
        Email = model.email.ToLower().Replace(" ", ""),
        Password = EncryptionHelpers.ComputeHash(model.password),
        Address = ""
      });
      await _dbContext.SaveChangesAsync();
      return StatusCode(200);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("RegisterStore")]
    public async Task<IActionResult> RegisterStore(StoreRegisterModel model)
    {
      if (await _dbContext.Users.Where(x => x.Email == model.email.ToLower().Replace(" ", "")).AnyAsync())
      {
        return StatusCode(409);
      }

      Store store = new Store()
      {
        StoreName = model.name,
        Lat = model.latitudine,
        Long = model.longitudine
      };
      var user = new User()
      {
        Email = model.email.ToLower().Replace(" ", ""),
        Password = EncryptionHelpers.ComputeHash(model.password),
        Address = "",
        StoreId = store.Id
      };

      await _dbContext.Users.AddAsync(user);
      await _dbContext.Stores.AddAsync(store);
      await _dbContext.SaveChangesAsync();

      return StatusCode(200);
    }

    [HttpGet]
    [Route("GetUserDetails")]
    public async Task<IActionResult> GetUserDetails()
    {
      return Ok(await _dbContext.Users.Where(x => x.Id == UserId).Select(x => new
      {
        x.Id,
        x.FirstName,
        x.LastName,
        x.Email,
        x.Address
      }).FirstOrDefaultAsync());
    }

    [HttpPost]
    [Route("UpdateUserDetails")]
    public async Task<IActionResult> UpdateUserDetails(User user)
    {
      var _user = await _dbContext.Users.FindAsync(UserId);
      _user.Email = user.Email.ToLower().Replace(" ", "");
      _user.Address = user.Address;
      _user.FirstName = user.FirstName;
      _user.LastName = user.LastName;

      if (!string.IsNullOrEmpty(user.Password))
      {
        _user.Password = EncryptionHelpers.ComputeHash(user.Password);
      }

      await _dbContext.SaveChangesAsync();
      return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Google")]
    public async Task<IActionResult> GoogleCallback(GoogleTokenModel model)
    {
      try
      {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {model.token}");

        var response = await _httpClient.GetAsync("https://www.googleapis.com/userinfo/v2/me");

        string email = null;

        if (response.IsSuccessStatusCode)
        {
          var responseJson = await response.Content.ReadAsStringAsync();
          var jsonDocument = JsonDocument.Parse(responseJson);

          if (jsonDocument.RootElement.TryGetProperty("email", out var emailProperty) && emailProperty.ValueKind == JsonValueKind.String)
          {
            email = emailProperty.GetString();
          }
        }
        else
        {
          var statusCode = response.StatusCode;
          var errorMessage = await response.Content.ReadAsStringAsync();

          return BadRequest(new { error = "Invalid Google token." });
        }

        if (email == null)
        {
          return BadRequest(new { error = "Invalid Google token." });
        }

        var user = await _dbContext.Users.Where(user => user.Email == email).FirstOrDefaultAsync();

        // note: nu stiu ce se intampla daca un user are deja emailu in db si dupa incearca sa dea login cu google

        if (user == null)
        {
          user = new User()
          {
            Email = email,
            Password = "",
            Address = "",
            FirstName = "",
            LastName = ""
          };

          await _dbContext.Users.AddAsync(user);
          await _dbContext.SaveChangesAsync();
        }

        string token = _jwtProvider.Generate(user);

        return Ok(token);
      }
      catch (Exception e)
      {
        return BadRequest(new { error = e.ToString() });
      }
    }
  }
}
