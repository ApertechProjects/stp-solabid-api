using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedUserIdToBidReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BIDReferances",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BIDReferances_UserId",
                table: "BIDReferances",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BIDReferances_AspNetUsers_UserId",
                table: "BIDReferances",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BIDReferances_AspNetUsers_UserId",
                table: "BIDReferances");

            migrationBuilder.DropIndex(
                name: "IX_BIDReferances_UserId",
                table: "BIDReferances");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BIDReferances");
        }
    }
}
