using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260317181948 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5000,
                column: "Name",
                value: "stock-transaction.read");

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5010,
                column: "Name",
                value: "stock-transaction.create");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5000,
                column: "Name",
                value: "stock.read");

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5010,
                column: "Name",
                value: "stock.update");
        }
    }
}
