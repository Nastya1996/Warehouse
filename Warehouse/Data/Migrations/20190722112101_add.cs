using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "ProductMoves",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeId",
                table: "ProductMoves",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductMoves_ProductId",
                table: "ProductMoves",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMoves_TypeId",
                table: "ProductMoves",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMoves_Products_ProductId",
                table: "ProductMoves",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductMoves_Types_TypeId",
                table: "ProductMoves",
                column: "TypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductMoves_Products_ProductId",
                table: "ProductMoves");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductMoves_Types_TypeId",
                table: "ProductMoves");

            migrationBuilder.DropIndex(
                name: "IX_ProductMoves_ProductId",
                table: "ProductMoves");

            migrationBuilder.DropIndex(
                name: "IX_ProductMoves_TypeId",
                table: "ProductMoves");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductMoves");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "ProductMoves");
        }
    }
}
