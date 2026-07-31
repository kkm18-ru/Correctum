using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Correctum.Migrations
{
    /// <inheritdoc />
    public partial class AddChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Marked",
                table: "Receipts");

            migrationBuilder.CreateTable(
                name: "ChangeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetadataChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Property = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceiptId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetadataChanges_ChangeHistory_ChangeId",
                        column: x => x.ChangeId,
                        principalTable: "ChangeHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MetadataChanges_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    ChangeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionChanges_ChangeHistory_ChangeId",
                        column: x => x.ChangeId,
                        principalTable: "ChangeHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionChanges_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true),
                    ChangeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceiptId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptChanges_ChangeHistory_ChangeId",
                        column: x => x.ChangeId,
                        principalTable: "ChangeHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptChanges_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetadataChanges_ChangeId",
                table: "MetadataChanges",
                column: "ChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataChanges_ReceiptId",
                table: "MetadataChanges",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionChanges_ChangeId",
                table: "PositionChanges",
                column: "ChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionChanges_PositionId",
                table: "PositionChanges",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptChanges_ChangeId",
                table: "ReceiptChanges",
                column: "ChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptChanges_ReceiptId",
                table: "ReceiptChanges",
                column: "ReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetadataChanges");

            migrationBuilder.DropTable(
                name: "PositionChanges");

            migrationBuilder.DropTable(
                name: "ReceiptChanges");

            migrationBuilder.DropTable(
                name: "ChangeHistory");

            migrationBuilder.AddColumn<bool>(
                name: "Marked",
                table: "Receipts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
