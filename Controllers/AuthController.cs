using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RelayChat_Identity.Models;
using RelayChat_Identity.Models.Dtos;
using RelayChat_Identity.Services;
using WebAuthn.Net.Models.Protocol.Enums;
using WebAuthn.Net.Models.Protocol.Json.AuthenticationCeremony.VerifyAssertion;
using WebAuthn.Net.Models.Protocol.Json.RegistrationCeremony.CreateCredential;
using WebAuthn.Net.Models.Protocol.RegistrationCeremony.CreateOptions;
using WebAuthn.Net.Services.AuthenticationCeremony;
using WebAuthn.Net.Services.AuthenticationCeremony.Models.CreateOptions;
using WebAuthn.Net.Services.AuthenticationCeremony.Models.VerifyAssertion;
using WebAuthn.Net.Services.RegistrationCeremony;
using WebAuthn.Net.Services.RegistrationCeremony.Models.CreateCredential;
using WebAuthn.Net.Services.RegistrationCeremony.Models.CreateOptions;
using WebAuthn.Net.Services.Serialization.Cose.Models.Enums;
using WebAuthn.Net.Services.Static;
using AuthenticationService=RelayChat_Identity.Services.AuthenticationService;

namespace RelayChat_Identity.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IRegistrationCeremonyService registrationCeremonyService,
    IAuthenticationCeremonyService authenticationCeremonyService,
    RegistrationCeremonyHandleService registrationCeremonyHandleService,
    AuthenticationCeremonyHandleService authenticationCeremonyHandleService,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IMemoryCache memoryCache,
    AuthenticationService authenticationService,
    RefreshTokenService refreshTokenService) : ControllerBase
{
    private const string RegisterUserCacheKey = "register-user";
    private const string PasskeyPassword = "PasskeyPassword";
    
    [HttpPost]
    [Route("registration/1")]
    public async Task<IActionResult> Register1([FromBody] RegisterDto registerDto)
    {
        var userExists = await userManager.FindByNameAsync(registerDto.Email);
        if (userExists != null)
            return Conflict("User already exists!");
        
        var result = await registrationCeremonyService.BeginCeremonyAsync(HttpContext, new BeginRegistrationCeremonyRequest(
            origins: new RegistrationCeremonyOriginParameters(["http://localhost:5173"]),
            topOrigins: null,
            rpDisplayName: "RelayChat",
            user: new PublicKeyCredentialUserEntity(
                name: registerDto.Email,
                id: Encoding.UTF8.GetBytes(registerDto.Email),
                displayName: registerDto.Displayname),
            challengeSize: 32,
            pubKeyCredParams:
            [
                CoseAlgorithm.ES256,
                CoseAlgorithm.ES384,
                CoseAlgorithm.ES512,
                CoseAlgorithm.RS256,
                CoseAlgorithm.RS384,
                CoseAlgorithm.RS512,
                CoseAlgorithm.PS256,
                CoseAlgorithm.PS384,
                CoseAlgorithm.PS512,
                CoseAlgorithm.EdDSA
            ],
            timeout: 300_000,
            excludeCredentials: RegistrationCeremonyExcludeCredentials.AllExisting(),
            authenticatorSelection: new AuthenticatorSelectionCriteria(
                authenticatorAttachment: null,
                residentKey: ResidentKeyRequirement.Required,
                requireResidentKey: true,
                userVerification: UserVerificationRequirement.Required),
            hints: null,
            attestation: null,
            attestationFormats: null,
            extensions: null
        ), CancellationToken.None);

        memoryCache.Set($"{RegisterUserCacheKey}-{result.RegistrationCeremonyId}", registerDto);

        await registrationCeremonyHandleService.SaveAsync(HttpContext, result.RegistrationCeremonyId);
        
        return Ok(new {PublicKey = result.Options});
    }
    
    [HttpPost]
    [Route("registration/2")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register2([FromBody] Registration2Dto registrationResponse)
    {
        var registrationCeremonyId = await registrationCeremonyHandleService.ReadAsync(HttpContext);

        if (registrationCeremonyId == null)
        {
            return UnprocessableEntity("Registration ceremony cookie missing");
        }

        if (!memoryCache.TryGetValue($"{RegisterUserCacheKey}-{registrationCeremonyId}", out RegisterDto? registerDto) || registerDto == null)
        {
            await registrationCeremonyHandleService.DeleteAsync(HttpContext);
            return NotFound("Internal User not found, try again");
        }
        
        memoryCache.Remove($"{RegisterUserCacheKey}-{registrationCeremonyId}");
        
        var result = await registrationCeremonyService.CompleteCeremonyAsync(httpContext: HttpContext,
            request: new CompleteRegistrationCeremonyRequest(
                registrationCeremonyId: registrationCeremonyId,
                description: registrationResponse.TokenDescription,
                response: registrationResponse.ResponseJson),
            cancellationToken: CancellationToken.None);

        if (result.HasError)
        {
            return Forbid();
        }
        
        await registrationCeremonyHandleService.DeleteAsync(HttpContext);

        var user = new User
        {
            Email = registerDto.Email,
            UserName = registerDto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            DisplayName = registerDto.Displayname,
        };

        var createResult = await userManager.CreateAsync(user, PasskeyPassword);
        if (!createResult.Succeeded)
        {
            return UnprocessableEntity(createResult.Errors.Select(e => e.Description).ToList());
        }
        
        var tokens = await SignInUser(registerDto.Email);
        if (tokens == null)
        {
            return Unauthorized();
        }
        
        return Ok(tokens);
    }

    [HttpPost("login/1")]
    public async Task<IActionResult> Login1()
    {
        var result = await authenticationCeremonyService.BeginCeremonyAsync(
            httpContext: HttpContext,
            request: new BeginAuthenticationCeremonyRequest(
                origins: new AuthenticationCeremonyOriginParameters(["http://localhost:5173"]),
                topOrigins: null,
                userHandle: null,
                challengeSize: 32,
                timeout: 300_000,
                allowCredentials: AuthenticationCeremonyIncludeCredentials.AllExisting(),
                userVerification: UserVerificationRequirement.Required,
                hints: null,
                attestation: null,
                attestationFormats: null,
                extensions: null),
            cancellationToken: CancellationToken.None);

        await authenticationCeremonyHandleService.SaveAsync(HttpContext, result.AuthenticationCeremonyId);

        return Ok(new {PublicKey = result.Options});
    }
    
    [HttpPost("login/2")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login2([FromBody] AuthenticationResponseJSON loginResponse)
    {
        var authenticationCeremonyId = await authenticationCeremonyHandleService.ReadAsync(HttpContext);
        
        if (authenticationCeremonyId == null)
        {
            return UnprocessableEntity("Authentication ceremony cookie missing");
        }
        
        var result = await authenticationCeremonyService.CompleteCeremonyAsync(
            httpContext: HttpContext,
            request: new CompleteAuthenticationCeremonyRequest(
                authenticationCeremonyId: authenticationCeremonyId,
                response: loginResponse),
            cancellationToken: CancellationToken.None);

        if (result.HasError)
            return Forbid();

        await authenticationCeremonyHandleService.DeleteAsync(HttpContext);

        if (!Base64Url.TryDecode(loginResponse.Response.UserHandle, out var userHandle))
        {
            return BadRequest("User handle not found");
        }

        var email = Encoding.UTF8.GetString(userHandle);

        var tokens = await SignInUser(email);
        if (tokens == null)
        {
            return Unauthorized();
        }
        
        return Ok(tokens);
    }
    
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
    {
        var accessToken = tokenDto.Token;
        var refreshToken = tokenDto.RefreshToken;

        var principal = authenticationService.GetPrincipalFromExpiredToken(accessToken);
        if (principal?.Identity?.Name == null)
        {
            return BadRequest("Invalid access token or refresh token");
        }

        var username = principal.Identity.Name;

        if (!await refreshTokenService.CheckRefreshToken(username, refreshToken))
        {
            return BadRequest("Invalid access token or refresh token");
        }

        var newAccessToken =
            authenticationService.CreateToken((ClaimsIdentity)principal.Identity, DateTime.UtcNow.AddHours(1));
        var newRefreshToken = await refreshTokenService.CreateRefreshToken(username);

        return Ok(new TokenDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    private async Task<TokenDto?> SignInUser(string email)
    {
        var signInResult = await signInManager.PasswordSignInAsync(email, PasskeyPassword, true, false);
        if (!signInResult.Succeeded || User.Identity == null)
        {
            return null;
        }
        
        var token = authenticationService.CreateToken((ClaimsIdentity)User.Identity, DateTime.UtcNow.AddHours(1));

        var refresh = await refreshTokenService.CreateRefreshToken(email);

        return new TokenDto { Token = token, RefreshToken = refresh };
    }

    public record Registration2Dto(RegistrationResponseJSON ResponseJson, string TokenDescription);
}