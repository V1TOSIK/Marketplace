using DeleteProductWorker;
using ProductModule.Application.DependencyInjection;
using ProductModule.Persistence.DependencyInjection;
using SharedKernel.Messaging.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddProductApplication();
builder.Services.AddProductPersistence(builder.Configuration);
builder.Services.AddRabbitMqService(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();