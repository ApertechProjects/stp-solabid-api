using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class editedNotRealisedPropName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotRealised",
                table: "ComparisonCharts");

            migrationBuilder.AddColumn<bool>(
                name: "IsRealisedToSyteLine",
                table: "ComparisonCharts",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRealisedToSyteLine",
                table: "ComparisonCharts");

            migrationBuilder.AddColumn<bool>(
                name: "NotRealised",
                table: "ComparisonCharts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
