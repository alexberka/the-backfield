using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class TeamPropertiesUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ColorSecondary",
                table: "Teams",
                newName: "ColorSecondaryHex");

            migrationBuilder.RenameColumn(
                name: "ColorPrimary",
                table: "Teams",
                newName: "ColorPrimaryHex");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ColorSecondaryHex",
                table: "Teams",
                newName: "ColorSecondary");

            migrationBuilder.RenameColumn(
                name: "ColorPrimaryHex",
                table: "Teams",
                newName: "ColorPrimary");
        }
    }
}
