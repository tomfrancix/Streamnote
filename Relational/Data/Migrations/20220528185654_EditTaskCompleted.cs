using Microsoft.EntityFrameworkCore.Migrations;

namespace Streamnote.Relational.Data.Migrations
{
    public partial class EditTaskCompleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Steps");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Steps",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Steps");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Steps",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
