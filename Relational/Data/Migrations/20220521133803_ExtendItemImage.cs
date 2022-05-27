using Microsoft.EntityFrameworkCore.Migrations;

namespace Streamnote.Relational.Data.Migrations
{
    public partial class ExtendItemImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "Items");
        }
    }
}
