using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class AddedComparisonChartApproveStages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComparisonChartApproveStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComparisonId = table.Column<int>(type: "int", nullable: false),
                    BidReferanceId = table.Column<int>(type: "int", nullable: false),
                    BidReferanceItemRowPointer = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Stage = table.Column<int>(type: "int", nullable: false),
                    ApproveStageDetailId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonChartApproveStages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComparisonChartApproveStages");
        }
    }
}
