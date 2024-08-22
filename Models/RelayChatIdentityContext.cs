using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RelayChat_Identity.Models;

public class RelayChatIdentityContext(DbContextOptions<RelayChatIdentityContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<FriendRequest> FriendRequests { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        const string aspNetTablePrefix = "AspNet";
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && tableName.StartsWith(aspNetTablePrefix))
                entityType.SetTableName(tableName[aspNetTablePrefix.Length..]);

        }

        modelBuilder
            .Entity<FriendRequest>()
            .Property(fr => fr.CreatedAt)
            .HasDefaultValueSql("getdate()");

        modelBuilder
            .Entity<User>()
            .HasMany<User>()
            .WithMany()
            .UsingEntity<FriendRequest>(
                r => r.HasOne<User>(fr => fr.Sender).WithMany(u => u.SentFriendRequests),
        l => l.HasOne<User>(fr => fr.Receiver).WithMany(u => u.ReceivedFriendRequests),
        fr => fr.HasKey(frInner => new {frInner.SenderId, frInner.ReceiverId}));

        modelBuilder
                .Entity<User>()
                .HasMany<Role>(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<IdentityUserRole<Guid>>(
                    userRole =>
                        userRole
                            .HasOne<Role>()
                            .WithMany()
                            .HasForeignKey(ur => ur.RoleId)
                            .IsRequired(),
                    userRole =>
                        userRole
                            .HasOne<User>()
                            .WithMany()
                            .HasForeignKey(ur => ur.UserId)
                            .IsRequired()
                );
    }
}