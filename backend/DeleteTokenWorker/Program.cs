using AuthModule.Application.DependencyInjection;
using AuthModule.Persistence.DependencyInjection;
using DeleteTokenWorker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddAuthApplication(builder.Configuration);
builder.Services.AddAuthPersistence(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();