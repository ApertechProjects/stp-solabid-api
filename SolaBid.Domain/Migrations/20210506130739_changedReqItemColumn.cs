using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changedReqItemColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestItem",
                table: "RELComparisonRequestItems",
                newName: "RequestItemRowPointer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestItemRowPointer",
                table: "RELComparisonRequestItems",
                newName: "RequestItem");
        }
    }
}
