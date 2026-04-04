using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260315094548 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTransaction_Branch_BranchRestaurantId_BranchId",
                table: "StockTransaction");

            migrationBuilder.DropIndex(
                name: "IX_StockTransaction_BranchRestaurantId_BranchId",
                table: "StockTransaction");

            migrationBuilder.DropColumn(
                name: "BranchRestaurantId",
                table: "StockTransaction");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransaction_Branch_RestaurantId_BranchId",
                table: "StockTransaction",
                columns: new[] { "RestaurantId", "BranchId" },
                principalTable: "Branch",
                principalColumns: new[] { "RestaurantId", "Id" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTransaction_Branch_RestaurantId_BranchId",
                table: "StockTransaction");

            migrationBuilder.AddColumn<Guid>(
                name: "BranchRestaurantId",
                table: "StockTransaction",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_StockTransaction_BranchRestaurantId_BranchId",
                table: "StockTransaction",
                columns: new[] { "BranchRestaurantId", "BranchId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransaction_Branch_BranchRestaurantId_BranchId",
                table: "StockTransaction",
                columns: new[] { "BranchRestaurantId", "BranchId" },
                principalTable: "Branch",
                principalColumns: new[] { "RestaurantId", "Id" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
