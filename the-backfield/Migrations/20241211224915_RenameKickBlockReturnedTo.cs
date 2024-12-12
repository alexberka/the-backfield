using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class RenameKickBlockReturnedTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnedTo",
                table: "KickBlocks");

            migrationBuilder.AddColumn<int>(
                name: "RecoveredAt",
                table: "KickBlocks",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecoveredAt",
                table: "KickBlocks");

            migrationBuilder.AddColumn<int>(
                name: "ReturnedTo",
                table: "KickBlocks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
