using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class changedMainRoleToGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainRole",
                table: "AspNetRoles",
                newName: "MainGroup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainGroup",
                table: "AspNetRoles",
                newName: "MainRole");
        }
    }
}
