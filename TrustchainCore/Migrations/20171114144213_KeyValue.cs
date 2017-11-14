using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class KeyValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeyValues",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyValues", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proof_WorkflowID",
                table: "Proof",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_KeyValues_Key",
                table: "KeyValues",
                column: "Key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyValues");

            migrationBuilder.DropIndex(
                name: "IX_Proof_WorkflowID",
                table: "Proof");
        }
    }
}
