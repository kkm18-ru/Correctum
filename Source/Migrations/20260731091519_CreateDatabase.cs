using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Correctum.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Marked = table.Column<bool>(type: "INTEGER", nullable: false),
                    RegId = table.Column<string>(type: "TEXT", nullable: true),
                    StorageId = table.Column<string>(type: "TEXT", nullable: true),
                    DocNumber = table.Column<uint>(type: "INTEGER", nullable: true),
                    SessionId = table.Column<uint>(type: "INTEGER", nullable: true),
                    DocInSession = table.Column<uint>(type: "INTEGER", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DocSign = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<byte>(type: "INTEGER", nullable: true),
                    CashSum = table.Column<ulong>(type: "INTEGER", nullable: true),
                    NonCashSum = table.Column<ulong>(type: "INTEGER", nullable: true),
                    AdvanceSum = table.Column<ulong>(type: "INTEGER", nullable: true),
                    CreditSum = table.Column<ulong>(type: "INTEGER", nullable: true),
                    ProvisionSum = table.Column<ulong>(type: "INTEGER", nullable: true),
                    TotalSum = table.Column<ulong>(type: "INTEGER", nullable: true),
                    BuyerContact = table.Column<string>(type: "TEXT", nullable: true),
                    TaxationSystem = table.Column<int>(type: "INTEGER", nullable: true),
                    RetailPlace = table.Column<string>(type: "TEXT", nullable: true),
                    RetailAddress = table.Column<string>(type: "TEXT", nullable: true),
                    SenderAddress = table.Column<string>(type: "TEXT", nullable: true),
                    OperatorName = table.Column<string>(type: "TEXT", nullable: true),
                    OperatorInn = table.Column<string>(type: "TEXT", nullable: true),
                    IsReadOnly = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<byte>(type: "INTEGER", nullable: true),
                    CalcMethod = table.Column<byte>(type: "INTEGER", nullable: true),
                    TaxRate = table.Column<byte>(type: "INTEGER", nullable: true),
                    Unit = table.Column<byte>(type: "INTEGER", nullable: true),
                    Price = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Quantity = table.Column<decimal>(type: "TEXT", nullable: true),
                    Cost = table.Column<ulong>(type: "INTEGER", nullable: true),
                    ReceiptId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ReceiptId",
                table: "Positions",
                column: "ReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Receipts");
        }
    }
}
