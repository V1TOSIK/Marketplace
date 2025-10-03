using DeleteMediaWorker;
using MediaModule.Application.DependencyInjection;
using MediaModule.Infrastructure.DependencyInjection;
using MediaModule.Persistence.DependencyInjection;
using SharedKernel.Messaging.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMediaApplication();
builder.Services.AddMediaPersistence(builder.Configuration);
builder.Services.AddMediaInfrastructure(builder.Configuration);
builder.Services.AddRabbitMqService(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
