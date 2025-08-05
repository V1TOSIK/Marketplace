using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Models;
using AuthModule.Infrastructure.Services;
using Google.Apis.Auth;
using AuthModule.Infrastructure.Options;
using Microsoft.Extensions.Options;

[Route("api/auth/oauth/google")]
public class GoogleAuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IJwtProvider _jwtProvider;
    private readonly ICookieService _cookieService;
    private readonly GoogleOptions _googleOptions;
    public GoogleAuthController(IAuthService authService,
        IJwtProvider jwtProvider,
        ICookieService cookieService,
        IOptions<GoogleOptions> googleOptions)
    {
        _authService = authService;
        _jwtProvider = jwtProvider;
        _cookieService = cookieService;
        _googleOptions = googleOptions.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleTokenRequest request, CancellationToken cancellationToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { _googleOptions.ClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

        var clientInfo = new ClientInfo
        {
            IpAddress = HttpContext.Items["ClientIp"]?.ToString() ?? "unknown",
            Device = HttpContext.Items["ClientDevice"]?.ToString() ?? "unknown"
        };

        var loginRequest = new LoginRequest
        {
            Email = payload.Email,
            Provider = "Google",
            ProviderUserId = payload.Subject
        };

        var result = await _authService.LoginOrRegisterOAuth(loginRequest, clientInfo, cancellationToken);
        if (result == null)
            return BadRequest("Authentication failed");

        result.Response.AccessToken = _jwtProvider.GenerateAccessToken(result.Response.UserId, result.Response.Role);

        _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

        return Ok(result.Response);
    }
}