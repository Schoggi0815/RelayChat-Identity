using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelayChat_Identity.Services;

namespace RelayChat_Identity.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UserController(UserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchUsers()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }
        
        return Ok(await userService.SearchUsers(Guid.Parse(userId)));
    }
}
