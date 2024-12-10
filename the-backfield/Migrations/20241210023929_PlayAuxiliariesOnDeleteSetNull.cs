using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class PlayAuxiliariesOnDeleteSetNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversion_Players_PasserId",
                table: "Conversion");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversion_Players_ReceiverId",
                table: "Conversion");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversion_Players_ReturnerId",
                table: "Conversion");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversion_Players_RusherId",
                table: "Conversion");

            migrationBuilder.DropForeignKey(
                name: "FK_ExtraPoints_Players_KickerId",
                table: "ExtraPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldGoals_Players_KickerId",
                table: "FieldGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_Fumbles_Players_FumbleCommittedById",
                table: "Fumbles");

            migrationBuilder.DropForeignKey(
                name: "FK_Fumbles_Players_FumbleForcedById",
                table: "Fumbles");

            migrationBuilder.DropForeignKey(
                name: "FK_Fumbles_Players_FumbleRecoveredById",
                table: "Fumbles");

            migrationBuilder.DropForeignKey(
                name: "FK_Interceptions_Players_InterceptedById",
                table: "Interceptions");

            migrationBuilder.DropForeignKey(
                name: "FK_KickBlocks_Players_BlockedById",
                table: "KickBlocks");

            migrationBuilder.DropForeignKey(
                name: "FK_KickBlocks_Players_RecoveredById",
                table: "KickBlocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Kickoffs_Players_KickerId",
                table: "Kickoffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Kickoffs_Players_ReturnerId",
                table: "Kickoffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Laterals_Players_NewCarrierId",
                table: "Laterals");

            migrationBuilder.DropForeignKey(
                name: "FK_PassDefenses_Players_DefenderId",
                table: "PassDefenses");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayPenalties_Players_PlayerId",
                table: "PlayPenalties");

            migrationBuilder.DropForeignKey(
                name: "FK_Punts_Players_KickerId",
                table: "Punts");

            migrationBuilder.DropForeignKey(
                name: "FK_Punts_Players_ReturnerId",
                table: "Punts");

            migrationBuilder.DropForeignKey(
                name: "FK_Rushes_Players_RusherId",
                table: "Rushes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tackles_Players_TacklerId",
                table: "Tackles");

            migrationBuilder.AlterColumn<int>(
                name: "TacklerId",
                table: "Tackles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "RusherId",
                table: "Rushes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ReturnerId",
                table: "Punts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "KickerId",
                table: "Punts",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DefenderId",
                table: "PassDefenses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "KickerId",
                table: "Kickoffs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "KickerId",
                table: "FieldGoals",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "KickerId",
                table: "ExtraPoints",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversion_Players_PasserId",
                table: "Conversion",
                column: "PasserId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversion_Players_ReceiverId",
                table: "Conversion",
                column: "ReceiverId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversion_Players_ReturnerId",
                table: "Conversion",
                column: "ReturnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversion_Players_RusherId",
                table: "Conversion",
                column: "RusherId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraPoints_Players_KickerId",
                table: "ExtraPoints",
                column: "KickerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldGoals_Players_KickerId",
                table: "FieldGoals",
                column: "KickerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Fumbles_Players_FumbleCommittedById",
                table: "Fumbles",
                column: "FumbleCommittedById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Fumbles_Players_FumbleForcedById",
                table: "Fumbles",
                column: "FumbleForcedById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Fumbles_Players_FumbleRecoveredById",
                table: "Fumbles",
                column: "FumbleRecoveredById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Interceptions_Players_InterceptedById",
                table: "Interceptions",
                column: "InterceptedById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_KickBlocks_Players_BlockedById",
                table: "KickBlocks",
                column: "BlockedById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_KickBlocks_Players_RecoveredById",
                table: "KickBlocks",
                column: "RecoveredById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Kickoffs_Players_KickerId",
                table: "Kickoffs",
                column: "KickerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Kickoffs_Players_ReturnerId",
                table: "Kickoffs",
                column: "ReturnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Laterals_Players_NewCarrierId",
                table: "Laterals",
                column: "NewCarrierId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PassDefenses_Players_DefenderId",
                table: "PassDefenses",
                column: "DefenderId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayPenalties_Players_PlayerId",
                table: "PlayPenalties",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Punts_Players_KickerId",
                table: "Punts",
                column: "KickerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Punts_Players_ReturnerId",
                table: "Punts",
                column: "ReturnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rushes_Players_RusherId",
                table: "Rushes",
                column: "RusherId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tackles_Players_TacklerId",
                table: "Tackles",
                column: "TacklerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversion_Players_PasserId",
                table: "Conversion");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversion_Players_ReceiverId",
                table: "Conversion");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversion_Players_ReturnerId",
                table: "Conversion");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversion_Players_RusherId",
                table: "Conversion");

            migrationBuilder.DropForeignKey(
                name: "FK_ExtraPoints_Players_KickerId",
                table: "ExtraPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldGoals_Players_KickerId",
                table: "FieldGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_Fumbles_Players_FumbleCommittedById",
                table: "Fumbles");

            migrationBuilder.DropForeignKey(
                name: "FK_Fumbles_Players_FumbleForcedById",
                table: "Fumbles");

            migrationBuilder.DropForeignKey(
                name: "FK_Fumbles_Players_FumbleRecoveredById",
                table: "Fumbles");

            migrationBuilder.DropForeignKey(
                name: "FK_Interceptions_Players_InterceptedById",
                table: "Interceptions");

            migrationBuilder.DropForeignKey(
                name: "FK_KickBlocks_Players_BlockedById",
                table: "KickBlocks");

            migrationBuilder.DropForeignKey(
                name: "FK_KickBlocks_Players_RecoveredById",
                table: "KickBlocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Kickoffs_Players_KickerId",
                table: "Kickoffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Kickoffs_Players_ReturnerId",
                table: "Kickoffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Laterals_Players_NewCarrierId",
                table: "Laterals");

            migrationBuilder.DropForeignKey(
                name: "FK_PassDefenses_Players_DefenderId",
                table: "PassDefenses");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayPenalties_Players_PlayerId",
                table: "PlayPenalties");

            migrationBuilder.DropForeignKey(
                name: "FK_Punts_Players_KickerId",
                table: "Punts");

            migrationBuilder.DropForeignKey(
                name: "FK_Punts_Players_ReturnerId",
                table: "Punts");

            migrationBuilder.DropForeignKey(
                name: "FK_Rushes_Players_RusherId",
                table: "Rushes");

            migrationBuilder.DropForeignKey(
                name: "FK_Tackles_Players_TacklerId",
                table: "Tackles");

            migrationBuilder.AlterColumn<int>(
                name: "TacklerId",
                table: "Tackles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RusherId",
                table: "Rushes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReturnerId",
                table: "Punts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "KickerId",
                table: "Punts",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DefenderId",
                table: "PassDefenses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "KickerId",
                table: "Kickoffs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "KickerId",
                table: "FieldGoals",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "KickerId",
                table: "ExtraPoints",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversion_Players_PasserId",
                table: "Conversion",
                column: "PasserId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversion_Players_ReceiverId",
                table: "Conversion",
                column: "ReceiverId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversion_Players_ReturnerId",
                table: "Conversion",
                column: "ReturnerId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversion_Players_RusherId",
                table: "Conversion",
                column: "RusherId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraPoints_Players_KickerId",
                table: "ExtraPoints",
                column: "KickerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldGoals_Players_KickerId",
                table: "FieldGoals",
                column: "KickerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fumbles_Players_FumbleCommittedById",
                table: "Fumbles",
                column: "FumbleCommittedById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fumbles_Players_FumbleForcedById",
                table: "Fumbles",
                column: "FumbleForcedById",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Fumbles_Players_FumbleRecoveredById",
                table: "Fumbles",
                column: "FumbleRecoveredById",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Interceptions_Players_InterceptedById",
                table: "Interceptions",
                column: "InterceptedById",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KickBlocks_Players_BlockedById",
                table: "KickBlocks",
                column: "BlockedById",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KickBlocks_Players_RecoveredById",
                table: "KickBlocks",
                column: "RecoveredById",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Kickoffs_Players_KickerId",
                table: "Kickoffs",
                column: "KickerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Kickoffs_Players_ReturnerId",
                table: "Kickoffs",
                column: "ReturnerId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Laterals_Players_NewCarrierId",
                table: "Laterals",
                column: "NewCarrierId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PassDefenses_Players_DefenderId",
                table: "PassDefenses",
                column: "DefenderId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayPenalties_Players_PlayerId",
                table: "PlayPenalties",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Punts_Players_KickerId",
                table: "Punts",
                column: "KickerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Punts_Players_ReturnerId",
                table: "Punts",
                column: "ReturnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rushes_Players_RusherId",
                table: "Rushes",
                column: "RusherId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tackles_Players_TacklerId",
                table: "Tackles",
                column: "TacklerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
