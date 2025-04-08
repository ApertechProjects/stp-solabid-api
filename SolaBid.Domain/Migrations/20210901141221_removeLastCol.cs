using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class removeLastCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonChartApproveStages_BIDReferances_BidReferanceId",
                table: "ComparisonChartApproveStages");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonChartApproveStages_BidReferanceId",
                table: "ComparisonChartApproveStages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartApproveStages_BidReferanceId",
                table: "ComparisonChartApproveStages",
                column: "BidReferanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonChartApproveStages_BIDReferances_BidReferanceId",
                table: "ComparisonChartApproveStages",
                column: "BidReferanceId",
                principalTable: "BIDReferances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
