using Microsoft.EntityFrameworkCore;
using TrustchainCore.Model;

namespace TrustchainCore.Repository
{
    public class TrustDBContext : DbContext
    {
        public DbSet<PackageModel> Packages { get; set; }
        public DbSet<TrustModel> Trusts { get; set; }
        public DbSet<SubjectModel> Subjects { get; set; }
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

            // Proof
            builder.Entity<ProofEntity>().HasIndex("Source");
            builder.Entity<ProofEntity>().HasIndex("WorkflowID");

            // Workflow
            builder.Entity<WorkflowContainer>().HasIndex("Type");
            builder.Entity<WorkflowContainer>().HasIndex("State");

            builder.Entity<KeyValue>().HasIndex(p => p.Key);

            base.OnModelCreating(builder);
        }

    }
}
