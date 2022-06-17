using Microsoft.EntityFrameworkCore.Migrations;

namespace Streamnote.Relational.Data.Migrations
{
    public partial class moveimagesdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
DECLARE @MyCursor CURSOR;
DECLARE 
    @Id AS INT,
    @Image AS VARBINARY,
    @ImageContentType AS VARCHAR
BEGIN
    SET @MyCursor = CURSOR FOR
    select Id, Image, ImageContentType from dbo.Items

    OPEN @MyCursor 
    FETCH NEXT FROM @MyCursor 
    INTO @Id, @Image, @ImageContentType

    WHILE @@FETCH_STATUS = 0
    BEGIN

      INSERT INTO Images (Created, Modified, Name, Bytes, ImageContentType, ItemId)
      VALUES (getdate(), getdate(), CONCAT('IMG_', @Id), @Image, @ImageContentType, @Id);

      FETCH NEXT FROM @MyCursor 
      INTO @Id, @Image, @ImageContentType
    END; 

    CLOSE @MyCursor;
    DEALLOCATE @MyCursor;
END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
