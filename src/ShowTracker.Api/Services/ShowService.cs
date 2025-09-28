using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;
using ShowTracker.Api.Interfaces;

namespace ShowTracker.Api.Services;

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
        QueryParameters<ShowSortBy> parameters)
    {
        var showsQuery = _dbContext.Shows
            .Include(s => s.Genres)
            .Include(s => s.ShowType)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(genre))
        { showsQuery = showsQuery.Where(s => s.Genres.Any(g => g.Name.ToLower() == genre.ToLower())); }

        if (!string.IsNullOrWhiteSpace(type))
        { showsQuery = showsQuery.Where(s => s.ShowType.Name.ToLower() == type.ToLower()); }

        showsQuery = (parameters.SortBy, parameters.SortOrder) switch
        {
            (ShowSortBy.Title, SortOrder.asc) => showsQuery.OrderBy(s => s.Title),
            (ShowSortBy.Title, SortOrder.desc) => showsQuery.OrderByDescending(s => s.Title),
            (ShowSortBy.ReleaseDate, SortOrder.asc) => showsQuery.OrderBy(s => s.ReleaseDate),
            _ => showsQuery.OrderByDescending(s => s.ReleaseDate)
        };

        var skip = (parameters.Page - 1) * parameters.PageSize;
        var shows = await showsQuery.Skip(skip).Take(parameters.PageSize).ToListAsync();

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
                .AsNoTracking() // Use AsNoTracking for read-only queries
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
        var existing = await _dbContext.Shows.FindAsync(id);
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
