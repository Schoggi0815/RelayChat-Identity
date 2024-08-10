using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RelayChat_Identity.Models;

namespace RelayChat_Identity.Services;

public class RefreshTokenService(RelayChatIdentityContext db, UserManager<User> userManager)
{
    public async Task<bool> CheckRefreshToken(string email, string token)
    {
        var user = await db.Users.Where(u => u.UserName == email).FirstOrDefaultAsync();

        return user != null && user.RefreshTokenExpiryTime > DateTime.Now && user.RefreshToken == token;
    }
    
    public async Task<string> CreateRefreshToken(string email)
    {
        var user = await db.Users.Where(u => u.UserName == email).FirstOrDefaultAsync();

        if (user == null)
        {
            throw new Exception($"User [{email}] not found");
        }

        var refresh = GenerateRefreshToken();

        user.RefreshToken = refresh;
        user.RefreshTokenExpiryTime = DateTimeOffset.Now.AddDays(7);
        await db.SaveChangesAsync();
        return refresh;
    }
    
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
