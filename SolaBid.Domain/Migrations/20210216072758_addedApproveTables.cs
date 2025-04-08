using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedApproveTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApproveRole_ApproveStageDetail_ApproveStageDetailId",
                table: "ApproveRole");

            migrationBuilder.DropForeignKey(
                name: "FK_ApproveStageDetail_ApproveStageMain_ApproveStageMainId",
                table: "ApproveStageDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApproveStageMain",
                table: "ApproveStageMain");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApproveStageDetail",
                table: "ApproveStageDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApproveRole",
                table: "ApproveRole");

            migrationBuilder.RenameTable(
                name: "ApproveStageMain",
                newName: "ApproveStageMains");

            migrationBuilder.RenameTable(
                name: "ApproveStageDetail",
                newName: "ApproveStageDetails");

            migrationBuilder.RenameTable(
                name: "ApproveRole",
                newName: "ApproveRoles");

            migrationBuilder.RenameIndex(
                name: "IX_ApproveStageDetail_ApproveStageMainId",
                table: "ApproveStageDetails",
                newName: "IX_ApproveStageDetails_ApproveStageMainId");

            migrationBuilder.RenameIndex(
                name: "IX_ApproveRole_ApproveStageDetailId",
                table: "ApproveRoles",
                newName: "IX_ApproveRoles_ApproveStageDetailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApproveStageMains",
                table: "ApproveStageMains",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApproveStageDetails",
                table: "ApproveStageDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApproveRoles",
                table: "ApproveRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApproveRoles_ApproveStageDetails_ApproveStageDetailId",
                table: "ApproveRoles",
                column: "ApproveStageDetailId",
                principalTable: "ApproveStageDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApproveStageDetails_ApproveStageMains_ApproveStageMainId",
                table: "ApproveStageDetails",
                column: "ApproveStageMainId",
                principalTable: "ApproveStageMains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApproveRoles_ApproveStageDetails_ApproveStageDetailId",
                table: "ApproveRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_ApproveStageDetails_ApproveStageMains_ApproveStageMainId",
                table: "ApproveStageDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApproveStageMains",
                table: "ApproveStageMains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApproveStageDetails",
                table: "ApproveStageDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApproveRoles",
                table: "ApproveRoles");

            migrationBuilder.RenameTable(
                name: "ApproveStageMains",
                newName: "ApproveStageMain");

            migrationBuilder.RenameTable(
                name: "ApproveStageDetails",
                newName: "ApproveStageDetail");

            migrationBuilder.RenameTable(
                name: "ApproveRoles",
                newName: "ApproveRole");

            migrationBuilder.RenameIndex(
                name: "IX_ApproveStageDetails_ApproveStageMainId",
                table: "ApproveStageDetail",
                newName: "IX_ApproveStageDetail_ApproveStageMainId");

            migrationBuilder.RenameIndex(
                name: "IX_ApproveRoles_ApproveStageDetailId",
                table: "ApproveRole",
                newName: "IX_ApproveRole_ApproveStageDetailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApproveStageMain",
                table: "ApproveStageMain",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApproveStageDetail",
                table: "ApproveStageDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApproveRole",
                table: "ApproveRole",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApproveRole_ApproveStageDetail_ApproveStageDetailId",
                table: "ApproveRole",
                column: "ApproveStageDetailId",
                principalTable: "ApproveStageDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApproveStageDetail_ApproveStageMain_ApproveStageMainId",
                table: "ApproveStageDetail",
                column: "ApproveStageMainId",
                principalTable: "ApproveStageMain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
