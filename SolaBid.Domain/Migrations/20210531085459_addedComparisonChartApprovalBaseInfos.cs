using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedComparisonChartApprovalBaseInfos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComparisonChartApprovalBaseInfosId",
                table: "ComparisonCharts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ComparisonChartApprovalBaseInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ComparisonChartId = table.Column<int>(type: "int", nullable: false),
                    ApproveStageDetailId = table.Column<int>(type: "int", nullable: false),
                    ApprovedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonChartApprovalBaseInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparisonChartApprovalBaseInfos_ApproveStageDetails_ApproveStageDetailId",
                        column: x => x.ApproveStageDetailId,
                        principalTable: "ApproveStageDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComparisonChartApprovalBaseInfos_AspNetUsers_ApprovedUserId",
                        column: x => x.ApprovedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts",
                column: "ComparisonChartApprovalBaseInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartApprovalBaseInfos_ApprovedUserId",
                table: "ComparisonChartApprovalBaseInfos",
                column: "ApprovedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartApprovalBaseInfos_ApproveStageDetailId",
                table: "ComparisonChartApprovalBaseInfos",
                column: "ApproveStageDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonCharts_ComparisonChartApprovalBaseInfos_ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts",
                column: "ComparisonChartApprovalBaseInfoId",
                principalTable: "ComparisonChartApprovalBaseInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonCharts_ComparisonChartApprovalBaseInfos_ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts");

            migrationBuilder.DropTable(
                name: "ComparisonChartApprovalBaseInfos");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonCharts_ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "ComparisonChartApprovalBaseInfosId",
                table: "ComparisonCharts");
        }
    }
}
