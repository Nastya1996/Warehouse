using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class OrderType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelled",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "ProductManagerId",
                table: "ProductOrders",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderType",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrders_ProductManagerId",
                table: "ProductOrders",
                column: "ProductManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrders_ProductManagers_ProductManagerId",
                table: "ProductOrders",
                column: "ProductManagerId",
                principalTable: "ProductManagers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrders_ProductManagers_ProductManagerId",
                table: "ProductOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProductOrders_ProductManagerId",
                table: "ProductOrders");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "ProductManagerId",
                table: "ProductOrders",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelled",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }
    }
}
