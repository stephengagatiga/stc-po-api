using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STC_PO_Api.Migrations
{
    public partial class AddedNewPOFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceDate",
                table: "POPendings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SIOrBI",
                table: "POPendings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TermsOfPayment",
                table: "POPendings",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "POPendings");

            migrationBuilder.DropColumn(
                name: "SIOrBI",
                table: "POPendings");

            migrationBuilder.DropColumn(
                name: "TermsOfPayment",
                table: "POPendings");
        }
    }
}
