using Microsoft.EntityFrameworkCore.Migrations;

namespace STC_PO_Api.Migrations
{
    public partial class AddCancellationInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelledByName",
                table: "POPendings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "POPendings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelledByName",
                table: "POPendings");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "POPendings");
        }
    }
}
