using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using AuthModule.Application.Dtos.Requests;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using AuthModule.Application.Interfaces.Services;
using AuthModule.Application.Dtos.Responses;
using MediatR;
using AuthModule.Application.OAuth.Commands.OAuthLogin;

namespace Marketplace.Api.Controllers
{
    [Route("api/auth/oauth/google")]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        private readonly ICookieService _cookieService;
        private readonly IMediator _mediator;
        private readonly GoogleOptions _googleOptions;
        public GoogleAuthController(
            ICookieService cookieService,
            IMediator mediator,
            IOptions<GoogleOptions> googleOptions)
        {
            _cookieService = cookieService;
            _mediator = mediator;
            _googleOptions = googleOptions.Value;
        }

        [HttpPost]
        public async Task<ActionResult<AuthorizeResponse>> GoogleLogin([FromBody] GoogleTokenRequest request, CancellationToken cancellationToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _googleOptions.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            var result = await _mediator.Send(new OAuthLoginCommand("Google", payload.Subject, payload.Email), cancellationToken);

            if (result.RefreshToken != null)
                _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            return Ok(result.Response);
        }
    }
}