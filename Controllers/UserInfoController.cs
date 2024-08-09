using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RelayChat_Identity.Controllers;

[ApiController]
[Route("api/v1/user")]
[Authorize]
public class UserInfoController : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyUserInfo()
    {
        return Ok(User.Identity?.Name);
    }
}
