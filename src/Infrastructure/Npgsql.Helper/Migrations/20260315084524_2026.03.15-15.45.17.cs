using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260315154517 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "Bill");

            migrationBuilder.RenameColumn(
                name: "CheckoutSessionId",
                table: "Payment",
                newName: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Payment",
                newName: "CheckoutSessionId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "Bill",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
