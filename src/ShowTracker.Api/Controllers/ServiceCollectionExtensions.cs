using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShowTracker.Api.Data;
using ShowTracker.Api.Entities;
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
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ShowStoreContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IShowService, ShowService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IActorService, ActorService>();
        services.AddScoped<IFavoritesService, FavoritesService>();
        services.AddScoped<IShowTypeService, ShowTypeService>();
        services.AddScoped<IShowGenresService, ShowGenresService>();
        services.AddScoped<IShowSeasonsService, ShowSeasonsService>();
        services.AddScoped<IRecommendationService, RecommendationService>();
        services.AddScoped<IEmailService, ConsoleEmailService>();
        services.AddScoped<IShowEpisodesService, ShowEpisodesService>();
        services.AddScoped<IRecommendationJobService, RecommendationJobService>();

        // Background Services
        services.AddHostedService<RecommendationEmailJob>();

        return services;
    }
}
