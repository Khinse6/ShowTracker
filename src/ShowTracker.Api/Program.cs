using ShowTracker.Api.Data;
using ShowTracker.Api.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("ShowStore");
builder.Services.AddSqlite<ShowStoreContext>(connString);

var app = builder.Build();

app.MapShowsEndpoints();
app.MigrateDB();

app.Run();
