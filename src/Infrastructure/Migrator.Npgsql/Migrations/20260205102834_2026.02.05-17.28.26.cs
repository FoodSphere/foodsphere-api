using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Migrator.Npgsql.Migrations
{
    /// <inheritdoc />
    public partial class _20260205172826 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerRole");

            migrationBuilder.DropTable(
                name: "Manager");

            migrationBuilder.CreateTable(
                name: "BranchManager",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    MasterId = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchManager", x => new { x.RestaurantId, x.BranchId, x.MasterId });
                    table.ForeignKey(
                        name: "FK_BranchManager_AspNetUsers_MasterId",
                        column: x => x.MasterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchManager_Branch_RestaurantId_BranchId",
                        columns: x => new { x.RestaurantId, x.BranchId },
                        principalTable: "Branch",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantManager",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MasterId = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantManager", x => new { x.RestaurantId, x.MasterId });
                    table.ForeignKey(
                        name: "FK_RestaurantManager_AspNetUsers_MasterId",
                        column: x => x.MasterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestaurantManager_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchManagerRole",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    ManagerId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<short>(type: "smallint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchManagerRole", x => new { x.RestaurantId, x.BranchId, x.ManagerId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_BranchManagerRole_BranchManager_RestaurantId_BranchId_Manag~",
                        columns: x => new { x.RestaurantId, x.BranchId, x.ManagerId },
                        principalTable: "BranchManager",
                        principalColumns: new[] { "RestaurantId", "BranchId", "MasterId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchManagerRole_Role_RestaurantId_RoleId",
                        columns: x => new { x.RestaurantId, x.RoleId },
                        principalTable: "Role",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantManagerRole",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagerId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<short>(type: "smallint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantManagerRole", x => new { x.RestaurantId, x.ManagerId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RestaurantManagerRole_RestaurantManager_RestaurantId_Manage~",
                        columns: x => new { x.RestaurantId, x.ManagerId },
                        principalTable: "RestaurantManager",
                        principalColumns: new[] { "RestaurantId", "MasterId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RestaurantManagerRole_Role_RestaurantId_RoleId",
                        columns: x => new { x.RestaurantId, x.RoleId },
                        principalTable: "Role",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchManager_MasterId",
                table: "BranchManager",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchManagerRole_RestaurantId_RoleId",
                table: "BranchManagerRole",
                columns: new[] { "RestaurantId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantManager_MasterId",
                table: "RestaurantManager",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantManagerRole_RestaurantId_RoleId",
                table: "RestaurantManagerRole",
                columns: new[] { "RestaurantId", "RoleId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchManagerRole");

            migrationBuilder.DropTable(
                name: "RestaurantManagerRole");

            migrationBuilder.DropTable(
                name: "BranchManager");

            migrationBuilder.DropTable(
                name: "RestaurantManager");

            migrationBuilder.CreateTable(
                name: "Manager",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    MasterId = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manager", x => new { x.RestaurantId, x.BranchId, x.MasterId });
                    table.ForeignKey(
                        name: "FK_Manager_AspNetUsers_MasterId",
                        column: x => x.MasterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Manager_Branch_RestaurantId_BranchId",
                        columns: x => new { x.RestaurantId, x.BranchId },
                        principalTable: "Branch",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ManagerRole",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<short>(type: "smallint", nullable: false),
                    ManagerId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<short>(type: "smallint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerRole", x => new { x.RestaurantId, x.BranchId, x.ManagerId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_ManagerRole_Manager_RestaurantId_BranchId_ManagerId",
                        columns: x => new { x.RestaurantId, x.BranchId, x.ManagerId },
                        principalTable: "Manager",
                        principalColumns: new[] { "RestaurantId", "BranchId", "MasterId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManagerRole_Role_RestaurantId_RoleId",
                        columns: x => new { x.RestaurantId, x.RoleId },
                        principalTable: "Role",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Manager_MasterId",
                table: "Manager",
                column: "MasterId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagerRole_RestaurantId_RoleId",
                table: "ManagerRole",
                columns: new[] { "RestaurantId", "RoleId" });
        }
    }
}
