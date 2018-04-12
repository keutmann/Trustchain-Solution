using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class NextExecution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Note",
            //    table: "Trust");

            migrationBuilder.AddColumn<long>(
                name: "NextExecution",
                table: "Workflow",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Created",
                table: "Trust",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "NextExecution",
            //    table: "Workflow");

            //migrationBuilder.DropColumn(
            //    name: "Created",
            //    table: "Trust");

            //migrationBuilder.AddColumn<string>(
            //    name: "Note",
            //    table: "Trust",
            //    nullable: true);
        }
    }
}
