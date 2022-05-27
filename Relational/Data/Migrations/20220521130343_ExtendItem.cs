using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Streamnote.Relational.Data.Migrations
{
    public partial class ExtendItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Items",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Items",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Items");
        }
    }
}
