using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sounds_New.Migrations
{
    /// <inheritdoc />
    public partial class AddTimestampsToCommentFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "DATETIME('now')",
                oldClrType: typeof(DateTime),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Comments",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValueSql: "DATETIME('now')");
        }
    }
}
