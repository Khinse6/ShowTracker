using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShowTracker.Api.Data;
using ShowTracker.Api.Entities;
using ShowTracker.Api.Services;
using ShowTracker.Api.Swagger;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------
// Database
// -----------------------
var connString = builder.Configuration.GetConnectionString("ShowStore");
builder.Services.AddSqlite<ShowStoreContext>(connString);

// -----------------------
// Identity
// -----------------------
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ShowStoreContext>()
    .AddDefaultTokenProviders();

// -----------------------
// JWT Authentication
// -----------------------
builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

// -----------------------
// Controllers
// -----------------------
builder.Services.AddControllers();

// -----------------------
// Services
// -----------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IFavoritesService, FavoritesService>();
builder.Services.AddScoped<IShowTypeService, ShowTypeService>();
builder.Services.AddScoped<IShowGenresService, ShowGenresService>();
builder.Services.AddScoped<IShowSeasonsService, ShowSeasonsService>();
builder.Services.AddScoped<IShowEpisodesService, ShowEpisodesService>();

// -----------------------
// Swagger / OpenAPI
// -----------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShowTracker API",
        Version = "v1",
        Description = "API for managing TV shows and seasons"
    });

    // JWT Bearer authentication support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Use the operation filter to show locks only on [Authorize] endpoints
    c.OperationFilter<AuthorizeCheckOperationFilter>();
});

// -----------------------
// Build App
// -----------------------
var app = builder.Build();

// -----------------------
// Middleware
// -----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShowTracker API v1");
    });
}

app.UseHttpsRedirection();

// Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Apply database migrations
await app.MigrateDBAsync();

app.Run();
