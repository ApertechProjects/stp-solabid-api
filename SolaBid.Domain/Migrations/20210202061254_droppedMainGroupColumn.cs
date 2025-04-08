using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class droppedMainGroupColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainGroup",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "BaseGroupId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaseGroupName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BaseGroupName",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "MainGroup",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
