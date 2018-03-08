using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;

namespace TrustchainCore.Repository
{
    public class TrustDBContext : DbContext
    {
        public DbSet<Package> Packages { get; set; }
        public DbSet<Trust> Trusts { get; set; }
        //public DbSet<Subject> Subjects { get; set; }
        //public DbSet<Claim> Claims { get; set; }
        public DbSet<Timestamp> Timestamps { get; set; }

        public DbSet<ProofEntity> Proofs { get; set; }
        public DbSet<WorkflowContainer> Workflows { get; set; }
        public DbSet<KeyValue> KeyValues { get; set; }

        public TrustDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Package>().HasKey(p => p.DatabaseID);
            builder.Entity<Package>().HasAlternateKey(p => p.Id);
            builder.Entity<Package>()
                .OwnsOne(c => c.Server);

            builder.Entity<Trust>().HasKey(p => p.DatabaseID);
            
            builder.Entity<Trust>().HasIndex(p => p.Id).IsUnique(true);
            builder.Entity<Trust>().HasIndex(p => new { p.IssuerAddress, p.SubjectAddress, p.Type, p.Scope }).IsUnique(true);
            builder.Entity<Trust>()
                .OwnsOne(c => c.Timestamp);

            builder.Entity<Timestamp>().HasKey(p => p.DatabaseID);

            // Proof
            builder.Entity<ProofEntity>().HasKey(p => p.DatabaseID);
            builder.Entity<ProofEntity>().HasIndex(p => p.Source);
            builder.Entity<ProofEntity>().HasIndex(p => p.WorkflowID);

            // Workflow
            builder.Entity<WorkflowContainer>().HasKey(p => p.DatabaseID);
            builder.Entity<WorkflowContainer>().HasIndex(p => p.Type);
            builder.Entity<WorkflowContainer>().HasIndex(p => p.State);


            builder.Entity<KeyValue>().HasIndex(p => p.Key);

            base.OnModelCreating(builder);
        }

    }
}
