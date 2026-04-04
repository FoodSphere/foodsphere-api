using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class TestShadowPropertyFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Bill_BillId",
                table: "OrderItem",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Bill_BillId",
                table: "OrderItem");
        }
    }
}
