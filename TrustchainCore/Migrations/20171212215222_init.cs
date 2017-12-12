using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeyValues",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyValues", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Algorithm = table.Column<string>(nullable: true),
                    Id = table.Column<byte[]>(nullable: true),
                    Server_Address = table.Column<byte[]>(nullable: true),
                    Server_Script = table.Column<string>(nullable: true),
                    Server_Signature = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package", x => x.DatabaseID);
                });

            migrationBuilder.CreateTable(
                name: "Proof",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Receipt = table.Column<byte[]>(nullable: true),
                    Registered = table.Column<long>(nullable: false),
                    Source = table.Column<byte[]>(nullable: true),
                    WorkflowID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proof", x => x.DatabaseID);
                });

            migrationBuilder.CreateTable(
                name: "Workflow",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflow", x => x.DatabaseID);
                });

            migrationBuilder.CreateTable(
                name: "Timestamp",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Algorithm = table.Column<string>(nullable: true),
                    Blockchain = table.Column<string>(nullable: true),
                    PackageID = table.Column<int>(nullable: false),
                    Recipt = table.Column<string>(nullable: true),
                    Service = table.Column<string>(nullable: true),
                    Time = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timestamp", x => x.DatabaseID);
                    table.ForeignKey(
                        name: "FK_Timestamp_Package_PackageID",
                        column: x => x.PackageID,
                        principalTable: "Package",
                        principalColumn: "DatabaseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trust",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Algorithm = table.Column<string>(nullable: true),
                    Id = table.Column<byte[]>(nullable: true),
                    PackageDatabaseID = table.Column<int>(nullable: true),
                    Issuer_Address = table.Column<byte[]>(nullable: true),
                    Issuer_Script = table.Column<string>(nullable: true),
                    Issuer_Signature = table.Column<byte[]>(nullable: true),
                    Timestamp_Algorithm = table.Column<string>(nullable: true),
                    Timestamp_Recipt = table.Column<byte[]>(nullable: true),
                    Timestamp_Timestamps = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trust", x => x.DatabaseID);
                    table.ForeignKey(
                        name: "FK_Trust_Package_PackageDatabaseID",
                        column: x => x.PackageDatabaseID,
                        principalTable: "Package",
                        principalColumn: "DatabaseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Claim",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Activate = table.Column<uint>(nullable: false),
                    Cost = table.Column<short>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Expire = table.Column<uint>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    Scope = table.Column<string>(nullable: true),
                    TrustID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claim", x => x.DatabaseID);
                    table.ForeignKey(
                        name: "FK_Claim_Trust_TrustID",
                        column: x => x.TrustID,
                        principalTable: "Trust",
                        principalColumn: "DatabaseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<byte[]>(nullable: true),
                    Alias = table.Column<string>(nullable: true),
                    ClaimIndexs = table.Column<byte[]>(nullable: true),
                    Kind = table.Column<string>(nullable: true),
                    TrustID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.DatabaseID);
                    table.ForeignKey(
                        name: "FK_Subject_Trust_TrustID",
                        column: x => x.TrustID,
                        principalTable: "Trust",
                        principalColumn: "DatabaseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claim_Index",
                table: "Claim",
                column: "Index");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_TrustID",
                table: "Claim",
                column: "TrustID");

            migrationBuilder.CreateIndex(
                name: "IX_KeyValues_Key",
                table: "KeyValues",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_Package_Id",
                table: "Package",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Proof_Source",
                table: "Proof",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Proof_WorkflowID",
                table: "Proof",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_Address",
                table: "Subject",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_TrustID",
                table: "Subject",
                column: "TrustID");

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_PackageID",
                table: "Timestamp",
                column: "PackageID");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_Id",
                table: "Trust",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_PackageDatabaseID",
                table: "Trust",
                column: "PackageDatabaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Workflow_State",
                table: "Workflow",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_Workflow_Type",
                table: "Workflow",
                column: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claim");

            migrationBuilder.DropTable(
                name: "KeyValues");

            migrationBuilder.DropTable(
                name: "Proof");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "Timestamp");

            migrationBuilder.DropTable(
                name: "Workflow");

            migrationBuilder.DropTable(
                name: "Trust");

            migrationBuilder.DropTable(
                name: "Package");
        }
    }
}
