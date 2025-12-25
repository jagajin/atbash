using Microsoft.EntityFrameworkCore;
using Atbash.Api.Models;

namespace Atbash.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<LogEntry> Logs { get; set; }
    public DbSet<User> Users { get; set; } // author

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Уникальный индекс на юз
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}