using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class UpdateAccountsToBox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxADUpdates_ActiveDirectoryUsers_ADGUID",
                table: "BoxADUpdates");

            migrationBuilder.DropIndex(
                name: "IX_BoxADUpdates_ADGUID",
                table: "BoxADUpdates");

            migrationBuilder.DropColumn(
                name: "ADGUID",
                table: "BoxADUpdates");

            migrationBuilder.AddColumn<string>(
                name: "BoxID",
                table: "BoxADUpdates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoxUserID",
                table: "BoxADUpdates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoxADUpdates_BoxUserID",
                table: "BoxADUpdates",
                column: "BoxUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxADUpdates_BoxUsers_BoxUserID",
                table: "BoxADUpdates",
                column: "BoxUserID",
                principalTable: "BoxUsers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoxADUpdates_BoxUsers_BoxUserID",
                table: "BoxADUpdates");

            migrationBuilder.DropIndex(
                name: "IX_BoxADUpdates_BoxUserID",
                table: "BoxADUpdates");

            migrationBuilder.DropColumn(
                name: "BoxID",
                table: "BoxADUpdates");

            migrationBuilder.DropColumn(
                name: "BoxUserID",
                table: "BoxADUpdates");

            migrationBuilder.AddColumn<byte[]>(
                name: "ADGUID",
                table: "BoxADUpdates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoxADUpdates_ADGUID",
                table: "BoxADUpdates",
                column: "ADGUID");

            migrationBuilder.AddForeignKey(
                name: "FK_BoxADUpdates_ActiveDirectoryUsers_ADGUID",
                table: "BoxADUpdates",
                column: "ADGUID",
                principalTable: "ActiveDirectoryUsers",
                principalColumn: "ADGUID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
