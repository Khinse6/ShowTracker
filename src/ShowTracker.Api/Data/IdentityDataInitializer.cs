using Microsoft.AspNetCore.Identity;
using ShowTracker.Api.Entities;

namespace ShowTracker.Api.Data;

public static class IdentityDataInitializer
{
    public static async Task Initialize(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // --- 1. Seed Roles ---
        await SeedRolesAsync(roleManager, logger);

        // --- 2. Seed Admin User ---
        await SeedAdminUserAsync(userManager, configuration, logger);

        // --- 3. Seed Regular Users ---
        await SeedRegularUsersAsync(userManager, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        if (!await roleManager.RoleExistsAsync("admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
            logger.LogInformation("Created 'admin' role.");
        }
        if (!await roleManager.RoleExistsAsync("user"))
        {
            await roleManager.CreateAsync(new IdentityRole("user"));
            logger.LogInformation("Created 'user' role.");
        }
    }

    private static async Task SeedAdminUserAsync(UserManager<User> userManager, IConfiguration configuration, ILogger logger)
    {
        var adminEmail = configuration["AdminUser:Email"] ?? "admin@showtracker.com";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Admin",
                EmailConfirmed = true,
                AcceptedTerms = true
            };

            var adminPassword = configuration["AdminUser:Password"] ?? "AdminPassword123!";
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
                logger.LogInformation("Admin user '{Email}' created successfully.", adminEmail);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("Failed to create admin user '{Email}'. Errors: {Errors}", adminEmail, errors);
            }
        }
    }

    private static async Task SeedRegularUsersAsync(UserManager<User> userManager, ILogger logger)
    {
        // User 1
        await SeedUserAsync(userManager, logger, "user1@showtracker.com", "Regular User One", "UserPassword1!");

        // User 2
        await SeedUserAsync(userManager, logger, "user2@showtracker.com", "Regular User Two", "UserPassword2!");
    }

    private static async Task SeedUserAsync(UserManager<User> userManager, ILogger logger, string email, string displayName, string password)
    {
        if (await userManager.FindByEmailAsync(email) != null) { return; }

        var user = new User { UserName = email, Email = email, DisplayName = displayName, EmailConfirmed = true, AcceptedTerms = true };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "user");
            logger.LogInformation("Regular user '{Email}' created successfully.", email);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogError("Failed to create regular user '{Email}'. Errors: {Errors}", email, errors);
        }
    }
}
