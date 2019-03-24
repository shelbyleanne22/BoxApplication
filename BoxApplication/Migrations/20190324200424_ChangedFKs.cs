using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class ChangedFKs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxADUpdates_ActiveDirectoryUsers_ADUserADGUID",
                table: "BoxADUpdates");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "BoxADUpdates");

            migrationBuilder.RenameColumn(
                name: "ADUserADGUID",
                table: "BoxADUpdates",
                newName: "ADGUID");

            migrationBuilder.RenameIndex(
                name: "IX_BoxADUpdates_ADUserADGUID",
                table: "BoxADUpdates",
                newName: "IX_BoxADUpdates_ADGUID");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxADUpdates_ActiveDirectoryUsers_ADGUID",
                table: "BoxADUpdates",
                column: "ADGUID",
                principalTable: "ActiveDirectoryUsers",
                principalColumn: "ADGUID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxADUpdates_ActiveDirectoryUsers_ADGUID",
                table: "BoxADUpdates");

            migrationBuilder.RenameColumn(
                name: "ADGUID",
                table: "BoxADUpdates",
                newName: "ADUserADGUID");

            migrationBuilder.RenameIndex(
                name: "IX_BoxADUpdates_ADGUID",
                table: "BoxADUpdates",
                newName: "IX_BoxADUpdates_ADUserADGUID");

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "BoxADUpdates",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_BoxADUpdates_ActiveDirectoryUsers_ADUserADGUID",
                table: "BoxADUpdates",
                column: "ADUserADGUID",
                principalTable: "ActiveDirectoryUsers",
                principalColumn: "ADGUID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
