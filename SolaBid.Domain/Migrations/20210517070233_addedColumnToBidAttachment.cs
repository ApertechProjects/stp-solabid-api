using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedColumnToBidAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "BIDAttachments",
                newName: "FileName");

            migrationBuilder.AddColumn<string>(
                name: "FileBaseType",
                table: "BIDAttachments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileBaseType",
                table: "BIDAttachments");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "BIDAttachments",
                newName: "FileType");
        }
    }
}
