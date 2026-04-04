using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260315214135 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bill_WorkerUser_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Bill");

            migrationBuilder.DropForeignKey(
                name: "FK_BillMember_Bill_BillId",
                table: "BillMember");

            migrationBuilder.DropForeignKey(
                name: "FK_BillMember_ConsumerUser_ConsumerId",
                table: "BillMember");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Bill_BillId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_WorkerUser_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Bill_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "IssuerBranchId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IssuerRestaurantId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IssuerBranchId",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "IssuerRestaurantId",
                table: "Bill");

            migrationBuilder.AddForeignKey(
                name: "FK_BillMember_Bill_BillId",
                table: "BillMember",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BillMember_ConsumerUser_ConsumerId",
                table: "BillMember",
                column: "ConsumerId",
                principalTable: "ConsumerUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Bill_BillId",
                table: "Order",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillMember_Bill_BillId",
                table: "BillMember");

            migrationBuilder.DropForeignKey(
                name: "FK_BillMember_ConsumerUser_ConsumerId",
                table: "BillMember");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Bill_BillId",
                table: "Order");

            migrationBuilder.AddColumn<short>(
                name: "IssuerBranchId",
                table: "Order",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IssuerRestaurantId",
                table: "Order",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "IssuerBranchId",
                table: "Bill",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IssuerRestaurantId",
                table: "Bill",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Order",
                columns: new[] { "IssuerRestaurantId", "IssuerBranchId", "IssuerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Bill_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Bill",
                columns: new[] { "IssuerRestaurantId", "IssuerBranchId", "IssuerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Bill_WorkerUser_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Bill",
                columns: new[] { "IssuerRestaurantId", "IssuerBranchId", "IssuerId" },
                principalTable: "WorkerUser",
                principalColumns: new[] { "RestaurantId", "BranchId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_BillMember_Bill_BillId",
                table: "BillMember",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BillMember_ConsumerUser_ConsumerId",
                table: "BillMember",
                column: "ConsumerId",
                principalTable: "ConsumerUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Bill_BillId",
                table: "Order",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_WorkerUser_IssuerRestaurantId_IssuerBranchId_IssuerId",
                table: "Order",
                columns: new[] { "IssuerRestaurantId", "IssuerBranchId", "IssuerId" },
                principalTable: "WorkerUser",
                principalColumns: new[] { "RestaurantId", "BranchId", "Id" });
        }
    }
}
