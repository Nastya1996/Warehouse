using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class move : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductMoves",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    BeforeId = table.Column<string>(nullable: true),
                    AfterId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Count = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMoves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductMoves_Warehouses_AfterId",
                        column: x => x.AfterId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductMoves_Warehouses_BeforeId",
                        column: x => x.BeforeId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductMoves_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductMoves_AfterId",
                table: "ProductMoves",
                column: "AfterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMoves_BeforeId",
                table: "ProductMoves",
                column: "BeforeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMoves_UserId",
                table: "ProductMoves",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductMoves");
        }
    }
}
