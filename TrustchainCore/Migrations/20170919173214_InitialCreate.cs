using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TrustchainCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeadModel",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Script = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeadModel", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServerModel",
                columns: table => new
                {
                    Id = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Signature = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HeadID = table.Column<int>(type: "INTEGER", nullable: true),
                    ServerId = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Package", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Package_HeadModel_HeadID",
                        column: x => x.HeadID,
                        principalTable: "HeadModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Package_ServerModel_ServerId",
                        column: x => x.ServerId,
                        principalTable: "ServerModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trust",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HeadID = table.Column<int>(type: "INTEGER", nullable: true),
                    PackageModelID = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<byte[]>(type: "BLOB", nullable: true),
                    TrustId = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trust", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Trust_HeadModel_HeadID",
                        column: x => x.HeadID,
                        principalTable: "HeadModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trust_Package_PackageModelID",
                        column: x => x.PackageModelID,
                        principalTable: "Package",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trust_ServerModel_ServerId",
                        column: x => x.ServerId,
                        principalTable: "ServerModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssuerModel",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IssuerId = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Signature = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Timestamp = table.Column<long>(type: "INTEGER", nullable: false),
                    TrustModelID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuerModel", x => x.ID);
                    table.ForeignKey(
                        name: "FK_IssuerModel_Trust_TrustModelID",
                        column: x => x.TrustModelID,
                        principalTable: "Trust",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimestampModel",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HashAlgorithm = table.Column<string>(type: "TEXT", nullable: true),
                    PackageModelID = table.Column<int>(type: "INTEGER", nullable: true),
                    Path = table.Column<byte[]>(type: "BLOB", nullable: true),
                    TrustModelID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimestampModel", x => x.ID);
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
                    IdType = table.Column<string>(type: "TEXT", nullable: true),
                    IssuerModelID = table.Column<int>(type: "INTEGER", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", nullable: true),
                    Signature = table.Column<byte[]>(type: "BLOB", nullable: true),
                    SubjectId = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Timestamp = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Subject_IssuerModel_IssuerModelID",
                        column: x => x.IssuerModelID,
                        principalTable: "IssuerModel",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssuerModel_TrustModelID",
                table: "IssuerModel",
                column: "TrustModelID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Package_HeadID",
                table: "Package",
                column: "HeadID");

            migrationBuilder.CreateIndex(
                name: "IX_Package_ServerId",
                table: "Package",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_IssuerModelID",
                table: "Subject",
                column: "IssuerModelID");

            migrationBuilder.CreateIndex(
                name: "IX_TimestampModel_PackageModelID",
                table: "TimestampModel",
                column: "PackageModelID");

            migrationBuilder.CreateIndex(
                name: "IX_TimestampModel_TrustModelID",
                table: "TimestampModel",
                column: "TrustModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_HeadID",
                table: "Trust",
                column: "HeadID");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_PackageModelID",
                table: "Trust",
                column: "PackageModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Trust_ServerId",
                table: "Trust",
                column: "ServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "TimestampModel");

            migrationBuilder.DropTable(
                name: "IssuerModel");

            migrationBuilder.DropTable(
                name: "Trust");

            migrationBuilder.DropTable(
                name: "Package");

            migrationBuilder.DropTable(
                name: "HeadModel");

            migrationBuilder.DropTable(
                name: "ServerModel");
        }
    }
}
