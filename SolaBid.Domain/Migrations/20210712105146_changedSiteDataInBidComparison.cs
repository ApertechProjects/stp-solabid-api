using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changedSiteDataInBidComparison : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiteDatabase",
                table: "BIDComparisons");

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "BIDComparisons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "BIDComparisons");

            migrationBuilder.AddColumn<string>(
                name: "SiteDatabase",
                table: "BIDComparisons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
