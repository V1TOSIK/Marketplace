using AuthModule.Persistence.DependencyInjection;
using AuthModule.Application.DependencyInjection;
using Marketplace.Api.Extentions;
using AuthModule.Infrastructure.DependencyInjection;
using AuthModule.Persistence;
using Microsoft.EntityFrameworkCore;
using Marketplace.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthPersistence(builder.Configuration);
builder.Services.AddAuthApplication(builder.Configuration);
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddApiAuthentication(builder.Configuration);
//перенести всі ці di в один окремий який буде єдиною точкою входу в auth module + перейменувати AuthUserModule -> AuthModule

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
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
