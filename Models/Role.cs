using Microsoft.AspNetCore.Identity;

namespace RelayChat_Identity.Models;

public class Role : IdentityRole<Guid>
{
    public List<User> Users { get; set; } = null!;
}