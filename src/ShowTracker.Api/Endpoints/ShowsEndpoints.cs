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
        {
            var shows = await dbContext.Shows
                .Include(s => s.Genres)
                .ToListAsync();

            var showDtos = shows.Select(s => s.ToSummaryDto()).ToList();

            return Results.Ok(showDtos);
        });

        // GET show by id
        group.MapGet("/{id}", async (int id, ShowStoreContext dbContext) =>
        {
            var show = await dbContext.Shows
                .Include(s => s.Genres)
                .Include(s => s.Seasons)
                    .ThenInclude(se => se.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null)
                return Results.NotFound();

            var showDto = show.ToDto();
            return Results.Ok(showDto);
        }).WithName(GetShowEndpoint);

        // POST create new show
        group.MapPost("/", async (ShowCreateDto createDto, ShowStoreContext dbContext) =>
        {
            var show = createDto.ToEntity();

            dbContext.Shows.Add(show);
            await dbContext.SaveChangesAsync();

            var showDto = show.ToDto();
            return Results.CreatedAtRoute(GetShowEndpoint, new { id = show.Id }, showDto);
        });

        // PUT update existing show
        group.MapPut("/{id}", async (int id, ShowUpdateDto updateDto, ShowStoreContext dbContext) =>
        {
            var show = await dbContext.Shows
                .Include(s => s.Seasons)
                .Include(s => s.Genres)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null) return Results.NotFound();


            updateDto.UpdateEntity(show);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE show
        group.MapDelete("/{id}", async (int id, ShowStoreContext dbContext) =>
        {
            var show = await dbContext.Shows
                .Include(s => s.Seasons)
                    .ThenInclude(se => se.Episodes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null) return Results.NotFound();

            dbContext.Shows.Remove(show);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        return group;
    }
}
