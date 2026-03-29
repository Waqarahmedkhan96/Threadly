using Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EfcRepositories;

public class AppContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<PostMedia> PostMedias => Set<PostMedia>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();

    public AppContext()
    {
    }

    public AppContext(DbContextOptions<AppContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        var dbPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "Server", "WebApi", "app.db")
        );

        var dbDirectory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrWhiteSpace(dbDirectory) && !Directory.Exists(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory);
        }

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PostLike>()
            .HasKey(pl => new { pl.PostId, pl.UserId });

        modelBuilder.Entity<PostLike>()
            .HasOne(pl => pl.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(pl => pl.PostId);

        modelBuilder.Entity<PostLike>()
            .HasOne(pl => pl.User)
            .WithMany()
            .HasForeignKey(pl => pl.UserId);

        modelBuilder.Entity<PostMedia>()
            .HasOne(pm => pm.Post)
            .WithMany(p => p.Media)
            .HasForeignKey(pm => pm.PostId);
    }
}