using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedComparisonChartReferance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartChatUserLastViews_ComparisonChartId",
                table: "ComparisonChartChatUserLastViews",
                column: "ComparisonChartId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonChartChatUserLastViews_ComparisonCharts_ComparisonChartId",
                table: "ComparisonChartChatUserLastViews",
                column: "ComparisonChartId",
                principalTable: "ComparisonCharts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonChartChatUserLastViews_ComparisonCharts_ComparisonChartId",
                table: "ComparisonChartChatUserLastViews");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonChartChatUserLastViews_ComparisonChartId",
                table: "ComparisonChartChatUserLastViews");
        }
    }
}
