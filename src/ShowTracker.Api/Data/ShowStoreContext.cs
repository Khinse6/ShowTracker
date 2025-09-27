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

        // Seed ShowTypes
        modelBuilder.Entity<ShowType>().HasData(
            new ShowType { Id = 1, Name = "TV Series" }
        );

        // Seed Shows
        modelBuilder.Entity<Show>().HasData(
            new Show
            {
                Id = 1,
                Title = "Stranger Things",
                Description = "A group of kids uncover supernatural mysteries in their small town.",
                ReleaseDate = new DateOnly(2016, 7, 15),
                ShowTypeId = 1
            },
            new Show
            {
                Id = 2,
                Title = "The Crown",
                Description = "Chronicles the life of Queen Elizabeth II from the 1940s onward.",
                ReleaseDate = new DateOnly(2016, 11, 4),
                ShowTypeId = 1
            },
            new Show
            {
                Id = 3,
                Title = "Bridgerton",
                Description = "Wealthy families navigate romance and scandal in Regency-era London.",
                ReleaseDate = new DateOnly(2020, 12, 25),
                ShowTypeId = 1
            }
        );

        // Seed Seasons
        modelBuilder.Entity<Season>().HasData(
            new Season { Id = 1, ShowId = 1, SeasonNumber = 1, ReleaseDate = new DateOnly(2016, 7, 15) },
            new Season { Id = 2, ShowId = 2, SeasonNumber = 1, ReleaseDate = new DateOnly(2016, 11, 4) },
            new Season { Id = 3, ShowId = 3, SeasonNumber = 1, ReleaseDate = new DateOnly(2020, 12, 25) }
        );

        // Seed Episodes
        modelBuilder.Entity<Episode>().HasData(
            new Episode { Id = 1, Title = "Chapter One: The Vanishing", Description = "A young boy disappears, revealing a mystery in the town.", EpisodeNumber = 1, ReleaseDate = new DateOnly(2016, 7, 15), SeasonId = 1 },
            new Episode { Id = 2, Title = "Wolferton Splash", Description = "The early reign of Queen Elizabeth II begins.", EpisodeNumber = 1, ReleaseDate = new DateOnly(2016, 11, 4), SeasonId = 2 },
            new Episode { Id = 3, Title = "Diamond of the First Water", Description = "Introduction to the Bridgerton family and London high society.", EpisodeNumber = 1, ReleaseDate = new DateOnly(2020, 12, 25), SeasonId = 3 }
        );

        // Seed Genres
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Drama" },
            new Genre { Id = 2, Name = "Sci-Fi" },
            new Genre { Id = 3, Name = "Horror" },
            new Genre { Id = 4, Name = "Romance" },
            new Genre { Id = 5, Name = "History" }
        );

        // Configure Show <-> Genre Many-to-Many
        modelBuilder.Entity<Show>()
            .HasMany(s => s.Genres)
            .WithMany(g => g.Shows)
            .UsingEntity<Dictionary<string, object>>(
                "ShowGenres",
                j => j.HasOne<Genre>().WithMany().HasForeignKey("GenresId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Show>().WithMany().HasForeignKey("ShowsId").OnDelete(DeleteBehavior.Cascade)
            );

        // Seed ShowGenres
        modelBuilder.Entity("ShowGenres").HasData(
            new { ShowsId = 1, GenresId = 2 },
            new { ShowsId = 1, GenresId = 3 },
            new { ShowsId = 1, GenresId = 1 },
            new { ShowsId = 2, GenresId = 1 },
            new { ShowsId = 2, GenresId = 5 },
            new { ShowsId = 3, GenresId = 1 },
            new { ShowsId = 3, GenresId = 4 }
        );
    }

}
