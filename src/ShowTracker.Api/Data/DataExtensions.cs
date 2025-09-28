using Microsoft.EntityFrameworkCore;

namespace ShowTracker.Api.Data;

public static class DataExtensions
{
    public static async Task MigrateDBAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ShowStoreContext>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        await dbContext.Database.MigrateAsync();
        await DbInitializer.Initialize(dbContext);
        await IdentityDataInitializer.Initialize(services, configuration, logger);
    }
}
