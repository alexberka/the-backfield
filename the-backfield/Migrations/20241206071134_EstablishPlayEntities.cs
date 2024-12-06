using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class EstablishPlayEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Positions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Positions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Players",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Players",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Hometown",
                table: "Players",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Players",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Penalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NoPlay = table.Column<bool>(type: "boolean", nullable: false),
                    AutoFirstDown = table.Column<bool>(type: "boolean", nullable: false),
                    Yardage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrevPlayId = table.Column<int>(type: "integer", nullable: true),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    FieldPositionStart = table.Column<int>(type: "integer", nullable: true),
                    FieldPositionEnd = table.Column<int>(type: "integer", nullable: true),
                    Down = table.Column<int>(type: "integer", nullable: false),
                    ToGain = table.Column<int>(type: "integer", nullable: true),
                    ClockStart = table.Column<int>(type: "integer", nullable: true),
                    ClockEnd = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plays_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plays_Plays_PrevPlayId",
                        column: x => x.PrevPlayId,
                        principalTable: "Plays",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Conversion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    PasserId = table.Column<int>(type: "integer", nullable: true),
                    ReceiverId = table.Column<int>(type: "integer", nullable: true),
                    RusherId = table.Column<int>(type: "integer", nullable: true),
                    ReturnerId = table.Column<int>(type: "integer", nullable: true),
                    Good = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversion_Players_PasserId",
                        column: x => x.PasserId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversion_Players_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversion_Players_ReturnerId",
                        column: x => x.ReturnerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversion_Players_RusherId",
                        column: x => x.RusherId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversion_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtraPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    KickerId = table.Column<int>(type: "integer", nullable: false),
                    Good = table.Column<bool>(type: "boolean", nullable: false),
                    Fake = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtraPoints_Players_KickerId",
                        column: x => x.KickerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtraPoints_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldGoals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    KickerId = table.Column<int>(type: "integer", nullable: false),
                    Good = table.Column<bool>(type: "boolean", nullable: false),
                    Fake = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FieldGoals_Players_KickerId",
                        column: x => x.KickerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldGoals_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fumbles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    FumbleCommittedById = table.Column<int>(type: "integer", nullable: false),
                    FumbledAt = table.Column<int>(type: "integer", nullable: false),
                    FumbleForcedById = table.Column<int>(type: "integer", nullable: true),
                    FumbleRecoveredById = table.Column<int>(type: "integer", nullable: true),
                    RecoveredAt = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fumbles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fumbles_Players_FumbleCommittedById",
                        column: x => x.FumbleCommittedById,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fumbles_Players_FumbleForcedById",
                        column: x => x.FumbleForcedById,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fumbles_Players_FumbleRecoveredById",
                        column: x => x.FumbleRecoveredById,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Fumbles_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interceptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    InterceptedById = table.Column<int>(type: "integer", nullable: false),
                    InterceptedAt = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interceptions_Players_InterceptedById",
                        column: x => x.InterceptedById,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interceptions_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KickBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    BlockedById = table.Column<int>(type: "integer", nullable: true),
                    RecoveredById = table.Column<int>(type: "integer", nullable: true),
                    ReturnedTo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KickBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KickBlocks_Players_BlockedById",
                        column: x => x.BlockedById,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KickBlocks_Players_RecoveredById",
                        column: x => x.RecoveredById,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KickBlocks_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kickoffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    KickerId = table.Column<int>(type: "integer", nullable: false),
                    ReturnerId = table.Column<int>(type: "integer", nullable: true),
                    FieldedAt = table.Column<int>(type: "integer", nullable: false),
                    Touchback = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kickoffs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kickoffs_Players_KickerId",
                        column: x => x.KickerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kickoffs_Players_ReturnerId",
                        column: x => x.ReturnerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Kickoffs_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Laterals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    NewCarrierId = table.Column<int>(type: "integer", nullable: false),
                    PossessionAt = table.Column<int>(type: "integer", nullable: true),
                    CarriedTo = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laterals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Laterals_Players_NewCarrierId",
                        column: x => x.NewCarrierId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Laterals_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PassDefenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    DefenderId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PassDefenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PassDefenses_Players_DefenderId",
                        column: x => x.DefenderId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PassDefenses_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    PasserId = table.Column<int>(type: "integer", nullable: false),
                    ReceiverId = table.Column<int>(type: "integer", nullable: true),
                    Completion = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passes_Players_PasserId",
                        column: x => x.PasserId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Passes_Players_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Passes_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayPenalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    PenaltyId = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: true),
                    TeamId = table.Column<int>(type: "integer", nullable: false),
                    Enforced = table.Column<bool>(type: "boolean", nullable: false),
                    EnforcedFrom = table.Column<int>(type: "integer", nullable: false),
                    NoPlay = table.Column<bool>(type: "boolean", nullable: false),
                    AutoFirstDown = table.Column<bool>(type: "boolean", nullable: false),
                    Yardage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayPenalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayPenalties_Penalties_PenaltyId",
                        column: x => x.PenaltyId,
                        principalTable: "Penalties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayPenalties_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlayPenalties_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Punts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    KickerId = table.Column<int>(type: "integer", nullable: false),
                    ReturnerId = table.Column<int>(type: "integer", nullable: false),
                    FieldedAt = table.Column<int>(type: "integer", nullable: true),
                    FairCatch = table.Column<bool>(type: "boolean", nullable: false),
                    Touchback = table.Column<bool>(type: "boolean", nullable: false),
                    Fake = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Punts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Punts_Players_KickerId",
                        column: x => x.KickerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Punts_Players_ReturnerId",
                        column: x => x.ReturnerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Punts_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rushes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    RusherId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rushes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rushes_Players_RusherId",
                        column: x => x.RusherId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rushes_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tackles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    TacklerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tackles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tackles_Players_TacklerId",
                        column: x => x.TacklerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tackles_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversion_PasserId",
                table: "Conversion",
                column: "PasserId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversion_PlayId",
                table: "Conversion",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversion_ReceiverId",
                table: "Conversion",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversion_ReturnerId",
                table: "Conversion",
                column: "ReturnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversion_RusherId",
                table: "Conversion",
                column: "RusherId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraPoints_KickerId",
                table: "ExtraPoints",
                column: "KickerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraPoints_PlayId",
                table: "ExtraPoints",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldGoals_KickerId",
                table: "FieldGoals",
                column: "KickerId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldGoals_PlayId",
                table: "FieldGoals",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Fumbles_FumbleCommittedById",
                table: "Fumbles",
                column: "FumbleCommittedById");

            migrationBuilder.CreateIndex(
                name: "IX_Fumbles_FumbleForcedById",
                table: "Fumbles",
                column: "FumbleForcedById");

            migrationBuilder.CreateIndex(
                name: "IX_Fumbles_FumbleRecoveredById",
                table: "Fumbles",
                column: "FumbleRecoveredById");

            migrationBuilder.CreateIndex(
                name: "IX_Fumbles_PlayId",
                table: "Fumbles",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Interceptions_InterceptedById",
                table: "Interceptions",
                column: "InterceptedById");

            migrationBuilder.CreateIndex(
                name: "IX_Interceptions_PlayId",
                table: "Interceptions",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_KickBlocks_BlockedById",
                table: "KickBlocks",
                column: "BlockedById");

            migrationBuilder.CreateIndex(
                name: "IX_KickBlocks_PlayId",
                table: "KickBlocks",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_KickBlocks_RecoveredById",
                table: "KickBlocks",
                column: "RecoveredById");

            migrationBuilder.CreateIndex(
                name: "IX_Kickoffs_KickerId",
                table: "Kickoffs",
                column: "KickerId");

            migrationBuilder.CreateIndex(
                name: "IX_Kickoffs_PlayId",
                table: "Kickoffs",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Kickoffs_ReturnerId",
                table: "Kickoffs",
                column: "ReturnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Laterals_NewCarrierId",
                table: "Laterals",
                column: "NewCarrierId");

            migrationBuilder.CreateIndex(
                name: "IX_Laterals_PlayId",
                table: "Laterals",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_PassDefenses_DefenderId",
                table: "PassDefenses",
                column: "DefenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PassDefenses_PlayId",
                table: "PassDefenses",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Passes_PasserId",
                table: "Passes",
                column: "PasserId");

            migrationBuilder.CreateIndex(
                name: "IX_Passes_PlayId",
                table: "Passes",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Passes_ReceiverId",
                table: "Passes",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayPenalties_PenaltyId",
                table: "PlayPenalties",
                column: "PenaltyId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayPenalties_PlayerId",
                table: "PlayPenalties",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayPenalties_PlayId",
                table: "PlayPenalties",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Plays_GameId",
                table: "Plays",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Plays_PrevPlayId",
                table: "Plays",
                column: "PrevPlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Punts_KickerId",
                table: "Punts",
                column: "KickerId");

            migrationBuilder.CreateIndex(
                name: "IX_Punts_PlayId",
                table: "Punts",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Punts_ReturnerId",
                table: "Punts",
                column: "ReturnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rushes_PlayId",
                table: "Rushes",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Rushes_RusherId",
                table: "Rushes",
                column: "RusherId");

            migrationBuilder.CreateIndex(
                name: "IX_Tackles_PlayId",
                table: "Tackles",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Tackles_TacklerId",
                table: "Tackles",
                column: "TacklerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conversion");

            migrationBuilder.DropTable(
                name: "ExtraPoints");

            migrationBuilder.DropTable(
                name: "FieldGoals");

            migrationBuilder.DropTable(
                name: "Fumbles");

            migrationBuilder.DropTable(
                name: "Interceptions");

            migrationBuilder.DropTable(
                name: "KickBlocks");

            migrationBuilder.DropTable(
                name: "Kickoffs");

            migrationBuilder.DropTable(
                name: "Laterals");

            migrationBuilder.DropTable(
                name: "PassDefenses");

            migrationBuilder.DropTable(
                name: "Passes");

            migrationBuilder.DropTable(
                name: "PlayPenalties");

            migrationBuilder.DropTable(
                name: "Punts");

            migrationBuilder.DropTable(
                name: "Rushes");

            migrationBuilder.DropTable(
                name: "Tackles");

            migrationBuilder.DropTable(
                name: "Penalties");

            migrationBuilder.DropTable(
                name: "Plays");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Positions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Abbreviation",
                table: "Positions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Hometown",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Players",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
