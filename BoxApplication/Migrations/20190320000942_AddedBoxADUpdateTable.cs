using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class AddedBoxADUpdateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveDirectoryUsers",
                columns: table => new
                {
                    ADEmail = table.Column<string>(nullable: false),
                    ADUsername = table.Column<string>(nullable: true),
                    ADFirstName = table.Column<string>(nullable: true),
                    ADStatus = table.Column<string>(nullable: true),
                    ADDateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveDirectoryUsers", x => x.ADEmail);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationActions",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    User = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationActions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BoxUsers",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true),
                    SpaceUsed = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxUsers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BoxADUpdates",
                columns: table => new
                {
                    BoxADUpdateID = table.Column<Guid>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    UpdateBoxOption = table.Column<bool>(nullable: false),
                    ADUserADEmail = table.Column<string>(nullable: true),
                    ADFieldChanged = table.Column<string>(nullable: true),
                    BoxPreviousData = table.Column<string>(nullable: true),
                    ADNewData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxADUpdates", x => x.BoxADUpdateID);
                    table.ForeignKey(
                        name: "FK_BoxADUpdates_ActiveDirectoryUsers_ADUserADEmail",
                        column: x => x.ADUserADEmail,
                        principalTable: "ActiveDirectoryUsers",
                        principalColumn: "ADEmail",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoxADUpdates_ADUserADEmail",
                table: "BoxADUpdates",
                column: "ADUserADEmail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationActions");

            migrationBuilder.DropTable(
                name: "BoxADUpdates");

            migrationBuilder.DropTable(
                name: "BoxUsers");

            migrationBuilder.DropTable(
                name: "ActiveDirectoryUsers");
        }
    }
}
