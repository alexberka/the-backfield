using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class DatabaseSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Abbreviation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocationName = table.Column<string>(type: "text", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: false),
                    HomeField = table.Column<string>(type: "text", nullable: false),
                    HomeLocation = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: false),
                    ColorPrimary = table.Column<string>(type: "text", nullable: false),
                    ColorSecondary = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SessionKey = table.Column<string>(type: "text", nullable: false),
                    SessionStart = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Uid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HomeTeamId = table.Column<int>(type: "integer", nullable: false),
                    HomeTeamScore = table.Column<int>(type: "integer", nullable: false),
                    AwayTeamId = table.Column<int>(type: "integer", nullable: false),
                    AwayTeamScore = table.Column<int>(type: "integer", nullable: false),
                    GameStart = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GamePeriods = table.Column<int>(type: "integer", nullable: false),
                    PeriodLength = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Hometown = table.Column<string>(type: "text", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    JerseyNumber = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    RushYards = table.Column<int>(type: "integer", nullable: false),
                    RushAttempts = table.Column<int>(type: "integer", nullable: false),
                    PassYards = table.Column<int>(type: "integer", nullable: false),
                    PassAttempts = table.Column<int>(type: "integer", nullable: false),
                    PassCompletions = table.Column<int>(type: "integer", nullable: false),
                    ReceivingYards = table.Column<int>(type: "integer", nullable: false),
                    Receptions = table.Column<int>(type: "integer", nullable: false),
                    Touchdowns = table.Column<int>(type: "integer", nullable: false),
                    Tackles = table.Column<int>(type: "integer", nullable: false),
                    InterceptionsThrown = table.Column<int>(type: "integer", nullable: false),
                    InterceptionsReceived = table.Column<int>(type: "integer", nullable: false),
                    FumblesCommitted = table.Column<int>(type: "integer", nullable: false),
                    FumblesForced = table.Column<int>(type: "integer", nullable: false),
                    FumblesRecovered = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameStats_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameStats_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerPosition",
                columns: table => new
                {
                    PlayersId = table.Column<int>(type: "integer", nullable: false),
                    PositionsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerPosition", x => new { x.PlayersId, x.PositionsId });
                    table.ForeignKey(
                        name: "FK_PlayerPosition_Players_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerPosition_Positions_PositionsId",
                        column: x => x.PositionsId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "Abbreviation", "Name" },
                values: new object[,]
                {
                    { 1, "QB", "Quarterback" },
                    { 2, "RB", "Running Back" },
                    { 3, "HB", "Halfback" },
                    { 4, "FB", "Fullback" },
                    { 5, "WR", "Wide Receiver" },
                    { 6, "TE", "Tight End" },
                    { 7, "OL", "Offensive Line" },
                    { 8, "C", "Center" },
                    { 9, "OG", "Offensive Guard" },
                    { 10, "LG", "Left Guard" },
                    { 11, "RG", "Right Guard" },
                    { 12, "OT", "Offensive Tackle" },
                    { 13, "LT", "Left Tackle" },
                    { 14, "RT", "Right Tackle" },
                    { 15, "DL", "Defensive Line" },
                    { 16, "NT", "Nose Tackle" },
                    { 17, "DE", "Defensive End" },
                    { 18, "DT", "Defensive Tackle" },
                    { 19, "LB", "Linebacker" },
                    { 20, "ILB", "Inside Linebacker" },
                    { 21, "MLB", "Middle Linebacker" },
                    { 22, "OLB", "Outside Linebacker" },
                    { 23, "EDGE", "Edge Rusher" },
                    { 24, "CB", "Cornerback" },
                    { 25, "S", "Safety" },
                    { 26, "FS", "Free Safety" },
                    { 27, "SS", "Strong Safety" },
                    { 28, "P", "Punter" },
                    { 29, "PK", "Place Kicker" },
                    { 30, "LS", "Long Snapper" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_AwayTeamId",
                table: "Games",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_HomeTeamId",
                table: "Games",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_GameId",
                table: "GameStats",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_PlayerId",
                table: "GameStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameStats_TeamId",
                table: "GameStats",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerPosition_PositionsId",
                table: "PlayerPosition",
                column: "PositionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId",
                table: "Players",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameStats");

            migrationBuilder.DropTable(
                name: "PlayerPosition");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
