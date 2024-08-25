using Microsoft.EntityFrameworkCore;
using RelayChat_Identity.Models;

namespace RelayChat_Identity.Services;

public class MessageService(RelayChatIdentityContext db)
{
    public async Task<List<DirectMessage>> GetChatMessages(Guid user1Id, Guid user2Id)
    {
        var messages = await db.DirectMessages
            .Where(dm =>
                dm.SenderId == user1Id && dm.ReceiverId == user2Id
                || dm.ReceiverId == user1Id && dm.SenderId == user2Id)
            .OrderBy(dm => dm.SentAt)
            .ToListAsync();

        return messages;
    }

    public async Task<DirectMessage> SendChatMessage(
        Guid fromUserId,
        Guid toUserId,
        string message,
        DateTimeOffset sentAt
    )
    {
        var directMessage = new DirectMessage
        {
            Message = message,
            SentAt = sentAt,
            ReceiverId = toUserId,
            SenderId = fromUserId,
        };

        db.DirectMessages.Add(directMessage);
        await db.SaveChangesAsync();

        return directMessage;
    }
}
