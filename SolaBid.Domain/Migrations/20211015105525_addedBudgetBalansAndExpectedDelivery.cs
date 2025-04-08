using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedBudgetBalansAndExpectedDelivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Budget",
                table: "BIDReferances",
                newName: "ExpectedDelivery");

            migrationBuilder.AddColumn<decimal>(
                name: "BudgetBalance",
                table: "BIDReferances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetBalance",
                table: "BIDReferances");

            migrationBuilder.RenameColumn(
                name: "ExpectedDelivery",
                table: "BIDReferances",
                newName: "Budget");
        }
    }
}
