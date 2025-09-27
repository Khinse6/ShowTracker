using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Services;

public interface IGenreService
{
    Task<List<string>> GetAllGenresAsync();
    Task<Genre> CreateGenreAsync(CreateGenreDto dto);
    Task<List<Genre>> CreateGenresAsync(List<CreateGenreDto> dtos);

}

public class GenreService : IGenreService
{
    private readonly ShowStoreContext _context;

    public GenreService(ShowStoreContext context)
    {
        _context = context;
    }

    public async Task<List<string>> GetAllGenresAsync()
    {
        return await _context.Genres.Select(g => g.Name).ToListAsync();
    }

    public async Task<Genre> CreateGenreAsync(CreateGenreDto dto)
    {
        var exists = await _context.Genres.AnyAsync(g => g.Name == dto.Name);
        if (exists) { throw new InvalidOperationException("Genre already exists."); }

        var genre = new Genre { Name = dto.Name };
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return genre;
    }
    public async Task<List<Genre>> CreateGenresAsync(List<CreateGenreDto> dtos)
    {
        if (dtos == null || !dtos.Any()) { return new List<Genre>(); }

        var genres = new List<Genre>();

        foreach (var dto in dtos)
        {
            // Skip duplicates
            if (await _context.Genres.AnyAsync(g => g.Name == dto.Name)) { continue; }

            genres.Add(new Genre { Name = dto.Name });
        }

        if (!genres.Any()) { return new List<Genre>(); }

        _context.Genres.AddRange(genres);
        await _context.SaveChangesAsync();

        return genres;
    }

}
