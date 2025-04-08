using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changedComparisonChartStruct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonCharts_ComparisonChartApprovalBaseInfos_ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonCharts_ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "ComparisonChartApprovalBaseInfosId",
                table: "ComparisonCharts");

            migrationBuilder.AlterColumn<string>(
                name: "TotalApprovedAmount",
                table: "ComparisonChartApprovalBaseInfos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartApprovalBaseInfos_ComparisonChartId",
                table: "ComparisonChartApprovalBaseInfos",
                column: "ComparisonChartId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonChartApprovalBaseInfos_ComparisonCharts_ComparisonChartId",
                table: "ComparisonChartApprovalBaseInfos",
                column: "ComparisonChartId",
                principalTable: "ComparisonCharts",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonChartApprovalBaseInfos_ComparisonCharts_ComparisonChartId",
                table: "ComparisonChartApprovalBaseInfos");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonChartApprovalBaseInfos_ComparisonChartId",
                table: "ComparisonChartApprovalBaseInfos");

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

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalApprovedAmount",
                table: "ComparisonChartApprovalBaseInfos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts",
                column: "ComparisonChartApprovalBaseInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonCharts_ComparisonChartApprovalBaseInfos_ComparisonChartApprovalBaseInfoId",
                table: "ComparisonCharts",
                column: "ComparisonChartApprovalBaseInfoId",
                principalTable: "ComparisonChartApprovalBaseInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
