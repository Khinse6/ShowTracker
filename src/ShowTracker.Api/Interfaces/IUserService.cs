using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Interfaces;

public interface IUserService
{
    Task<UserPersonalDataDto?> GetUserDataAsync(string userId);
    Task<bool> DeleteUserAsync(string userId);
}
