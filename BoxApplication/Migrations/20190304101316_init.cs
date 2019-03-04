using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveDirectoryUser",
                columns: table => new
                {
                    ADEmail = table.Column<string>(nullable: false),
                    ADUsername = table.Column<string>(nullable: true),
                    ADFirstName = table.Column<string>(nullable: true),
                    ADStatus = table.Column<string>(nullable: true),
                    ADDateInactive = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveDirectoryUser", x => x.ADEmail);
                });

            migrationBuilder.CreateTable(
                name: "BoxUsers",
                columns: table => new
                {
                    BoxID = table.Column<string>(nullable: false),
                    BoxName = table.Column<string>(nullable: true),
                    BoxLogin = table.Column<string>(nullable: true),
                    BoxSpaceUsed = table.Column<long>(nullable: false),
                    BoxDateCreated = table.Column<DateTime>(nullable: false),
                    BoxDateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxUsers", x => x.BoxID);
                });

            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    ApplicationActionID = table.Column<Guid>(nullable: false),
                    ApplicationActionADForeignKey = table.Column<string>(nullable: true),
                    ApplicationActionType = table.Column<string>(nullable: true),
                    ApplicationActionDescription = table.Column<string>(nullable: true),
                    ApplicationActionObjectModified = table.Column<string>(nullable: true),
                    ApplicationActionDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.ApplicationActionID);
                    table.ForeignKey(
                        name: "FK_Action_ActiveDirectoryUser_ApplicationActionADForeignKey",
                        column: x => x.ApplicationActionADForeignKey,
                        principalTable: "ActiveDirectoryUser",
                        principalColumn: "ADEmail",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_ApplicationActionADForeignKey",
                table: "Action",
                column: "ApplicationActionADForeignKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "BoxUsers");

            migrationBuilder.DropTable(
                name: "ActiveDirectoryUser");
        }
    }
}
