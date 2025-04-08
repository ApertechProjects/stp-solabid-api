using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedDiscounteType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountTypeId",
                table: "BIDReferances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DiscountTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountTypeName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BIDReferances_DiscountTypeId",
                table: "BIDReferances",
                column: "DiscountTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_BIDReferances_DiscountTypes_DiscountTypeId",
                table: "BIDReferances",
                column: "DiscountTypeId",
                principalTable: "DiscountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BIDReferances_DiscountTypes_DiscountTypeId",
                table: "BIDReferances");

            migrationBuilder.DropTable(
                name: "DiscountTypes");

            migrationBuilder.DropIndex(
                name: "IX_BIDReferances_DiscountTypeId",
                table: "BIDReferances");

            migrationBuilder.DropColumn(
                name: "DiscountTypeId",
                table: "BIDReferances");
        }
    }
}
