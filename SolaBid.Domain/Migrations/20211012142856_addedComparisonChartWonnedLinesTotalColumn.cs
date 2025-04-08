using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedComparisonChartWonnedLinesTotalColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WonnedLineTotalAZN",
                table: "ComparisonCharts",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WonnedLineTotalUSD",
                table: "ComparisonCharts",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WonnedLineTotalAZN",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "WonnedLineTotalUSD",
                table: "ComparisonCharts");
        }
    }
}
