using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RelayChat_Identity.Services;

namespace RelayChat_Identity.Hubs;

[Authorize]
public class DirectMessageHub(MessageService messageService) : Hub
{
    public const string UserGroupName = "user";
    
    public async Task SendMessage(string message, Guid receiverId, DateTimeOffset sentAt)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return;
        }
        
        if (message.Length > 4096)
        {
            return;
        }

        if (sentAt.AddMinutes(1) < DateTimeOffset.Now)
        {
            return;
        }
        
        if (sentAt.AddMinutes(-1) > DateTimeOffset.Now)
        {
            return;
        }
        
        var dm = await messageService.SendChatMessage(Guid.Parse(userId), receiverId, message, DateTimeOffset.Now);
        
        var send1 = Clients.Group($"{UserGroupName}-{dm.ReceiverId}").SendAsync("ReceiveMessage", dm.ToDto());
        var send2 = Clients.Group($"{UserGroupName}-{dm.SenderId}").SendAsync("ReceiveMessage", dm.ToDto());
        await Task.WhenAll(send1, send2);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return;
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{UserGroupName}-{userId}");
    }
}
