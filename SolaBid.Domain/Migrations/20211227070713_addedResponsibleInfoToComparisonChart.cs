using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedResponsibleInfoToComparisonChart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResponsibilityDate",
                table: "ComparisonCharts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsiblePerson",
                table: "ComparisonCharts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponsibilityDate",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "ResponsiblePerson",
                table: "ComparisonCharts");
        }
    }
}
