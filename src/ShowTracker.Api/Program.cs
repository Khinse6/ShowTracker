using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShowTracker.Api.Data;
using ShowTracker.Api.Services; // <-- add this for IShowService / ShowService

var builder = WebApplication.CreateBuilder(args);

// -----------------------
// Database
// -----------------------
var connString = builder.Configuration.GetConnectionString("ShowStore");
builder.Services.AddSqlite<ShowStoreContext>(connString);

// -----------------------
// Controllers
// -----------------------
builder.Services.AddControllers();

// -----------------------
// Services
// -----------------------
// Register the interface + implementation
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IShowGenresService, ShowGenresService>();

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
});

// -----------------------
// Build the app
// -----------------------
var app = builder.Build();

// -----------------------
// Middleware pipeline
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

// Map controller routes
app.MapControllers();

// Apply migrations (if you have this extension method)
await app.MigrateDBAsync();

app.Run();
