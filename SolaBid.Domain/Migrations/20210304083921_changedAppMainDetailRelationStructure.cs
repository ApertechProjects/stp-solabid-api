using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changedAppMainDetailRelationStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApproveRoleApproveStageDetails",
                table: "ApproveRoleApproveStageDetails");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ApproveRoleApproveStageDetails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApproveRoleApproveStageDetails",
                table: "ApproveRoleApproveStageDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRoleApproveStageDetails_ApproveRoleId",
                table: "ApproveRoleApproveStageDetails",
                column: "ApproveRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApproveRoleApproveStageDetails",
                table: "ApproveRoleApproveStageDetails");

            migrationBuilder.DropIndex(
                name: "IX_ApproveRoleApproveStageDetails_ApproveRoleId",
                table: "ApproveRoleApproveStageDetails");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ApproveRoleApproveStageDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApproveRoleApproveStageDetails",
                table: "ApproveRoleApproveStageDetails",
                columns: new[] { "ApproveRoleId", "ApproveStageDetailId" });
        }
    }
}
