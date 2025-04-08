using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedComparisonCahrtTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "SendStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComparisonCharts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingleSource = table.Column<bool>(type: "bit", nullable: false),
                    ComProcurementSpecialist = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    BIDComparisonId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    SendStatusId = table.Column<int>(type: "int", nullable: false),
                    ApproveStatusId = table.Column<int>(type: "int", nullable: false),
                    ApproveStageId = table.Column<int>(type: "int", nullable: false),
                    RejectReasonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonCharts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparisonCharts_ApproveStageMains_ApproveStageId",
                        column: x => x.ApproveStageId,
                        principalTable: "ApproveStageMains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComparisonCharts_ApproveStatuses_ApproveStatusId",
                        column: x => x.ApproveStatusId,
                        principalTable: "ApproveStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComparisonCharts_BIDComparisons_BIDComparisonId",
                        column: x => x.BIDComparisonId,
                        principalTable: "BIDComparisons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComparisonCharts_ComparisonChartRejectReasons_RejectReasonId",
                        column: x => x.RejectReasonId,
                        principalTable: "ComparisonChartRejectReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComparisonCharts_SendStatus_SendStatusId",
                        column: x => x.SendStatusId,
                        principalTable: "SendStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComparisonCharts_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_ApproveStageId",
                table: "ComparisonCharts",
                column: "ApproveStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_ApproveStatusId",
                table: "ComparisonCharts",
                column: "ApproveStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_BIDComparisonId",
                table: "ComparisonCharts",
                column: "BIDComparisonId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_RejectReasonId",
                table: "ComparisonCharts",
                column: "RejectReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_SendStatusId",
                table: "ComparisonCharts",
                column: "SendStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_StatusId",
                table: "ComparisonCharts",
                column: "StatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComparisonCharts");

            migrationBuilder.DropTable(
                name: "ComparisonChartRejectReasons");

            migrationBuilder.DropTable(
                name: "SendStatus");
        }
    }
}
