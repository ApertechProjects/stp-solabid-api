using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedOneTimePoandAnnex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Annex",
                table: "ComparisonCharts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OneTimePo",
                table: "ComparisonCharts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Annex",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "OneTimePo",
                table: "ComparisonCharts");
        }
    }
}
