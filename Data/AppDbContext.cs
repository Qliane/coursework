using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Pages;
public class AppDbContext : IdentityDbContext{

    public DbSet<List> Lists { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Report> Reports { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base (options){
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Настройка отношений
        builder.Entity<List>()
            .HasOne(l => l.User)
            .WithMany(u => u.Lists)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Item>()
            .HasOne(i => i.List)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.ListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}