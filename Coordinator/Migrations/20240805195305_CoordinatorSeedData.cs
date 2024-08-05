using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class CoordinatorSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("2377f000-4be8-4718-bd24-94d282657cd5"), "Stock.API" },
                    { new Guid("ead83734-fae7-4c88-b117-6d59c537eea7"), "Order.API" },
                    { new Guid("ffde8fd5-a23b-4ef5-a40c-997eb33055de"), "Payment.API" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("2377f000-4be8-4718-bd24-94d282657cd5"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("ead83734-fae7-4c88-b117-6d59c537eea7"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("ffde8fd5-a23b-4ef5-a40c-997eb33055de"));
        }
    }
}
