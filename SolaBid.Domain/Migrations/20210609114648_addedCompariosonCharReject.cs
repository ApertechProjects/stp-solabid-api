using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedCompariosonCharReject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComparisonChartRejects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RejectReason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    RejectDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RejectedStageDetailId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ComparisonChartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonChartRejects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparisonChartRejects_ApproveStageDetails_RejectedStageDetailId",
                        column: x => x.RejectedStageDetailId,
                        principalTable: "ApproveStageDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparisonChartRejects_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparisonChartRejects_ComparisonCharts_ComparisonChartId",
                        column: x => x.ComparisonChartId,
                        principalTable: "ComparisonCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartRejects_ComparisonChartId",
                table: "ComparisonChartRejects",
                column: "ComparisonChartId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartRejects_RejectedStageDetailId",
                table: "ComparisonChartRejects",
                column: "RejectedStageDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartRejects_UserId",
                table: "ComparisonChartRejects",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComparisonChartRejects");
        }
    }
}
