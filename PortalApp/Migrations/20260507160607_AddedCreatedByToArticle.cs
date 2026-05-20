using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedCreatedByToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Articles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CreatedBy",
                table: "Articles",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedBy",
                table: "Articles",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_CreatedBy",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_CreatedBy",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Articles");
        }
    }
}
