using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class FileCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileModelImgId",
                table: "ProductManagers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductManagers_FileModelImgId",
                table: "ProductManagers",
                column: "FileModelImgId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductManagers_Files_FileModelImgId",
                table: "ProductManagers",
                column: "FileModelImgId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductManagers_Files_FileModelImgId",
                table: "ProductManagers");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropIndex(
                name: "IX_ProductManagers_FileModelImgId",
                table: "ProductManagers");

            migrationBuilder.DropColumn(
                name: "FileModelImgId",
                table: "ProductManagers");
        }
    }
}
