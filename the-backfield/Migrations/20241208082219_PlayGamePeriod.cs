using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class PlayGamePeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GamePeriod",
                table: "Plays",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GamePeriod",
                table: "Plays");
        }
    }
}
