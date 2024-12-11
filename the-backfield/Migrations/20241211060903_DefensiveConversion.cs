using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace the_backfield.Migrations
{
    public partial class DefensiveConversion : Migration
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
                name: "FK_Conversion_Plays_PlayId",
                table: "Conversion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversion",
                table: "Conversion");

            migrationBuilder.RenameTable(
                name: "Conversion",
                newName: "Conversions");

            migrationBuilder.RenameIndex(
                name: "IX_Conversion_RusherId",
                table: "Conversions",
                newName: "IX_Conversions_RusherId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversion_ReturnerId",
                table: "Conversions",
                newName: "IX_Conversions_ReturnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversion_ReceiverId",
                table: "Conversions",
                newName: "IX_Conversions_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversion_PlayId",
                table: "Conversions",
                newName: "IX_Conversions_PlayId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversion_PasserId",
                table: "Conversions",
                newName: "IX_Conversions_PasserId");

            migrationBuilder.AlterColumn<int>(
                name: "FieldedAt",
                table: "Kickoffs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "DefensiveConversion",
                table: "ExtraPoints",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReturnerId",
                table: "ExtraPoints",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DefensiveConversion",
                table: "Conversions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversions",
                table: "Conversions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExtraPoints_ReturnerId",
                table: "ExtraPoints",
                column: "ReturnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversions_Players_PasserId",
                table: "Conversions",
                column: "PasserId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversions_Players_ReceiverId",
                table: "Conversions",
                column: "ReceiverId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversions_Players_ReturnerId",
                table: "Conversions",
                column: "ReturnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversions_Players_RusherId",
                table: "Conversions",
                column: "RusherId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversions_Plays_PlayId",
                table: "Conversions",
                column: "PlayId",
                principalTable: "Plays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraPoints_Players_ReturnerId",
                table: "ExtraPoints",
                column: "ReturnerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversions_Players_PasserId",
                table: "Conversions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversions_Players_ReceiverId",
                table: "Conversions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversions_Players_ReturnerId",
                table: "Conversions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversions_Players_RusherId",
                table: "Conversions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversions_Plays_PlayId",
                table: "Conversions");

            migrationBuilder.DropForeignKey(
                name: "FK_ExtraPoints_Players_ReturnerId",
                table: "ExtraPoints");

            migrationBuilder.DropIndex(
                name: "IX_ExtraPoints_ReturnerId",
                table: "ExtraPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversions",
                table: "Conversions");

            migrationBuilder.DropColumn(
                name: "DefensiveConversion",
                table: "ExtraPoints");

            migrationBuilder.DropColumn(
                name: "ReturnerId",
                table: "ExtraPoints");

            migrationBuilder.DropColumn(
                name: "DefensiveConversion",
                table: "Conversions");

            migrationBuilder.RenameTable(
                name: "Conversions",
                newName: "Conversion");

            migrationBuilder.RenameIndex(
                name: "IX_Conversions_RusherId",
                table: "Conversion",
                newName: "IX_Conversion_RusherId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversions_ReturnerId",
                table: "Conversion",
                newName: "IX_Conversion_ReturnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversions_ReceiverId",
                table: "Conversion",
                newName: "IX_Conversion_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversions_PlayId",
                table: "Conversion",
                newName: "IX_Conversion_PlayId");

            migrationBuilder.RenameIndex(
                name: "IX_Conversions_PasserId",
                table: "Conversion",
                newName: "IX_Conversion_PasserId");

            migrationBuilder.AlterColumn<int>(
                name: "FieldedAt",
                table: "Kickoffs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversion",
                table: "Conversion",
                column: "Id");

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
                name: "FK_Conversion_Plays_PlayId",
                table: "Conversion",
                column: "PlayId",
                principalTable: "Plays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
