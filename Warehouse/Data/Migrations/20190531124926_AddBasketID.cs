using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class AddBasketID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductManagers_Products_ProductId",
                table: "ProductManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Types_ProductTypeId",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "ProductTypeId",
                table: "Products",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "ProductManagers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductManagers_Products_ProductId",
                table: "ProductManagers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Types_ProductTypeId",
                table: "Products",
                column: "ProductTypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductManagers_Products_ProductId",
                table: "ProductManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Types_ProductTypeId",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "ProductTypeId",
                table: "Products",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "ProductManagers",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_ProductManagers_Products_ProductId",
                table: "ProductManagers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Types_ProductTypeId",
                table: "Products",
                column: "ProductTypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
