using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedRevisionNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComparisonCharts_BIDComparisonId",
                table: "ComparisonCharts");

            migrationBuilder.AddColumn<int>(
                name: "ReviseNumber",
                table: "BIDComparisons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_BIDComparisonId",
                table: "ComparisonCharts",
                column: "BIDComparisonId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComparisonCharts_BIDComparisonId",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "ReviseNumber",
                table: "BIDComparisons");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_BIDComparisonId",
                table: "ComparisonCharts",
                column: "BIDComparisonId");
        }
    }
}
