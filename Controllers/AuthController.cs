using System.Text;
using Fido2NetLib;
using Microsoft.AspNetCore.Mvc;
using RelayChat_Identity.Models.Dtos;

namespace RelayChat_Identity.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IFido2 fido2) : ControllerBase
{
    [HttpPost]
    [Route("registration/1")]
    //[ProducesResponseType(typeof(PublicKeyCredentialCreationOptionsJSON), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = new Fido2User
        {
            DisplayName = loginDto.Displayname,
            Name = loginDto.Email,
            Id = Encoding.UTF8.GetBytes(loginDto.Email),
        };

        var options = fido2.RequestNewCredential(user, []);
        
        //HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

        return Ok(new {PublicKey = options});
    }
    
    [HttpPost]
    [Route("registration/2")]
    public async Task<IActionResult> Login([FromBody] AuthenticatorAttestationRawResponse registrationResponse)
    {
        fido2.MakeNewCredentialAsync(registrationResponse, )
    }
}