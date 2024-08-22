using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using RelayChat_Identity.Models.Dtos;

namespace RelayChat_Identity.Models;

public class User : IdentityUser<Guid>, IDtoAble<UnrelatedUserDto>
{
    public List<Role> Roles { get; set; } = [];

    [MaxLength(255)]
    public required string DisplayName { get; set; }

    [MaxLength(255)]
    public string? RefreshToken { get; set; }

    public DateTimeOffset RefreshTokenExpiryTime { get; set; }
    
    public UnrelatedUserDto ToDto() => new(this);
}