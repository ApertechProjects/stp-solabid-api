using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class notEssential : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BIDReferances_DiscountTypes_DiscountTypeId",
                table: "BIDReferances");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountTypeId",
                table: "BIDReferances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BIDReferances_DiscountTypes_DiscountTypeId",
                table: "BIDReferances",
                column: "DiscountTypeId",
                principalTable: "DiscountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BIDReferances_DiscountTypes_DiscountTypeId",
                table: "BIDReferances");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountTypeId",
                table: "BIDReferances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BIDReferances_DiscountTypes_DiscountTypeId",
                table: "BIDReferances",
                column: "DiscountTypeId",
                principalTable: "DiscountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
