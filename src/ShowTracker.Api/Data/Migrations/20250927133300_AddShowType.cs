using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShowTracker.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShowType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShowTypeId",
                table: "Shows",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ShowTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ShowTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "TV Series" });

            migrationBuilder.UpdateData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 1,
                column: "ShowTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 2,
                column: "ShowTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Shows",
                keyColumn: "Id",
                keyValue: 3,
                column: "ShowTypeId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Shows_ShowTypeId",
                table: "Shows",
                column: "ShowTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_ShowTypes_ShowTypeId",
                table: "Shows",
                column: "ShowTypeId",
                principalTable: "ShowTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shows_ShowTypes_ShowTypeId",
                table: "Shows");

            migrationBuilder.DropTable(
                name: "ShowTypes");

            migrationBuilder.DropIndex(
                name: "IX_Shows_ShowTypeId",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "ShowTypeId",
                table: "Shows");
        }
    }
}
