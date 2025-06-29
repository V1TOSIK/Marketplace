using AuthModule.Application.Exceptions;
using AuthModule.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AuthModule.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Set(string key, string value, DateTime expirationTime)
        {
            CookieOptions options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expirationTime
            };

            if (string.IsNullOrEmpty(key))
                throw new InvalidCookieParameterException("Cookie key cannot be null or empty.");

            if (string.IsNullOrEmpty(value))
                throw new InvalidCookieParameterException("Cookie value cannot be null or empty.");

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);
        }

        public string? Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new InvalidCookieParameterException("Cookie key cannot be null or empty.");

            return _httpContextAccessor.HttpContext?.Request.Cookies[key];
        }
     
        public void Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new InvalidCookieParameterException("Cookie key cannot be null or empty.");

            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
        }
    }
}
