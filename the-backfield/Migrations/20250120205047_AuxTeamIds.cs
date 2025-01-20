using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class AuxTeamIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Touchdowns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Tackles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CedingTeamId",
                table: "Safeties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Rushes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnTeamId",
                table: "Punts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Punts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Passes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "PassDefenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "PrevCarrierId",
                table: "Laterals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "NewCarrierId",
                table: "Laterals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Laterals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnTeamId",
                table: "Kickoffs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Kickoffs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlockedByTeamId",
                table: "KickBlocks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecoveredByTeamId",
                table: "KickBlocks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "InterceptedById",
                table: "Interceptions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Interceptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "FumbleCommittedById",
                table: "Fumbles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "FumbleCommittedByTeamId",
                table: "Fumbles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FumbleForcedByTeamId",
                table: "Fumbles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FumbleRecoveredByTeamId",
                table: "Fumbles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "FieldGoals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnTeamId",
                table: "ExtraPoints",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "ExtraPoints",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReturnTeamId",
                table: "Conversions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Conversions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Touchdowns");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Tackles");

            migrationBuilder.DropColumn(
                name: "CedingTeamId",
                table: "Safeties");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Rushes");

            migrationBuilder.DropColumn(
                name: "ReturnTeamId",
                table: "Punts");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Punts");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Passes");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "PassDefenses");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Laterals");

            migrationBuilder.DropColumn(
                name: "ReturnTeamId",
                table: "Kickoffs");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Kickoffs");

            migrationBuilder.DropColumn(
                name: "BlockedByTeamId",
                table: "KickBlocks");

            migrationBuilder.DropColumn(
                name: "RecoveredByTeamId",
                table: "KickBlocks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Interceptions");

            migrationBuilder.DropColumn(
                name: "FumbleCommittedByTeamId",
                table: "Fumbles");

            migrationBuilder.DropColumn(
                name: "FumbleForcedByTeamId",
                table: "Fumbles");

            migrationBuilder.DropColumn(
                name: "FumbleRecoveredByTeamId",
                table: "Fumbles");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "FieldGoals");

            migrationBuilder.DropColumn(
                name: "ReturnTeamId",
                table: "ExtraPoints");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "ExtraPoints");

            migrationBuilder.DropColumn(
                name: "ReturnTeamId",
                table: "Conversions");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Conversions");

            migrationBuilder.AlterColumn<int>(
                name: "PrevCarrierId",
                table: "Laterals",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NewCarrierId",
                table: "Laterals",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InterceptedById",
                table: "Interceptions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FumbleCommittedById",
                table: "Fumbles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
