using Microsoft.EntityFrameworkCore.Migrations;

namespace STC_PO_Api.Migrations
{
    public partial class UpdatePONumberName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "POPendings");

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "POPendings",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR shared.PONumbers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "POPendings");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "POPendings",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR shared.PONumbers");
        }
    }
}
