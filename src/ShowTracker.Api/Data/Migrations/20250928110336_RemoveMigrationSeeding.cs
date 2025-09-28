using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShowTracker.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMigrationSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Episodes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ShowActors",
                keyColumns: new[] { "ActorId", "ShowId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ShowActors",
                keyColumns: new[] { "ActorId", "ShowId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "ShowActors",
                keyColumns: new[] { "ActorId", "ShowId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "ShowActors",
                keyColumns: new[] { "ActorId", "ShowId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "ShowActors",
                keyColumns: new[] { "ActorId", "ShowId" },
                keyValues: new object[] { 5, 3 });

            migrationBuilder.DeleteData(
                table: "ShowGenres",
                keyColumns: new[] { "GenresId", "ShowsId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "ShowGenres",
                keyColumns: new[] { "GenresId", "ShowsId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "ShowGenres",
                keyColumns: new[] { "GenresId", "ShowsId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "ShowGenres",
                keyColumns: new[] { "GenresId", "ShowsId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "ShowGenres",
                keyColumns: new[] { "GenresId", "ShowsId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "ShowGenres",
                keyColumns: new[] { "GenresId", "ShowsId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "ShowGenres",
                keyColumns: new[] { "GenresId", "ShowsId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Actors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Seasons",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ShowTypes",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Actors",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Winona Ryder" },
                    { 2, "David Harbour" },
                    { 3, "Olivia Colman" },
                    { 4, "Phoebe Dynevor" },
                    { 5, "Regé-Jean Page" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Drama" },
                    { 2, "Sci-Fi" },
                    { 3, "Horror" },
                    { 4, "Romance" },
                    { 5, "History" }
                });

            migrationBuilder.InsertData(
                table: "ShowTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "TV Series" });

            migrationBuilder.InsertData(
                table: "Shows",
                columns: new[] { "Id", "Description", "ReleaseDate", "ShowTypeId", "Title" },
                values: new object[,]
                {
                    { 1, "A group of kids uncover supernatural mysteries in their small town.", new DateOnly(2016, 7, 15), 1, "Stranger Things" },
                    { 2, "Chronicles the life of Queen Elizabeth II from the 1940s onward.", new DateOnly(2016, 11, 4), 1, "The Crown" },
                    { 3, "Wealthy families navigate romance and scandal in Regency-era London.", new DateOnly(2020, 12, 25), 1, "Bridgerton" }
                });

            migrationBuilder.InsertData(
                table: "Seasons",
                columns: new[] { "Id", "ReleaseDate", "SeasonNumber", "ShowId" },
                values: new object[,]
                {
                    { 1, new DateOnly(2016, 7, 15), 1, 1 },
                    { 2, new DateOnly(2016, 11, 4), 1, 2 },
                    { 3, new DateOnly(2020, 12, 25), 1, 3 }
                });

            migrationBuilder.InsertData(
                table: "ShowActors",
                columns: new[] { "ActorId", "ShowId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 2 },
                    { 4, 3 },
                    { 5, 3 }
                });

            migrationBuilder.InsertData(
                table: "ShowGenres",
                columns: new[] { "GenresId", "ShowsId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 3 },
                    { 5, 2 }
                });

            migrationBuilder.InsertData(
                table: "Episodes",
                columns: new[] { "Id", "Description", "EpisodeNumber", "ReleaseDate", "SeasonId", "Title" },
                values: new object[,]
                {
                    { 1, "A young boy disappears, revealing a mystery in the town.", 1, new DateOnly(2016, 7, 15), 1, "Chapter One: The Vanishing" },
                    { 2, "The early reign of Queen Elizabeth II begins.", 1, new DateOnly(2016, 11, 4), 2, "Wolferton Splash" },
                    { 3, "Introduction to the Bridgerton family and London high society.", 1, new DateOnly(2020, 12, 25), 3, "Diamond of the First Water" }
                });
        }
    }
}
