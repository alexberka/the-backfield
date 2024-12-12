using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class LateralFumbleUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrevCarrierId",
                table: "Laterals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "RecoveredAt",
                table: "Fumbles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FumbledAt",
                table: "Fumbles",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Laterals_PrevCarrierId",
                table: "Laterals",
                column: "PrevCarrierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Laterals_Players_PrevCarrierId",
                table: "Laterals",
                column: "PrevCarrierId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Laterals_Players_PrevCarrierId",
                table: "Laterals");

            migrationBuilder.DropIndex(
                name: "IX_Laterals_PrevCarrierId",
                table: "Laterals");

            migrationBuilder.DropColumn(
                name: "PrevCarrierId",
                table: "Laterals");

            migrationBuilder.AlterColumn<int>(
                name: "RecoveredAt",
                table: "Fumbles",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FumbledAt",
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
