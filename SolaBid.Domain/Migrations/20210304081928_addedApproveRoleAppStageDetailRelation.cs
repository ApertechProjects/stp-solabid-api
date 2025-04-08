using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedApproveRoleAppStageDetailRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApproveRoles_ApproveStageDetails_ApproveStageDetailId",
                table: "ApproveRoles");

            migrationBuilder.DropIndex(
                name: "IX_ApproveRoles_ApproveStageDetailId",
                table: "ApproveRoles");

            migrationBuilder.DropColumn(
                name: "AmountFrom",
                table: "ApproveRoles");

            migrationBuilder.DropColumn(
                name: "AmountTo",
                table: "ApproveRoles");

            migrationBuilder.DropColumn(
                name: "ApproveStageDetailId",
                table: "ApproveRoles");

            migrationBuilder.CreateTable(
                name: "ApproveRoleApproveStageDetails",
                columns: table => new
                {
                    ApproveRoleId = table.Column<int>(type: "int", nullable: false),
                    ApproveStageDetailId = table.Column<int>(type: "int", nullable: false),
                    AmountFrom = table.Column<int>(type: "int", nullable: false),
                    AmountTo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveRoleApproveStageDetails", x => new { x.ApproveRoleId, x.ApproveStageDetailId });
                    table.ForeignKey(
                        name: "FK_ApproveRoleApproveStageDetails_ApproveRoles_ApproveRoleId",
                        column: x => x.ApproveRoleId,
                        principalTable: "ApproveRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApproveRoleApproveStageDetails_ApproveStageDetails_ApproveStageDetailId",
                        column: x => x.ApproveStageDetailId,
                        principalTable: "ApproveStageDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRoleApproveStageDetails_ApproveStageDetailId",
                table: "ApproveRoleApproveStageDetails",
                column: "ApproveStageDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApproveRoleApproveStageDetails");

            migrationBuilder.AddColumn<int>(
                name: "AmountFrom",
                table: "ApproveRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AmountTo",
                table: "ApproveRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproveStageDetailId",
                table: "ApproveRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRoles_ApproveStageDetailId",
                table: "ApproveRoles",
                column: "ApproveStageDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApproveRoles_ApproveStageDetails_ApproveStageDetailId",
                table: "ApproveRoles",
                column: "ApproveStageDetailId",
                principalTable: "ApproveStageDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
