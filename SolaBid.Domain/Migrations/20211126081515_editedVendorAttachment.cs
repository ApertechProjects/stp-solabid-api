using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class editedVendorAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vendorAttachments_Vendors_VendorId",
                table: "vendorAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vendorAttachments",
                table: "vendorAttachments");

            migrationBuilder.RenameTable(
                name: "vendorAttachments",
                newName: "VendorAttachments");

            migrationBuilder.RenameIndex(
                name: "IX_vendorAttachments_VendorId",
                table: "VendorAttachments",
                newName: "IX_VendorAttachments_VendorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendorAttachments",
                table: "VendorAttachments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorAttachments_Vendors_VendorId",
                table: "VendorAttachments",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorAttachments_Vendors_VendorId",
                table: "VendorAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VendorAttachments",
                table: "VendorAttachments");

            migrationBuilder.RenameTable(
                name: "VendorAttachments",
                newName: "vendorAttachments");

            migrationBuilder.RenameIndex(
                name: "IX_VendorAttachments_VendorId",
                table: "vendorAttachments",
                newName: "IX_vendorAttachments_VendorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vendorAttachments",
                table: "vendorAttachments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_vendorAttachments_Vendors_VendorId",
                table: "vendorAttachments",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
