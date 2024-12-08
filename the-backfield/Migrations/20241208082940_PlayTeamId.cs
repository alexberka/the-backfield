using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class PlayTeamId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Plays",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plays_TeamId",
                table: "Plays",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plays_Teams_TeamId",
                table: "Plays",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plays_Teams_TeamId",
                table: "Plays");

            migrationBuilder.DropIndex(
                name: "IX_Plays_TeamId",
                table: "Plays");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Plays");
        }
    }
}
