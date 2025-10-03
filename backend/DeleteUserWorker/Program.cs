using SharedKernel.Messaging.RabbitMQ;
using DeleteUserWorker;
using AuthModule.Persistence.DependencyInjection;
using AuthModule.Application.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddAuthApplication(builder.Configuration);
builder.Services.AddAuthPersistence(builder.Configuration);
builder.Services.AddRabbitMqService(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();