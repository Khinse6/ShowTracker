using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public class GenreService : IGenreService
{
    private readonly ShowStoreContext _context;

    public GenreService(ShowStoreContext context)
    {
        _context = context;
    }

    public async Task<List<GenreDto>> GetAllGenresAsync(QueryParameters<GenreSortBy> parameters)
    {

        var genresQuery = _context.Genres.AsQueryable();

        genresQuery = (parameters.SortBy, parameters.SortOrder) switch
        {
            (GenreSortBy.Name, SortOrder.desc) => genresQuery.OrderByDescending(g => g.Name),
            (GenreSortBy.Name, SortOrder.asc) => genresQuery.OrderBy(g => g.Name),
            _ => genresQuery.OrderBy(g => g.Name)
        };

        var skip = (parameters.Page - 1) * parameters.PageSize;
        var genres = await genresQuery
            .Skip(skip)
            .Take(parameters.PageSize)
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

    public async Task UpdateGenreAsync(int id, UpdateGenreDto dto)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            throw new KeyNotFoundException("Genre not found");
        }

        genre.Name = dto.Name;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteGenreAsync(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
        {
            throw new KeyNotFoundException("Genre not found");
        }

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
    }
}
