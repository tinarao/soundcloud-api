using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sounds_New.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedLikeSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Like_Tracks_TrackId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Users_TrackId",
                table: "Like");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Like",
                table: "Like");

            migrationBuilder.RenameTable(
                name: "Like",
                newName: "Likes");

            migrationBuilder.RenameIndex(
                name: "IX_Like_UserId",
                table: "Likes",
                newName: "IX_Likes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Like_TrackId_UserId",
                table: "Likes",
                newName: "IX_Likes_TrackId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Like_TrackId",
                table: "Likes",
                newName: "IX_Likes_TrackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Tracks_TrackId",
                table: "Likes",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_TrackId",
                table: "Likes",
                column: "TrackId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Tracks_TrackId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_TrackId",
                table: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.RenameTable(
                name: "Likes",
                newName: "Like");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_UserId",
                table: "Like",
                newName: "IX_Like_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_TrackId_UserId",
                table: "Like",
                newName: "IX_Like_TrackId_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_TrackId",
                table: "Like",
                newName: "IX_Like_TrackId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Like",
                table: "Like",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Tracks_TrackId",
                table: "Like",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Users_TrackId",
                table: "Like",
                column: "TrackId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
