using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class Attributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
     name: "Claim");

            migrationBuilder.CreateTable(
                name: "Claim",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Activate = table.Column<uint>(nullable: false),
                    Cost = table.Column<short>(nullable: false),
                    Attributes = table.Column<string>(nullable: true),
                    Expire = table.Column<uint>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    Scope = table.Column<string>(nullable: true),
                    TrustDatabaseID = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claim", x => x.DatabaseID);
                    table.ForeignKey(
                        name: "FK_Claim_Trust_TrustDatabaseID",
                        column: x => x.TrustDatabaseID,
                        principalTable: "Trust",
                        principalColumn: "DatabaseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claim_Index",
                table: "Claim",
                column: "Index");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_TrustDatabaseID",
                table: "Claim",
                column: "TrustDatabaseID");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claim");
        }
    }
}
