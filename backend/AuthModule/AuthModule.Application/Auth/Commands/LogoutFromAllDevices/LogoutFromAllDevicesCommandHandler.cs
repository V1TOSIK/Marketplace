using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthModule.Application.Auth.Commands.LogoutFromAllDevices
{
    public class LogoutFromAllDevicesCommandHandler : IRequestHandler<LogoutFromAllDevicesCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<LogoutFromAllDevicesCommandHandler> _logger;

        public LogoutFromAllDevicesCommandHandler(IRefreshTokenRepository refreshTokenRepository,
            IAuthUnitOfWork unitOfWork,
            ILogger<LogoutFromAllDevicesCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(LogoutFromAllDevicesCommand command, CancellationToken cancellationToken)
        {
            await _refreshTokenRepository.RevokeAllAsync(command.UserId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Auth Module(LogoutFromAllDevicesCommandHandler)] All devices logged out for user with ID {userId}.", command.UserId);
        }
    }
}
