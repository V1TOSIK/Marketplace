namespace AuthModule.Application.Interfaces.Services;

public interface ICookieService
{
    void Set(string key, string value, DateTime expirationTime);
    string? Get(string key);
    void Delete(string key);
    void SetRefreshTokenCookieIfNotNull(string token, DateTime expiration);
    void SetDeviceIdCookieIfNotNull(Guid? deviceId);
    string? GetRefreshToken();
    Guid GetDeviceId();
    void DeleteRefreshTokenCookie();
}
