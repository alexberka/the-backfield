using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class PassDeleteBehavior : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passes_Players_PasserId",
                table: "Passes");

            migrationBuilder.DropForeignKey(
                name: "FK_Passes_Players_ReceiverId",
                table: "Passes");

            migrationBuilder.AlterColumn<int>(
                name: "PasserId",
                table: "Passes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Passes_Players_PasserId",
                table: "Passes",
                column: "PasserId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Passes_Players_ReceiverId",
                table: "Passes",
                column: "ReceiverId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passes_Players_PasserId",
                table: "Passes");

            migrationBuilder.DropForeignKey(
                name: "FK_Passes_Players_ReceiverId",
                table: "Passes");

            migrationBuilder.AlterColumn<int>(
                name: "PasserId",
                table: "Passes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Passes_Players_PasserId",
                table: "Passes",
                column: "PasserId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Passes_Players_ReceiverId",
                table: "Passes",
                column: "ReceiverId",
                principalTable: "Players",
                principalColumn: "Id");
        }
    }
}
