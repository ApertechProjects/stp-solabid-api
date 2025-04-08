using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class removedSendStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonCharts_SendStatuses_SendStatusId",
                table: "ComparisonCharts");

            migrationBuilder.DropTable(
                name: "SendStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ComparisonCharts_SendStatusId",
                table: "ComparisonCharts");

            migrationBuilder.DropColumn(
                name: "SendStatusId",
                table: "ComparisonCharts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SendStatusId",
                table: "ComparisonCharts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SendStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonCharts_SendStatusId",
                table: "ComparisonCharts",
                column: "SendStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonCharts_SendStatuses_SendStatusId",
                table: "ComparisonCharts",
                column: "SendStatusId",
                principalTable: "SendStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
