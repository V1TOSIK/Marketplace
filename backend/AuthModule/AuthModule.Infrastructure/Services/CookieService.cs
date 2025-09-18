using AuthModule.Application.Exceptions;
using AuthModule.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuthModule.Infrastructure.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CookieService> _logger;
        public CookieService(IHttpContextAccessor httpContextAccessor,
            ILogger<CookieService> cookieService)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = cookieService;
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
            {
                _logger.LogError("Attempted to set a cookie with an empty key.");
                throw new InvalidCookieParameterException("Cookie key cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(value))
            {
                _logger.LogError("Attempted to set a cookie with an empty value.");
                throw new InvalidCookieParameterException("Cookie value cannot be null or empty.");
            }

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);
        }

        public string? Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogError("Attempted to get a cookie with an empty key.");
                throw new InvalidCookieParameterException("Cookie key cannot be null or empty.");
            }

            return _httpContextAccessor.HttpContext?.Request.Cookies[key];
        }
     
        public void Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogError("Attempted to delete a cookie with an empty key.");
                throw new InvalidCookieParameterException("Cookie key cannot be null or empty.");
            }

            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
        }
    }
}
