using adesoft.adepos.webview.Data.Model.ElectronicBilling;
using adesoft.adepos.webview.Data.Model.PL;
using adesoft.adepos.webview.Data.Model.Simex;
using adesoft.adepos.webview.Data.Model.VIews;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    /// <summary>
    /// Para actualizar BD Add-Migration Initial Update-Database    Remove-Migration
    /// </summary>
    public class AdeposDBContext : DbContext
    {
        //static AdeposDBContext _instance;

        //public static AdeposDBContext Instance
        //{
        //    get { return _instance ?? (_instance = new AdeposDBContext()); }
        //}
        //private IJSRuntime _jsRuntime;
        public string _connectionString { get; set; }
        public DbSet<ActionApp> ActionApps { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RoleApp> RoleApps { get; set; }

        public DbSet<UserApp> UserApps { get; set; }

        public DbSet<Category> Categorys { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<UnitMeasurement> UnitMeasurements { get; set; }



        public DbSet<DetailTransactionGeneric> DetailTransactionGenerics { get; set; }

        public DbSet<DeletedDetailTransactionGeneric> DeletedDetailTransactionGenerics { get; set; }

        public DbSet<Tercero> Terceros { get; set; }

        public DbSet<TransactionGeneric> TransactionGenerics { get; set; }


        public DbSet<InventoryXTransaction> InventoryXTransactions { get; set; }

        public DbSet<MovementInventory> MovementInventorys { get; set; }

        public DbSet<TypeTax> TypeTaxs { get; set; }

        public DbSet<TypeTercero> TypeTerceros { get; set; }


        public DbSet<TypeTransaction> TypeTransactions { get; set; }

        public DbSet<ItemKit> ItemKits { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }




        public DbSet<StateTransactionGeneric> StateTransactionGenerics { get; set; }


        public DbSet<TypePerson> TypePersons { get; set; }




        public DbSet<AccountingAccount> AccountingAccounts { get; set; }


        public DbSet<TypeAccountingAccount> TypeAccountingAccounts { get; set; }




        public DbSet<ControlStateTransaction> ControlStateTransactions { get; set; }

        public DbSet<AlertXOrder> AlertXOrders { get; set; }


        public DbSet<Company> Companys { get; set; }


        public DbSet<Parameter> Parameters { get; set; }

        public DbSet<NominaNovedad> NominaNovedads { get; set; }

        public DbSet<CodeNovedad> CodeNovedads { get; set; }

        public DbSet<NominaProgramation> NominaProgramations { get; set; }

        public DbSet<LocationGeneric> LocationGenerics { get; set; }

        public DbSet<RequestCertificate> RequestCertificates { get; set; }

        public DbSet<ReportDynamic> ReportDynamics { get; set; }

        public DbSet<DetailReportDynamic> DetailReportDynamics { get; set; }

        public DbSet<TypeActivity> TypeActivitys { get; set; }

        public DbSet<Production> Productions { get; set; }

        public DbSet<DetailProduction> DetailProductions { get; set; }

        public DbSet<DetailProductionTercero> DetailProductionTerceros { get; set; }

        public DbSet<RendimientoObject> RendimientoObjects { get; set; }


        public DbSet<SnapshotInventoryQuantify> SnapshotInventoryQuantifys { get; set; }


        public DbSet<SnapshotBiableValueMonth> SnapshotBiableValueMonths { get; set; }

        public DbSet<OportunidadesCRM> OportunidadesCRM { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderPicture> OrderPictures { get; set; }

        public DbSet<OrderPallet> OrderPallets { get; set; }

        public DbSet<OrderPalletProduct> OrderPalletProducts { get; set; }

        public DbSet<OrderUploadControl> OrderUploadControl { get; set; }

        public DbSet<OrderComment> OrderComments { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }

        public DbSet<OrderProductLog> OrderProductLogs { get; set; }

        public DbSet<OrderProductVersion> OrderProductVersions { get; set; }

        public DbSet<LogisticMasterData> LogisticMasterData { get; set; }

        public DbSet<LastSyncOrder> LastSyncOrders { get; set; }

        public DbSet<BudgetRent> BudgetRents { get; set; }

        public DbSet<LedgerEstimate> LedgerEstimates { get; set; }

        public DbSet<ZoneProduct> ZoneProducts { get; set; }

        #region Simex

        public DbSet<Sales> SimexSales { get; set; }

        public DbSet<Presupuesto> SimexPresupuesto { get; set; }

        public DbSet<QtyMinimum> SimexQtyMinimum { get; set; }

        public DbSet<Cartera> SimexCartera { get; set; }

        public DbSet<LastUpdateModule> SimexLastUpdateModuleLog { get; set; }

        public DbSet<InventSum> SimexInventSum { get; set; }

        public DbSet<SalesOrder> SimexSalesOrder { get; set; }

        #endregion

        public DbSet<SalesInvoice> SalesInvoices { get; set; }

        public DbSet<SalesInvoiceNote>  SalesInvoiceNotes { get; set; }

        public DbSet<ClosingInvoiced> ClosingsInvoiced { get; set; }

        public DbSet<ClosingInvoicedNote> ClosingInvoicedNotes { get; set; }

        public DbSet<OPInvoiced> OPsInvoiced { get; set; }

        public DbSet<OPInvoicedNote> OPInvoicedNotes { get; set; }

        public DbSet<SViewPO> vwPOs { get; set; }

        public DbSet<OrderNotification> OrderNotifications { get; set; }

        //public DbSet<Rendimiento> Rendimientos { get; set; }
        public AdeposDBContext(DbContextOptions<AdeposDBContext> options)
           : base(options)
        {

            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.ChangeTracker.CascadeDeleteTiming = CascadeTiming.Never;
            this.ChangeTracker.DeleteOrphansTiming = CascadeTiming.Never;
            //  this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //  this.ChangeTracker.AutoDetectChangesEnabled = false;
            //this.tokestate = token;
            //_jsRuntime = jsRuntime;

            //this.ChangeTracker.AutoDetectChangesEnabled = false;
            //this.ChangeTracker.LazyLoadingEnabled = false;
            //  this.ChangeTracker.tr
            //, TokenAuthenticationStateProvider stateauthe
            // string v = Task.FromResult(GetTooken(stateauthe)).Result.Result;
            //  GetTooken(stateauthe);

        }

        public AdeposDBContext(string connectionString) : base(GetOptions(connectionString))
        {
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.ChangeTracker.CascadeDeleteTiming = CascadeTiming.Never;
            this.ChangeTracker.DeleteOrphansTiming = CascadeTiming.Never;
            // this.ChangeTracker.LazyLoadingEnabled = false;

            // this.ChangeTracker.la

            //this.tokestate = token;
            //_jsRuntime = jsRuntime;

            //this.ChangeTracker.AutoDetectChangesEnabled = false;
            //this.ChangeTracker.LazyLoadingEnabled = false;
            //  this.ChangeTracker.tr
            //, TokenAuthenticationStateProvider stateauthe
            // string v = Task.FromResult(GetTooken(stateauthe)).Result.Result;
            //  GetTooken(stateauthe);

        }


        private static DbContextOptions GetOptions(string connectionString)
        {
            var options = new DbContextOptionsBuilder();
            options.EnableSensitiveDataLogging(false);
            return SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString).Options;
        }
        public async Task<string> GetTooken(TokenAuthenticationStateProvider stateauthe)
        {
            string x = await stateauthe.GetTokenAsync();
            return x;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Permission>()
                .HasKey(c => new { c.ActionAppId, c.RoleAppId });

            modelBuilder.Entity<ItemKit>()
                .HasKey(c => new { c.ItemFatherId, c.ItemId });

            modelBuilder.Entity<Sales>()
                .HasKey(c => new { c.DocumentNum, c.Element });

            modelBuilder.Entity<Presupuesto>()
                .HasKey(p => new { p.YearId, p.MonthId, p.ZoneId });

            modelBuilder.Entity<InventSum>()
                .HasKey(sum => new { sum.ItemId, sum.InventLocationId });

            modelBuilder.Entity<QtyMinimum>()
                .HasKey(sum => new { sum.ItemId });

            modelBuilder.Entity<SalesOrder>()
                .HasKey(s => new { s.SalesId, s.ItemId });            

            modelBuilder.Entity<Order>()
               //.HasKey(o => new { o.Id });
                .HasKey(o => new { o.Id, o.OrderType });

            modelBuilder.Entity<OrderNotification>()
                .HasOne(n => n.Order)
                .WithMany(o => o.Notifications)
                .HasForeignKey(n => new { n.OrderId, n.OrderType });


            modelBuilder.Entity<OrderProductVersion>()
                .HasKey(o => new { o.Id });

            modelBuilder.Entity<OrderPicture>()
                .HasKey(op => new { op.Id });

            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => new { op.Id });

            modelBuilder.Entity<OrderProductLog>()
                .HasKey(op => new { op.Id });

            modelBuilder.Entity<OrderPallet>()
                .HasKey(os => new { os.Id });

            modelBuilder.Entity<OrderPalletProduct>()
                .HasKey(osp => new { osp.Id });

            modelBuilder.Entity<OrderUploadControl>()
                .HasKey(op => new { op.Id });

            modelBuilder.Entity<LastSyncOrder>()
                .HasKey(l => new { l.Id });

            modelBuilder.Entity<OrderPallet>()
            .HasOne<ZoneProduct>(p => p.ZoneProduct)
            .WithMany(z => z.OrderPallets)
            .HasForeignKey(p => p.ZoneProductId);

            modelBuilder.Entity<OrderPalletProduct>()
            .HasOne<OrderPallet>(pp => pp.OrderPallet)
            .WithMany(p => p.OrderPalletProducts)
            .HasForeignKey(pp => pp.OrderPalletId);

            /*modelBuilder.Entity<OrderPalletProduct>()
            .HasOne<OrderProduct>(pp => pp.OrderProduct)
            .WithMany(p => p.OrderPalletProducts)
            .HasForeignKey(pp => pp.OrderProductId);*/

            modelBuilder.Entity<Item>()
            .HasOne<ZoneProduct>(i => i.ZoneProduct)
            .WithMany(z => z.Items)
            .HasForeignKey(i => i.ZoneProductId);

            modelBuilder.Entity<OrderProductLog>()
            .HasOne<OrderProductVersion>(i => i.OrderProductVersion)
            .WithMany(z => z.OrderProductLogs)
            .HasForeignKey(i => i.OrderProductVersionId);

            modelBuilder.Entity<OrderProduct>()
            .HasOne<ZoneProduct>(i => i.ZoneProduct)
            .WithMany(z => z.OrderProducts)
            .HasForeignKey(i => i.ZoneProductId);

            modelBuilder.Entity<OrderProductLog>()
            .HasOne<ZoneProduct>(i => i.ZoneProduct)
            .WithMany(z => z.OrderProductLogs)
            .HasForeignKey(i => i.ZoneProductId);

            modelBuilder.Entity<OrderPalletProduct>()
            .HasOne<ZoneProduct>(pp => pp.ZoneProduct)
            .WithMany(p => p.OrderPalletProducts)
            .HasForeignKey(pp => pp.ZoneProductId);

            modelBuilder.Entity<UserApp>()
            .HasOne<ZoneProduct>(pp => pp.ZoneProduct)
            .WithMany(p => p.Users)
            .HasForeignKey(pp => pp.ZoneProductId);

            //      modelBuilder.Entity<Rendimiento>()
            //.HasKey(c => new { c.TerceroId, c.DateActivity , c.TypeActivityId });

            //       modelBuilder.Entity<CreditPayment>()
            //.HasKey(c => new { c.CreditDetailId, c.CuotaNum });
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    Database.SetInitializer<AdeposDBContext>(null);
        //    base.OnModelCreating(modelBuilder);
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.EnableSensitiveDataLogging();

        //    //try
        //    //{
        //    //    TokenAuthenticationStateProvider st2 = tokestate;
        //    //    Task<AuthenticationState> Taskresul = Task.Run<AuthenticationState>(async () => await tokestate.GetAuthenticationStateAsync());
        //    //    AuthenticationState state = Taskresul.Result;
        //    //    _connectionString = state.User.Claims.Where(x => x.Type == "ConnectionString").Select(x => x.Value).FirstOrDefault();
        //    //}
        //    //catch
        //    //{
        //    //if (_jsRuntime != null)
        //    //{
        //    //    object connects = Task.Run<object>(async () => await _jsRuntime.InvokeAsync<object>("blazorExtensions.ReadCookie", "connectionString")).Result;
        //    //    if (connects != null)
        //    //    {
        //    //        //}
        //    //        optionsBuilder.UseSqlServer(connects.ToString());
        //    //    }
        //    //}
        //    // optionsBuilder.UseLazyLoadingProxies
        //    //    optionsBuilder.UseSqlServer;

        //}

        public void DetachAll()
        {

            //this.ChangeTracker.AcceptAllChanges();
            foreach (EntityEntry dbEntityEntry in this.ChangeTracker.Entries())
            {
                // dbEntityEntry.DetectChanges();

                dbEntityEntry.State = EntityState.Detached;
            }
        }
    }
}
