using adesoft.adepos.webview.Data.Model.VIews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class AdeposReportsContext : DbContext
    {
        public AdeposReportsContext(DbContextOptions<AdeposReportsContext> options)
           : base(options)
        {

            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.ChangeTracker.CascadeDeleteTiming = CascadeTiming.Never;
            this.ChangeTracker.DeleteOrphansTiming = CascadeTiming.Never;

        }

        public AdeposReportsContext(string connectionString) : base(GetOptions(connectionString))
        {
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.ChangeTracker.CascadeDeleteTiming = CascadeTiming.Never;
            this.ChangeTracker.DeleteOrphansTiming = CascadeTiming.Never;

        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            var options = new DbContextOptionsBuilder();
            options.EnableSensitiveDataLogging(false);
            return SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString).Options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DistributionAdminTable>()
                .HasKey(d => new { d.AdminId, d.ZoneId, d.SectorId, d.ZoneParentId, d.Year });

            modelBuilder.Entity<SViewEBNumber>()
                .HasKey(d => new { d.CustomerNum, d.InvoiceNum });

            base.OnModelCreating(modelBuilder);
        }

        public void DetachAll()
        {

            //this.ChangeTracker.AcceptAllChanges();
            foreach (EntityEntry dbEntityEntry in this.ChangeTracker.Entries())
            {
                // dbEntityEntry.DetectChanges();

                dbEntityEntry.State = EntityState.Detached;
            }
        }

        public DbSet<BalanceTransRent> BalanceTransRent { get; set; }

        public DbSet<CommonDataTable> CommonDataTable { get; set; }

        public DbSet<DistributionAdminTable> DistributionAdminTable { get; set; }

        public DbSet<DistributionAdminTable> DistributionAdminTableasdasd { get; set; }

        public DbSet<SViewCustomer> vwCustomers { get; set; }

        public DbSet<SViewEBNumber> vwEBNumbers { get; set; }
    }
}
