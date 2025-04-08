using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedComparisonChartHold : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComparisonChartHolds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoldReason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    HoldDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApproveStageDetailId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ComparisonChartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonChartHolds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparisonChartHolds_ApproveStageDetails_ApproveStageDetailId",
                        column: x => x.ApproveStageDetailId,
                        principalTable: "ApproveStageDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparisonChartHolds_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparisonChartHolds_ComparisonCharts_ComparisonChartId",
                        column: x => x.ComparisonChartId,
                        principalTable: "ComparisonCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartHolds_ApproveStageDetailId",
                table: "ComparisonChartHolds",
                column: "ApproveStageDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartHolds_ComparisonChartId",
                table: "ComparisonChartHolds",
                column: "ComparisonChartId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartHolds_UserId",
                table: "ComparisonChartHolds",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComparisonChartHolds");
        }
    }
}
