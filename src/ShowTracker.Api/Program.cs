using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using ShowTracker.Api.Data;
using ShowTracker.Api.Extensions;
using ShowTracker.Api.Swagger;
using System.Reflection;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// -----------------------
// Library Configuration
// -----------------------
QuestPDF.Settings.License = LicenseType.Community;
// -----------------------
// Controllers
// -----------------------
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Vite/CRA default ports
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services
    .AddDatabaseServices(builder.Configuration)
    .AddIdentityAndAuthentication(builder.Configuration)
    .AddApplicationServices();

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

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

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

// Enable CORS
app.UseCors(MyAllowSpecificOrigins);

// Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Apply database migrations
await app.MigrateDBAsync();

app.Run();
