using adesoft.adeposx.report.Models.ElectronicBilling;
using adesoft.adeposx.report.Models.PL;
using adesoft.adeposx.report.Models.Simex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models
{
    public class ModelGeneralReport
    {

        public IEnumerable<DTOTransactionReport> ListResultsTransac { get; set; }

        public IEnumerable<DTOTransactionReport> ReturnResultadosTransaction()
        {
            return ListResultsTransac;
        }
        public IEnumerable<DTOComprasReport> ListResultsCompras { get; set; }

        public IEnumerable<DTOComprasReport> ReturnResultadosCompras()
        {
            return ListResultsCompras;
        }
        public IEnumerable<DTOReportBalanceMonth> ListResultsBalanceMonth { get; set; }

        public IEnumerable<DTOReportBalanceMonth> ReturnResultsBalanceMonth()
        {
            return ListResultsBalanceMonth;
        }
        public IEnumerable<DTOInventary> ListResultsInventary { get; set; }

        public IEnumerable<DTOInventary> ReturnResultadosInventario()
        {
            return ListResultsInventary;
        }

        public IEnumerable<DTOParamContable> ListResultadosParams { get; set; }
        public IEnumerable<DTOParamContable> ReturnResultadosParams()
        {
            return ListResultadosParams;
        }

        public IEnumerable<DTORendimiento> ListDTORendimientos { get; set; }
        public IEnumerable<DTORendimiento> ReturnListDTORendimientos()
        {
            return ListDTORendimientos;
        }

        public IEnumerable<DTONominaNovedad> ListDTONominaNovedad { get; set; }
        public IEnumerable<DTONominaNovedad> ReturnListDTONominaNovedad()
        {
            return ListDTONominaNovedad;
        }

        public IEnumerable<DTORptMovEquipo> ListDTODTORptMovEquipo { get; set; }
        public IEnumerable<DTORptMovEquipo> ReturnListDTORptMovEquipo()
        {
            return ListDTODTORptMovEquipo;
        }

        public IEnumerable<DTOOportunidadesCRM> ListDTOOportunidadesCRMs { get; set; }
        public IEnumerable<DTOOportunidadesCRM> ReturnListListDTOOportunidadesCRM()
        {
            return ListDTOOportunidadesCRMs;
        }

        public IEnumerable<DTOSimexSalesReport> ListDTOSimexSalesReport { get; set; }
        public IEnumerable<DTOSimexSalesReport> ReturnListDTOSimexSalesReport()
        {
            return ListDTOSimexSalesReport;
        }

        public IEnumerable<DTOSimexSalesReportDetail> ListDTOSimexSalesReportDetail { get; set; }
        public IEnumerable<DTOSimexSalesReportDetail> ReturnListDTOSimexSalesReportDetail()
        {
            return ListDTOSimexSalesReportDetail;
        }

        public IEnumerable<DTOSimexInventSumReport> ListDTOSimexInventSumReport { get; set; }
        public IEnumerable<DTOSimexInventSumReport> ReturnListDTOSimexInventSumReport()
        {
            return ListDTOSimexInventSumReport;
        }

        public IEnumerable<DTOSimexCarteraReport> ListDTOSimexCarteraReport { get; set; }
        public IEnumerable<DTOSimexCarteraReport> ReturnListDTOSimexCarteraReport()
        {
            return ListDTOSimexCarteraReport;
        }

        public IEnumerable<DTOSimexSalesOrderReport> ListDTOSimexSalesOrderReport { get; set; }
        public IEnumerable<DTOSimexSalesOrderReport> ReturnListDTOSimexSalesOrderReport()
        {
            return ListDTOSimexSalesOrderReport;
        }

        public IEnumerable<DTOHREmployReport> ListDTOHREmployReport { get; set; }
        public IEnumerable<DTOHREmployReport> ReturnListDTOHREmployReport()
        {
            return ListDTOHREmployReport;
        }

        public IEnumerable<DTOOrderReport> ListDTOOrderReport { get; set; }
        public IEnumerable<DTOOrderReport> ReturnListDTOOrderReport()
        {
            return ListDTOOrderReport;
        }

        public IEnumerable<DTOSummaryRentReport> ListDTOSummaryRentReport { get; set; }
        public IEnumerable<DTOSummaryRentReport> ReturnDTOSummaryRentReport()
        {
            return ListDTOSummaryRentReport;
        }

        public IEnumerable<DTOPendingInvoiceReport> ListDTOPendingInvoiceReport { get; set; }
        public IEnumerable<DTOPendingInvoiceReport> ReturnDTOPendingInvoiceReport()
        {
            return ListDTOPendingInvoiceReport;
        }

        public IEnumerable<DTOSalesInvoice> ListDTOSalesInvoice { get; set; }
        public IEnumerable<DTOSalesInvoice> ReturnDTOSalesInvoice()
        {
            return ListDTOSalesInvoice;
        }

        public IEnumerable<DTOOPInvoiced> ListDTOOPInvoiced { get; set; }
        public IEnumerable<DTOOPInvoiced> ReturnDTOOPInvoiced()
        {
            return ListDTOOPInvoiced;
        }

        public IEnumerable<DTOClosingInvoiced> ListDTOClosingInvoiced { get; set; }
        public IEnumerable<DTOClosingInvoiced> ReturnDTOClosingInvoiced()
        {
            return ListDTOClosingInvoiced;
        }

        public IEnumerable<DTOSalesInvoiceTracking> ListDTOSalesInvoiceTracking { get; set; }
        public IEnumerable<DTOSalesInvoiceTracking> ReturnDTOSalesInvoiceTracking()
        {
            return ListDTOSalesInvoiceTracking;
        }

        public IEnumerable<DTOOrder> ListDTOOrder { get; set; }
        public IEnumerable<DTOOrder> ReturnDTOOrder()
        {
            return ListDTOOrder;
        }

        public IEnumerable<DTOOrderProduct> ListDTOOrderProduct { get; set; }
        public IEnumerable<DTOOrderProduct> ReturnDTOOrderProduct()
        {
            return ListDTOOrderProduct;
        }

        public IEnumerable<DTOOrderPallet> ListDTOOrderPallet { get; set; }
        public IEnumerable<DTOOrderPallet> ReturnDTOOrderPallet()
        {
            return ListDTOOrderPallet;
        }

        public IEnumerable<DTOOrderPalletProduct> ListDTOOrderPalletProduct { get; set; }
        public IEnumerable<DTOOrderPalletProduct> ReturnDTOOrderPalletProduct()
        {
            return ListDTOOrderPalletProduct;
        }

        public IEnumerable<DTOPickingStatusDetail> ListDTOPickingStatusDetail { get; set; }
        public IEnumerable<DTOPickingStatusDetail> ReturnDTOPickingStatusDetail()
        {
            return ListDTOPickingStatusDetail;
        }
    }
}