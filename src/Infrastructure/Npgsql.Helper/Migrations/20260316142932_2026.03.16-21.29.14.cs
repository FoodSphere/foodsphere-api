using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260316212914 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_ConsumerUser_ConsumerId",
                table: "Bill");

            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Restaurant_RestaurantId",
                table: "Branch");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Restaurant_RestaurantId",
                table: "Ingredient");

            migrationBuilder.DropForeignKey(
                name: "FK_Menu_Restaurant_RestaurantId",
                table: "Menu");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuComponent_Restaurant_RestaurantId",
                table: "MenuComponent");

            migrationBuilder.DropForeignKey(
                name: "FK_Queuing_ConsumerUser_ConsumerId",
                table: "Queuing");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission");

            migrationBuilder.AddColumn<Guid>(
                name: "ConsumerUserId",
                table: "Bill",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bill_ConsumerUserId",
                table: "Bill",
                column: "ConsumerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_ConsumerUser_ConsumerId",
                table: "Bill",
                column: "ConsumerId",
                principalTable: "ConsumerUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_ConsumerUser_ConsumerUserId",
                table: "Bill",
                column: "ConsumerUserId",
                principalTable: "ConsumerUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Restaurant_RestaurantId",
                table: "Branch",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Restaurant_RestaurantId",
                table: "Ingredient",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Restaurant_RestaurantId",
                table: "Menu",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuComponent_Restaurant_RestaurantId",
                table: "MenuComponent",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Queuing_ConsumerUser_ConsumerId",
                table: "Queuing",
                column: "ConsumerId",
                principalTable: "ConsumerUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_ConsumerUser_ConsumerId",
                table: "Bill");

            migrationBuilder.DropForeignKey(
                name: "FK_Bill_ConsumerUser_ConsumerUserId",
                table: "Bill");

            migrationBuilder.DropForeignKey(
                name: "FK_Branch_Restaurant_RestaurantId",
                table: "Branch");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Restaurant_RestaurantId",
                table: "Ingredient");

            migrationBuilder.DropForeignKey(
                name: "FK_Menu_Restaurant_RestaurantId",
                table: "Menu");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuComponent_Restaurant_RestaurantId",
                table: "MenuComponent");

            migrationBuilder.DropForeignKey(
                name: "FK_Queuing_ConsumerUser_ConsumerId",
                table: "Queuing");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission");

            migrationBuilder.DropIndex(
                name: "IX_Bill_ConsumerUserId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "ConsumerUserId",
                table: "Bill");

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_ConsumerUser_ConsumerId",
                table: "Bill",
                column: "ConsumerId",
                principalTable: "ConsumerUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Branch_Restaurant_RestaurantId",
                table: "Branch",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Restaurant_RestaurantId",
                table: "Ingredient",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Restaurant_RestaurantId",
                table: "Menu",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuComponent_Restaurant_RestaurantId",
                table: "MenuComponent",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Queuing_ConsumerUser_ConsumerId",
                table: "Queuing",
                column: "ConsumerId",
                principalTable: "ConsumerUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermission_Permission_PermissionId",
                table: "RolePermission",
                column: "PermissionId",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
