using Microsoft.EntityFrameworkCore.Migrations;

namespace Streamnote.Relational.Data.Migrations
{
    public partial class AddCommentCountToItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentCount",
                table: "Items");
        }
    }
}
