using Microsoft.EntityFrameworkCore.Migrations;

namespace STC_PO_Api.Migrations
{
    public partial class RemoveSequence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "PONumbers",
                schema: "shared");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateSequence<int>(
                name: "PONumbers",
                schema: "shared",
                startValue: 37700L);
        }
    }
}
