using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShowTracker.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGenresAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenreShow");

            migrationBuilder.DropTable(
                name: "Genres");
        }
    }
}
