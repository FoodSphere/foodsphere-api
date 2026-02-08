using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Migrator.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class _20260208152052 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_BillId_OrderId",
                table: "OrderItem");

            migrationBuilder.AddColumn<short>(
                name: "Id",
                table: "OrderItem",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Branch",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem",
                columns: new[] { "BillId", "OrderId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderItem");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Branch",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem",
                columns: new[] { "BillId", "RestaurantId", "OrderId", "MenuId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_BillId_OrderId",
                table: "OrderItem",
                columns: new[] { "BillId", "OrderId" });
        }
    }
}
