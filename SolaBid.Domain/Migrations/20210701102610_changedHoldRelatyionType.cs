using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changedHoldRelatyionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComparisonChartHolds_ComparisonChartId",
                table: "ComparisonChartHolds");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartHolds_ComparisonChartId",
                table: "ComparisonChartHolds",
                column: "ComparisonChartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComparisonChartHolds_ComparisonChartId",
                table: "ComparisonChartHolds");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartHolds_ComparisonChartId",
                table: "ComparisonChartHolds",
                column: "ComparisonChartId",
                unique: true);
        }
    }
}
