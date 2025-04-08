using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedGroupSItwWarehouses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupSiteWarehouse_AspNetRoles_AppRoleId",
                table: "GroupSiteWarehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupSiteWarehouse_Sites_SiteId",
                table: "GroupSiteWarehouse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupSiteWarehouse",
                table: "GroupSiteWarehouse");

            migrationBuilder.RenameTable(
                name: "GroupSiteWarehouse",
                newName: "GroupSiteWarehouses");

            migrationBuilder.RenameIndex(
                name: "IX_GroupSiteWarehouse_SiteId",
                table: "GroupSiteWarehouses",
                newName: "IX_GroupSiteWarehouses_SiteId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupSiteWarehouse_AppRoleId",
                table: "GroupSiteWarehouses",
                newName: "IX_GroupSiteWarehouses_AppRoleId");

            migrationBuilder.AlterColumn<string>(
                name: "WarehouseName",
                table: "GroupSiteWarehouses",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WarehouseCode",
                table: "GroupSiteWarehouses",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupSiteWarehouses",
                table: "GroupSiteWarehouses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupSiteWarehouses_AspNetRoles_AppRoleId",
                table: "GroupSiteWarehouses",
                column: "AppRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupSiteWarehouses_Sites_SiteId",
                table: "GroupSiteWarehouses",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupSiteWarehouses_AspNetRoles_AppRoleId",
                table: "GroupSiteWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupSiteWarehouses_Sites_SiteId",
                table: "GroupSiteWarehouses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupSiteWarehouses",
                table: "GroupSiteWarehouses");

            migrationBuilder.RenameTable(
                name: "GroupSiteWarehouses",
                newName: "GroupSiteWarehouse");

            migrationBuilder.RenameIndex(
                name: "IX_GroupSiteWarehouses_SiteId",
                table: "GroupSiteWarehouse",
                newName: "IX_GroupSiteWarehouse_SiteId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupSiteWarehouses_AppRoleId",
                table: "GroupSiteWarehouse",
                newName: "IX_GroupSiteWarehouse_AppRoleId");

            migrationBuilder.AlterColumn<string>(
                name: "WarehouseName",
                table: "GroupSiteWarehouse",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "WarehouseCode",
                table: "GroupSiteWarehouse",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupSiteWarehouse",
                table: "GroupSiteWarehouse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupSiteWarehouse_AspNetRoles_AppRoleId",
                table: "GroupSiteWarehouse",
                column: "AppRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupSiteWarehouse_Sites_SiteId",
                table: "GroupSiteWarehouse",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
