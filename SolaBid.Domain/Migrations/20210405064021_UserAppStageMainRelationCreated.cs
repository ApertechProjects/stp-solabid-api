using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class UserAppStageMainRelationCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EditedUserId",
                table: "ApproveStageMains",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedUserId",
                table: "ApproveStageMains",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApproveStageMains_CreatedUserId",
                table: "ApproveStageMains",
                column: "CreatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveStageMains_EditedUserId",
                table: "ApproveStageMains",
                column: "EditedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApproveStageMains_AspNetUsers_CreatedUserId",
                table: "ApproveStageMains",
                column: "CreatedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApproveStageMains_AspNetUsers_EditedUserId",
                table: "ApproveStageMains",
                column: "EditedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApproveStageMains_AspNetUsers_CreatedUserId",
                table: "ApproveStageMains");

            migrationBuilder.DropForeignKey(
                name: "FK_ApproveStageMains_AspNetUsers_EditedUserId",
                table: "ApproveStageMains");

            migrationBuilder.DropIndex(
                name: "IX_ApproveStageMains_CreatedUserId",
                table: "ApproveStageMains");

            migrationBuilder.DropIndex(
                name: "IX_ApproveStageMains_EditedUserId",
                table: "ApproveStageMains");

            migrationBuilder.AlterColumn<string>(
                name: "EditedUserId",
                table: "ApproveStageMains",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedUserId",
                table: "ApproveStageMains",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
