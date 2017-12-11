using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;

namespace TrustchainCore.Repository
{
    public class TrustDBContext2 : DbContext
    {
        public DbSet<Package> Packages { get; set; }
        public DbSet<Trust> Trusts { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Timestamp> Timestamps { get; set; }

        public DbSet<ProofEntity> Proofs { get; set; }
        public DbSet<WorkflowContainer> Workflows { get; set; }
        public DbSet<KeyValue> KeyValues { get; set; }

        public TrustDBContext2(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Package>().HasKey("DatabaseID");
            builder.Entity<Package>().HasIndex("Id");
            builder.Entity<Package>()
                .OwnsOne(c => c.Server);

            builder.Entity<Trust>().HasKey("DatabaseID");
            builder.Entity<Trust>().HasIndex("Id");
            builder.Entity<Trust>()
                .OwnsOne(c => c.Issuer);
            builder.Entity<Trust>()
                .OwnsOne(c => c.Timestamp);

            builder.Entity<Subject>().HasKey("DatabaseID");
            builder.Entity<Subject>().HasIndex("Id");

            builder.Entity<Claim>().HasKey("DatabaseID");
            builder.Entity<Claim>().HasIndex("Index");

            builder.Entity<Timestamp>().HasKey("DatabaseID");
            //builder.Entity<Timestamp>().HasIndex("");

            // Proof
            builder.Entity<ProofEntity>().HasKey("DatabaseID");
            builder.Entity<ProofEntity>().HasIndex("Source");
            builder.Entity<ProofEntity>().HasIndex("WorkflowID");

            // Workflow
            builder.Entity<WorkflowContainer>().HasKey("DatabaseID");
            builder.Entity<WorkflowContainer>().HasIndex("Type");
            builder.Entity<WorkflowContainer>().HasIndex("State");

            builder.Entity<KeyValue>().HasIndex(p => p.Key);

            base.OnModelCreating(builder);
        }

    }
}
