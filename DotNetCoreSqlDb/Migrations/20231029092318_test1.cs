using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetCoreSqlDb.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElectricalTestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobNameOrNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CircuitLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CircuitNameOrDesignation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisualInspection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProtectionSizeOrType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NeutralNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfPhases = table.Column<int>(type: "int", nullable: true),
                    CableSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EarthSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortCircuitPass = table.Column<bool>(type: "bit", nullable: true),
                    InterconnectPass = table.Column<bool>(type: "bit", nullable: true),
                    PolarityPass = table.Column<bool>(type: "bit", nullable: true),
                    ContinuityOhms = table.Column<double>(type: "float", nullable: true),
                    InsulationResistance = table.Column<double>(type: "float", nullable: true),
                    FaultLoopImpedance = table.Column<double>(type: "float", nullable: true),
                    RcdTripTime = table.Column<double>(type: "float", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TesterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectricalTestResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectricalTestResults");
        }
    }
}
