using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class ChangedADProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ADDateInactive",
                table: "ActiveDirectoryUser",
                newName: "ADDateModified");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ADDateModified",
                table: "ActiveDirectoryUser",
                newName: "ADDateInactive");
        }
    }
}
