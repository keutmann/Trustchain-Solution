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
        public DbSet<TrustModel> Trust { get; set; }
        public DbSet<SubjectModel> Subject { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Filename=./blog.db");
        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TrustModel>().HasKey(m => m.TrustId);
            builder.Entity<SubjectModel>().HasKey(m => m.Id);
            builder.Entity<SubjectModel>().HasKey(m => m.TrustId);
            base.OnModelCreating(builder);
        }

    }
}
