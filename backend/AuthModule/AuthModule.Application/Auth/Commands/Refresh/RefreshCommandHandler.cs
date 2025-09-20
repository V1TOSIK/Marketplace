using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Models;
using AuthModule.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthModule.Application.Auth.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResult>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly ILogger<RefreshCommandHandler> _logger;
        public RefreshCommandHandler(
            IAuthUserRepository authUserRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAuthUnitOfWork unitOfWork,
            IAuthService authService,
            ILogger<RefreshCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _authService = authService;
            _logger = logger;
        }

        public async Task<AuthResult> Handle(RefreshCommand command, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken);

            if (token == null || token.IsRevoked)
            {
                _logger.LogWarning("[Auth Module(RefreshCommandHandler)] Refresh token {token} not found or already revoked.", command.RefreshToken);
                throw new RefreshTokenOperationException("Refresh token not found or already revoked.");
            }

            if (token.ExpirationDate < DateTime.UtcNow)
            {
                token.Revoke();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("[Auth Module(RefreshCommandHandler)] Refresh token {token} has expired.", command.RefreshToken);
                throw new RefreshTokenOperationException("Refresh token has expired.");
            }

            var user = await _authUserRepository.GetByIdAsync(token.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("[Auth Module(RefreshCommandHandler)] User with ID {UserId} not found.", token.UserId);
                throw new UserOperationException("User not found.");
            }
            AuthResult response = null!;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                response = await _authService.BuildAuthResult(user, token.Id, cancellationToken);
                token.Revoke();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

            _logger.LogInformation("[Auth Module(RefreshCommandHandler)] Refresh token for user with ID {userId} refreshed successfully.", user.Id);
            return response;
        }
    }
}
