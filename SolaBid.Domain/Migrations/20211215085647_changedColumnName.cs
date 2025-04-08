using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changedColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LinePercent",
                table: "RELComparisonRequestItems",
                newName: "LinePercentValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LinePercentValue",
                table: "RELComparisonRequestItems",
                newName: "LinePercent");
        }
    }
}
