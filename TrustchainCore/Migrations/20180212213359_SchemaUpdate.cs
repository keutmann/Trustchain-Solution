using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class SchemaUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Claim",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Claim",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Claim");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Claim");


        }
    }
}
