using Microsoft.AspNetCore.Mvc;
using WebAuthn.Net.Services.Static;

namespace RelayChat_Identity.Controllers;

[ApiController]
[Route("api/v1/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        return Ok(Base64Url.Encode(Guid.Parse("6fc1774e-9314-4aec-3f94-08dcb8b30bed").ToByteArray()));
    }
}
