using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SolaBid.Domain.Migrations
{
    public partial class addedBIDTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApproveStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BIDRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BIDRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RELComparisonRequestItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestItem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LineDescription = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RELComparisonRequestItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WonStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WonStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BIDComparisons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComparisonNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BIDRequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BIDComparisons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BIDComparisons_BIDRequests_BIDRequestId",
                        column: x => x.BIDRequestId,
                        principalTable: "BIDRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BIDReferances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Requester = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BIDNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ComparisonDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ComparisonDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ComparisonChartPrepared = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ProjectWarehouse = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DeliveryTerm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeliveryDescription = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    DeliveryTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayementTerm = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentDescription = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentCurrTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AZNTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    USDTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ApproveStatusId = table.Column<int>(type: "int", nullable: false),
                    WonStatusId = table.Column<int>(type: "int", nullable: false),
                    BIDComparisonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BIDReferances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BIDReferances_ApproveStatuses_ApproveStatusId",
                        column: x => x.ApproveStatusId,
                        principalTable: "ApproveStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BIDReferances_BIDComparisons_BIDComparisonId",
                        column: x => x.BIDComparisonId,
                        principalTable: "BIDComparisons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BIDReferances_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BIDReferances_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BIDReferances_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BIDReferances_WonStatuses_WonStatusId",
                        column: x => x.WonStatusId,
                        principalTable: "WonStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BIDComparisons_BIDRequestId",
                table: "BIDComparisons",
                column: "BIDRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_BIDReferances_ApproveStatusId",
                table: "BIDReferances",
                column: "ApproveStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BIDReferances_BIDComparisonId",
                table: "BIDReferances",
                column: "BIDComparisonId");

            migrationBuilder.CreateIndex(
                name: "IX_BIDReferances_SiteId",
                table: "BIDReferances",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BIDReferances_StatusId",
                table: "BIDReferances",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BIDReferances_VendorId",
                table: "BIDReferances",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_BIDReferances_WonStatusId",
                table: "BIDReferances",
                column: "WonStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BIDReferances");

            migrationBuilder.DropTable(
                name: "RELComparisonRequestItems");

            migrationBuilder.DropTable(
                name: "ApproveStatuses");

            migrationBuilder.DropTable(
                name: "BIDComparisons");

            migrationBuilder.DropTable(
                name: "Statuses");

            migrationBuilder.DropTable(
                name: "WonStatuses");

            migrationBuilder.DropTable(
                name: "BIDRequests");
        }
    }
}
