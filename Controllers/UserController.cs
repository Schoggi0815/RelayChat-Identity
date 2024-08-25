using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelayChat_Identity.Models.Dtos;
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

    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }

        var friends = await userService.GetFriends(Guid.Parse(userId));
        return Ok(friends.ToDtos());
    }

    [HttpGet("friend-requests/sent")]
    public async Task<IActionResult> GetSentFriendRequests()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }

        var requests = await userService.GetSentFriendRequests(Guid.Parse(userId));
        return Ok(requests.ToDtos());
    }
    
    [HttpGet("friend-requests/received")]
    public async Task<IActionResult> GetReceivedFriendRequests()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }

        var requests = await userService.GetReceivedFriendRequests(Guid.Parse(userId));
        return Ok(requests.ToDtos());
    }
    
    [HttpPost("friend-requests/{senderId:guid}/accept")]
    public async Task<IActionResult> GetReceivedFriendRequests(Guid senderId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }

        var sender = await userService.AcceptFriendRequest(senderId, Guid.Parse(userId));
        if (sender == null)
        {
            return NotFound("Friend request not found");
        }

        return Ok(sender.ToDto());
    }
    
    [HttpPost("friend-requests/read/{senderId:guid}")]
    public async Task<IActionResult> MarkFriendRequestAsRead(Guid senderId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }

        var unreadFriendRequests = await userService.MarkFriendRequestAsRead(senderId, Guid.Parse(userId));
        if (unreadFriendRequests == null)
        {
            return NotFound("Friend request not found");
        }

        return Ok(unreadFriendRequests.ToDtos());
    }
    
    [HttpPost("friend-requests/read")]
    public async Task<IActionResult> MarkAllFriendRequestAsRead()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }

        await userService.MarkAllFriendRequestsAsRead(Guid.Parse(userId));

        return NoContent();
    }
    
    [HttpGet("friend-requests/unread")]
    public async Task<IActionResult> GetUnreadFriendRequests()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Forbid();
        }

        var unreadFriendRequests = await userService.GetUnreadFriendRequests(Guid.Parse(userId));

        return Ok(unreadFriendRequests.ToDtos());
    }

    [HttpPost("{receiverId:guid}/friends")]
    public async Task<IActionResult> SendFriendRequest(Guid receiverId)
    {
        var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (senderId == null)
        {
            return Forbid();
        }

        var friendRequest = await userService.SendFriendRequest(Guid.Parse(senderId), receiverId);
        if (friendRequest == null)
        {
            return UnprocessableEntity();
        }

        return Ok(friendRequest.ToDto());
    }
}
