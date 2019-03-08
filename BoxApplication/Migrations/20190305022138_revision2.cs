using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class revision2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BoxUsersList",
                table: "BoxUsersList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActiveDirectoryUser",
                table: "ActiveDirectoryUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Action",
                table: "Action");

            migrationBuilder.RenameTable(
                name: "BoxUsersList",
                newName: "BoxUsers");

            migrationBuilder.RenameTable(
                name: "ActiveDirectoryUser",
                newName: "ActiveDirectoryUsers");

            migrationBuilder.RenameTable(
                name: "Action",
                newName: "ApplicationActions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoxUsers",
                table: "BoxUsers",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActiveDirectoryUsers",
                table: "ActiveDirectoryUsers",
                column: "ADEmail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationActions",
                table: "ApplicationActions",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BoxUsers",
                table: "BoxUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationActions",
                table: "ApplicationActions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActiveDirectoryUsers",
                table: "ActiveDirectoryUsers");

            migrationBuilder.RenameTable(
                name: "BoxUsers",
                newName: "BoxUsersList");

            migrationBuilder.RenameTable(
                name: "ApplicationActions",
                newName: "Action");

            migrationBuilder.RenameTable(
                name: "ActiveDirectoryUsers",
                newName: "ActiveDirectoryUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoxUsersList",
                table: "BoxUsersList",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Action",
                table: "Action",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActiveDirectoryUser",
                table: "ActiveDirectoryUser",
                column: "ADEmail");
        }
    }
}
