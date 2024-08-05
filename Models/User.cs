using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RelayChat_Identity.Models;

public class User : IdentityUser<Guid>
{
    public List<Role> Roles { get; set; } = [];
    [MaxLength(1024)]
    public string? RefreshToken { get; set; }

    public DateTimeOffset RefreshTokenExpiryTime { get; set; }
}