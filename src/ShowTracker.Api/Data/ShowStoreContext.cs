using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Data;

public class ShowStoreContext : DbContext
{
    public ShowStoreContext(DbContextOptions<ShowStoreContext> options) : base(options) { }

    public DbSet<Show> Shows => Set<Show>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Episode> Episodes => Set<Episode>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed Shows
        modelBuilder.Entity<Show>().HasData(
            new Show
            {
                Id = 1,
                Title = "Stranger Things",
                Description = "A group of kids uncover supernatural mysteries in their small town.",
                ReleaseDate = new DateOnly(2016, 7, 15)
            },
            new Show
            {
                Id = 2,
                Title = "The Crown",
                Description = "Chronicles the life of Queen Elizabeth II from the 1940s onward.",
                ReleaseDate = new DateOnly(2016, 11, 4)
            },
            new Show
            {
                Id = 3,
                Title = "Bridgerton",
                Description = "Wealthy families navigate romance and scandal in Regency-era London.",
                ReleaseDate = new DateOnly(2020, 12, 25)
            }
        );

        // Seed Seasons
        modelBuilder.Entity<Season>().HasData(
            new Season
            {
                Id = 1,
                ShowId = 1,
                SeasonNumber = 1,
                ReleaseDate = new DateOnly(2016, 7, 15)
            },
            new Season
            {
                Id = 2,
                ShowId = 2,
                SeasonNumber = 1,
                ReleaseDate = new DateOnly(2016, 11, 4)
            },
            new Season
            {
                Id = 3,
                ShowId = 3,
                SeasonNumber = 1,
                ReleaseDate = new DateOnly(2020, 12, 25)
            }
        );

        // Seed Episodes
        modelBuilder.Entity<Episode>().HasData(
            new Episode
            {
                Id = 1,
                Title = "Chapter One: The Vanishing",
                Description = "A young boy disappears, revealing a mystery in the town.",
                EpisodeNumber = 1,
                ReleaseDate = new DateOnly(2016, 7, 15),
                SeasonId = 1
            },
            new Episode
            {
                Id = 2,
                Title = "Wolferton Splash",
                Description = "The early reign of Queen Elizabeth II begins.",
                EpisodeNumber = 1,
                ReleaseDate = new DateOnly(2016, 11, 4),
                SeasonId = 2
            },
            new Episode
            {
                Id = 3,
                Title = "Diamond of the First Water",
                Description = "Introduction to the Bridgerton family and London high society.",
                EpisodeNumber = 1,
                ReleaseDate = new DateOnly(2020, 12, 25),
                SeasonId = 3
            }
        );

        // Seed Genres
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Drama" },
            new Genre { Id = 2, Name = "Sci-Fi" },
            new Genre { Id = 3, Name = "Horror" },
            new Genre { Id = 4, Name = "Romance" },
            new Genre { Id = 5, Name = "History" }
        );

        // Many-to-Many: Show <-> Genre with cascading deletes
        modelBuilder.Entity<Show>()
            .HasMany(s => s.Genres)
            .WithMany(g => g.Shows)
            .UsingEntity<Dictionary<string, object>>(
                "ShowGenres", // Shadow join table
                j => j.HasOne<Genre>()
                      .WithMany()
                      .HasForeignKey("GenresId")
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Show>()
                      .WithMany()
                      .HasForeignKey("ShowsId")
                      .OnDelete(DeleteBehavior.Cascade)
            );

        // Seed Show-Genre join table
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
