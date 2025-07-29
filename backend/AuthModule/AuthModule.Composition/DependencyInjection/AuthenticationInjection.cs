using AuthModule.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthModule.Composition.DependencyInjection
{
    public static class AuthenticationInjection
    {
        public static void AddAuthentication(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JWT").Get<JwtOptions>();
            services.Configure<JwtOptions>(configuration.GetSection("JWT"));

            if (jwtOptions == null || string.IsNullOrEmpty(jwtOptions.SecretKey))
            {
                throw new ArgumentException("JWT options are not configured properly.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

            services.AddAuthorization();
        }
    }
}
