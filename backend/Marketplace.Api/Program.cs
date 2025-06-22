using AuthUserModule.Persistence.DependencyInjection;
using AuthUserModule.Application.DependencyInjection;
using Marketplace.Api.Extentions;
using AuthUserModule.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Options;
using AuthUserModule.Infrastructure.Options;
using AuthUserModule.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtOptions = builder.Configuration.GetSection("JWT").Get<JwtOptions>();

builder.Services.AddAuthPersistence(builder.Configuration);
builder.Services.AddAuthApplication(builder.Configuration);
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddApiAuthentication(jwtOptions);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Exception: " + ex);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Server error: " + ex);
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
