using Microsoft.EntityFrameworkCore;
using AuditApp.API.Models;

namespace AuditApp.API.Data
{
    public class AuditDbContext : DbContext
    {
        public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Auditor> Auditors { get; set; }
        public DbSet<Audit> Audits { get; set; }
        public DbSet<Finding> Findings { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }
        
        // New modules
        public DbSet<Settlement> Settlements { get; set; }
        public DbSet<DataPersistence> DataPersistences { get; set; }
        public DbSet<PeriodicFailover> PeriodicFailovers { get; set; }
        public DbSet<AuditSiteCreation> AuditSiteCreations { get; set; }
        public DbSet<AuditSiteCopy> AuditSiteCopies { get; set; }
        public DbSet<RestoreAuditSite> RestoreAuditSites { get; set; }
        public DbSet<DataAnalysis> DataAnalyses { get; set; }
        public DbSet<DataSampling> DataSamplings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed data or configure relationships if needed
            modelBuilder.Entity<Audit>()
                .HasOne(a => a.Client)
                .WithMany()
                .HasForeignKey(a => a.ClientId);

            modelBuilder.Entity<Audit>()
                .HasOne(a => a.Auditor)
                .WithMany()
                .HasForeignKey(a => a.AuditorId);

            // Configure decimal precision for new modules
            modelBuilder.Entity<Settlement>()
                .Property(s => s.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DataAnalysis>()
                .Property(d => d.CompliancePercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<DataSampling>()
                .Property(d => d.SamplingRatePercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<DataSampling>()
                .Property(d => d.MarginOfError)
                .HasPrecision(5, 2);
        }
    }
}
