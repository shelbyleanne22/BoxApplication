using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BoxApplication.Migrations
{
    public partial class Initial : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveDirectoryUser");
        }
    }
}
