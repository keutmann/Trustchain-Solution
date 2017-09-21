using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;

namespace TrustchainCore.Repository
{
    public class TrustDBContext : DbContext
    {
        public DbSet<PackageModel> Package { get; set; }
        public DbSet<TrustModel> Trusts { get; set; }
        public DbSet<SubjectModel> Subject { get; set; }

        public TrustDBContext(DbContextOptions options) : base(options)
        {
        }

        //protected TrustDBContext()
        //{
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Filename=./trust.db");
        }
        


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PackageModel>().HasIndex("PackageId");

            builder.Entity<PackageModel>()
                .OwnsOne(c => c.Head);

            builder.Entity<PackageModel>()
                .OwnsOne(c => c.Server);

            builder.Entity<TrustModel>().HasIndex("TrustId");

            builder.Entity<TrustModel>()
                .OwnsOne(c => c.Head);

            builder.Entity<TrustModel>()
                .OwnsOne(c => c.Server);

            builder.Entity<SubjectModel>().HasIndex("SubjectId");

            base.OnModelCreating(builder);
        }

    }
}
