using MediaModule.Application.Interfaces.Services;
using MediaModule.Infrastructure.Options;
using MediaModule.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace MediaModule.Infrastructure.DependencyInjection
{
    public static class MinIoInjection
    {
        public static IServiceCollection AddMinIo(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MinIoOptions>(configuration.GetSection("MINIO"));

            services.AddSingleton<IMinioClient>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MinIoOptions>>().Value;

                if (options == null || string.IsNullOrWhiteSpace(options.Endpoint))
                    throw new ArgumentException("MinIO configuration is missing or invalid.");

                return new MinioClient()
                    .WithEndpoint(options.Endpoint)
                    .WithCredentials(options.AccessKey, options.SecretKey)
                    .WithSSL(options.UseSSL)
                    .Build();
            });

            services.AddScoped<IMediaProvider, MinIoProvider>();

            return services;
        }
    }
}
