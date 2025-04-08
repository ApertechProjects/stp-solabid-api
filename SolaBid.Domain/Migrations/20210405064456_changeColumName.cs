using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changeColumName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegDate",
                table: "ApproveStageMains",
                newName: "EditedDate");

            migrationBuilder.RenameColumn(
                name: "EditDate",
                table: "ApproveStageMains",
                newName: "CreatedDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EditedDate",
                table: "ApproveStageMains",
                newName: "RegDate");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ApproveStageMains",
                newName: "EditDate");
        }
    }
}
