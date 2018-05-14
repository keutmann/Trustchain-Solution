using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class Replaced : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Replaced",
                table: "Trust",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<uint>(
                name: "Created",
                table: "Package",
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Replaced",
                table: "Trust");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Package");
        }
    }
}
