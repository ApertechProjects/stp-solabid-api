using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedComparisonRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BIDComparisonId",
                table: "RELComparisonRequestItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RELComparisonRequestItems_BIDComparisonId",
                table: "RELComparisonRequestItems",
                column: "BIDComparisonId");

            migrationBuilder.AddForeignKey(
                name: "FK_RELComparisonRequestItems_BIDComparisons_BIDComparisonId",
                table: "RELComparisonRequestItems",
                column: "BIDComparisonId",
                principalTable: "BIDComparisons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RELComparisonRequestItems_BIDComparisons_BIDComparisonId",
                table: "RELComparisonRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_RELComparisonRequestItems_BIDComparisonId",
                table: "RELComparisonRequestItems");

            migrationBuilder.DropColumn(
                name: "BIDComparisonId",
                table: "RELComparisonRequestItems");
        }
    }
}
