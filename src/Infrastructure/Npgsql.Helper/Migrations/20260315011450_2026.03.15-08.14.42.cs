using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260315081442 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.CreateTable(
                name: "StockTransaction",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IngredientId = table.Column<short>(type: "smallint", nullable: false),
                    BillId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderId = table.Column<short>(type: "smallint", nullable: true),
                    OrderItemId = table.Column<short>(type: "smallint", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    BranchRestaurantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransaction", x => new { x.RestaurantId, x.BranchId, x.Id });
                    table.ForeignKey(
                        name: "FK_StockTransaction_Branch_BranchRestaurantId_BranchId",
                        columns: x => new { x.BranchRestaurantId, x.BranchId },
                        principalTable: "Branch",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTransaction_Ingredient_RestaurantId_IngredientId",
                        columns: x => new { x.RestaurantId, x.IngredientId },
                        principalTable: "Ingredient",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTransaction_OrderItem_BillId_OrderId_OrderItemId",
                        columns: x => new { x.BillId, x.OrderId, x.OrderItemId },
                        principalTable: "OrderItem",
                        principalColumns: new[] { "BillId", "OrderId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransaction_BillId_OrderId_OrderItemId",
                table: "StockTransaction",
                columns: new[] { "BillId", "OrderId", "OrderItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransaction_BranchRestaurantId_BranchId",
                table: "StockTransaction",
                columns: new[] { "BranchRestaurantId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransaction_RestaurantId_IngredientId",
                table: "StockTransaction",
                columns: new[] { "RestaurantId", "IngredientId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockTransaction");

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    IngredientId = table.Column<short>(type: "smallint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,4)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => new { x.RestaurantId, x.BranchId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_Stock_Branch_RestaurantId_BranchId",
                        columns: x => new { x.RestaurantId, x.BranchId },
                        principalTable: "Branch",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stock_Ingredient_RestaurantId_IngredientId",
                        columns: x => new { x.RestaurantId, x.IngredientId },
                        principalTable: "Ingredient",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stock_RestaurantId_IngredientId",
                table: "Stock",
                columns: new[] { "RestaurantId", "IngredientId" });
        }
    }
}
