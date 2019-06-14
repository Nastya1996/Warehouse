using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class AddBasket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_ProductManagers_ProductManagerId",
                table: "Baskets");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrder_Orders_OrderId",
                table: "ProductOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrder_Products_ProductId",
                table: "ProductOrder");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOrder",
                table: "ProductOrder");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Baskets");

            migrationBuilder.RenameTable(
                name: "ProductOrder",
                newName: "ProductOrders");

            migrationBuilder.RenameColumn(
                name: "Sum",
                table: "Orders",
                newName: "Sale");

            migrationBuilder.RenameColumn(
                name: "ProductManagerId",
                table: "Baskets",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Baskets_ProductManagerId",
                table: "Baskets",
                newName: "IX_Baskets_AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOrder_ProductId",
                table: "ProductOrders",
                newName: "IX_ProductOrders_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOrder_OrderId",
                table: "ProductOrders",
                newName: "IX_ProductOrders_OrderId");

            migrationBuilder.AddColumn<decimal>(
                name: "FinallPrice",
                table: "Orders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Orders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<long>(
                name: "Count",
                table: "ProductOrders",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<decimal>(
                name: "FinallyPrice",
                table: "ProductOrders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Sale",
                table: "ProductOrders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOrders",
                table: "ProductOrders",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProductBaskets",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Count = table.Column<long>(nullable: false),
                    AddDate = table.Column<DateTime>(nullable: false),
                    ProductId = table.Column<string>(nullable: true),
                    BasketId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBaskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBaskets_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBaskets_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBaskets_BasketId",
                table: "ProductBaskets",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBaskets_ProductId",
                table: "ProductBaskets",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_AspNetUsers_AppUserId",
                table: "Baskets",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrders_Orders_OrderId",
                table: "ProductOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrders_Products_ProductId",
                table: "ProductOrders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_AspNetUsers_AppUserId",
                table: "Baskets");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrders_Orders_OrderId",
                table: "ProductOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductOrders_Products_ProductId",
                table: "ProductOrders");

            migrationBuilder.DropTable(
                name: "ProductBaskets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductOrders",
                table: "ProductOrders");

            migrationBuilder.DropColumn(
                name: "FinallPrice",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FinallyPrice",
                table: "ProductOrders");

            migrationBuilder.DropColumn(
                name: "Sale",
                table: "ProductOrders");

            migrationBuilder.RenameTable(
                name: "ProductOrders",
                newName: "ProductOrder");

            migrationBuilder.RenameColumn(
                name: "Sale",
                table: "Orders",
                newName: "Sum");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Baskets",
                newName: "ProductManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Baskets_AppUserId",
                table: "Baskets",
                newName: "IX_Baskets_ProductManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOrders_ProductId",
                table: "ProductOrder",
                newName: "IX_ProductOrder_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductOrders_OrderId",
                table: "ProductOrder",
                newName: "IX_ProductOrder_OrderId");

            migrationBuilder.AddColumn<long>(
                name: "Quantity",
                table: "Baskets",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "Count",
                table: "ProductOrder",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductOrder",
                table: "ProductOrder",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_ProductManagers_ProductManagerId",
                table: "Baskets",
                column: "ProductManagerId",
                principalTable: "ProductManagers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrder_Orders_OrderId",
                table: "ProductOrder",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductOrder_Products_ProductId",
                table: "ProductOrder",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
