using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changedrowcolumnname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestItemRowPointer",
                table: "RELComparisonRequestItems",
                newName: "RowPointer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RowPointer",
                table: "RELComparisonRequestItems",
                newName: "RequestItemRowPointer");
        }
    }
}
