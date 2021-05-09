using Microsoft.EntityFrameworkCore.Migrations;

namespace ValdymoSistema.Migrations
{
    public partial class AddedLightEventAndBrightness : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentBrightness",
                table: "Lights",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentBrightness",
                table: "Lights");
        }
    }
}
