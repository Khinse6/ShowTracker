using Microsoft.EntityFrameworkCore;

namespace ShowTracker.Api.Data;

public static class DataExtensions
{
    public static void MigrateDB(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShowStoreContext>();
        dbContext.Database.Migrate();
    }
}
