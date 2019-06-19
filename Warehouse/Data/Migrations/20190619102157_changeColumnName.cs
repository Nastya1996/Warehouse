using Microsoft.EntityFrameworkCore.Migrations;

namespace Warehouse.Data.Migrations
{
    public partial class changeColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddDate",
                table: "ProductManagers",
                newName: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "ProductManagers",
                newName: "AddDate");
        }
    }
}
