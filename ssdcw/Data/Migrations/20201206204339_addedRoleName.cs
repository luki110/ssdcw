using Microsoft.EntityFrameworkCore.Migrations;

namespace ssdcw.Data.Migrations
{
    public partial class addedRoleName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rolename",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rolename",
                table: "AspNetUsers");
        }
    }
}
