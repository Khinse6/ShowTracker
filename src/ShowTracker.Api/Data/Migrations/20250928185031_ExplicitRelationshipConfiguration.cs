using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowTracker.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExplicitRelationshipConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFavorites_AspNetUsers_UserId",
                table: "UserFavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFavorites_Shows_ShowId",
                table: "UserFavorites");

            migrationBuilder.DropTable(
                name: "ShowActors");

            migrationBuilder.DropTable(
                name: "ShowGenres");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserFavorites",
                newName: "FavoritedByUsersId");

            migrationBuilder.RenameColumn(
                name: "ShowId",
                table: "UserFavorites",
                newName: "FavoriteShowsId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFavorites_UserId",
                table: "UserFavorites",
                newName: "IX_UserFavorites_FavoritedByUsersId");

            migrationBuilder.CreateTable(
                name: "ShowActor",
                columns: table => new
                {
                    ActorsId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowActor", x => new { x.ActorsId, x.ShowsId });
                    table.ForeignKey(
                        name: "FK_ShowActor_Actors_ActorsId",
                        column: x => x.ActorsId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowActor_Shows_ShowsId",
                        column: x => x.ShowsId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowGenre",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowGenre", x => new { x.GenresId, x.ShowsId });
                    table.ForeignKey(
                        name: "FK_ShowGenre_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowGenre_Shows_ShowsId",
                        column: x => x.ShowsId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowActor_ShowsId",
                table: "ShowActor",
                column: "ShowsId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenre_ShowsId",
                table: "ShowGenre",
                column: "ShowsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavorites_AspNetUsers_FavoritedByUsersId",
                table: "UserFavorites",
                column: "FavoritedByUsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavorites_Shows_FavoriteShowsId",
                table: "UserFavorites",
                column: "FavoriteShowsId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFavorites_AspNetUsers_FavoritedByUsersId",
                table: "UserFavorites");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFavorites_Shows_FavoriteShowsId",
                table: "UserFavorites");

            migrationBuilder.DropTable(
                name: "ShowActor");

            migrationBuilder.DropTable(
                name: "ShowGenre");

            migrationBuilder.RenameColumn(
                name: "FavoritedByUsersId",
                table: "UserFavorites",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "FavoriteShowsId",
                table: "UserFavorites",
                newName: "ShowId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFavorites_FavoritedByUsersId",
                table: "UserFavorites",
                newName: "IX_UserFavorites_UserId");

            migrationBuilder.CreateTable(
                name: "ShowActors",
                columns: table => new
                {
                    ActorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShowId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowActors", x => new { x.ActorId, x.ShowId });
                    table.ForeignKey(
                        name: "FK_ShowActors_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowActors_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ShowActors_ShowId",
                table: "ShowActors",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowGenres_ShowsId",
                table: "ShowGenres",
                column: "ShowsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavorites_AspNetUsers_UserId",
                table: "UserFavorites",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFavorites_Shows_ShowId",
                table: "UserFavorites",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
