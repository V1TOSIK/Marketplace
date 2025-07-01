namespace AuthModule.Application.Interfaces
{
    public interface ICookieService
    {
        void Set(string key, string value, DateTime expirationTime);
        string? Get(string key);
        void Delete(string key);
    }
}
