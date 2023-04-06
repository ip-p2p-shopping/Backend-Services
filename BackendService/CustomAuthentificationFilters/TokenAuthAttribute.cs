using System;
using System.Net;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace YourNamespace
{
    public class TokenAuthAttribute : TypeFilterAttribute
    {
        public TokenAuthAttribute() : base(typeof(TokenAuthFilter))
        {
        }
    }

    public class TokenAuthFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"];

            // if(authHeader != StringValues.Empty && authHeader[0].StartsWith("Bearer "))
            // {
            //     var token = authHeader[0].Substring("Bearer ".Length).Trim();
            //     var principal = TokenGenerator.ValidateToken(token);

            //     if (principal != null)
            //     {
            //         context.HttpContext.User = principal;
            //         return;
            //     }
            // }

            context.Result = new UnauthorizedResult();
        }
    }
}