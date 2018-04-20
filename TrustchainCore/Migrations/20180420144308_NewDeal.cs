﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class NewDeal : Migration
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
                    Server_Address = table.Column<byte[]>(nullable: true),
                    Server_Signature = table.Column<byte[]>(nullable: true),
                    Server_Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package", x => x.DatabaseID);
                    table.UniqueConstraint("AK_Package_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workflow",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(nullable: true),
                    NextExecution = table.Column<long>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflow", x => x.DatabaseID);
                });

            migrationBuilder.CreateTable(
                name: "Trust",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Activate = table.Column<uint>(nullable: false),
                    Algorithm = table.Column<string>(nullable: true),
                    Claim = table.Column<string>(nullable: true),
                    Cost = table.Column<short>(nullable: false),
                    Created = table.Column<uint>(nullable: false),
                    Expire = table.Column<uint>(nullable: false),
                    Id = table.Column<byte[]>(nullable: true),
                    PackageDatabaseID = table.Column<int>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Issuer_Address = table.Column<byte[]>(nullable: true),
                    Issuer_Signature = table.Column<byte[]>(nullable: true),
                    Issuer_Type = table.Column<string>(nullable: true),
                    Scope_Type = table.Column<string>(nullable: true),
                    Scope_Value = table.Column<string>(nullable: true),
                    Subject_Address = table.Column<byte[]>(nullable: true),
                    Subject_Signature = table.Column<byte[]>(nullable: true),
                    Subject_Type = table.Column<string>(nullable: true)
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
                name: "Timestamp",
                columns: table => new
                {
                    DatabaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Algorithm = table.Column<string>(nullable: true),
                    Blockchain = table.Column<string>(nullable: true),
                    PackageDatabaseID = table.Column<int>(nullable: true),
                    Receipt = table.Column<byte[]>(nullable: true),
                    Registered = table.Column<long>(nullable: false),
                    Service = table.Column<string>(nullable: true),
                    Source = table.Column<byte[]>(nullable: true),
                    TrustDatabaseID = table.Column<int>(nullable: true),
                    WorkflowID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timestamp", x => x.DatabaseID);
                    table.ForeignKey(
                        name: "FK_Timestamp_Package_PackageDatabaseID",
                        column: x => x.PackageDatabaseID,
                        principalTable: "Package",
                        principalColumn: "DatabaseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Timestamp_Trust_TrustDatabaseID",
                        column: x => x.TrustDatabaseID,
                        principalTable: "Trust",
                        principalColumn: "DatabaseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyValues_Key",
                table: "KeyValues",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_PackageDatabaseID",
                table: "Timestamp",
                column: "PackageDatabaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_Source",
                table: "Timestamp",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_TrustDatabaseID",
                table: "Timestamp",
                column: "TrustDatabaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Timestamp_WorkflowID",
                table: "Timestamp",
                column: "WorkflowID");

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
                name: "IX_Trust_Issuer_Address",
                table: "Trust",
                column: "Issuer_Address");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_Subject_Address",
                table: "Trust",
                column: "Subject_Address");

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
