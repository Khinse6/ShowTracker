using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Endpoints;

public static class ShowsEndpoints
{
    const string GetShowEndpoint = "GetShow";
    public static RouteGroupBuilder MapShowsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/shows");

        // GET all shows (summary)
        group.MapGet("/", async (ShowStoreContext dbContext) =>
            await dbContext.Shows
                .Include(show => show.Genres)
                .Select(show => show.ToShowSummaryDto())
                .AsNoTracking()
                .ToListAsync());

        // GET show by id
        group.MapGet("/{id}", async (int id, ShowStoreContext dbContext) =>
        {
            Show? show = await dbContext.Shows
            .Include(s => s.Genres)
            .Include(s => s.Seasons)
                .ThenInclude(se => se.Episodes)
            .FirstOrDefaultAsync(s => s.Id == id);

            return show is null ? Results.NotFound() : Results.Ok(show.ToShowDetailsDto());

        }).WithName(GetShowEndpoint);

        // POST create new show
        group.MapPost("/", async (ShowCreateDto newShow, ShowStoreContext dbContext) =>
        {
            Show show = newShow.ToEntity();
            dbContext.Shows.Add(show);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetShowEndpoint, new { id = show.Id }, show.ToShowSummaryDto());
        });

        // PUT update existing show
        group.MapPut("/{id}", async (int id, ShowUpdateDto updateDto, ShowStoreContext dbContext) =>
        {
            Show? existingShow = await dbContext.Shows.FindAsync(id);

            if (existingShow is null) { return Results.NotFound(); }

            dbContext.Entry(existingShow).CurrentValues.SetValues(updateDto.ToEntity());
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE show
        group.MapDelete("/{id}", async (int id, ShowStoreContext dbContext) =>
        {
            await dbContext.Shows
                .Where(show => show.Id == id)
                .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }
}
