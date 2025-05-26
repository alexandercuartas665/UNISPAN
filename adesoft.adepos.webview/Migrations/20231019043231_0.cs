using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace adesoft.adepos.webview.Migrations
{
    public partial class _0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingAccounts");

            migrationBuilder.DropTable(
                name: "ActionApps");

            migrationBuilder.DropTable(
                name: "AlertXOrders");

            migrationBuilder.DropTable(
                name: "BudgetRents");

            migrationBuilder.DropTable(
                name: "ClosingInvoicedNotes");

            migrationBuilder.DropTable(
                name: "CodeNovedads");

            migrationBuilder.DropTable(
                name: "Companys");

            migrationBuilder.DropTable(
                name: "ControlStateTransactions");

            migrationBuilder.DropTable(
                name: "DeletedDetailTransactionGenerics");

            migrationBuilder.DropTable(
                name: "DetailProductions");

            migrationBuilder.DropTable(
                name: "DetailProductionTerceros");

            migrationBuilder.DropTable(
                name: "DetailReportDynamics");

            migrationBuilder.DropTable(
                name: "DetailTransactionGenerics");

            migrationBuilder.DropTable(
                name: "InventoryXTransactions");

            migrationBuilder.DropTable(
                name: "ItemKits");

            migrationBuilder.DropTable(
                name: "LastSyncOrders");

            migrationBuilder.DropTable(
                name: "LedgerEstimates");

            migrationBuilder.DropTable(
                name: "LocationGenerics");

            migrationBuilder.DropTable(
                name: "LogisticMasterData");

            migrationBuilder.DropTable(
                name: "MovementInventorys");

            migrationBuilder.DropTable(
                name: "NominaNovedads");

            migrationBuilder.DropTable(
                name: "NominaProgramations");

            migrationBuilder.DropTable(
                name: "OPInvoicedNotes");

            migrationBuilder.DropTable(
                name: "OportunidadesCRM");

            migrationBuilder.DropTable(
                name: "OrderComments");

            migrationBuilder.DropTable(
                name: "OrderPalletProducts");

            migrationBuilder.DropTable(
                name: "OrderPictures");

            migrationBuilder.DropTable(
                name: "OrderProductLogs");

            migrationBuilder.DropTable(
                name: "OrderProducts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "OrderUploadControl");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "RendimientoObjects");

            migrationBuilder.DropTable(
                name: "ReportDynamics");

            migrationBuilder.DropTable(
                name: "RequestCertificates");

            migrationBuilder.DropTable(
                name: "SalesInvoiceNotes");

            migrationBuilder.DropTable(
                name: "SimexCartera");

            migrationBuilder.DropTable(
                name: "SimexInventSum");

            migrationBuilder.DropTable(
                name: "SimexLastUpdateModuleLog");

            migrationBuilder.DropTable(
                name: "SimexPresupuesto");

            migrationBuilder.DropTable(
                name: "SimexQtyMinimum");

            migrationBuilder.DropTable(
                name: "SimexSales");

            migrationBuilder.DropTable(
                name: "SimexSalesOrder");

            migrationBuilder.DropTable(
                name: "SnapshotBiableValueMonths");

            migrationBuilder.DropTable(
                name: "SnapshotInventoryQuantifys");

            migrationBuilder.DropTable(
                name: "Terceros");

            migrationBuilder.DropTable(
                name: "TypeActivitys");

            migrationBuilder.DropTable(
                name: "UserApps");

            migrationBuilder.DropTable(
                name: "vwPOs");

            migrationBuilder.DropTable(
                name: "TypeAccountingAccounts");

            migrationBuilder.DropTable(
                name: "ClosingsInvoiced");

            migrationBuilder.DropTable(
                name: "Productions");

            migrationBuilder.DropTable(
                name: "TransactionGenerics");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "OPsInvoiced");

            migrationBuilder.DropTable(
                name: "OrderPallets");

            migrationBuilder.DropTable(
                name: "OrderProductVersions");

            migrationBuilder.DropTable(
                name: "SalesInvoices");

            migrationBuilder.DropTable(
                name: "TypePersons");

            migrationBuilder.DropTable(
                name: "TypeTerceros");

            migrationBuilder.DropTable(
                name: "RoleApps");

            migrationBuilder.DropTable(
                name: "StateTransactionGenerics");

            migrationBuilder.DropTable(
                name: "TypeTransactions");

            migrationBuilder.DropTable(
                name: "Categorys");

            migrationBuilder.DropTable(
                name: "TypeTaxs");

            migrationBuilder.DropTable(
                name: "UnitMeasurements");

            migrationBuilder.DropTable(
                name: "ZoneProducts");
        }
    }
}
