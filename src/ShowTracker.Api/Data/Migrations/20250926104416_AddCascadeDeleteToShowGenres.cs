using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShowTracker.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToShowGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenreShow");

            migrationBuilder.CreateTable(
                name: "ShowGenres",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowGenres", x => new { x.GenresId, x.ShowsId });
                    table.ForeignKey(
                        name: "FK_ShowGenres_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowGenres_Shows_ShowsId",
                        column: x => x.ShowsId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenres_ShowsId",
                table: "ShowGenres",
                column: "ShowsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowGenres");

            migrationBuilder.CreateTable(
                name: "GenreShow",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreShow", x => new { x.GenresId, x.ShowsId });
                    table.ForeignKey(
                        name: "FK_GenreShow_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreShow_Shows_ShowsId",
                        column: x => x.ShowsId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GenreShow",
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

            migrationBuilder.CreateIndex(
                name: "IX_GenreShow_ShowsId",
                table: "GenreShow",
                column: "ShowsId");
        }
    }
}
