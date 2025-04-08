using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class AddedGroupApproveRoleRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupApproveRoles",
                columns: table => new
                {
                    AppRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApproveRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupApproveRoles", x => new { x.AppRoleId, x.ApproveRoleId });
                    table.ForeignKey(
                        name: "FK_GroupApproveRoles_ApproveRoles_ApproveRoleId",
                        column: x => x.ApproveRoleId,
                        principalTable: "ApproveRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupApproveRoles_AspNetRoles_AppRoleId",
                        column: x => x.AppRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupApproveRoles_ApproveRoleId",
                table: "GroupApproveRoles",
                column: "ApproveRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupApproveRoles");
        }
    }
}
