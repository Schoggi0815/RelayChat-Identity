using Microsoft.EntityFrameworkCore;
using RelayChat_Identity.Models;
using RelayChat_Identity.Models.Dtos;

namespace RelayChat_Identity.Services;

public class UserService(RelayChatIdentityContext db)
{
    public async Task<List<UnrelatedUserDto>> SearchUsers(Guid currentUserId)
    {
        var users = await db.Users.Where(u => u.Id != currentUserId).ToListAsync();
        return users.ToDtos().ToList();
    }

    public async Task<List<User>> GetFriends(Guid userId)
    {
        var friendRequests = db.FriendRequests.Include(fr => fr.Sender).Include(fr => fr.Receiver)
            .Where(fr => fr.Accepted);

        var sent = await friendRequests.Where(fr => fr.SenderId == userId).Select(fr => fr.Receiver).ToListAsync();
        var received = await friendRequests.Where(fr => fr.ReceiverId == userId).Select(fr => fr.Sender).ToListAsync();

        var friends = sent.Concat(received);
        return friends.Select(f => f!).ToList();
    }

    public async Task<List<FriendRequest>> GetSentFriendRequests(Guid userId)
    {
        var friendRequests = db.FriendRequests.Where(fr => fr.SenderId == userId && !fr.Accepted)
            .Include(fr => fr.Receiver);
        
        return await friendRequests.ToListAsync();
    }

    public async Task<List<FriendRequest>> GetReceivedFriendRequests(Guid userId)
    {
        var friendRequests = db.FriendRequests.Where(fr => fr.ReceiverId == userId && !fr.Accepted)
            .Include(fr => fr.Sender);

        return await friendRequests.ToListAsync();
    }

    public async Task<FriendRequest?> SendFriendRequest(Guid senderId, Guid receiverId)
    {
        var receiver = await db.Users.FindAsync(receiverId);
        if (receiver == null)
        {
            return null;
        }
        
        var existing = await db.FriendRequests.SingleOrDefaultAsync(fr =>
            fr.SenderId == senderId && fr.ReceiverId == receiverId
            || fr.ReceiverId == senderId && fr.SenderId == receiverId);

        if (existing != null)
        {
            return null;
        }

        var friendRequest = new FriendRequest
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Receiver = receiver,
            Accepted = false,
        };

        db.FriendRequests.Add(friendRequest);
        await db.SaveChangesAsync();
        return friendRequest;
    }

    public async Task<User?> AcceptFriendRequest(Guid senderId, Guid receiverId)
    {
        var friendRequest = await db.FriendRequests.Include(fr => fr.Sender).SingleOrDefaultAsync(fr =>
            fr.SenderId == senderId && fr.ReceiverId == receiverId && !fr.Accepted);

        if (friendRequest == null)
        {
            return null;
        }

        friendRequest.Accepted = true;
        friendRequest.AcceptedAt = DateTimeOffset.Now;
        await db.SaveChangesAsync();

        return friendRequest.Sender;
    }

    public async Task<List<FriendRequest>?> MarkFriendRequestAsRead(Guid senderId, Guid receiverId)
    {
        var friendRequest = await db.FriendRequests.SingleOrDefaultAsync(fr =>
            fr.SenderId == senderId && fr.ReceiverId == receiverId && !fr.Read);

        if (friendRequest == null)
        {
            return null;
        }

        friendRequest.Read = true;
        await db.SaveChangesAsync();

        var unreadFriendRequests = await db.FriendRequests
            .Include(fr => fr.Sender)
            .Where(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId && !fr.Read)
            .ToListAsync();

        return unreadFriendRequests;
    }
    
    public async Task MarkAllFriendRequestsAsRead(Guid receiverId)
    {
        var friendRequests = await db.FriendRequests.Where(fr => fr.ReceiverId == receiverId && !fr.Read).ToListAsync();
        foreach (var friendRequest in friendRequests)
        {
            friendRequest.Read = true;
        }

        await db.SaveChangesAsync();
    }
    
    public async Task<List<FriendRequest>> GetUnreadFriendRequests(Guid receiverId)
    {
        var friendRequests = await db.FriendRequests
            .Include(fr => fr.Sender)
            .Where(fr => fr.ReceiverId == receiverId && !fr.Read)
            .ToListAsync();

        return friendRequests;
    }

    public async Task<FriendRequest?> RemoveFriend(Guid userId, Guid friendId)
    {
        var friendRequest = await db.FriendRequests
            .Where(fr =>
                fr.SenderId == userId && fr.ReceiverId == friendId
                || fr.ReceiverId == userId && fr.SenderId == friendId)
            .Where(fr => fr.Accepted)
            .SingleOrDefaultAsync();

        if (friendRequest == null)
        {
            return null;
        }

        db.Entry(friendRequest).State = EntityState.Deleted;
        await db.SaveChangesAsync();

        return friendRequest;
    }
}
