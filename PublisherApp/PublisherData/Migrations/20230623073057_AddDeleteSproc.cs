using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublisherData.Migrations;

/// <inheritdoc />
public partial class AddDeleteSproc : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            @"CREATE PROCEDURE DeleteCover
                @coverId int
              AS
              DELETE FROM covers WHERE CoverId = @coverId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE DeleteCover");
    }
}
