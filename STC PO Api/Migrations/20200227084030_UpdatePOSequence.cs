using Microsoft.EntityFrameworkCore.Migrations;

namespace STC_PO_Api.Migrations
{
    public partial class UpdatePOSequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateSequence<int>(
                name: "PONumbers",
                schema: "shared",
                startValue: 37700L);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "POPendings",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR shared.PONumbers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "PONumbers",
                schema: "shared");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "POPendings");
        }
    }
}
