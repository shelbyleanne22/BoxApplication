using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class revision1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Action_ActiveDirectoryUser_ApplicationActionADForeignKey",
                table: "Action");

            migrationBuilder.DropIndex(
                name: "IX_Action_ApplicationActionADForeignKey",
                table: "Action");

            migrationBuilder.DropColumn(
                name: "ApplicationActionADForeignKey",
                table: "Action");

            migrationBuilder.DropColumn(
                name: "ApplicationActionDescription",
                table: "Action");

            migrationBuilder.RenameColumn(
                name: "BoxSpaceUsed",
                table: "BoxUsersList",
                newName: "SpaceUsed");

            migrationBuilder.RenameColumn(
                name: "BoxName",
                table: "BoxUsersList",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "BoxLogin",
                table: "BoxUsersList",
                newName: "Login");

            migrationBuilder.RenameColumn(
                name: "BoxDateModified",
                table: "BoxUsersList",
                newName: "DateModified");

            migrationBuilder.RenameColumn(
                name: "BoxDateCreated",
                table: "BoxUsersList",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "BoxID",
                table: "BoxUsersList",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "ApplicationActionType",
                table: "Action",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "ApplicationActionObjectModified",
                table: "Action",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ApplicationActionDate",
                table: "Action",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "ApplicationActionID",
                table: "Action",
                newName: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpaceUsed",
                table: "BoxUsersList",
                newName: "BoxSpaceUsed");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "BoxUsersList",
                newName: "BoxName");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "BoxUsersList",
                newName: "BoxLogin");

            migrationBuilder.RenameColumn(
                name: "DateModified",
                table: "BoxUsersList",
                newName: "BoxDateModified");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "BoxUsersList",
                newName: "BoxDateCreated");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "BoxUsersList",
                newName: "BoxID");

            migrationBuilder.RenameColumn(
                name: "User",
                table: "Action",
                newName: "ApplicationActionType");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Action",
                newName: "ApplicationActionObjectModified");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Action",
                newName: "ApplicationActionDate");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Action",
                newName: "ApplicationActionID");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationActionADForeignKey",
                table: "Action",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationActionDescription",
                table: "Action",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Action_ApplicationActionADForeignKey",
                table: "Action",
                column: "ApplicationActionADForeignKey");

            migrationBuilder.AddForeignKey(
                name: "FK_Action_ActiveDirectoryUser_ApplicationActionADForeignKey",
                table: "Action",
                column: "ApplicationActionADForeignKey",
                principalTable: "ActiveDirectoryUser",
                principalColumn: "ADEmail",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
