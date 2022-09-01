using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PablitoJere.Migrations
{
    public partial class FixComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUser",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IdentityUserId",
                table: "Comments",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_IdentityUserId",
                table: "Comments",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_IdentityUserId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_IdentityUserId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "IdentityUser",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
