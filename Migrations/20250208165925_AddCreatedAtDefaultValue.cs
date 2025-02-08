using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sounds_New.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "DATETIME('now')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Slug",
                table: "Users",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Slug",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "DATETIME('now')");
        }
    }
}
