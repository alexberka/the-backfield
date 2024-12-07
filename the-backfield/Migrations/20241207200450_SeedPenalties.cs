using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class SeedPenalties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rushes_PlayId",
                table: "Rushes");

            migrationBuilder.DropIndex(
                name: "IX_Punts_PlayId",
                table: "Punts");

            migrationBuilder.DropIndex(
                name: "IX_Passes_PlayId",
                table: "Passes");

            migrationBuilder.DropIndex(
                name: "IX_Kickoffs_PlayId",
                table: "Kickoffs");

            migrationBuilder.DropIndex(
                name: "IX_KickBlocks_PlayId",
                table: "KickBlocks");

            migrationBuilder.DropIndex(
                name: "IX_Interceptions_PlayId",
                table: "Interceptions");

            migrationBuilder.DropIndex(
                name: "IX_FieldGoals_PlayId",
                table: "FieldGoals");

            migrationBuilder.DropIndex(
                name: "IX_ExtraPoints_PlayId",
                table: "ExtraPoints");

            migrationBuilder.DropIndex(
                name: "IX_Conversion_PlayId",
                table: "Conversion");

            migrationBuilder.AddColumn<bool>(
                name: "LossOfDown",
                table: "PlayPenalties",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BasePenaltyId",
                table: "Penalties",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LossOfDown",
                table: "Penalties",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Penalties",
                type: "integer",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Penalties",
                columns: new[] { "Id", "AutoFirstDown", "BasePenaltyId", "LossOfDown", "Name", "NoPlay", "UserId", "Yardage" },
                values: new object[,]
                {
                    { 1, false, null, false, "Penalty", true, null, 0 },
                    { 2, false, null, false, "Chop Block", true, null, 15 },
                    { 3, false, null, false, "Clipping", true, null, 15 },
                    { 4, true, null, false, "Defensive Holding", true, null, 5 },
                    { 5, false, null, false, "Defensive Offside", true, null, 5 },
                    { 6, true, null, false, "Defensive Pass Interference", true, null, 0 },
                    { 7, false, null, false, "Defensive Too Many Men on Field", true, null, 5 },
                    { 8, false, null, false, "Delay of Game", true, null, 5 },
                    { 9, false, null, false, "Encroachment", true, null, 5 },
                    { 10, true, null, false, "Facemask", true, null, 15 },
                    { 11, false, null, false, "Fair Catch Interference", true, null, 15 },
                    { 12, false, null, false, "False Start", true, null, 5 },
                    { 13, true, null, false, "Hip-Drop Tackle", true, null, 15 },
                    { 14, true, null, false, "Horse-Collar Tackle", true, null, 15 },
                    { 15, false, null, false, "Illegal Block in the Back", true, null, 10 },
                    { 16, true, null, false, "Illegal Contact", true, null, 5 },
                    { 17, false, null, false, "Illegal Formation", true, null, 5 },
                    { 18, false, null, true, "Illegal Forward Pass", true, null, 5 },
                    { 19, false, null, false, "Illegal Kicking Loose Ball", true, null, 10 },
                    { 20, false, null, false, "Illegal Motion", true, null, 5 },
                    { 21, false, null, false, "Illegal Shift", true, null, 5 },
                    { 22, false, null, false, "Illegal Substitution", true, null, 5 },
                    { 23, false, null, true, "Illegal Touch - Player Out of Bounds", true, null, 0 },
                    { 24, false, null, true, "Illegal Touch - Ineligible Receiver", true, null, 5 },
                    { 25, true, null, false, "Illegal Use of Hands", true, null, 10 },
                    { 26, true, null, false, "Impermissible Use of the Helmet", true, null, 15 },
                    { 27, false, null, false, "Ineligible Player Downfield", true, null, 5 },
                    { 28, false, null, true, "Intentional Grounding", true, null, 10 },
                    { 29, false, null, false, "Invalid Fair Catch Signal", true, null, 5 },
                    { 30, false, null, false, "Kickoff Out of Bounds", true, null, 0 },
                    { 31, false, null, false, "Leaping", true, null, 15 },
                    { 32, false, null, false, "Neutral Zone Infraction", true, null, 5 },
                    { 33, false, null, false, "Offensive Holding", true, null, 10 },
                    { 34, false, null, false, "Offensive Offside", true, null, 5 },
                    { 35, false, null, false, "Offensive Pass Interference", true, null, 10 },
                    { 36, false, null, false, "Offensive Too Many Men on Field", true, null, 5 },
                    { 37, true, null, false, "Roughing the Kicker", true, null, 15 },
                    { 38, true, null, false, "Roughing the Passer", false, null, 15 },
                    { 39, false, null, false, "Running into the Kicker", true, null, 5 },
                    { 40, false, null, false, "Taunting", false, null, 15 },
                    { 41, true, null, false, "Tripping", true, null, 15 },
                    { 42, true, null, false, "Unnecessary Roughness", false, null, 15 },
                    { 43, true, null, false, "Unsportsmanlike Conduct", false, null, 15 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rushes_PlayId",
                table: "Rushes",
                column: "PlayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Punts_PlayId",
                table: "Punts",
                column: "PlayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passes_PlayId",
                table: "Passes",
                column: "PlayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kickoffs_PlayId",
                table: "Kickoffs",
                column: "PlayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KickBlocks_PlayId",
                table: "KickBlocks",
                column: "PlayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interceptions_PlayId",
                table: "Interceptions",
                column: "PlayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldGoals_PlayId",
                table: "FieldGoals",
                column: "PlayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExtraPoints_PlayId",
                table: "ExtraPoints",
                column: "PlayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversion_PlayId",
                table: "Conversion",
                column: "PlayId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rushes_PlayId",
                table: "Rushes");

            migrationBuilder.DropIndex(
                name: "IX_Punts_PlayId",
                table: "Punts");

            migrationBuilder.DropIndex(
                name: "IX_Passes_PlayId",
                table: "Passes");

            migrationBuilder.DropIndex(
                name: "IX_Kickoffs_PlayId",
                table: "Kickoffs");

            migrationBuilder.DropIndex(
                name: "IX_KickBlocks_PlayId",
                table: "KickBlocks");

            migrationBuilder.DropIndex(
                name: "IX_Interceptions_PlayId",
                table: "Interceptions");

            migrationBuilder.DropIndex(
                name: "IX_FieldGoals_PlayId",
                table: "FieldGoals");

            migrationBuilder.DropIndex(
                name: "IX_ExtraPoints_PlayId",
                table: "ExtraPoints");

            migrationBuilder.DropIndex(
                name: "IX_Conversion_PlayId",
                table: "Conversion");

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Penalties",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DropColumn(
                name: "LossOfDown",
                table: "PlayPenalties");

            migrationBuilder.DropColumn(
                name: "BasePenaltyId",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "LossOfDown",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Penalties");

            migrationBuilder.CreateIndex(
                name: "IX_Rushes_PlayId",
                table: "Rushes",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Punts_PlayId",
                table: "Punts",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Passes_PlayId",
                table: "Passes",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Kickoffs_PlayId",
                table: "Kickoffs",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_KickBlocks_PlayId",
                table: "KickBlocks",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Interceptions_PlayId",
                table: "Interceptions",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldGoals_PlayId",
                table: "FieldGoals",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraPoints_PlayId",
                table: "ExtraPoints",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversion_PlayId",
                table: "Conversion",
                column: "PlayId");
        }
    }
}
