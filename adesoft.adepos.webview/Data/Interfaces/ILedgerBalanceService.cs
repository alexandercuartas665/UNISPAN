using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.Simex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Interfaces
{
    public interface ILedgerBalanceService
    {
        public void AddLedgerBalanceFilter(DTOLedgerBalanceFilter filter);

        public DTOLedgerBalanceFilter GetLedgerBalanceFilter(string filterId);

        public List<BalanceTransRent> GetBalanceTransRents();

        public List<DTOSummaryRentReport> GetSummaryRents(string guidFilter);

        public List<CommonDataTable> GetCommonDataTable(string categoryId);

        public List<BudgetRent> GetBudgetRents(int year, int month, string categoryId);

        public BudgetRent CreateOrUpdate(BudgetRent budgetRent);

        public bool RemoveBudgetRent(BudgetRent budgetRent);

        public List<LedgerEstimate> GetLedgerEstimates(int year, int month, string categoryId);

        public LedgerEstimate CreateOrUpdate(LedgerEstimate ledgerEstimate);

        public CommonDataTable CreateOrUpdate(CommonDataTable commonDataTable);

        public bool RemoveLedgerEstimate(LedgerEstimate ledgerEstimate);

        public LastUpdateModule GetLastUpdateModule(string module);

        public int UpdateCommercialData();
    }
}
