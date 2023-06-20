using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PublisherData.Migrations
{
    /// <inheritdoc />
    public partial class addstoredproc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE PROCEDURE dbo.AuthorsPublishedInYearRange
                @yearstart int,@yearend int
                AS
                SELECT DISTINCT Authors.*
                FROM authors
                LEFT JOIN Books ON Authors.AuthorId=books.authorId
                WHERE Year(books.PublishDate)>=@yearstart AND Year(books.PublishDate)<=@yearend
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE dbo.AuthorsPublishedInYearRange");
        }
    }
}
