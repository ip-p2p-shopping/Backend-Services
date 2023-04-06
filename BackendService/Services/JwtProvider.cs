using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendService.Data.Models;
using Microsoft.IdentityModel.Tokens;

namespace BackendService.Services;

public class JwtProvider
{
    public string Generate(User user)
    {
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Name, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("59255cb7-d05c-403b-81a5-bf7c995d1f9f")), // TODO Get this from env variables
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            "p2pShoppingBackend", // TODO Get this from env variables
            "p2pShoppingMobile", // TODO Get this from env variables
            claims, 
            null, 
            DateTime.UtcNow.AddYears(1), // TODO Shouldn't be this long 
            signingCredentials);
        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }
}