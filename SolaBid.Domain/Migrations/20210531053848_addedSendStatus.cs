using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedSendStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonCharts_SendStatus_SendStatusId",
                table: "ComparisonCharts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SendStatus",
                table: "SendStatus");

            migrationBuilder.RenameTable(
                name: "SendStatus",
                newName: "SendStatuses");

            migrationBuilder.AlterColumn<string>(
                name: "StatusName",
                table: "SendStatuses",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SendStatuses",
                table: "SendStatuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonCharts_SendStatuses_SendStatusId",
                table: "ComparisonCharts",
                column: "SendStatusId",
                principalTable: "SendStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonCharts_SendStatuses_SendStatusId",
                table: "ComparisonCharts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SendStatuses",
                table: "SendStatuses");

            migrationBuilder.RenameTable(
                name: "SendStatuses",
                newName: "SendStatus");

            migrationBuilder.AlterColumn<string>(
                name: "StatusName",
                table: "SendStatus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SendStatus",
                table: "SendStatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonCharts_SendStatus_SendStatusId",
                table: "ComparisonCharts",
                column: "SendStatusId",
                principalTable: "SendStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
