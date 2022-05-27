using Microsoft.EntityFrameworkCore.Migrations;

namespace Streamnote.Relational.Data.Migrations
{
    public partial class AddDarkModeOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DarkMode",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DarkMode",
                table: "AspNetUsers");
        }
    }
}
