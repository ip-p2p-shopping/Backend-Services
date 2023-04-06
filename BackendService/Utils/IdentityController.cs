using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BackendService.Utils;

public class IdentityController : ControllerBase
{
    public string UserId => HttpContext.User.Identity is ClaimsIdentity identity ? 
        identity.Claims.Where(x => 
            x.Type == JwtRegisteredClaimNames.Name)
            .FirstOrDefault().Value : String.Empty;
}