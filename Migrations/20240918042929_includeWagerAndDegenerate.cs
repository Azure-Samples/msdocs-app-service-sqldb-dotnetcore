using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetCoreSqlDb.Migrations
{
    /// <inheritdoc />
    public partial class includeWagerAndDegenerate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Degenerate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CashWallet = table.Column<double>(type: "float", nullable: false),
                    BetsPlaced = table.Column<int>(type: "int", nullable: false),
                    BetsWon = table.Column<int>(type: "int", nullable: false),
                    TotalWagesPlaced = table.Column<double>(type: "float", nullable: false),
                    TotalWagesWon = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degenerate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WagerItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    SportType = table.Column<int>(type: "int", nullable: false),
                    WagerValue = table.Column<double>(type: "float", nullable: false),
                    BetType = table.Column<int>(type: "int", nullable: false),
                    Odds = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Outcome = table.Column<bool>(type: "bit", nullable: true),
                    TimeOfBet = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WagerTeamA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WagerTeamB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WagerJuice = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WagerItem", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Degenerate");

            migrationBuilder.DropTable(
                name: "WagerItem");
        }
    }
}
