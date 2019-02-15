using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class InitialRedone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "BoxUser",
                columns: table => new
                {
                    BoxID = table.Column<string>(nullable: false),
                    BoxADForeignKey = table.Column<string>(nullable: true),
                    BoxName = table.Column<string>(nullable: true),
                    BoxLogin = table.Column<string>(nullable: true),
                    BoxSpaceUsed = table.Column<int>(nullable: false),
                    BoxStatus = table.Column<string>(nullable: true),
                    BoxDateCreated = table.Column<DateTime>(nullable: false),
                    BoxDateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxUser", x => x.BoxID);
                    table.ForeignKey(
                        name: "FK_BoxUser_ActiveDirectoryUser_BoxADForeignKey",
                        column: x => x.BoxADForeignKey,
                        principalTable: "ActiveDirectoryUser",
                        principalColumn: "ADEmail",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_ApplicationActionADForeignKey",
                table: "Action",
                column: "ApplicationActionADForeignKey");

            migrationBuilder.CreateIndex(
                name: "IX_BoxUser_BoxADForeignKey",
                table: "BoxUser",
                column: "BoxADForeignKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "BoxUser");
        }
    }
}
