using Microsoft.EntityFrameworkCore;
using Atbash.Api.Models;

namespace Atbash.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<TextEntry> Texts { get; set; }
    public DbSet<LogEntry> Logs { get; set; }
    public DbSet<User> Users { get; set; } // author

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Уникальный индекс на юз
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        //связь textentry->user
        modelBuilder.Entity<TextEntry>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}