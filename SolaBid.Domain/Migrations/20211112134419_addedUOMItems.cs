using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedUOMItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Conv",
                table: "RELComparisonRequestItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConvQuantity",
                table: "RELComparisonRequestItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ConvUnitPrice",
                table: "RELComparisonRequestItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PUOMFullText",
                table: "RELComparisonRequestItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PUOMValue",
                table: "RELComparisonRequestItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Conv",
                table: "RELComparisonRequestItems");

            migrationBuilder.DropColumn(
                name: "ConvQuantity",
                table: "RELComparisonRequestItems");

            migrationBuilder.DropColumn(
                name: "ConvUnitPrice",
                table: "RELComparisonRequestItems");

            migrationBuilder.DropColumn(
                name: "PUOMFullText",
                table: "RELComparisonRequestItems");

            migrationBuilder.DropColumn(
                name: "PUOMValue",
                table: "RELComparisonRequestItems");
        }
    }
}
