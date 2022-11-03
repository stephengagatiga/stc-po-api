using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace STC_PO_Api.Migrations
{
    public partial class AddExternalAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "POExternalAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    File = table.Column<byte[]>(nullable: false),
                    Size = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POExternalAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POExternalAttachments_POPendings_POId",
                        column: x => x.POId,
                        principalTable: "POPendings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_POExternalAttachments_POId",
                table: "POExternalAttachments",
                column: "POId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "POExternalAttachments");
        }
    }
}
