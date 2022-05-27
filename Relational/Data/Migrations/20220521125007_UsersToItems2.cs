using Microsoft.EntityFrameworkCore.Migrations;

namespace Streamnote.Relational.Data.Migrations
{
    public partial class UsersToItems2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_AspNetUsers_UserId1",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_UserId1",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Items",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserId",
                table: "Items",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_AspNetUsers_UserId",
                table: "Items",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_AspNetUsers_UserId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_UserId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Items",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_UserId1",
                table: "Items",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_AspNetUsers_UserId1",
                table: "Items",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
