using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSphere.Npgsql.Helper.Migrations
{
    /// <inheritdoc />
    public partial class _20260324160847 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Tag",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(13)",
                oldMaxLength: 13);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Tag",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
