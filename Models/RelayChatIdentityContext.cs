using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RelayChat_Identity.Models;

public class RelayChatIdentityContext(DbContextOptions<RelayChatIdentityContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<Test> Tests { get; set; }
    
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