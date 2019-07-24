using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class BasketWarehouseId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WarehouseId",
                table: "Baskets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_WarehouseId",
                table: "Baskets",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Warehouses_WarehouseId",
                table: "Baskets",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Warehouses_WarehouseId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_WarehouseId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Baskets");
        }
    }
}
