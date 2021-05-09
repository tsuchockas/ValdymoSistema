using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ValdymoSistema.Migrations
{
    public partial class AddedLightEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LightEvents",
                columns: table => new
                {
                    LightEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LightId = table.Column<Guid>(type: "uuid", nullable: true),
                    CurrentLightState = table.Column<int>(type: "integer", nullable: false),
                    Brightness = table.Column<int>(type: "integer", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EnergyUsage = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LightEvents", x => x.LightEventId);
                    table.ForeignKey(
                        name: "FK_LightEvents_Lights_LightId",
                        column: x => x.LightId,
                        principalTable: "Lights",
                        principalColumn: "LightId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LightEvents_LightId",
                table: "LightEvents",
                column: "LightId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LightEvents");
        }
    }
}
