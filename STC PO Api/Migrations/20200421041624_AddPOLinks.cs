using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace STC_PO_Api.Migrations
{
    public partial class AddPOLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "POLinks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POPendingId = table.Column<int>(nullable: false),
                    POGuid = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    PendingAction = table.Column<int>(nullable: false),
                    ResponseAction = table.Column<int>(nullable: false),
                    ResponseDate = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POLinks_POPendings_POPendingId",
                        column: x => x.POPendingId,
                        principalTable: "POPendings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_POLinks_POPendingId",
                table: "POLinks",
                column: "POPendingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "POLinks");
        }
    }
}
