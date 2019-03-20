using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class test1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveDirectoryUsers",
                columns: table => new
                {
                    ADGUID = table.Column<byte[]>(nullable: false),
                    ADEmail = table.Column<string>(nullable: true),
                    ADUsername = table.Column<string>(nullable: true),
                    ADFirstName = table.Column<string>(nullable: true),
                    ADStatus = table.Column<string>(nullable: true),
                    ADDateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveDirectoryUsers", x => x.ADGUID);
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
                name: "BoxADUpdates",
                columns: table => new
                {
                    BoxADUpdateID = table.Column<Guid>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    UpdateBoxOption = table.Column<bool>(nullable: false),
                    ADUserADGUID = table.Column<byte[]>(nullable: true),
                    ADFieldChanged = table.Column<string>(nullable: true),
                    BoxPreviousData = table.Column<string>(nullable: true),
                    ADNewData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxADUpdates", x => x.BoxADUpdateID);
                    table.ForeignKey(
                        name: "FK_BoxADUpdates_ActiveDirectoryUsers_ADUserADGUID",
                        column: x => x.ADUserADGUID,
                        principalTable: "ActiveDirectoryUsers",
                        principalColumn: "ADGUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BoxUsers",
                columns: table => new
                {
                    ID = table.Column<string>(nullable: false),
                    ADGUID = table.Column<byte[]>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true),
                    SpaceUsed = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoxUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BoxUsers_ActiveDirectoryUsers_ADGUID",
                        column: x => x.ADGUID,
                        principalTable: "ActiveDirectoryUsers",
                        principalColumn: "ADGUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoxADUpdates_ADUserADGUID",
                table: "BoxADUpdates",
                column: "ADUserADGUID");

            migrationBuilder.CreateIndex(
                name: "IX_BoxUsers_ADGUID",
                table: "BoxUsers",
                column: "ADGUID");
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
