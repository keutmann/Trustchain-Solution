using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class trustdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PackageId = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Head_Script = table.Column<string>(type: "TEXT", nullable: true),
                    Head_Version = table.Column<string>(type: "TEXT", nullable: true),
                    Server_Id = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Server_Signature = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Proof",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Receipt = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Source = table.Column<byte[]>(type: "BLOB", nullable: true),
                    WorkflowID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proof", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Workflow",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<string>(type: "TEXT", nullable: true),
                    Tag = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflow", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Trust",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IssuerId = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PackageModelID = table.Column<int>(type: "INTEGER", nullable: false),
                    Signature = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Timestamp2 = table.Column<long>(type: "INTEGER", nullable: false),
                    TrustId = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Head_Script = table.Column<string>(type: "TEXT", nullable: true),
                    Head_Version = table.Column<string>(type: "TEXT", nullable: true),
                    Server_Id = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Server_Signature = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trust", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Trust_Package_PackageModelID",
                        column: x => x.PackageModelID,
                        principalTable: "Package",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Activate = table.Column<uint>(type: "INTEGER", nullable: false),
                    Claim = table.Column<string>(type: "TEXT", nullable: true),
                    Cost = table.Column<int>(type: "INTEGER", nullable: false),
                    Expire = table.Column<uint>(type: "INTEGER", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", nullable: true),
                    Signature = table.Column<byte[]>(type: "BLOB", nullable: true),
                    SubjectId = table.Column<byte[]>(type: "BLOB", nullable: true),
                    SubjectType = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<int>(type: "INTEGER", nullable: false),
                    TrustModelID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Subject_Trust_TrustModelID",
                        column: x => x.TrustModelID,
                        principalTable: "Trust",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimestampModel",
                columns: table => new
                {
                    TimestampModelID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HashAlgorithm = table.Column<string>(type: "TEXT", nullable: true),
                    PackageModelID = table.Column<int>(type: "INTEGER", nullable: true),
                    Receipt = table.Column<byte[]>(type: "BLOB", nullable: true),
                    TrustModelID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimestampModel", x => x.TimestampModelID);
                    table.ForeignKey(
                        name: "FK_TimestampModel_Package_PackageModelID",
                        column: x => x.PackageModelID,
                        principalTable: "Package",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimestampModel_Trust_TrustModelID",
                        column: x => x.TrustModelID,
                        principalTable: "Trust",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Package_PackageId",
                table: "Package",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Proof_Source",
                table: "Proof",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_SubjectId",
                table: "Subject",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_TrustModelID",
                table: "Subject",
                column: "TrustModelID");

            migrationBuilder.CreateIndex(
                name: "IX_TimestampModel_PackageModelID",
                table: "TimestampModel",
                column: "PackageModelID");

            migrationBuilder.CreateIndex(
                name: "IX_TimestampModel_TrustModelID",
                table: "TimestampModel",
                column: "TrustModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_PackageModelID",
                table: "Trust",
                column: "PackageModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_TrustId",
                table: "Trust",
                column: "TrustId");

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
                name: "Proof");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "TimestampModel");

            migrationBuilder.DropTable(
                name: "Workflow");

            migrationBuilder.DropTable(
                name: "Trust");

            migrationBuilder.DropTable(
                name: "Package");
        }
    }
}
