// using Microsoft.AspNetCore.Builder;

// Microsoft.AspNetCore.Builder.WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// // Configure the application services
// builder.Services.AddControllers();
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = TokenGenerator.SecurityKey,
//         ValidateIssuer = false,
//         ValidateAudience = false
//     };
// });

// // Configure the HTTP request pipeline
// builder.Map("/api", app =>
// {
//     app.UseRouting();
//     app.UseAuthentication();
//     app.UseAuthorization();
//     app.UseEndpoints(endpoints =>
//     {
//         endpoints.MapControllers();
//     });
// });

// var app = builder.Build();
// app.Run();