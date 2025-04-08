using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedComparisonSingleSourceReasonRelationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonChartSingleSourceReasons_ComparisonCharts_ComparisonChartId",
                table: "ComparisonChartSingleSourceReasons");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonChartSingleSourceReasons_ComparisonChartId",
                table: "ComparisonChartSingleSourceReasons");

            migrationBuilder.DropColumn(
                name: "ComparisonChartId",
                table: "ComparisonChartSingleSourceReasons");

            migrationBuilder.CreateTable(
                name: "RELComparisonChartSingleSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComparisonChartSingleSourceReasonId = table.Column<int>(type: "int", nullable: false),
                    ComparisonChartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RELComparisonChartSingleSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RELComparisonChartSingleSources_ComparisonCharts_ComparisonChartId",
                        column: x => x.ComparisonChartId,
                        principalTable: "ComparisonCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RELComparisonChartSingleSources_ComparisonChartSingleSourceReasons_ComparisonChartSingleSourceReasonId",
                        column: x => x.ComparisonChartSingleSourceReasonId,
                        principalTable: "ComparisonChartSingleSourceReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RELComparisonChartSingleSources_ComparisonChartId",
                table: "RELComparisonChartSingleSources",
                column: "ComparisonChartId");

            migrationBuilder.CreateIndex(
                name: "IX_RELComparisonChartSingleSources_ComparisonChartSingleSourceReasonId",
                table: "RELComparisonChartSingleSources",
                column: "ComparisonChartSingleSourceReasonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RELComparisonChartSingleSources");

            migrationBuilder.AddColumn<int>(
                name: "ComparisonChartId",
                table: "ComparisonChartSingleSourceReasons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartSingleSourceReasons_ComparisonChartId",
                table: "ComparisonChartSingleSourceReasons",
                column: "ComparisonChartId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonChartSingleSourceReasons_ComparisonCharts_ComparisonChartId",
                table: "ComparisonChartSingleSourceReasons",
                column: "ComparisonChartId",
                principalTable: "ComparisonCharts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
