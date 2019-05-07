using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ProductId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "WareHouseId",
                table: "ProductManagers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProductOrder",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    ProductId = table.Column<string>(nullable: true),
                    OrderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductOrder_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductOrder_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductManagers_WareHouseId",
                table: "ProductManagers",
                column: "WareHouseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrder_OrderId",
                table: "ProductOrder",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOrder_ProductId",
                table: "ProductOrder",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductManagers_Warehouses_WareHouseId",
                table: "ProductManagers",
                column: "WareHouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductManagers_Warehouses_WareHouseId",
                table: "ProductManagers");

            migrationBuilder.DropTable(
                name: "ProductOrder");

            migrationBuilder.DropIndex(
                name: "IX_ProductManagers_WareHouseId",
                table: "ProductManagers");

            migrationBuilder.AlterColumn<string>(
                name: "WareHouseId",
                table: "ProductManagers",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Count",
                table: "Orders",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Products_ProductId",
                table: "Orders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
