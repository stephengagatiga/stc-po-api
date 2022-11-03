using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace STC_PO_Api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "POApprovers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    AmountLogicalOperatorPHP = table.Column<string>(nullable: false),
                    AmountPHP = table.Column<decimal>(nullable: false),
                    AmountLogicalOperatorUSD = table.Column<string>(nullable: false),
                    AmountUSD = table.Column<decimal>(nullable: false),
                    AmountLogicalOperatorEUR = table.Column<string>(nullable: false),
                    AmountEUR = table.Column<decimal>(nullable: false),
                    SignatureImg = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POApprovers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "POPendings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReferenceNumber = table.Column<string>(nullable: true),
                    SupplierId = table.Column<int>(nullable: true),
                    SupplierName = table.Column<string>(nullable: true),
                    SupplierAddress = table.Column<string>(nullable: true),
                    ContactPersonId = table.Column<int>(nullable: true),
                    ContactPersonName = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    EstimatedArrival = table.Column<DateTime>(nullable: true),
                    EstimatedArrivalString = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    ApproverId = table.Column<int>(nullable: true),
                    ApproverName = table.Column<string>(nullable: true),
                    ApproverEmail = table.Column<string>(nullable: true),
                    Discount = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    InternalNote = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    RequestorName = table.Column<string>(nullable: false),
                    RequestorEmail = table.Column<string>(nullable: false),
                    CreatedByName = table.Column<string>(nullable: false),
                    POPendingItemsJsonString = table.Column<string>(nullable: true),
                    Guid = table.Column<Guid>(nullable: false),
                    TextLineBreakCount = table.Column<string>(nullable: false),
                    ApprovedOn = table.Column<DateTime>(nullable: true),
                    ReceivedOn = table.Column<DateTime>(nullable: true),
                    CancelledOn = table.Column<DateTime>(nullable: true),
                    HasBeenApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POPendings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "POSuppliers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSuppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "POAttachments",
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
                    table.PrimaryKey("PK_POAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POAttachments_POPendings_POId",
                        column: x => x.POId,
                        principalTable: "POPendings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "POAuditTrails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POPendingId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POAuditTrails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POAuditTrails_POPendings_POPendingId",
                        column: x => x.POPendingId,
                        principalTable: "POPendings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "POPendingItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POPendingId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Total = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POPendingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POPendingItems_POPendings_POPendingId",
                        column: x => x.POPendingId,
                        principalTable: "POPendings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "POSupplierContactPersons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    POSupplierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSupplierContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POSupplierContactPersons_POSuppliers_POSupplierId",
                        column: x => x.POSupplierId,
                        principalTable: "POSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_POAttachments_POId",
                table: "POAttachments",
                column: "POId");

            migrationBuilder.CreateIndex(
                name: "IX_POAuditTrails_POPendingId",
                table: "POAuditTrails",
                column: "POPendingId");

            migrationBuilder.CreateIndex(
                name: "IX_POPendingItems_POPendingId",
                table: "POPendingItems",
                column: "POPendingId");

            migrationBuilder.CreateIndex(
                name: "IX_POSupplierContactPersons_POSupplierId",
                table: "POSupplierContactPersons",
                column: "POSupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "POApprovers");

            migrationBuilder.DropTable(
                name: "POAttachments");

            migrationBuilder.DropTable(
                name: "POAuditTrails");

            migrationBuilder.DropTable(
                name: "POPendingItems");

            migrationBuilder.DropTable(
                name: "POSupplierContactPersons");

            migrationBuilder.DropTable(
                name: "POPendings");

            migrationBuilder.DropTable(
                name: "POSuppliers");
        }
    }
}
