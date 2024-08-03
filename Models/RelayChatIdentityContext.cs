using Microsoft.EntityFrameworkCore;

namespace RelayChat_Identity.Models;

public class RelayChatIdentityContext(DbContextOptions<RelayChatIdentityContext> options) : DbContext(options)
{
    public DbSet<Test> Tests { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}