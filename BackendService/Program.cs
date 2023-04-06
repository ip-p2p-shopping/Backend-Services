using BackendService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using System.Collections.Generic;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddDataProtection();
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication("cookie").AddCookie("cookie");

string sqlServerConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Using {sqlServerConnectionString} as database connection string");

builder.Services.AddDbContext<ShoppingDbContext>(options => 
    options.UseSqlServer(sqlServerConnectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var dbContext = service.GetService<ShoppingDbContext>();
    dbContext?.Database.EnsureCreated();
    dbContext?.Database.Migrate();
}

app.MapGet("/username", (HttpContext context) =>
{
    return context.User.FindFirst("usr").Value;
});

app.MapGet("/login", async (HttpContext context) =>
{
    // auth.SignIn();


    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "anton"));
    var identity = new ClaimsIdentity(claims, "cookie");
    context.User = new System.Security.Claims.ClaimsPrincipal(identity);


    await context.SignInAsync("cookie", user);

    return "ok";
});

app.UseAuthentication();

// app.Use((context, next) =>
// {
//     var idp = context.RequestServices.GetRequiredService<IDataProtectionProvider>();
//     var protector = idp.CreateProtector("auth-cookie");

//     var authCookie = context.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
//     var protectedPayload = authCookie.Split("=").Last();
//     var payload = protector.Unprotect(protectedPayload);
//     if(payload==null) payload="username:anton";
//     var parts = payload.Split(":");
//     var key =  parts[0];
//     var value = parts[1];

//     var claims = new List<Claim>();
//     claims.Add(new Claim(key, value));
//     var identity = new ClaimsIdentity();
//     context.User = new System.Security.Claims.ClaimsPrincipal(identity);

//     return next();
// });




// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();





// public class AuthService {
//     private readonly IDataProtectionProvider _idp;
//     private readonly IHttpContextAccessor _accessor;
//     public AuthService(IDataProtectionProvider idp, IHttpContextAccessor accessor) {
//         _idp = idp;
//         _accessor = accessor;
//     }

//     public void SignIn() {
//         var protector = _idp.CreateProtector("auth-cookie");
//         _accessor.HttpContext.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:anton")}";
//     }
// }