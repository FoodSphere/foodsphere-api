using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260324115541 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientTag");

            migrationBuilder.DropTable(
                name: "MenuTag");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Tag",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TagIngredient",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<short>(type: "smallint", nullable: false),
                    TagId = table.Column<short>(type: "smallint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagIngredient", x => new { x.RestaurantId, x.IngredientId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagIngredient_Ingredient_RestaurantId_IngredientId",
                        columns: x => new { x.RestaurantId, x.IngredientId },
                        principalTable: "Ingredient",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagIngredient_Tag_RestaurantId_TagId",
                        columns: x => new { x.RestaurantId, x.TagId },
                        principalTable: "Tag",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TagMenu",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuId = table.Column<short>(type: "smallint", nullable: false),
                    TagId = table.Column<short>(type: "smallint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagMenu", x => new { x.RestaurantId, x.MenuId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagMenu_Menu_RestaurantId_MenuId",
                        columns: x => new { x.RestaurantId, x.MenuId },
                        principalTable: "Menu",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagMenu_Tag_RestaurantId_TagId",
                        columns: x => new { x.RestaurantId, x.TagId },
                        principalTable: "Tag",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagIngredient_RestaurantId_TagId",
                table: "TagIngredient",
                columns: new[] { "RestaurantId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_TagMenu_RestaurantId_TagId",
                table: "TagMenu",
                columns: new[] { "RestaurantId", "TagId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagIngredient");

            migrationBuilder.DropTable(
                name: "TagMenu");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tag");

            migrationBuilder.CreateTable(
                name: "IngredientTag",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientId = table.Column<short>(type: "smallint", nullable: false),
                    TagId = table.Column<short>(type: "smallint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientTag", x => new { x.RestaurantId, x.IngredientId, x.TagId });
                    table.ForeignKey(
                        name: "FK_IngredientTag_Ingredient_RestaurantId_IngredientId",
                        columns: x => new { x.RestaurantId, x.IngredientId },
                        principalTable: "Ingredient",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientTag_Tag_RestaurantId_TagId",
                        columns: x => new { x.RestaurantId, x.TagId },
                        principalTable: "Tag",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuTag",
                columns: table => new
                {
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuId = table.Column<short>(type: "smallint", nullable: false),
                    TagId = table.Column<short>(type: "smallint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuTag", x => new { x.RestaurantId, x.MenuId, x.TagId });
                    table.ForeignKey(
                        name: "FK_MenuTag_Menu_RestaurantId_MenuId",
                        columns: x => new { x.RestaurantId, x.MenuId },
                        principalTable: "Menu",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuTag_Tag_RestaurantId_TagId",
                        columns: x => new { x.RestaurantId, x.TagId },
                        principalTable: "Tag",
                        principalColumns: new[] { "RestaurantId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientTag_RestaurantId_TagId",
                table: "IngredientTag",
                columns: new[] { "RestaurantId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_MenuTag_RestaurantId_TagId",
                table: "MenuTag",
                columns: new[] { "RestaurantId", "TagId" });
        }
    }
}
