using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class ChangeOrderClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSelled",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelled",
                table: "Orders");
        }
    }
}
