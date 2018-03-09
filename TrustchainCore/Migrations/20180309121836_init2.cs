using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class init2 : Migration
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
                    Id = table.Column<byte[]>(nullable: false),
                    ServerAddress = table.Column<byte[]>(nullable: true),
                    ServerScript = table.Column<string>(nullable: true),
                    ServerSignature = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package", x => x.DatabaseID);
                    table.UniqueConstraint("AK_Package_Id", x => x.Id);
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
                    PackageDatabaseID = table.Column<int>(nullable: false),
                    Recipt = table.Column<string>(nullable: true),
                    Service = table.Column<string>(nullable: true),
                    Time = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timestamp", x => x.DatabaseID);
                    table.ForeignKey(
                        name: "FK_Timestamp_Package_PackageDatabaseID",
                        column: x => x.PackageDatabaseID,
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
                    Activate = table.Column<uint>(nullable: false),
                    Algorithm = table.Column<string>(nullable: true),
                    Attributes = table.Column<string>(nullable: true),
                    Cost = table.Column<short>(nullable: false),
                    Expire = table.Column<uint>(nullable: false),
                    Id = table.Column<byte[]>(nullable: true),
                    IssuerAddress = table.Column<byte[]>(nullable: true),
                    IssuerScript = table.Column<string>(nullable: true),
                    IssuerSignature = table.Column<byte[]>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    PackageDatabaseID = table.Column<int>(nullable: true),
                    Scope = table.Column<string>(nullable: true),
                    SubjectAddress = table.Column<byte[]>(nullable: true),
                    SubjectScript = table.Column<string>(nullable: true),
                    SubjectSignature = table.Column<byte[]>(nullable: true),
                    TimestampAlgorithm = table.Column<string>(nullable: true),
                    TimestampRecipt = table.Column<byte[]>(nullable: true),
                    Timestamps = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_KeyValues_Key",
                table: "KeyValues",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_Proof_Source",
                table: "Proof",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Proof_WorkflowID",
                table: "Proof",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_PackageDatabaseID",
                table: "Timestamp",
                column: "PackageDatabaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_Id",
                table: "Trust",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trust_PackageDatabaseID",
                table: "Trust",
                column: "PackageDatabaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_IssuerAddress_SubjectAddress_Type_Scope",
                table: "Trust",
                columns: new[] { "IssuerAddress", "SubjectAddress", "Type", "Scope" },
                unique: true);

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
                name: "KeyValues");

            migrationBuilder.DropTable(
                name: "Proof");

            migrationBuilder.DropTable(
                name: "Timestamp");

            migrationBuilder.DropTable(
                name: "Trust");

            migrationBuilder.DropTable(
                name: "Workflow");

            migrationBuilder.DropTable(
                name: "Package");
        }
    }
}
