using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelayChat_Identity.Models.Dtos;
using RelayChat_Identity.Services;

namespace RelayChat_Identity.Controllers;

[ApiController]
[Route("api/v1/messages")]
[Authorize]
public class MessageController(MessageService messageService) : ControllerBase
{
    [HttpGet("{otherUserId:guid}")]
    public async Task<IActionResult> GetChatMessages(Guid otherUserId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }

        var messages = await messageService.GetChatMessages(Guid.Parse(userId), otherUserId);
        return Ok(messages.ToDtos());
    }
}