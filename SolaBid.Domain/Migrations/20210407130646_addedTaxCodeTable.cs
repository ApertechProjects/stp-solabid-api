using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedTaxCodeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxCode",
                table: "Vendors");

            migrationBuilder.AddColumn<int>(
                name: "TaxCodeId",
                table: "Vendors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaxCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxCodeNumber = table.Column<int>(type: "int", nullable: false),
                    TaxCodeName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxCodes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_TaxCodeId",
                table: "Vendors",
                column: "TaxCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_TaxCodes_TaxCodeId",
                table: "Vendors",
                column: "TaxCodeId",
                principalTable: "TaxCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_TaxCodes_TaxCodeId",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "TaxCodes");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_TaxCodeId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "TaxCodeId",
                table: "Vendors");

            migrationBuilder.AddColumn<int>(
                name: "TaxCode",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
