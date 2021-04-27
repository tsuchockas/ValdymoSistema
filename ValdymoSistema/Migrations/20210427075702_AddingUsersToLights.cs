using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ValdymoSistema.Migrations
{
    public partial class AddingUsersToLights : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lights_AspNetUsers_UserId",
                table: "Lights");

            migrationBuilder.DropIndex(
                name: "IX_Lights_UserId",
                table: "Lights");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Lights");

            migrationBuilder.CreateTable(
                name: "LightUser",
                columns: table => new
                {
                    LightsLightId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LightUser", x => new { x.LightsLightId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_LightUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LightUser_Lights_LightsLightId",
                        column: x => x.LightsLightId,
                        principalTable: "Lights",
                        principalColumn: "LightId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LightUser_UsersId",
                table: "LightUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LightUser");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Lights",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lights_UserId",
                table: "Lights",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lights_AspNetUsers_UserId",
                table: "Lights",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
