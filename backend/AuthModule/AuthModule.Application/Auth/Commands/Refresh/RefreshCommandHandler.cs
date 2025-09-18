using AuthModule.Application.Auth.Commands.Login;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Interfaces.Repositories;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Models;
using AuthModule.Domain.Entities;
using AuthModule.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;

namespace AuthModule.Application.Auth.Commands.Refresh
{
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthResult>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly ILogger<RefreshCommandHandler> _logger;
        public RefreshCommandHandler(
            IAuthUserRepository authUserRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ICurrentUserService currentUserService,
            IAuthUnitOfWork unitOfWork,
            IAuthService authService,
            ILogger<RefreshCommandHandler> logger)
        {
            _authUserRepository = authUserRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _authService = authService;
            _logger = logger;
        }

        public async Task<AuthResult> Handle(RefreshCommand command, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken);

            if (token == null || token.IsRevoked)
            {
                _logger.LogWarning("[Auth Module] Refresh token {token} not found or already revoked.", command.RefreshToken);
                throw new RefreshTokenOperationException("Refresh token not found or already revoked.");
            }

            if (token.ExpirationDate < DateTime.UtcNow)
            {
                token.Revoke();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("[Auth Module] Refresh token {token} has expired.", command.RefreshToken);
                throw new RefreshTokenOperationException("Refresh token has expired.");
            }

            var user = await _authUserRepository.GetByIdAsync(token.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("[Auth Module] User with ID {UserId} not found.", token.UserId);
                throw new UserOperationException("User not found.");
            }
            AuthResult response = null!;
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                response = await _authService.BuildAuthResult(user, token.Id, cancellationToken);
                token.Revoke();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }, cancellationToken);

            _logger.LogInformation("[Auth Module] Refresh token for user with ID {userId} refreshed successfully.", user.Id);
            return response;
        }
    }
}
