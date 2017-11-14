﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using TrustchainCore.Repository;

namespace TrustchainCore.Migrations
{
    [DbContext(typeof(TrustDBContext))]
    [Migration("20171114144213_KeyValue")]
    partial class KeyValue
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("TrustchainCore.Model.KeyValue", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key");

                    b.Property<byte[]>("Value");

                    b.HasKey("ID");

                    b.HasIndex("Key");

                    b.ToTable("KeyValues");
                });

            modelBuilder.Entity("TrustchainCore.Model.PackageModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("PackageId");

                    b.HasKey("ID");

                    b.HasIndex("PackageId");

                    b.ToTable("Package");
                });

            modelBuilder.Entity("TrustchainCore.Model.ProofEntity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Receipt");

                    b.Property<byte[]>("Source");

                    b.Property<int>("WorkflowID");

                    b.HasKey("ID");

                    b.HasIndex("Source");

                    b.HasIndex("WorkflowID");

                    b.ToTable("Proof");
                });

            modelBuilder.Entity("TrustchainCore.Model.SubjectModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("Activate");

                    b.Property<string>("Claim");

                    b.Property<int>("Cost");

                    b.Property<uint>("Expire");

                    b.Property<string>("Scope");

                    b.Property<byte[]>("Signature");

                    b.Property<byte[]>("SubjectId");

                    b.Property<string>("SubjectType");

                    b.Property<int>("Timestamp");

                    b.Property<int>("TrustModelID");

                    b.HasKey("ID");

                    b.HasIndex("SubjectId");

                    b.HasIndex("TrustModelID");

                    b.ToTable("Subject");
                });

            modelBuilder.Entity("TrustchainCore.Model.TimestampModel", b =>
                {
                    b.Property<int>("TimestampModelID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HashAlgorithm");

                    b.Property<int?>("PackageModelID");

                    b.Property<byte[]>("Receipt");

                    b.Property<int?>("TrustModelID");

                    b.HasKey("TimestampModelID");

                    b.HasIndex("PackageModelID");

                    b.HasIndex("TrustModelID");

                    b.ToTable("TimestampModel");
                });

            modelBuilder.Entity("TrustchainCore.Model.TrustModel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("IssuerId");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<int>("PackageModelID");

                    b.Property<byte[]>("Signature");

                    b.Property<long>("Timestamp2");

                    b.Property<byte[]>("TrustId");

                    b.HasKey("ID");

                    b.HasIndex("PackageModelID");

                    b.HasIndex("TrustId");

                    b.ToTable("Trust");
                });

            modelBuilder.Entity("TrustchainCore.Model.WorkflowContainer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data");

                    b.Property<string>("State");

                    b.Property<string>("Tag");

                    b.Property<string>("Type");

                    b.HasKey("ID");

                    b.HasIndex("State");

                    b.HasIndex("Type");

                    b.ToTable("Workflow");
                });

            modelBuilder.Entity("TrustchainCore.Model.PackageModel", b =>
                {
                    b.OwnsOne("TrustchainCore.Model.HeadModel", "Head", b1 =>
                        {
                            b1.Property<int>("ID");

                            b1.Property<string>("Script");

                            b1.Property<string>("Version");

                            b1.ToTable("Package");

                            b1.HasOne("TrustchainCore.Model.PackageModel")
                                .WithOne("Head")
                                .HasForeignKey("TrustchainCore.Model.HeadModel", "ID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("TrustchainCore.Model.ServerModel", "Server", b1 =>
                        {
                            b1.Property<int>("PackageModelID");

                            b1.Property<byte[]>("Id");

                            b1.Property<byte[]>("Signature");

                            b1.ToTable("Package");

                            b1.HasOne("TrustchainCore.Model.PackageModel")
                                .WithOne("Server")
                                .HasForeignKey("TrustchainCore.Model.ServerModel", "PackageModelID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("TrustchainCore.Model.SubjectModel", b =>
                {
                    b.HasOne("TrustchainCore.Model.TrustModel")
                        .WithMany("Subjects")
                        .HasForeignKey("TrustModelID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TrustchainCore.Model.TimestampModel", b =>
                {
                    b.HasOne("TrustchainCore.Model.PackageModel")
                        .WithMany("Timestamp")
                        .HasForeignKey("PackageModelID");

                    b.HasOne("TrustchainCore.Model.TrustModel")
                        .WithMany("Timestamp")
                        .HasForeignKey("TrustModelID");
                });

            modelBuilder.Entity("TrustchainCore.Model.TrustModel", b =>
                {
                    b.HasOne("TrustchainCore.Model.PackageModel")
                        .WithMany("Trust")
                        .HasForeignKey("PackageModelID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("TrustchainCore.Model.HeadModel", "Head", b1 =>
                        {
                            b1.Property<int>("ID");

                            b1.Property<string>("Script");

                            b1.Property<string>("Version");

                            b1.ToTable("Trust");

                            b1.HasOne("TrustchainCore.Model.TrustModel")
                                .WithOne("Head")
                                .HasForeignKey("TrustchainCore.Model.HeadModel", "ID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("TrustchainCore.Model.ServerModel", "Server", b1 =>
                        {
                            b1.Property<int?>("TrustModelID");

                            b1.Property<byte[]>("Id");

                            b1.Property<byte[]>("Signature");

                            b1.ToTable("Trust");

                            b1.HasOne("TrustchainCore.Model.TrustModel")
                                .WithOne("Server")
                                .HasForeignKey("TrustchainCore.Model.ServerModel", "TrustModelID")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
