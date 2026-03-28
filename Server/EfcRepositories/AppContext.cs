using Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace EfcRepositories;
public class AppContext : DbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Prefer the local WebApi database file relative to the current running assembly.
        var baseDir = System.AppContext.BaseDirectory;
        var candidatePaths = new[]
        {
            Path.Combine(baseDir, "app.db"),
            Path.Combine(baseDir, "..", "app.db"),
            Path.Combine(baseDir, "..", "..", "app.db"),
            Path.Combine(baseDir, "..", "..", "..", "app.db"),
            Path.Combine(baseDir, "..", "..", "..", "..", "WebApi", "app.db"),
            Path.Combine(Directory.GetCurrentDirectory(), "app.db"),
            Path.Combine(Directory.GetCurrentDirectory(), "Server", "WebApi", "app.db")
        };

        var dbPath = candidatePaths.FirstOrDefault(Path.Exists)
                     ?? Path.GetFullPath(Path.Combine(baseDir, "..", "..", "WebApi", "app.db"));

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }
}
