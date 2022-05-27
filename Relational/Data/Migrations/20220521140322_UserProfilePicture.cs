using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Streamnote.Relational.Data.Migrations
{
    public partial class UserProfilePicture : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePicture",
                table: "AspNetUsers",
                newName: "ImageContentType");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ImageContentType",
                table: "AspNetUsers",
                newName: "ProfilePicture");
        }
    }
}
