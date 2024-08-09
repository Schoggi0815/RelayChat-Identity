using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RelayChat_Identity.Models;
using RelayChat_Identity.Models.Dtos;
using WebAuthn.Net.Models.Protocol.Enums;
using WebAuthn.Net.Models.Protocol.Json.AuthenticationCeremony.VerifyAssertion;
using WebAuthn.Net.Models.Protocol.Json.RegistrationCeremony.CreateCredential;
using WebAuthn.Net.Models.Protocol.Json.RegistrationCeremony.CreateOptions;
using WebAuthn.Net.Models.Protocol.RegistrationCeremony.CreateOptions;
using WebAuthn.Net.Services.AuthenticationCeremony;
using WebAuthn.Net.Services.AuthenticationCeremony.Models.CreateOptions;
using WebAuthn.Net.Services.AuthenticationCeremony.Models.VerifyAssertion;
using WebAuthn.Net.Services.RegistrationCeremony;
using WebAuthn.Net.Services.RegistrationCeremony.Models.CreateCredential;
using WebAuthn.Net.Services.RegistrationCeremony.Models.CreateOptions;
using WebAuthn.Net.Services.Serialization.Cose.Models.Enums;

namespace RelayChat_Identity.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IRegistrationCeremonyService registrationCeremonyService, IAuthenticationCeremonyService authenticationCeremonyService) : ControllerBase
{
    [HttpPost]
    [Route("registration/1")]
    public async Task<IActionResult> Register1([FromBody] LoginDto loginDto)
    {
        var result = await registrationCeremonyService.BeginCeremonyAsync(HttpContext, new BeginRegistrationCeremonyRequest(
            origins: new RegistrationCeremonyOriginParameters(["http://localhost:5173"]),
            topOrigins: null,
            rpDisplayName: "RelayChat",
            user: new PublicKeyCredentialUserEntity(
                name: loginDto.Username,
                id: [0x01, 0x03, 0x03, 0x07],
                displayName: loginDto.Displayname),
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
        
        return Ok(new {Options = new {PublicKey = result.Options}, result.RegistrationCeremonyId});
    }
    
    [HttpPost]
    [Route("registration/2")]
    public async Task<IActionResult> Register2([FromBody] RegistrationResponse registrationResponse)
    {
        var result = await registrationCeremonyService.CompleteCeremonyAsync(httpContext: HttpContext,
            request: new CompleteRegistrationCeremonyRequest(
                registrationCeremonyId: registrationResponse.RegistrationCeremonyId,
                description: "Passkey",
                response: registrationResponse.ResponseJson),
            cancellationToken: CancellationToken.None);

        if (!result.HasError)
        {
            
            
            return Ok();
        }
        return Forbid();
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

        return Ok(new {Options = new {PublicKey = result.Options}, result.AuthenticationCeremonyId});
    }
    
    [HttpPost("login/2")]
    public async Task<IActionResult> Login2([FromBody] LoginResponse loginResponse)
    {
        var result = await authenticationCeremonyService.CompleteCeremonyAsync(
            httpContext: HttpContext,
            request: new CompleteAuthenticationCeremonyRequest(
                authenticationCeremonyId: loginResponse.AuthenticationCeremonyId,
                response: loginResponse.ResponseJson),
            cancellationToken: CancellationToken.None);

        if (!result.HasError)
        {
            return Ok();
        }
        return Forbid();
    }

    public record RegistrationResponse(RegistrationResponseJSON ResponseJson, string RegistrationCeremonyId);

    public record LoginResponse(AuthenticationResponseJSON ResponseJson, string AuthenticationCeremonyId);
}