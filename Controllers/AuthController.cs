using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RelayChat_Identity.Models;
using RelayChat_Identity.Models.Dtos;
using WebAuthn.Net.Models.Protocol.Enums;
using WebAuthn.Net.Models.Protocol.Json.RegistrationCeremony.CreateOptions;
using WebAuthn.Net.Models.Protocol.RegistrationCeremony.CreateOptions;
using WebAuthn.Net.Services.RegistrationCeremony;
using WebAuthn.Net.Services.RegistrationCeremony.Models.CreateOptions;
using WebAuthn.Net.Services.Serialization.Cose.Models.Enums;

namespace RelayChat_Identity.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IRegistrationCeremonyService registrationCeremonyService) : ControllerBase
{
    [HttpPost]
    [Route("registration/1")]
    [ProducesResponseType(typeof(PublicKeyCredentialCreationOptionsJSON), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await registrationCeremonyService.BeginCeremonyAsync(HttpContext, new BeginRegistrationCeremonyRequest(
            origins: null,
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
        
        return Ok(result.Options);
    }
}