using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class AddSafetyTouchdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Safeties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    CedingPlayerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Safeties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Safeties_Players_CedingPlayerId",
                        column: x => x.CedingPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Safeties_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Touchdowns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayId = table.Column<int>(type: "integer", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Touchdowns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Touchdowns_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Touchdowns_Plays_PlayId",
                        column: x => x.PlayId,
                        principalTable: "Plays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Safeties_CedingPlayerId",
                table: "Safeties",
                column: "CedingPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Safeties_PlayId",
                table: "Safeties",
                column: "PlayId");

            migrationBuilder.CreateIndex(
                name: "IX_Touchdowns_PlayerId",
                table: "Touchdowns",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Touchdowns_PlayId",
                table: "Touchdowns",
                column: "PlayId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Safeties");

            migrationBuilder.DropTable(
                name: "Touchdowns");
        }
    }
}
