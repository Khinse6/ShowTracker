using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Data;

public class ShowStoreContext : IdentityDbContext<User>
{
    public ShowStoreContext(DbContextOptions<ShowStoreContext> options) : base(options) { }

    public virtual DbSet<Show> Shows { get; set; }
    public virtual DbSet<Season> Seasons { get; set; }
    public virtual DbSet<Episode> Episodes { get; set; }
    public virtual DbSet<Actor> Actors { get; set; }
    public virtual DbSet<Genre> Genres { get; set; }
    public virtual DbSet<ShowType> ShowTypes { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the one-to-many relationship between User and RefreshToken
        modelBuilder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId);

        // Configure the many-to-many relationship between Show and Genre
        modelBuilder.Entity<Show>()
            .HasMany(s => s.Genres)
            .WithMany(g => g.Shows)
            .UsingEntity<Dictionary<string, object>>(
                "ShowGenre",
                j => j.HasOne<Genre>().WithMany().OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Show>().WithMany().OnDelete(DeleteBehavior.Cascade)
            );

        // Configure the many-to-many relationship for User Favorites
        modelBuilder.Entity<User>()
            .HasMany(u => u.FavoriteShows)
            .WithMany(s => s.FavoritedByUsers)
            .UsingEntity<Dictionary<string, object>>(
                "UserFavorites",
                j => j.HasOne<Show>().WithMany().OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<User>().WithMany().OnDelete(DeleteBehavior.Cascade)
            );

        // Configure the many-to-many relationship between Show and Actor
        modelBuilder.Entity<Show>()
            .HasMany(s => s.Actors)
            .WithMany(a => a.Shows)
            .UsingEntity<Dictionary<string, object>>(
                "ShowActor",
                j => j.HasOne<Actor>().WithMany().OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Show>().WithMany().OnDelete(DeleteBehavior.Cascade)
            );

        // Configure the one-to-many relationship between ShowType and Show
        modelBuilder.Entity<ShowType>()
            .HasMany(st => st.Shows)
            .WithOne(s => s.ShowType)
            .HasForeignKey(s => s.ShowTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the one-to-many relationship between Show and Season
        modelBuilder.Entity<Show>()
            .HasMany(s => s.Seasons)
            .WithOne(se => se.Show)
            .HasForeignKey(se => se.ShowId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the one-to-many relationship between Season and Episode
        modelBuilder.Entity<Season>()
            .HasMany(se => se.Episodes)
            .WithOne(ep => ep.Season)
            .HasForeignKey(ep => ep.SeasonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
