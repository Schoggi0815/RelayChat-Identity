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
}
