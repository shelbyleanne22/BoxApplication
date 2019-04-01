using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class UpdatedActiveDirectory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ADFirstName",
                table: "ActiveDirectoryUsers");

            migrationBuilder.RenameColumn(
                name: "ADUsername",
                table: "ActiveDirectoryUsers",
                newName: "ADFullName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ADFullName",
                table: "ActiveDirectoryUsers",
                newName: "ADUsername");

            migrationBuilder.AddColumn<string>(
                name: "ADFirstName",
                table: "ActiveDirectoryUsers",
                nullable: true);
        }
    }
}
