using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ShowTracker.Api.Data;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public class ShowActorService : IShowActorService
{
    private readonly ShowStoreContext _context;
    private readonly IMemoryCache _cache;

    public ShowActorService(ShowStoreContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IEnumerable<ActorSummaryDto>> GetActorsByShowIdAsync(int showId)
    {
        var show = await _context.Shows
            .AsNoTracking()
            .Include(s => s.Actors)
            .FirstOrDefaultAsync(s => s.Id == showId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found.");
        }

        return show.Actors
            .Select(a => a.ToSummaryDto()).ToList();
    }

    public async Task AddActorToShowAsync(int showId, int actorId)
    {
        var show = await _context.Shows
            .Include(s => s.Actors)
            .FirstOrDefaultAsync(s => s.Id == showId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found.");
        }

        var actor = await _context.Actors.FindAsync(actorId);
        if (actor == null)
        {
            throw new KeyNotFoundException("Actor not found.");
        }

        if (!show.Actors.Any(a => a.Id == actorId))
        {
            show.Actors.Add(actor);
            await _context.SaveChangesAsync();
            _cache.Remove("shows-all"); // Invalidate cache
        }
    }

    public async Task RemoveActorFromShowAsync(int showId, int actorId)
    {
        var show = await _context.Shows
            .Include(s => s.Actors)
            .FirstOrDefaultAsync(s => s.Id == showId);

        if (show == null)
        {
            throw new KeyNotFoundException("Show not found.");
        }

        var actor = show.Actors.FirstOrDefault(a => a.Id == actorId);
        if (actor == null)
        {
            // Actor is not associated with the show, so we can consider the operation successful.
            return;
        }

        show.Actors.Remove(actor);
        await _context.SaveChangesAsync();
        _cache.Remove("shows-all"); // Invalidate cache
    }
}
