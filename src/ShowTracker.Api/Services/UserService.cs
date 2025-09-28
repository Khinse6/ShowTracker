using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Data;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Mappings;

namespace ShowTracker.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ShowStoreContext _context;

    public UserService(UserManager<User> userManager, ShowStoreContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // This will cascade delete related data thanks to EF Core's configuration
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<UserPersonalDataDto?> GetUserDataAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.FavoriteShows)
                .ThenInclude(s => s.Genres)
            .Include(u => u.FavoriteShows)
                .ThenInclude(s => s.ShowType)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return null;
        }

        return user.ToPersonalDataDto();
    }
}
