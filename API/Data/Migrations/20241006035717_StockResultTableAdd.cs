using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class StockResultTableAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    T = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    V = table.Column<int>(type: "int", nullable: false),
                    Vw = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    O = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    C = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    H = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    L = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    N = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockResults");
        }
    }
}
