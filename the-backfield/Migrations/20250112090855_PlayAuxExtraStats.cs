using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class PlayAuxExtraStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Touchdowns_PlayId",
                table: "Touchdowns");

            migrationBuilder.DropIndex(
                name: "IX_Safeties_PlayId",
                table: "Safeties");

            migrationBuilder.AddColumn<int>(
                name: "Yardage",
                table: "Rushes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Distance",
                table: "Punts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnYardage",
                table: "Punts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Sack",
                table: "Passes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Spike",
                table: "Passes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "YardageType",
                table: "Laterals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Distance",
                table: "Kickoffs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnYardage",
                table: "Kickoffs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LooseBallYardage",
                table: "KickBlocks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnYardage",
                table: "KickBlocks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnYardage",
                table: "Interceptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LooseBallYardage",
                table: "Fumbles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnYardage",
                table: "Fumbles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "YardageType",
                table: "Fumbles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Distance",
                table: "FieldGoals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Touchdowns_PlayId",
                table: "Touchdowns",
                column: "PlayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Safeties_PlayId",
                table: "Safeties",
                column: "PlayId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Touchdowns_PlayId",
                table: "Touchdowns");

            migrationBuilder.DropIndex(
                name: "IX_Safeties_PlayId",
                table: "Safeties");

            migrationBuilder.DropColumn(
                name: "Yardage",
                table: "Rushes");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Punts");

            migrationBuilder.DropColumn(
                name: "ReturnYardage",
                table: "Punts");

            migrationBuilder.DropColumn(
                name: "Sack",
                table: "Passes");

            migrationBuilder.DropColumn(
                name: "Spike",
                table: "Passes");

            migrationBuilder.DropColumn(
                name: "YardageType",
                table: "Laterals");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Kickoffs");

            migrationBuilder.DropColumn(
                name: "ReturnYardage",
                table: "Kickoffs");

            migrationBuilder.DropColumn(
                name: "LooseBallYardage",
                table: "KickBlocks");

            migrationBuilder.DropColumn(
                name: "ReturnYardage",
                table: "KickBlocks");

            migrationBuilder.DropColumn(
                name: "ReturnYardage",
                table: "Interceptions");

            migrationBuilder.DropColumn(
                name: "LooseBallYardage",
                table: "Fumbles");

            migrationBuilder.DropColumn(
                name: "ReturnYardage",
                table: "Fumbles");

            migrationBuilder.DropColumn(
                name: "YardageType",
                table: "Fumbles");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "FieldGoals");

            migrationBuilder.CreateIndex(
                name: "IX_Touchdowns_PlayId",
                table: "Touchdowns",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Safeties_PlayId",
                table: "Safeties",
                column: "PlayId");
        }
    }
}
