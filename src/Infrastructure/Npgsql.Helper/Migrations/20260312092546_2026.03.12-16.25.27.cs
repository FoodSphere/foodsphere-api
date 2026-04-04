using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260312162527 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "WorkerRole");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "WorkerPortal");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "RolePermission");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "RestaurantStaffRole");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "Queuing");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "OrderingPortal");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "MenuTag");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "MenuIngredient");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "MenuComponent");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "IngredientTag");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "BranchStaffRole");

            migrationBuilder.DropColumn(
                name: "DeleteTime",
                table: "BillMember");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "WorkerUser",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "WorkerRole",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "WorkerPortal",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Tag",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Table",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Stock",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "RolePermission",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Role",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "RestaurantStaffRole",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "RestaurantStaff",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "RestaurantStaff",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Restaurant",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Restaurant",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Queuing",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Permission",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "OrderItem",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "OrderingPortal",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Order",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "MenuTag",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "MenuIngredient",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "MenuComponent",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Menu",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "IngredientTag",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Ingredient",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Coupon",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "ConsumerUser",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "BranchStaffRole",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "BranchStaff",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "BranchStaff",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Branch",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "BillMember",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Bill",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1000,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1010,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 2000,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 2010,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 2020,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3000,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3010,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 4000,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 4010,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5000,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5010,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 6000,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 6010,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7000,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7010,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7020,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7030,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 8000,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9000,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9010,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9020,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9030,
                column: "CreateTime",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "WorkerUser");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "WorkerRole");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "WorkerPortal");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Table");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Stock");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "RolePermission");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "RestaurantStaffRole");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "RestaurantStaff");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "RestaurantStaff");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Queuing");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "OrderingPortal");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "MenuTag");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "MenuIngredient");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "MenuComponent");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Menu");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "IngredientTag");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "ConsumerUser");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "BranchStaffRole");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "BranchStaff");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "BranchStaff");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Branch");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "BillMember");

            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Bill");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "WorkerRole",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "WorkerPortal",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "Tag",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "Stock",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "RolePermission",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "Role",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "RestaurantStaffRole",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "Queuing",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "OrderingPortal",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "MenuTag",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "MenuIngredient",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "MenuComponent",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "IngredientTag",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "BranchStaffRole",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTime",
                table: "BillMember",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
