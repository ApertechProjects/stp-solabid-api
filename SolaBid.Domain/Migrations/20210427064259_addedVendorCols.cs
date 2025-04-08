using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedVendorCols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryTerms",
                table: "Vendors",
                newName: "PaymentTerm");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryTerm",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTerm",
                table: "Vendors");

            migrationBuilder.RenameColumn(
                name: "PaymentTerm",
                table: "Vendors",
                newName: "DeliveryTerms");
        }
    }
}
