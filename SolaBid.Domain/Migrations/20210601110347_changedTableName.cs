using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changedTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonCharts_ComparisonChartRejectReasons_RejectReasonId",
                table: "ComparisonCharts");

            migrationBuilder.DropTable(
                name: "ComparisonChartRejectReasons");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonCharts_RejectReasonId",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "RejectReasonId",
                table: "ComparisonCharts");

            migrationBuilder.CreateTable(
                name: "ComparisonChartSingleSourceReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingleSourceReasonName = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    ComparisonChartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonChartSingleSourceReasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparisonChartSingleSourceReasons_ComparisonCharts_ComparisonChartId",
                        column: x => x.ComparisonChartId,
                        principalTable: "ComparisonCharts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonChartSingleSourceReasons_ComparisonChartId",
                table: "ComparisonChartSingleSourceReasons",
                column: "ComparisonChartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComparisonChartSingleSourceReasons");

            migrationBuilder.AddColumn<int>(
                name: "RejectReasonId",
                table: "ComparisonCharts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComparisonChartRejectReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RejectReasonName = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonChartRejectReasons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_RejectReasonId",
                table: "ComparisonCharts",
                column: "RejectReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonCharts_ComparisonChartRejectReasons_RejectReasonId",
                table: "ComparisonCharts",
                column: "RejectReasonId",
                principalTable: "ComparisonChartRejectReasons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
