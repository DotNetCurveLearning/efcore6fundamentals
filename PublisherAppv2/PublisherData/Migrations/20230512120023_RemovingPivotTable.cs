﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PublisherData.Migrations
{
    /// <inheritdoc />
    public partial class RemovingPivotTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "ArtistId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "ArtistId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Artists",
                keyColumn: "ArtistId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Covers",
                keyColumn: "CoverId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Covers",
                keyColumn: "CoverId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Covers",
                keyColumn: "CoverId",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Artists",
                columns: new[] { "ArtistId", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, "Pablo", "Picasso" },
                    { 2, "Dee", "Bell" },
                    { 3, "Katharine", "Kuharic" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "AuthorId", "BasePrice", "PublishDate", "Title" },
                values: new object[,]
                {
                    { 1, 1, 0m, new DateTime(1989, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "In God's Ear" },
                    { 2, 2, 0m, new DateTime(2013, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "A tale for the time being" },
                    { 3, 3, 0m, new DateTime(1969, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The left hand of darkness" }
                });

            migrationBuilder.InsertData(
                table: "Covers",
                columns: new[] { "CoverId", "DesignIdeas", "DigitalOnly" },
                values: new object[,]
                {
                    { 1, "How about a left hand in the dark?", false },
                    { 2, "Should we put a clock?", true },
                    { 3, "A big ear in the clouds?", false }
                });
        }
    }
}