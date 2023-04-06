using BackendService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.Abstractions;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDataProtection();

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

app.MapGet("/username", (HttpContext context, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");

    var authCookie = context.Request.Headers.Cookie.FirstOrDefault(x => x.StartsWith("auth="));
    return authCookie;
});

app.MapGet("/login", (HttpContext context, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");
    context.Response.Headers["set-cookie"] = $"auth={protector.Protect("usr:anton")}";
    return "ok";
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Header-Key", "Header-Value");
    await next();
});




// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
