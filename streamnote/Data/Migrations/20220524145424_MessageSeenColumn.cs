using Microsoft.EntityFrameworkCore.Migrations;

namespace streamnote.Data.Migrations
{
    public partial class MessageSeenColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MessageSeen",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageSeen",
                table: "Messages");
        }
    }
}
