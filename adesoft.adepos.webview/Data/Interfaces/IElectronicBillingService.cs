using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.DTO.ElectronicBilling;
using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Interfaces
{
    public interface IElectronicBillingService
    {
        public void AddReportFilter(DTOElectronicBillingFilter filter);

        public DTOElectronicBillingFilter GetReportFilter(string reportFilterId);

        public List<DTOSalesInvoice> GetSalesInvoices(DTOElectronicBillingFilter filter);

        public DTOSalesInvoice GetSalesInvoice(string salesInvoiceId);

        public Task<DTOSalesInvoiceNote> CreateOrUpdateAsync(DTOSalesInvoiceNote dtoSalesInvoiceNote);

        public Task<DTOOPInvoicedNote> CreateOrUpdateAsync(DTOOPInvoicedNote dtoOPInvoiceNote);

        public Task<DTOClosingInvoicedNote> CreateOrUpdateAsync(DTOClosingInvoicedNote dtoClosingInvoicedNote);

        public DTOSalesInvoice CreateOrUpdate(DTOSalesInvoice salesInvoice);

        public DTOOPInvoiced CreateOrUpdate(DTOOPInvoiced opInvoiced);

        public DTOClosingInvoiced CreateOrUpdate(DTOClosingInvoiced dtoClosingInvoiced);

        public void UploadFiles(Parameter parameter, string templateName);

        public List<int> GetYears();

        public List<object> GetMonths();

        public List<string> GetPeriods(int pendingType);

        public Task<DTOSalesInvoice> ConfirmAsync(DTOConfirmSalesInvoice confirmSalesInvoice);

        public Task<DTOSalesInvoice> CancelAsync(DTOConfirmSalesInvoice confirmSalesInvoice);

        public Task<DTOClosingInvoiced> ConfirmAsync(DTOConfirmClosingInvoiced confirmClosingInvoiced);

        public bool Confirm(List<DTOSalesInvoice> salesInvoicesSelected);

        public bool Confirm(List<DTOOPInvoiced> opsInvoicedSelected);

        public bool Confirm(List<DTOClosingInvoiced> closingsInvoicedSelected);

        public List<DTOClosingInvoiced> GetClosingsInvoiced(DTOElectronicBillingFilter filter);

        public DTOClosingInvoiced GetClosingInvoiced(string closingInvoicedId);

        public List<DTOOPInvoiced> GetOPsInvoiced(DTOElectronicBillingFilter filter);

        public DTOOPInvoiced GetOPInvoiced(string opInvoicedId);

        public List<CommonData> GetCustomers();

        public List<CommonData> GetPOs();

        public List<CommonData> GetWorks(string customerAccount);

        public List<CommonData> GetAdministrators(string customerAccount, string workId);

        public Task<DTOOPInvoiced> ConfirmAsync(DTOConfirmOPInvoiced confirmOPInvoiced);

        public DTOClosingInvoicedNote GetClosingInvoicedNote(string closingInvoicedNoteId);

        public DTOOPInvoicedNote GetOPInvoicedNote(string opInvoicedNoteId);

        public Expression<Func<TItem, object>> GroupByExpression<TItem>(string[] propertyNames);

        public List<DTOPendingInvoiceReport> GetPendingInvoiceReports(string guidfilter);

        public List<DTOSalesInvoice> GetSalesInvoices(string guidfilter);

        public List<DTOOPInvoiced> GetOPsInvoiced(string guidfilter);

        public List<DTOClosingInvoiced> GetClosingsInvoiced(string guidfilter);

        public CommonData GetZoneParent(string customerAccount, string workId);

        public List<DTOSalesInvoiceTracking> GetSalesInvoiceTrackings(string guidfilter);

        public IQueryable ApplyQuery<T>(IQueryable<T> items, Radzen.Query query) where T : class;

        public Task<DTOSalesInvoice> EnableSalesInvoiceAsync(DTOConfirmSalesInvoice confirmSalesInvoice, bool complete = false);

        public DateTime GetLastPeriod(int pendingType);
    }
}
