using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260323221515 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRole_Role_RestaurantId_RoleId",
                table: "WorkerRole");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRole_WorkerUser_RestaurantId_BranchId_WorkerId",
                table: "WorkerRole");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRole_Role_RestaurantId_RoleId",
                table: "WorkerRole",
                columns: new[] { "RestaurantId", "RoleId" },
                principalTable: "Role",
                principalColumns: new[] { "RestaurantId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRole_WorkerUser_RestaurantId_BranchId_WorkerId",
                table: "WorkerRole",
                columns: new[] { "RestaurantId", "BranchId", "WorkerId" },
                principalTable: "WorkerUser",
                principalColumns: new[] { "RestaurantId", "BranchId", "Id" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRole_Role_RestaurantId_RoleId",
                table: "WorkerRole");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkerRole_WorkerUser_RestaurantId_BranchId_WorkerId",
                table: "WorkerRole");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRole_Role_RestaurantId_RoleId",
                table: "WorkerRole",
                columns: new[] { "RestaurantId", "RoleId" },
                principalTable: "Role",
                principalColumns: new[] { "RestaurantId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkerRole_WorkerUser_RestaurantId_BranchId_WorkerId",
                table: "WorkerRole",
                columns: new[] { "RestaurantId", "BranchId", "WorkerId" },
                principalTable: "WorkerUser",
                principalColumns: new[] { "RestaurantId", "BranchId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
