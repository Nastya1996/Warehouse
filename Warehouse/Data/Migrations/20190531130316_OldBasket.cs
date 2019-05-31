using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class OldBasket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductBaskets");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddDate",
                table: "Baskets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BasketId",
                table: "Baskets",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Count",
                table: "Baskets",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "Baskets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_ProductId",
                table: "Baskets",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Products_ProductId",
                table: "Baskets",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Products_ProductId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_ProductId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "AddDate",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "BasketId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Baskets");

            migrationBuilder.CreateTable(
                name: "ProductBaskets",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AddDate = table.Column<DateTime>(nullable: false),
                    BasketId = table.Column<string>(nullable: true),
                    Count = table.Column<long>(nullable: false),
                    ProductId = table.Column<string>(nullable: true)
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
        }
    }
}
