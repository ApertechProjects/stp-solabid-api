using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedComparisonRelationTry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RELComparisonRequestItems_BIDComparisons_BIDComparisonId",
                table: "RELComparisonRequestItems");

            migrationBuilder.RenameColumn(
                name: "BIDComparisonId",
                table: "RELComparisonRequestItems",
                newName: "BIDReferanceId");

            migrationBuilder.RenameIndex(
                name: "IX_RELComparisonRequestItems_BIDComparisonId",
                table: "RELComparisonRequestItems",
                newName: "IX_RELComparisonRequestItems_BIDReferanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_RELComparisonRequestItems_BIDReferances_BIDReferanceId",
                table: "RELComparisonRequestItems",
                column: "BIDReferanceId",
                principalTable: "BIDReferances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RELComparisonRequestItems_BIDReferances_BIDReferanceId",
                table: "RELComparisonRequestItems");

            migrationBuilder.RenameColumn(
                name: "BIDReferanceId",
                table: "RELComparisonRequestItems",
                newName: "BIDComparisonId");

            migrationBuilder.RenameIndex(
                name: "IX_RELComparisonRequestItems_BIDReferanceId",
                table: "RELComparisonRequestItems",
                newName: "IX_RELComparisonRequestItems_BIDComparisonId");

            migrationBuilder.AddForeignKey(
                name: "FK_RELComparisonRequestItems_BIDComparisons_BIDComparisonId",
                table: "RELComparisonRequestItems",
                column: "BIDComparisonId",
                principalTable: "BIDComparisons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
