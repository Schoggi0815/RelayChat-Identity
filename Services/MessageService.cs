using Microsoft.EntityFrameworkCore;
using RelayChat_Identity.Models;
using RelayChat_Identity.Models.Dtos;

namespace RelayChat_Identity.Services;

public class MessageService(RelayChatIdentityContext db)
{
    public async Task<PaginationDto<DirectMessage>> GetChatMessages(Guid user1Id, Guid user2Id, int offset, int take)
    {
        var messagesQuery = db.DirectMessages
            .Where(dm =>
                dm.SenderId == user1Id && dm.ReceiverId == user2Id
                || dm.ReceiverId == user1Id && dm.SenderId == user2Id)
            .OrderByDescending(dm => dm.SentAt)
            .Skip(offset);
        
        var messageCount = await messagesQuery.CountAsync();
        var messages = await messagesQuery.Take(take).ToListAsync();

        return new PaginationDto<DirectMessage>
        {
            Take = take,
            Offset = offset,
            Items = messages,
            HasMore = messageCount > take,
        };
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
