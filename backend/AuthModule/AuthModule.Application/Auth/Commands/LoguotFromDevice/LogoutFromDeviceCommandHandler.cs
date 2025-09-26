using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthModule.Application.Auth.Commands.LoguotFromDevice
{
    public class LogoutFromDeviceCommandHandler : IRequestHandler<LogoutFromDeviceCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly ILogger<LogoutFromDeviceCommandHandler> _logger;
        public LogoutFromDeviceCommandHandler(IRefreshTokenRepository refreshTokenRepository,
            IAuthUnitOfWork unitOfWork,
            ILogger<LogoutFromDeviceCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(LogoutFromDeviceCommand command, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken);
            token.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[Auth Module(LogoutFromDeviceCommandhandler)] User logged out from device with refresh token {refreshToken}.", command.RefreshToken);
        }
    }
}
