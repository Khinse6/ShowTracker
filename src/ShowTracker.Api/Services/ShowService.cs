using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public class ShowService : IShowService
{
	private readonly ShowStoreContext _dbContext;

	public ShowService(ShowStoreContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IEnumerable<ShowSummaryDto>> GetAllShowsAsync()
	{
		return await _dbContext.Shows
				.Include(s => s.Genres)
				.Select(s => s.ToShowSummaryDto())
				.AsNoTracking()
				.ToListAsync();
	}

	public async Task<ShowDetailsDto?> GetShowByIdAsync(int id)
	{
		var show = await _dbContext.Shows
				.Include(s => s.Genres)
				.Include(s => s.Seasons)
						.ThenInclude(se => se.Episodes)
				.FirstOrDefaultAsync(s => s.Id == id);

		return show?.ToShowDetailsDto();
	}

	public async Task<ShowSummaryDto> CreateShowAsync(ShowCreateDto dto)
	{
		var show = dto.ToEntity();
		_dbContext.Shows.Add(show);
		await _dbContext.SaveChangesAsync();
		return show.ToShowSummaryDto();
	}

	public async Task UpdateShowAsync(int id, ShowUpdateDto dto)
	{
		var existing = await _dbContext.Shows
				.Include(s => s.Genres)
				.Include(s => s.Seasons)
				.FirstOrDefaultAsync(s => s.Id == id);

		if (existing is null)
			throw new KeyNotFoundException("Show not found");

		dto.UpdateEntity(existing);
		await _dbContext.SaveChangesAsync();
	}

	public async Task DeleteShowAsync(int id)
	{
		var show = await _dbContext.Shows.FindAsync(id);
		if (show is null)
			throw new KeyNotFoundException("Show not found");

		_dbContext.Shows.Remove(show);
		await _dbContext.SaveChangesAsync();
	}
}
