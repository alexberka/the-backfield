using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class InitialPlay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plays_Games_GameId",
                table: "Plays");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Plays",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "Plays",
                columns: new[] { "Id", "ClockEnd", "ClockStart", "Down", "FieldPositionEnd", "FieldPositionStart", "GameId", "Notes", "PrevPlayId", "ToGain" },
                values: new object[] { -1, null, null, 0, null, null, null, "Game Start", null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_Plays_Games_GameId",
                table: "Plays",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plays_Games_GameId",
                table: "Plays");

            migrationBuilder.DeleteData(
                table: "Plays",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "Plays",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Plays_Games_GameId",
                table: "Plays",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
