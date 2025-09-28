using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ShowTracker.Api.Data;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Interfaces;
using ShowTracker.Api.Services;
using ShowTracker.Api.Settings;
using System.Text;

namespace ShowTracker.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("ShowStore");
        services.AddSqlite<ShowStoreContext>(connString);
        return services;
    }

    public static IServiceCollection AddIdentityAndAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind the JWT settings from appsettings.json to the JwtSettings class
        var jwtSettingsSection = configuration.GetSection("Jwt");
        services.Configure<JwtSettings>(jwtSettingsSection);

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ShowStoreContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            // Use the IOptions pattern to get strongly-typed settings
            var serviceProvider = services.BuildServiceProvider();
            var jwtSettings = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;

            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Use Scrutor to scan the assembly and register services by convention
        services.Scan(scan => scan
            // Scan from the assembly where IShowService is defined (your current project)
            .FromAssemblyOf<IShowService>()
                // Register classes that implement an interface with a matching name (e.g., ShowService -> IShowService)
                .AddClasses(classes => classes.InNamespaces("ShowTracker.Api.Services"))
                    .AsMatchingInterface()
                    .WithScopedLifetime());

        // Manually register any services that don't follow the convention
        services.AddScoped<IEmailService, ConsoleEmailService>();

        // Background Services
        services.AddHostedService<RecommendationEmailJob>();

        return services;
    }
}
