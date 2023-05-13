using System.Text;
using BackendService.Data;
using BackendService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer((options) =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "p2pShoppingBackend", // TODO Get this from env variables
            ValidAudience = "p2pShoppingMobile", // TODO Get this from env variables
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("59255cb7-d05c-403b-81a5-bf7c995d1f9f")) // TODO Get this from env variables
        };
    });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddSingleton<JwtProvider>();

string sqlServerConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Using {sqlServerConnectionString} as database connection string");

builder.Services.AddDbContext<ShoppingDbContext>(options => 
    options.UseSqlServer(sqlServerConnectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var dbContext = service.GetService<ShoppingDbContext>();
    // dbContext?.Database.EnsureDeleted();    
    dbContext?.Database.Migrate();
}


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

// global cors policy
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .AllowCredentials()); // allow credentials

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
