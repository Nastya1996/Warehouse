using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class FileCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductManagers_Files_FileModelImgId",
                table: "ProductManagers");

            migrationBuilder.DropIndex(
                name: "IX_ProductManagers_FileModelImgId",
                table: "ProductManagers");

            migrationBuilder.DropColumn(
                name: "FileModelImgId",
                table: "ProductManagers");

            migrationBuilder.AddColumn<string>(
                name: "FileModelImgId",
                table: "Products",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_FileModelImgId",
                table: "Products",
                column: "FileModelImgId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Files_FileModelImgId",
                table: "Products",
                column: "FileModelImgId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Files_FileModelImgId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_FileModelImgId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FileModelImgId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "FileModelImgId",
                table: "ProductManagers",
                nullable: true);

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
    }
}
