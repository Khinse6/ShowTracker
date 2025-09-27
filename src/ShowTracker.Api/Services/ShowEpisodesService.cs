using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

using ShowTracker.Api.Dtos;

public interface IShowEpisodesService
{
    Task<List<EpisodeDto>> GetEpisodesForSeasonAsync(int seasonId);
    Task<EpisodeDto?> GetEpisodeAsync(int seasonId, int episodeId);
    Task<EpisodeDto> CreateEpisodeAsync(int seasonId, CreateEpisodeDto dto);
    Task<List<EpisodeDto>> CreateEpisodesAsync(int seasonId, List<CreateEpisodeDto> dtos);
    Task UpdateEpisodeAsync(int seasonId, int episodeId, UpdateEpisodeDto dto);
    Task DeleteEpisodeAsync(int seasonId, int episodeId);
}

public class ShowEpisodesService : IShowEpisodesService
{
    private readonly ShowStoreContext _context;

    public ShowEpisodesService(ShowStoreContext context)
    {
        _context = context;
    }

    public async Task<List<EpisodeDto>> GetEpisodesForSeasonAsync(int seasonId)
    {
        var season = await _context.Seasons
            .Include(s => s.Episodes)
            .FirstOrDefaultAsync(s => s.Id == seasonId);

        if (season == null) { throw new KeyNotFoundException("Season not found"); }

        return season.Episodes.Select(e => e.ToDto()).ToList();
    }

    public async Task<EpisodeDto?> GetEpisodeAsync(int seasonId, int episodeId)
    {
        var episode = await _context.Episodes
            .FirstOrDefaultAsync(e => e.Id == episodeId && e.SeasonId == seasonId);

        return episode?.ToDto();
    }

    public async Task<EpisodeDto> CreateEpisodeAsync(int seasonId, CreateEpisodeDto dto)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) { throw new KeyNotFoundException("Season not found"); }

        var episode = new Episode
        {
            Title = dto.Title,
            Description = dto.Description,
            EpisodeNumber = dto.EpisodeNumber,
            ReleaseDate = dto.ReleaseDate,
            SeasonId = seasonId
        };

        _context.Episodes.Add(episode);
        await _context.SaveChangesAsync();
        return episode.ToDto();
    }

    public async Task<List<EpisodeDto>> CreateEpisodesAsync(int seasonId, List<CreateEpisodeDto> dtos)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null) { throw new KeyNotFoundException("Season not found"); }

        var episodes = dtos.Select(dto => new Episode
        {
            Title = dto.Title,
            Description = dto.Description,
            EpisodeNumber = dto.EpisodeNumber,
            ReleaseDate = dto.ReleaseDate,
            SeasonId = seasonId
        }).ToList();

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Episodes.AddRange(episodes);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return episodes.Select(e => e.ToDto()).ToList();
    }


    public async Task UpdateEpisodeAsync(int seasonId, int episodeId, UpdateEpisodeDto dto)
    {
        var episode = await _context.Episodes
            .FirstOrDefaultAsync(e => e.Id == episodeId && e.SeasonId == seasonId);

        if (episode == null) { throw new KeyNotFoundException("Episode not found"); }

        episode.Title = dto.Title;
        episode.Description = dto.Description;
        episode.EpisodeNumber = dto.EpisodeNumber;
        episode.ReleaseDate = dto.ReleaseDate;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteEpisodeAsync(int seasonId, int episodeId)
    {
        var episode = await _context.Episodes
            .FirstOrDefaultAsync(e => e.Id == episodeId && e.SeasonId == seasonId);

        if (episode == null) { throw new KeyNotFoundException("Episode not found"); }

        _context.Episodes.Remove(episode);
        await _context.SaveChangesAsync();
    }
}
