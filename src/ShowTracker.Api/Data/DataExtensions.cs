using Microsoft.EntityFrameworkCore;

namespace ShowTracker.Api.Data;

public static class DataExtensions
{
    public static async Task MigrateDBAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShowStoreContext>();
        await dbContext.Database.MigrateAsync();
    }
}
