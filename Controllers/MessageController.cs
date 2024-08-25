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
    
    [HttpPost("{otherUserId:guid}")]
    public async Task<IActionResult> SendMessage(Guid otherUserId, [FromBody] SendMessageDto sendMessageDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }

        if (sendMessageDto.Message.Length > 4096)
        {
            return UnprocessableEntity("Message too long");
        }

        if (sendMessageDto.SentAt.AddMinutes(1) < DateTimeOffset.Now)
        {
            return UnprocessableEntity("Message too old");
        }
        
        if (sendMessageDto.SentAt.AddMinutes(-1) > DateTimeOffset.Now)
        {
            return UnprocessableEntity("Message in the future");
        }

        var messages = await messageService.SendChatMessage(Guid.Parse(userId), otherUserId, sendMessageDto.Message, sendMessageDto.SentAt);
        return Ok(messages.ToDto());
    }
}