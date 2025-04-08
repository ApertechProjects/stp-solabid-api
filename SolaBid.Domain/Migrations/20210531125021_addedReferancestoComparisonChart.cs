using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedReferancestoComparisonChart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ComparisonId",
                table: "ComparisonChartApproveStages",
                newName: "ComparisonChartId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ComparisonCharts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "ComparisonCharts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedDate",
                table: "ComparisonCharts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EditedUserId",
                table: "ComparisonCharts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_CreatedUserId",
                table: "ComparisonCharts",
                column: "CreatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_EditedUserId",
                table: "ComparisonCharts",
                column: "EditedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartApproveStages_ComparisonChartId",
                table: "ComparisonChartApproveStages",
                column: "ComparisonChartId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonChartApproveStages_ComparisonCharts_ComparisonChartId",
                table: "ComparisonChartApproveStages",
                column: "ComparisonChartId",
                principalTable: "ComparisonCharts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonCharts_AspNetUsers_CreatedUserId",
                table: "ComparisonCharts",
                column: "CreatedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonCharts_AspNetUsers_EditedUserId",
                table: "ComparisonCharts",
                column: "EditedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonChartApproveStages_ComparisonCharts_ComparisonChartId",
                table: "ComparisonChartApproveStages");

            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonCharts_AspNetUsers_CreatedUserId",
                table: "ComparisonCharts");

            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonCharts_AspNetUsers_EditedUserId",
                table: "ComparisonCharts");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonCharts_CreatedUserId",
                table: "ComparisonCharts");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonCharts_EditedUserId",
                table: "ComparisonCharts");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonChartApproveStages_ComparisonChartId",
                table: "ComparisonChartApproveStages");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "EditedDate",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "EditedUserId",
                table: "ComparisonCharts");

            migrationBuilder.RenameColumn(
                name: "ComparisonChartId",
                table: "ComparisonChartApproveStages",
                newName: "ComparisonId");
        }
    }
}
