using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "ProductManagers");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Warehouses",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Units",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductId",
                table: "ProductManagers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductManagers_ProductId",
                table: "ProductManagers",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductManagers_Products_ProductId",
                table: "ProductManagers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductManagers_Products_ProductId",
                table: "ProductManagers");

            migrationBuilder.DropIndex(
                name: "IX_ProductManagers_ProductId",
                table: "ProductManagers");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductManagers");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Warehouses",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Units",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "ProductManagers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
