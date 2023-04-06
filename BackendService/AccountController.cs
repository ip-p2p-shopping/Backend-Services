using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.Data.Models;
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
        
        public AccountController(ShoppingDbContext dbContext, JwtProvider jwtProvider)
        {
            _dbContext = dbContext;
            _jwtProvider = jwtProvider;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _dbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return StatusCode(404);
            }
            if (user.Password != EncryptionHelpers.ComputeHash(password))
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
        public async Task<IActionResult> Register(string email, string password)
        {
            await _dbContext.Users.AddAsync(new User()
            {
                Email = email,
                Password = EncryptionHelpers.ComputeHash(password)
            });
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails()
        {
            return Ok(await _dbContext.Users.FindAsync(UserId));
        }
    }
}
