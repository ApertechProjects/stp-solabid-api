using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedApproveRoleTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApproveStageMain",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApproveStageName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveStageMain", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApproveStageDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApproveStageDetailName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    ApproveStageMainId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveStageDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApproveStageDetail_ApproveStageMain_ApproveStageMainId",
                        column: x => x.ApproveStageMainId,
                        principalTable: "ApproveStageMain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApproveRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApproveRoleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    AmountFrom = table.Column<int>(type: "int", nullable: false),
                    AmountTo = table.Column<int>(type: "int", nullable: false),
                    ApproveStageDetailId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApproveRole_ApproveStageDetail_ApproveStageDetailId",
                        column: x => x.ApproveStageDetailId,
                        principalTable: "ApproveStageDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRole_ApproveStageDetailId",
                table: "ApproveRole",
                column: "ApproveStageDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveStageDetail_ApproveStageMainId",
                table: "ApproveStageDetail",
                column: "ApproveStageMainId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApproveRole");

            migrationBuilder.DropTable(
                name: "ApproveStageDetail");

            migrationBuilder.DropTable(
                name: "ApproveStageMain");
        }
    }
}
