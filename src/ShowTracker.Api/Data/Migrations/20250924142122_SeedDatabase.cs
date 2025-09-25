using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShowTracker.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Shows",
                columns: new[] { "Id", "Description", "ReleaseDate", "Title" },
                values: new object[,]
                {
                    { 1, "A group of kids uncover supernatural mysteries in their small town.", new DateOnly(2016, 7, 15), "Stranger Things" },
                    { 2, "Chronicles the life of Queen Elizabeth II from the 1940s onward.", new DateOnly(2016, 11, 4), "The Crown" },
                    { 3, "Wealthy families navigate romance and scandal in Regency-era London.", new DateOnly(2020, 12, 25), "Bridgerton" }
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
                table: "Episodes",
                columns: new[] { "Id", "Description", "EpisodeNumber", "ReleaseDate", "SeasonId", "Title" },
                values: new object[,]
                {
                    { 1, "A young boy disappears, revealing a mystery in the town.", 1, new DateOnly(2016, 7, 15), 1, "Chapter One: The Vanishing" },
                    { 2, "The early reign of Queen Elizabeth II begins.", 1, new DateOnly(2016, 11, 4), 2, "Wolferton Splash" },
                    { 3, "Introduction to the Bridgerton family and London high society.", 1, new DateOnly(2020, 12, 25), 3, "Diamond of the First Water" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
