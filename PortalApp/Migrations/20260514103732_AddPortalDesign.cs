using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPortalDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedBy",
                table: "Articles");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Articles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Articles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ApplicationUserId",
                table: "Articles",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ApprovedBy",
                table: "Articles",
                column: "ApprovedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_ApplicationUserId",
                table: "Articles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_ApprovedBy",
                table: "Articles",
                column: "ApprovedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedBy",
                table: "Articles",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_ApplicationUserId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_ApprovedBy",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedBy",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ApplicationUserId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ApprovedBy",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Articles");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedBy",
                table: "Articles",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
