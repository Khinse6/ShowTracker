using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Data;

public class ShowStoreContext : IdentityDbContext<User>
{
    public ShowStoreContext(DbContextOptions<ShowStoreContext> options) : base(options) { }

    public DbSet<Show> Shows => Set<Show>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ShowType> ShowTypes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Important for Identity

        // Configure many-to-many: User <-> Show (favorites)
        modelBuilder.Entity<User>()
            .HasMany(u => u.FavoriteShows)
            .WithMany(s => s.FavoritedByUsers)
            .UsingEntity<Dictionary<string, object>>(
                "UserFavorites",
                j => j.HasOne<Show>().WithMany().HasForeignKey("ShowId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade)
            );

        // Configure one-to-many: User <-> RefreshToken
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Show <-> Genre Many-to-Many
        modelBuilder.Entity<Show>()
            .HasMany(s => s.Genres)
            .WithMany(g => g.Shows)
            .UsingEntity<Dictionary<string, object>>(
                "ShowGenres",
                j => j.HasOne<Genre>().WithMany().HasForeignKey("GenresId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Show>().WithMany().HasForeignKey("ShowsId").OnDelete(DeleteBehavior.Cascade)
            );

        modelBuilder.Entity<Show>()
        .HasMany(s => s.Actors)
        .WithMany(a => a.Shows)
        .UsingEntity<Dictionary<string, object>>(
            "ShowActors",
            j => j.HasOne<Actor>().WithMany().HasForeignKey("ActorId").OnDelete(DeleteBehavior.Cascade),
            j => j.HasOne<Show>().WithMany().HasForeignKey("ShowId").OnDelete(DeleteBehavior.Cascade)
        );

        // NOTE: All seeding is now done programmatically in DbInitializer.cs.
        // The SeedData.cs file can be removed.
    }

}
