using AuthModule.Application.Interfaces.Repositories;

namespace DeleteTokenWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public Worker(ILogger<Worker> logger,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _logger = logger;
            _refreshTokenRepository = refreshTokenRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await _refreshTokenRepository.DeleteExpiredAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
