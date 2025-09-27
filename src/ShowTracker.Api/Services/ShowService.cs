using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public interface IShowService
{
    Task<List<ShowSummaryDto>> GetAllShowsAsync(string? genre, string? type, string? sortBy, bool descending, int page, int pageSize);
    Task<ShowDetailsDto?> GetShowByIdAsync(int id);
    Task<ShowSummaryDto> CreateShowAsync(CreateShowDto dto);
    Task<List<ShowSummaryDto>> CreateShowsAsync(List<CreateShowDto> dtos);
    Task UpdateShowAsync(int id, UpdateShowDto dto);
    Task DeleteShowAsync(int id);
}

public class ShowService : IShowService
{
    private readonly ShowStoreContext _dbContext;

    public ShowService(ShowStoreContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ShowSummaryDto>> GetAllShowsAsync(
        string? genre,
        string? type,
        string? sortBy,
        bool descending,
        int page,
        int pageSize)
    {
        var showsQuery = _dbContext.Shows
            .Include(s => s.Genres)
            .Include(s => s.ShowType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(genre))
        { showsQuery = showsQuery.Where(s => s.Genres.Any(g => g.Name == genre)); }

        if (!string.IsNullOrWhiteSpace(type))
        { showsQuery = showsQuery.Where(s => s.ShowType.Name == type); }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            showsQuery = sortBy.ToLower() switch
            {
                "title" => descending ? showsQuery.OrderByDescending(s => s.Title) : showsQuery.OrderBy(s => s.Title),
                "releasedate" => descending ? showsQuery.OrderByDescending(s => s.ReleaseDate) : showsQuery.OrderBy(s => s.ReleaseDate),
                _ => showsQuery
            };
        }

        var skip = (page - 1) * pageSize;
        var shows = await showsQuery.Skip(skip).Take(pageSize).ToListAsync();

        return shows.Select(s => s.ToShowSummaryDto()).ToList();
    }


    public async Task<ShowDetailsDto?> GetShowByIdAsync(int id)
    {
        var show = await _dbContext.Shows
                .Include(s => s.Genres)
                .Include(s => s.ShowType)
                .Include(s => s.Seasons)
                        .ThenInclude(se => se.Episodes)
                .Include(s => s.Actors)
                .FirstOrDefaultAsync(s => s.Id == id);

        return show?.ToShowDetailsDto();
    }

    public async Task<ShowSummaryDto> CreateShowAsync(CreateShowDto newShowDto)
    {
        var show = newShowDto.ToEntity();
        _dbContext.Shows.Add(show);
        await _dbContext.SaveChangesAsync();
        return show.ToShowSummaryDto();
    }

    public async Task<List<ShowSummaryDto>> CreateShowsAsync(List<CreateShowDto> dtos)
    {
        if (dtos == null || !dtos.Any()) { return new List<ShowSummaryDto>(); }

        var shows = dtos.Select(dto => dto.ToEntity()).ToList();

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            _dbContext.Shows.AddRange(shows);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return shows.Select(s => s.ToShowSummaryDto()).ToList();
    }

    public async Task UpdateShowAsync(int id, UpdateShowDto updateShowDto)
    {
        var existing = await _dbContext.Shows
                .Include(s => s.Genres)
                .Include(s => s.Seasons)
                .FirstOrDefaultAsync(s => s.Id == id);

        if (existing is null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        updateShowDto.UpdateEntity(existing);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteShowAsync(int id)
    {
        var show = await _dbContext.Shows.FindAsync(id);
        if (show is null)
        {
            throw new KeyNotFoundException("Show not found");
        }

        _dbContext.Shows.Remove(show);
        await _dbContext.SaveChangesAsync();
    }
}
