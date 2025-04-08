using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedAdditionalPrivilege : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdditionalPrivileges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrivilegeName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalPrivileges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupAdditionalPrivileges",
                columns: table => new
                {
                    AdditionalPrivilegeId = table.Column<int>(type: "int", nullable: false),
                    AppRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAdditionalPrivileges", x => new { x.AdditionalPrivilegeId, x.AppRoleId });
                    table.ForeignKey(
                        name: "FK_GroupAdditionalPrivileges_AdditionalPrivileges_AdditionalPrivilegeId",
                        column: x => x.AdditionalPrivilegeId,
                        principalTable: "AdditionalPrivileges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAdditionalPrivileges_AspNetRoles_AppRoleId",
                        column: x => x.AppRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdditionalPrivileges_AppRoleId",
                table: "GroupAdditionalPrivileges",
                column: "AppRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupAdditionalPrivileges");

            migrationBuilder.DropTable(
                name: "AdditionalPrivileges");
        }
    }
}
