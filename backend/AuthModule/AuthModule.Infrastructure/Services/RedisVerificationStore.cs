using AuthModule.Application.Interfaces.Services;
using StackExchange.Redis;

namespace AuthModule.Infrastructure.Services
{
    public class RedisVerificationStore : IVerificationStore
    {
        private readonly IDatabase _database;
        public RedisVerificationStore(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task SaveCodeAsync(string key, string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Key and code cannot be null or empty.");
            var expiry = TimeSpan.FromMinutes(5);
            await _database.StringSetAsync(key, code, expiry, When.NotExists);
        }

        private async Task<string> GetCodeAsync(string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.");
            var code = await _database.StringGetAsync(key);
            if (code.IsNullOrEmpty)
                throw new ArgumentException("Code none in db");
            return code.ToString();
        }

        public async Task<bool> VerifyCodeAsync(string key, string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Key and code cannot be null or empty.");

            var storedCode = await GetCodeAsync(key, cancellationToken);

            if (storedCode == code)
            {
                await _database.KeyDeleteAsync(key);
                return true;
            }
            return false;
        }
    }
}
