using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Data;

public static class DbInitializer
{
    public static async Task Initialize(ShowStoreContext context)
    {
        // Check if there are already shows. If so, assume DB is seeded.
        if (await context.Shows.AnyAsync())
        {
            return;
        }

        // If the tables are empty, we might have a lingering sequence from previous data.
        // This command resets the auto-increment counters for SQLite.
        await context.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name IN ('Shows', 'Seasons', 'Episodes', 'Genres', 'Actors', 'ShowTypes');");

        // Seed ShowTypes
        var tvSeriesType = new ShowType { Name = "TV Series" };
        context.ShowTypes.Add(tvSeriesType);

        // Seed Genres
        var genres = new[]
        {
            new Genre { Name = "Drama" },
            new Genre { Name = "Sci-Fi" },
            new Genre { Name = "Horror" },
            new Genre { Name = "Romance" },
            new Genre { Name = "History" }
        };
        await context.Genres.AddRangeAsync(genres); // genres array is created here

        // Seed Actors
        var actors = new[]
        {
            new Actor { Name = "Winona Ryder" },
            new Actor { Name = "David Harbour" },
            new Actor { Name = "Olivia Colman" }, // actors array is created here
            new Actor { Name = "Phoebe Dynevor" },
            new Actor { Name = "Regé-Jean Page" }
        };
        await context.Actors.AddRangeAsync(actors);

        // Seed Shows
        var shows = new[]
        {
            new Show
            {
                Title = "Stranger Things",
                Description = "A group of kids uncover supernatural mysteries in their small town.",
                ReleaseDate = new DateOnly(2016, 7, 15),
                ShowType = tvSeriesType
            },
            new Show
            {
                Title = "The Crown",
                Description = "Chronicles the life of Queen Elizabeth II from the 1940s onward.",
                ReleaseDate = new DateOnly(2016, 11, 4),
                ShowType = tvSeriesType
            },
            new Show
            {
                Title = "Bridgerton",
                Description = "Wealthy families navigate romance and scandal in Regency-era London.",
                ReleaseDate = new DateOnly(2020, 12, 25),
                ShowType = tvSeriesType
            }
        };
        await context.Shows.AddRangeAsync(shows);

        // Save changes to get IDs and allow for robust lookups
        await context.SaveChangesAsync();

        // Create dictionaries for robust, non-index-based lookups
        var genresDict = await context.Genres.ToDictionaryAsync(g => g.Name);
        var actorsDict = await context.Actors.ToDictionaryAsync(a => a.Name);
        var showsDict = await context.Shows.ToDictionaryAsync(s => s.Title);

        // Establish Relationships
        showsDict["Stranger Things"].Genres.AddRange(new[] { genresDict["Sci-Fi"], genresDict["Horror"], genresDict["Drama"] });
        showsDict["The Crown"].Genres.AddRange(new[] { genresDict["Drama"], genresDict["History"] });
        showsDict["Bridgerton"].Genres.AddRange(new[] { genresDict["Drama"], genresDict["Romance"] });

        showsDict["Stranger Things"].Actors.AddRange(new[] { actorsDict["Winona Ryder"], actorsDict["David Harbour"] });
        showsDict["The Crown"].Actors.Add(actorsDict["Olivia Colman"]);
        showsDict["Bridgerton"].Actors.AddRange(new[] { actorsDict["Phoebe Dynevor"], actorsDict["Regé-Jean Page"] });

        // Save the many-to-many relationship changes
        await context.SaveChangesAsync();

        // Seed Seasons
        var seasons = new[]
        {
            new Season { Show = showsDict["Stranger Things"], SeasonNumber = 1, ReleaseDate = new DateOnly(2016, 7, 15) },
            new Season { Show = showsDict["The Crown"], SeasonNumber = 1, ReleaseDate = new DateOnly(2016, 11, 4) },
            new Season { Show = showsDict["Bridgerton"], SeasonNumber = 1, ReleaseDate = new DateOnly(2020, 12, 25) }
        };
        await context.Seasons.AddRangeAsync(seasons);
        await context.SaveChangesAsync();

        // Create a dictionary for robust season lookups
        var seasonsDict = await context.Seasons.ToDictionaryAsync(s => new { s.ShowId, s.SeasonNumber });

        // Seed Episodes
        var episodes = new[]
        {
            new Episode { Title = "Chapter One: The Vanishing", Description = "A young boy disappears, revealing a mystery in the town.", EpisodeNumber = 1, ReleaseDate = new DateOnly(2016, 7, 15), Season = seasonsDict[new { ShowId = showsDict["Stranger Things"].Id, SeasonNumber = 1 }] },
            new Episode { Title = "Wolferton Splash", Description = "The early reign of Queen Elizabeth II begins.", EpisodeNumber = 1, ReleaseDate = new DateOnly(2016, 11, 4), Season = seasonsDict[new { ShowId = showsDict["The Crown"].Id, SeasonNumber = 1 }] },
            new Episode { Title = "Diamond of the First Water", Description = "Introduction to the Bridgerton family and London high society.", EpisodeNumber = 1, ReleaseDate = new DateOnly(2020, 12, 25), Season = seasonsDict[new { ShowId = showsDict["Bridgerton"].Id, SeasonNumber = 1 }] }
        };
        await context.Episodes.AddRangeAsync(episodes);

        // Final Save
        await context.SaveChangesAsync();
    }
}
