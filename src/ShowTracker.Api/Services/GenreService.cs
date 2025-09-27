using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public interface IGenreService
{
    Task<List<GenreDto>> GetAllGenresAsync(string? sortBy, bool descending, int page, int pageSize);
    Task<GenreDto?> GetGenreByIdAsync(int id);
    Task<GenreDto> CreateGenreAsync(CreateGenreDto dto);
    Task<List<GenreDto>> CreateGenresAsync(List<CreateGenreDto> dtos);
}

public class GenreService : IGenreService
{
    private readonly ShowStoreContext _context;

    public GenreService(ShowStoreContext context)
    {
        _context = context;
    }

    public async Task<List<GenreDto>> GetAllGenresAsync(string? sortBy, bool descending, int page, int pageSize)
    {

        var genresQuery = _context.Genres.AsQueryable();

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            genresQuery = sortBy.ToLower() switch
            {
                "name" => descending ? genresQuery.OrderByDescending(g => g.Name) : genresQuery.OrderBy(g => g.Name),
                _ => genresQuery
            };
        }

        var skip = (page - 1) * pageSize;
        var genres = await genresQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return genres.Select(genre => genre.ToDto()).ToList();
    }

    public async Task<GenreDto?> GetGenreByIdAsync(int id)
    {
        var genre = await _context.Genres.FindAsync(id);

        return genre?.ToDto();
    }

    public async Task<GenreDto> CreateGenreAsync(CreateGenreDto dto)
    {
        var exists = await _context.Genres.AnyAsync(g => g.Name == dto.Name);
        if (exists)
        { throw new InvalidOperationException("Genre already exists."); }

        var genre = dto.ToEntity(); // use mapper
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return genre.ToDto();
    }

    public async Task<List<GenreDto>> CreateGenresAsync(List<CreateGenreDto> dtos)
    {
        if (dtos == null || !dtos.Any()) { return new List<GenreDto>(); }

        var genres = new List<Genre>();
        foreach (var dto in dtos)
        {
            if (await _context.Genres.AnyAsync(g => g.Name == dto.Name)) { continue; }
            genres.Add(dto.ToEntity());
        }

        if (!genres.Any()) { return new List<GenreDto>(); }

        _context.Genres.AddRange(genres);
        await _context.SaveChangesAsync();

        return genres.Select(g => g.ToDto()).ToList();
    }
}
