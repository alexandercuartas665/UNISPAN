using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.Interfaces;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.Simex;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data
{
    public class LedgerBalanceService: ILedgerBalanceService
    {
        private readonly IConfiguration _configuration;
        private readonly AdeposReportsContext _reportContext;
        private readonly AdeposDBContext _dbContext;
        private readonly ConnectionDB _connectionDB;

        public static List<DTOLedgerBalanceFilter> LedgerBalanceFilters = new List<DTOLedgerBalanceFilter>();

        public LedgerBalanceService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            if (_connectionDB == null)
                _connectionDB = SecurityService.GetConnectionDefault();

            _reportContext = new AdeposReportsContext(_configuration.GetConnectionString("UnispanReports"));
            _dbContext = new AdeposDBContext(_connectionDB.Connection);
        }

        public void AddLedgerBalanceFilter(DTOLedgerBalanceFilter ledgerBalanceFilter)
        {
            LedgerBalanceFilters.Add(ledgerBalanceFilter);
        }

        public DTOLedgerBalanceFilter GetLedgerBalanceFilter(string filterId)
        {
            return LedgerBalanceFilters.Where(b => b.FilterId.Equals(filterId)).FirstOrDefault();
        }

        public List<BalanceTransRent> GetBalanceTransRents()
        {
            try
            {
                var balanceTransRents = _reportContext.BalanceTransRent.Where(b => b.Period.Equals("202201")).ToList();
                return balanceTransRents;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DTOSummaryRentReport> GetSummaryRents(string guidFilter)
        {
            try
            {                
                var dataReport = new List<DTOSummaryRentReport>();
                var ledgerBalanceFilter = this.GetLedgerBalanceFilter(guidFilter);

                var lambda = this.GroupByExpression<BalanceTransRent>(ledgerBalanceFilter.GroupBy.ToArray());

                var balanceTransRents = new List<BalanceTransRent>();

                var months = ledgerBalanceFilter.MonthsSelected.ToList();

                var result = _reportContext.BalanceTransRent
                        .Where(b => b.Year.Equals(ledgerBalanceFilter.Year)
                        && months.Contains(b.Month))
                        .ToList();

                if(ledgerBalanceFilter.GroupBy.Where(g => g.Equals("AdminId")).FirstOrDefault() != null)
                {
                    var administrators = this.GetCommonDataTable("Administrator");
                    foreach (var administrator in administrators.Where(a => !string.IsNullOrEmpty(a.CommonDataTitle)).ToList())
                    {
                        foreach (var balanceTransRent in result.Where(r => r.AdminId.Equals(administrator.Id)).ToList())
                        {
                            balanceTransRent.AdminName = administrator.CommonDataTitle;
                        }
                    }
                }

                var distributionAdminTable = _reportContext.DistributionAdminTable
                                            .Where(d => d.Year.Equals(ledgerBalanceFilter.Year) && d.Active)
                                            .ToList();  

                if(distributionAdminTable.Count() == 0)
                {
                    distributionAdminTable = _reportContext.DistributionAdminTable
                                            .Where(d => d.Year.Equals(ledgerBalanceFilter.Year - 1) && d.Active)
                                            .ToList();
                }

                var categoryId = ledgerBalanceFilter.GroupBy.LastOrDefault();
                categoryId = categoryId.Equals("AdminId") ? "Administrator" : categoryId.Equals("ZoneParentId") ? "Zone" : categoryId.Equals("SectorId") ? "Sector" : categoryId.Equals("CustomerAccount") ? "Customer" : "";
                
                var totBalanceAmount = result                                        
                                        .Select(b => b.BalanceAmount)
                                        .Sum();                

                if (!categoryId.Equals("Sector") && !categoryId.Equals("Customer"))
                    totBalanceAmount = 0;

                var groupList = result.AsQueryable()
                        .GroupBy(lambda.Compile())
                        .Select(b => this.NewSelect(ledgerBalanceFilter.GroupBy, b, Math.Abs(totBalanceAmount)))
                        .ToList();                

                foreach (var summaryRentReport in groupList)
                {
                    var commonDataId = string.Empty;
                    summaryRentReport.Year = ledgerBalanceFilter.Year;

                    switch(ledgerBalanceFilter.GroupBy.Count())
                    {
                        case 1:
                            commonDataId = summaryRentReport.LevelKey1;
                            break;

                        case 2:
                            commonDataId = summaryRentReport.LevelKey2;
                            break;

                        case 3:
                            commonDataId = summaryRentReport.LevelKey3;
                            break;
                    }

                    foreach(var month in months)
                    {
                        switch (month)
                        {
                            case 1:
                                summaryRentReport.Title1 = "ENERO";
                                break;

                            case 2:
                                summaryRentReport.Title2 = "FEBRERO";
                                break;

                            case 3:
                                summaryRentReport.Title3 = "MARZO";
                                break;

                            case 4:
                                summaryRentReport.Title4 = "ABRIL";
                                break;

                            case 5:
                                summaryRentReport.Title5 = "MAYO";
                                break;

                            case 6:
                                summaryRentReport.Title6 = "JUNIO";
                                break;

                            case 7:
                                summaryRentReport.Title7 = "JULIO";
                                break;

                            case 8:
                                summaryRentReport.Title8 = "AGOSTO";
                                break;

                            case 9:
                                summaryRentReport.Title9 = "SEPTIEMBRE";
                                break;

                            case 10:
                                summaryRentReport.Title10 = "OCTUBRE";
                                break;

                            case 11:
                                summaryRentReport.Title11 = "NOVIEMBRE";
                                break;

                            case 12:
                                summaryRentReport.Title12 = "DICIEMBRE";
                                break;
                        }

                        decimal amountBudget = 0;
                        switch(categoryId)
                        {
                            case "Administrator": 
                                {
                                    var budgetRents = _dbContext.BudgetRents
                                        .Where(b => b.Year.Equals(ledgerBalanceFilter.Year)
                                        && b.Month.Equals(month)
                                        && b.CategoryId.Equals(categoryId)
                                        && b.CommonDataId.Equals(commonDataId));                                        

                                    if(budgetRents.Count() != 0)
                                    {
                                        switch (ledgerBalanceFilter.GroupBy.Count())
                                        {
                                            case 2:
                                                if (ledgerBalanceFilter.GroupBy.FirstOrDefault().Equals("ZoneParentId"))
                                                {
                                                    var dis = _reportContext.DistributionAdminTable
                                                        .Where(c => c.AdminId.Equals(commonDataId)
                                                        && c.ZoneParentId.Equals(summaryRentReport.LevelKey1)
                                                        && c.Year.Equals(ledgerBalanceFilter.Year)
                                                        && c.Active)
                                                        .FirstOrDefault();
                                                    if (dis != null)
                                                    {
                                                        if(budgetRents.Count() > 1)
                                                        {                                                            
                                                            var budgetRent = budgetRents.FirstOrDefault(b => b.ZoneId.Equals(summaryRentReport.LevelKey1));
                                                            if (!(budgetRent is null))
                                                                amountBudget = budgetRent.AmountBudget;
                                                            else
                                                            {
                                                                decimal ab = 0;
                                                                budgetRents.Where(b => string.IsNullOrEmpty(b.ZoneId)).ToList().ForEach(b => ab += b.AmountBudget);
                                                                amountBudget = ab;
                                                            }
                                                        }                                           
                                                        else
                                                            amountBudget = budgetRents.FirstOrDefault().AmountBudget;
                                                    }
                                                }
                                                else if (ledgerBalanceFilter.GroupBy.FirstOrDefault().Equals("SectorId"))
                                                {
                                                    var dis = _reportContext.DistributionAdminTable
                                                        .Where(c => c.AdminId.Equals(commonDataId)
                                                        && c.SectorId.Equals(summaryRentReport.LevelKey1)
                                                        && c.Year.Equals(ledgerBalanceFilter.Year)
                                                        && c.Active)
                                                        .FirstOrDefault();
                                                    if (dis != null)
                                                        amountBudget = budgetRents.FirstOrDefault().AmountBudget;
                                                }
                                                break;

                                            case 3:
                                                int index = ledgerBalanceFilter.GroupBy.ToList().IndexOf("ZoneParentId");
                                                switch (index)
                                                {
                                                    case 0:
                                                        {
                                                            var dis = _reportContext.DistributionAdminTable
                                                            .Where(c => c.AdminId.Equals(commonDataId)
                                                            && c.ZoneParentId.Equals(summaryRentReport.LevelKey1)
                                                            && c.Year.Equals(ledgerBalanceFilter.Year)
                                                            && c.Active)
                                                            .FirstOrDefault();
                                                            if (dis != null)
                                                                amountBudget = budgetRents.FirstOrDefault().AmountBudget;

                                                            break;
                                                        }

                                                    case 1:
                                                        {
                                                            var dis = _reportContext.DistributionAdminTable
                                                            .Where(c => c.AdminId.Equals(commonDataId)
                                                            && c.ZoneParentId.Equals(summaryRentReport.LevelKey2)
                                                            && c.Year.Equals(ledgerBalanceFilter.Year)
                                                            && c.Active
                                                            )
                                                            .FirstOrDefault();
                                                            if (dis != null)
                                                                amountBudget = budgetRents.FirstOrDefault().AmountBudget;

                                                            break;
                                                        }

                                                    default:
                                                        amountBudget = budgetRents.FirstOrDefault().AmountBudget;
                                                        break;
                                                }                                                
                                                break;

                                            default:
                                                {
                                                    if (budgetRents.Count() > 1)                                                    
                                                        budgetRents.ToList().ForEach(b => amountBudget += b.AmountBudget);                                                    
                                                    else
                                                        amountBudget = budgetRents.FirstOrDefault().AmountBudget;
                                                    break;
                                                }                                                
                                        }                                        
                                    }
                                    
                                    break;
                                }

                            case "Zone":
                                {
                                    var distAdmins = new List<string>();
                                    var lambda2 = this.GroupByExpression<DistributionAdminTable>((new List<string> { "AdminId" }).ToArray());
                                    switch (categoryId)
                                    {
                                        case "Zone":
                                            {
                                                distAdmins = distributionAdminTable
                                                    .Where(d => d.ZoneParentId.Equals(commonDataId)
                                                    && d.Active)
                                                    .GroupBy(lambda2.Compile())
                                                    .Select(d => d.Max(d => d.AdminId))
                                                    .ToList();

                                                _dbContext.BudgetRents
                                                    .Where(b => b.ZoneId.Equals(commonDataId)
                                                        && b.Year.Equals(ledgerBalanceFilter.Year)
                                                        && b.Month.Equals(month))
                                                    .ToList().ForEach(b =>
                                                    {
                                                        distAdmins.Remove(b.CommonDataId);
                                                        amountBudget += b.AmountBudget;
                                                    });

                                                break;
                                            }

                                        case "Sector":
                                            {
                                                distAdmins = distributionAdminTable
                                                    .Where(d => d.SectorId.Equals(commonDataId)
                                                    && d.Active)
                                                    .GroupBy(lambda2.Compile())
                                                    .Select(d => d.Max(d => d.AdminId))
                                                    .ToList();
                                                break;
                                            }                                                                                            
                                    }

                                    amountBudget += _dbContext.BudgetRents
                                        .Where(b => b.Year.Equals(ledgerBalanceFilter.Year)
                                        && b.Month.Equals(month)
                                        && b.CategoryId.Equals("Administrator")
                                        && distAdmins.Contains(b.CommonDataId))
                                        .Select(b => b.AmountBudget)
                                        .Sum();

                                    break;
                                }
                        }

                        if (amountBudget != 0)
                        {
                            summaryRentReport.TotPpto += amountBudget;

                            switch (month)
                            {
                                case 1:
                                    summaryRentReport.Ppto1 = amountBudget;
                                    if(summaryRentReport.BalanceAmt1 != 0 && summaryRentReport.Ppto1 != 0)
                                        summaryRentReport.Comp1 = summaryRentReport.BalanceAmt1 / summaryRentReport.Ppto1;
                                    break;

                                case 2:
                                    summaryRentReport.Ppto2 = amountBudget;
                                    if (summaryRentReport.BalanceAmt2 != 0 && summaryRentReport.Ppto2 != 0)
                                        summaryRentReport.Comp2 = summaryRentReport.BalanceAmt2 / summaryRentReport.Ppto2;
                                    break;

                                case 3:
                                    summaryRentReport.Ppto3 = amountBudget;
                                    if (summaryRentReport.BalanceAmt3 != 0 && summaryRentReport.Ppto3 != 0)
                                        summaryRentReport.Comp3 = summaryRentReport.BalanceAmt3 / summaryRentReport.Ppto3;
                                    break;

                                case 4:
                                    summaryRentReport.Ppto4 = amountBudget;
                                    if (summaryRentReport.BalanceAmt4 != 0 && summaryRentReport.Ppto4 != 0)
                                        summaryRentReport.Comp4 = summaryRentReport.BalanceAmt4 / summaryRentReport.Ppto4;
                                    break;

                                case 5:
                                    summaryRentReport.Ppto5 = amountBudget;
                                    if (summaryRentReport.BalanceAmt5 != 0 && summaryRentReport.Ppto5 != 0)
                                        summaryRentReport.Comp5 = summaryRentReport.BalanceAmt5 / summaryRentReport.Ppto5;
                                    break;

                                case 6:
                                    summaryRentReport.Ppto6 = amountBudget;
                                    if (summaryRentReport.BalanceAmt6 != 0 && summaryRentReport.Ppto6 != 0)
                                        summaryRentReport.Comp6 = summaryRentReport.BalanceAmt6 / summaryRentReport.Ppto6;
                                    break;

                                case 7:
                                    summaryRentReport.Ppto7 = amountBudget;
                                    if (summaryRentReport.BalanceAmt7 != 0 && summaryRentReport.Ppto7 != 0)
                                        summaryRentReport.Comp7 = summaryRentReport.BalanceAmt7 / summaryRentReport.Ppto7;
                                    break;

                                case 8:
                                    summaryRentReport.Ppto8 = amountBudget;
                                    if (summaryRentReport.BalanceAmt8 != 0 && summaryRentReport.Ppto8 != 0)
                                        summaryRentReport.Comp8 = summaryRentReport.BalanceAmt8 / summaryRentReport.Ppto8;
                                    break;

                                case 9:
                                    summaryRentReport.Ppto9 = amountBudget;
                                    if (summaryRentReport.BalanceAmt9 != 0 && summaryRentReport.Ppto9 != 0)
                                        summaryRentReport.Comp9 = summaryRentReport.BalanceAmt9 / summaryRentReport.Ppto9;
                                    break;

                                case 10:
                                    summaryRentReport.Ppto10 = amountBudget;
                                    if (summaryRentReport.BalanceAmt10 != 0 && summaryRentReport.Ppto10 != 0)
                                        summaryRentReport.Comp10 = summaryRentReport.BalanceAmt10 / summaryRentReport.Ppto10;
                                    break;

                                case 11:
                                    summaryRentReport.Ppto11 = amountBudget;
                                    if (summaryRentReport.BalanceAmt11 != 0 && summaryRentReport.Ppto11 != 0)
                                        summaryRentReport.Comp11 = summaryRentReport.BalanceAmt11 / summaryRentReport.Ppto11;
                                    break;

                                case 12:
                                    summaryRentReport.Ppto12 = amountBudget;
                                    if (summaryRentReport.BalanceAmt12 != 0 && summaryRentReport.Ppto12 != 0)
                                        summaryRentReport.Comp12 = summaryRentReport.BalanceAmt12 / summaryRentReport.Ppto12;
                                    break;
                            }
                        }
                    }

                    if(summaryRentReport.TotBalanceAmt != 0 && summaryRentReport.TotPpto != 0)
                        summaryRentReport.TotComp = summaryRentReport.TotBalanceAmt / summaryRentReport.TotPpto;
                }

                if (categoryId.Equals("Sector") || categoryId.Equals("Customer"))
                {
                    if (ledgerBalanceFilter.GroupBy.Count().Equals(1))
                    {                    
                        int ranking = 1;
                        foreach (var item in groupList.OrderByDescending(g => g.TotBalanceAmt).ToList())
                        {
                            item.TotRanking = ranking;
                            ranking++;
                        }

                        foreach (var month in ledgerBalanceFilter.MonthsSelected)
                        {
                            ranking = 1;
                            switch (month)
                            {
                                case 1:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt1).ToList())
                                    {
                                        item.Ranking1 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 2:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt2).ToList())
                                    {
                                        item.Ranking2 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 3:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt3).ToList())
                                    {
                                        item.Ranking3 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 4:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt4).ToList())
                                    {
                                        item.Ranking4 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 5:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt5).ToList())
                                    {
                                        item.Ranking5 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 6:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt6).ToList())
                                    {
                                        item.Ranking6 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 7:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt7).ToList())
                                    {
                                        item.Ranking7 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 8:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt8).ToList())
                                    {
                                        item.Ranking8 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 9:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt9).ToList())
                                    {
                                        item.Ranking9 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 10:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt10).ToList())
                                    {
                                        item.Ranking10 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 11:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt11).ToList())
                                    {
                                        item.Ranking11 = ranking;
                                        ranking++;
                                    }
                                    break;

                                case 12:
                                    foreach (var item in groupList.OrderByDescending(g => g.BalanceAmt12).ToList())
                                    {
                                        item.Ranking12 = ranking;
                                        ranking++;
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    else if (ledgerBalanceFilter.GroupBy.Count().Equals(2))
                    {                        
                        var groupListL1 = groupList.GroupBy(g => g.Level1).ToList();
                        foreach (var l1 in groupListL1)
                        {
                            int ranking = 1;
                            var value = l1.Key;

                            foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.TotBalanceAmt).ToList())
                            {
                                item.TotRanking = ranking;
                                ranking++;
                            }

                            foreach (var month in ledgerBalanceFilter.MonthsSelected)
                            {
                                ranking = 1;
                                switch (month)
                                {
                                    case 1:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt1).ToList())
                                        {
                                            item.Ranking1 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 2:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt2).ToList())
                                        {
                                            item.Ranking2 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 3:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt3).ToList())
                                        {
                                            item.Ranking3 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 4:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt4).ToList())
                                        {
                                            item.Ranking4 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 5:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt5).ToList())
                                        {
                                            item.Ranking5 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 6:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt6).ToList())
                                        {
                                            item.Ranking6 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 7:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt7).ToList())
                                        {
                                            item.Ranking7 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 8:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt8).ToList())
                                        {
                                            item.Ranking8 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 9:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt9).ToList())
                                        {
                                            item.Ranking9 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 10:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt10).ToList())
                                        {
                                            item.Ranking10 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 11:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt11).ToList())
                                        {
                                            item.Ranking11 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    case 12:
                                        foreach (var item in groupList.Where(g => g.Level1.Equals(value)).OrderByDescending(g => g.BalanceAmt12).ToList())
                                        {
                                            item.Ranking12 = ranking;
                                            ranking++;
                                        }
                                        break;

                                    default:                                        
                                        break;
                                }
                            }
                        }
                    }
                    else if (ledgerBalanceFilter.GroupBy.Count().Equals(3))
                    {
                        var groupListL1 = groupList.GroupBy(g => g.Level1).ToList();
                        foreach (var l1 in groupListL1)
                        {
                            var value = l1.Key;

                            var groupList2 = groupList.Where(g => g.Level1.Equals(value)).GroupBy(g => g.Level2).ToList();
                            foreach (var l2 in groupList2)
                            {
                                int ranking = 1;
                                var value2 = l2.Key;

                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.TotBalanceAmt).ToList())
                                {
                                    item.TotRanking = ranking;
                                    ranking++;
                                }

                                foreach (var month in ledgerBalanceFilter.MonthsSelected)
                                {
                                    ranking = 1;
                                    switch (month)
                                    {
                                        case 1:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt1).ToList())
                                            {
                                                item.Ranking1 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 2:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt2).ToList())
                                            {
                                                item.Ranking2 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 3:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt3).ToList())
                                            {
                                                item.Ranking3 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 4:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt4).ToList())
                                            {
                                                item.Ranking4 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 5:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt5).ToList())
                                            {
                                                item.Ranking5 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 6:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt6).ToList())
                                            {
                                                item.Ranking6 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 7:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt7).ToList())
                                            {
                                                item.Ranking7 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 8:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt8).ToList())
                                            {
                                                item.Ranking8 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 9:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt9).ToList())
                                            {
                                                item.Ranking9 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 10:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt10).ToList())
                                            {
                                                item.Ranking10 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 11:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt11).ToList())
                                            {
                                                item.Ranking11 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        case 12:
                                            foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2)).OrderByDescending(g => g.BalanceAmt12).ToList())
                                            {
                                                item.Ranking12 = ranking;
                                                ranking++;
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }                            
                        }
                    }
                    else if (ledgerBalanceFilter.GroupBy.Count().Equals(4))
                    {
                        var groupListL1 = groupList.GroupBy(g => g.Level1).ToList();
                        foreach (var l1 in groupListL1)
                        {
                            var value = l1.Key;

                            var groupList2 = groupList.Where(g => g.Level1.Equals(value)).GroupBy(g => g.Level2).ToList();
                            foreach (var l2 in groupList2)
                            {                                
                                var value2 = l2.Key;

                                var groupList3 = groupList.Where(g => g.Level2.Equals(value2)).GroupBy(g => g.Level3).ToList();
                                foreach (var l3 in groupList3)
                                {
                                    int ranking = 1;
                                    var value3 = l3.Key;

                                    foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.TotBalanceAmt).ToList())
                                    {
                                        item.TotRanking = ranking;
                                        ranking++;
                                    }

                                    foreach (var month in ledgerBalanceFilter.MonthsSelected)
                                    {
                                        ranking = 1;
                                        switch (month)
                                        {
                                            case 1:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt1).ToList())
                                                {
                                                    item.Ranking1 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 2:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt2).ToList())
                                                {
                                                    item.Ranking2 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 3:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt3).ToList())
                                                {
                                                    item.Ranking3 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 4:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt4).ToList())
                                                {
                                                    item.Ranking4 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 5:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt5).ToList())
                                                {
                                                    item.Ranking5 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 6:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt6).ToList())
                                                {
                                                    item.Ranking6 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 7:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt7).ToList())
                                                {
                                                    item.Ranking7 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 8:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt8).ToList())
                                                {
                                                    item.Ranking8 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 9:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt9).ToList())
                                                {
                                                    item.Ranking9 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 10:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt10).ToList())
                                                {
                                                    item.Ranking10 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 11:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt11).ToList())
                                                {
                                                    item.Ranking11 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            case 12:
                                                foreach (var item in groupList.Where(g => g.Level1.Equals(value) && g.Level2.Equals(value2) && g.Level3.Equals(value3)).OrderByDescending(g => g.BalanceAmt12).ToList())
                                                {
                                                    item.Ranking12 = ranking;
                                                    ranking++;
                                                }
                                                break;

                                            default:
                                                break;
                                        }
                                    }
                                }                                
                            }
                        }
                    }
                }

                int groupBy = ledgerBalanceFilter.GroupBy.Count();
                for (int i = 1; i < groupBy; i++)
                {
                    var gb = ledgerBalanceFilter.GroupBy.ToArray()[i - 1];
                    switch (i)
                    {
                        case 1:
                            switch (gb)
                            {
                                case "SectorId":
                                    groupList = groupList.Where(g => ledgerBalanceFilter.SectorsSelected.Contains(g.LevelKey1)).ToList();
                                    break;

                                case "ZoneParentId":
                                    groupList = groupList.Where(g => ledgerBalanceFilter.ZonesSelected.Contains(g.LevelKey1)).ToList();
                                    break;

                                case "AdminId":
                                    groupList = groupList.Where(g => ledgerBalanceFilter.AdministratorsSelected.Contains(g.LevelKey1)).ToList();
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case 2:
                            switch (gb)
                            {
                                case "SectorId":
                                    groupList = groupList.Where(g => ledgerBalanceFilter.SectorsSelected.Contains(g.LevelKey2)).ToList();
                                    break;

                                case "ZoneParentId":
                                    groupList = groupList.Where(g => ledgerBalanceFilter.ZonesSelected.Contains(g.LevelKey2)).ToList();
                                    break;

                                case "AdminId":
                                    groupList = groupList.Where(g => ledgerBalanceFilter.AdministratorsSelected.Contains(g.LevelKey2)).ToList();
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case 3:
                            switch (gb)
                            {
                                case "SectorId":
                                    groupList = groupList.Where(g => ledgerBalanceFilter.SectorsSelected.Contains(g.LevelKey3)).ToList();
                                    break;

                                case "ZoneParentId":
                                    groupList = groupList.Where(g => ledgerBalanceFilter.ZonesSelected.Contains(g.LevelKey3)).ToList();
                                    break;

                                case "AdminId":
                                    groupList = groupList.Where(g => ledgerBalanceFilter.AdministratorsSelected.Contains(g.LevelKey3)).ToList();
                                    break;

                                default:
                                    break;
                            }
                            break;

                        default:
                            break;
                    }
                }

                if (ledgerBalanceFilter.TopSelected != 0)
                {
                    return groupList.AsQueryable()
                        .OrderByDescending(s => s.TotBalanceAmt)
                        .Take(ledgerBalanceFilter.TopSelected)
                        .ToList();
                }
                else
                {
                    return groupList;
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Expression<Func<TItem, object>> GroupByExpression<TItem>(string[] propertyNames)
        {
            var properties = propertyNames.Select(name => typeof(TItem).GetProperty(name)).ToArray();
            var propertyTypes = properties.Select(p => p.PropertyType).ToArray();
            var tupleTypeDefinition = typeof(Tuple).Assembly.GetType("System.Tuple`" + properties.Length);
            var tupleType = tupleTypeDefinition.MakeGenericType(propertyTypes);
            var constructor = tupleType.GetConstructor(propertyTypes);
            var param = Expression.Parameter(typeof(TItem), "item");
            var body = Expression.New(constructor, properties.Select(p => Expression.Property(param, p)));
            var expr = Expression.Lambda<Func<TItem, object>>(body, param);
            return expr;
        }        

        private DTOSummaryRentReport NewSelect(IEnumerable<string> groupBy, IGrouping<object, BalanceTransRent> s, decimal totBalanceAmount)
        {
            DTOSummaryRentReport summaryRentReport = new DTOSummaryRentReport();

            int column = 1;

            foreach (var gb in groupBy)
            {
                var key = "";
                var value = "";
                var label = "";

                switch (gb)
                {
                    case "AdminId":
                        key = s.Max(s => s.AdminId.ToString());
                        value = s.Max(s => s.AdminName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_ADMINISTRATOR" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN ADMINISTRADOR" : value;
                        label = "ADMINISTRADOR";
                        break;

                    case "ZoneParentId":
                        key = s.Max(s => s.ZoneParentId.ToString());
                        value = s.Max(s => s.ZoneParentName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_ZONE" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN ZONA" : value;
                        label = "ZONA";
                        break;

                    case "SectorId":
                        key = s.Max(s => s.SectorId.ToString());
                        value = s.Max(s => s.SectorName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_SECTOR" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN SECTOR" : value;
                        label = "SECTOR";
                        break;

                    case "CustomerAccount":
                        key = s.Max(s => s.CustomerAccount.ToString());
                        value = s.Max(s => s.CustomerName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_CUSTOMER" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN CLIENTE" : value;
                        label = "CLIENTE";
                        break;

                    default:
                        break;
                }

                switch (column)
                {
                    case 1:
                        summaryRentReport.LevelKey1 = key.Trim();
                        summaryRentReport.Level1 = value.Trim();
                        summaryRentReport.LevelTitle1 = label.Trim();
                        break;

                    case 2:
                        summaryRentReport.LevelKey2 = key.Trim();
                        summaryRentReport.Level2 = value.Trim();
                        summaryRentReport.LevelTitle2 = label.Trim();
                        break;

                    case 3:
                        summaryRentReport.LevelKey3 = key.Trim();
                        summaryRentReport.Level3 = value.Trim();
                        summaryRentReport.LevelTitle3 = label.Trim();
                        break;

                    case 4:
                        summaryRentReport.LevelKey4 = key.Trim();
                        summaryRentReport.Level4 = value.Trim();
                        summaryRentReport.LevelTitle4 = label.Trim();
                        break;

                    default:
                        break;
                }

                column++;
            }

            summaryRentReport.BalanceAmt1 = s.Sum(s => s.BalanceAmount1);
            summaryRentReport.BalanceAmt2 = s.Sum(s => s.BalanceAmount2);
            summaryRentReport.BalanceAmt3 = s.Sum(s => s.BalanceAmount3);
            summaryRentReport.BalanceAmt4 = s.Sum(s => s.BalanceAmount4);
            summaryRentReport.BalanceAmt5 = s.Sum(s => s.BalanceAmount5);
            summaryRentReport.BalanceAmt6 = s.Sum(s => s.BalanceAmount6);
            summaryRentReport.BalanceAmt7 = s.Sum(s => s.BalanceAmount7);
            summaryRentReport.BalanceAmt8 = s.Sum(s => s.BalanceAmount8);
            summaryRentReport.BalanceAmt9 = s.Sum(s => s.BalanceAmount9);
            summaryRentReport.BalanceAmt10 = s.Sum(s => s.BalanceAmount10);
            summaryRentReport.BalanceAmt11 = s.Sum(s => s.BalanceAmount11);
            summaryRentReport.BalanceAmt12 = s.Sum(s => s.BalanceAmount12);

            summaryRentReport.TotBalanceAmt = summaryRentReport.BalanceAmt1;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt2;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt3;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt4;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt5;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt6;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt7;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt8;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt9;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt10;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt11;
            summaryRentReport.TotBalanceAmt += summaryRentReport.BalanceAmt12;

            if(totBalanceAmount != 0)
            {
                if (summaryRentReport.TotBalanceAmt != 0)
                    summaryRentReport.TotComp = summaryRentReport.TotBalanceAmt / totBalanceAmount;

                if (summaryRentReport.BalanceAmt1 != 0)
                    summaryRentReport.Comp1 = summaryRentReport.BalanceAmt1 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt2 != 0)
                    summaryRentReport.Comp2 = summaryRentReport.BalanceAmt2 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt3 != 0)
                    summaryRentReport.Comp3 = summaryRentReport.BalanceAmt3 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt4 != 0)
                    summaryRentReport.Comp4 = summaryRentReport.BalanceAmt4 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt5 != 0)
                    summaryRentReport.Comp5 = summaryRentReport.BalanceAmt5 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt6 != 0)
                    summaryRentReport.Comp6 = summaryRentReport.BalanceAmt6 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt7 != 0)
                    summaryRentReport.Comp7 = summaryRentReport.BalanceAmt7 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt8 != 0)
                    summaryRentReport.Comp8 = summaryRentReport.BalanceAmt8 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt9 != 0)
                    summaryRentReport.Comp9 = summaryRentReport.BalanceAmt9 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt10 != 0)
                    summaryRentReport.Comp10 = summaryRentReport.BalanceAmt10 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt11 != 0)
                    summaryRentReport.Comp11 = summaryRentReport.BalanceAmt11 / totBalanceAmount;

                if (summaryRentReport.BalanceAmt12 != 0)
                    summaryRentReport.Comp12 = summaryRentReport.BalanceAmt12 / totBalanceAmount;
            }

            return summaryRentReport;
        }

        public List<CommonDataTable> GetCommonDataTable(string categoryId)
        {
            try
            {
                var commonDataTable = _reportContext.CommonDataTable
                    .Where(c => c.CategoryId.Equals(categoryId))
                    .GroupBy(c => new { c.Id, c.Description, c.CommonDataTitle })
                    .Select(b => new CommonDataTable()
                    {
                        Id = b.Key.Id,
                        Description = b.Key.Description,
                        CommonDataTitle = b.Key.CommonDataTitle
                    })
                    .ToList();
                
                return commonDataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LedgerEstimate> GetLedgerEstimates(int year, int month, string categoryId)        
        {
            try
            {
                var ledgerEstimates = _dbContext.LedgerEstimates
                    .Where(b => b.Year.Equals(year)
                    && b.Month.Equals(month)
                    && b.CategoryId.Equals(categoryId))
                    .ToList();

                return ledgerEstimates;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BudgetRent> GetBudgetRents(int year, int month, string categoryId)
        {
            try
            {
                var budgetRents = _dbContext.BudgetRents
                    .Where(b => b.Year.Equals(year)
                    && b.Month.Equals(month)
                    && b.CategoryId.Equals(categoryId))
                    .ToList();

                return budgetRents;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LedgerEstimate CreateOrUpdate(LedgerEstimate ledgerEstimate)
        {
            try
            {
                if (ledgerEstimate.Id != 0)
                {
                    var ledgerEstimateUpd = _dbContext.LedgerEstimates
                        .Where(b => b.Id.Equals(ledgerEstimate.Id))
                        .FirstOrDefault();

                    ledgerEstimateUpd.AmountEstimate = ledgerEstimate.AmountEstimate;

                    _dbContext.LedgerEstimates.Update(ledgerEstimate);
                }
                else
                {
                    var ledgerEstimateUpd = _dbContext.LedgerEstimates
                        .Where(b => b.Year.Equals(ledgerEstimate.Year)
                        && b.Month.Equals(ledgerEstimate.Month)
                        && b.CategoryId.Equals(ledgerEstimate.CategoryId)
                        && b.CommonDataId.Equals(ledgerEstimate.CommonDataId))
                        .FirstOrDefault();
                    if (ledgerEstimateUpd is null)
                    {
                        var commonDataTable = _reportContext.CommonDataTable
                        .Where(c => c.CategoryId.Equals(ledgerEstimate.CategoryId)
                        && c.Id.Equals(ledgerEstimate.CommonDataId))
                        .FirstOrDefault();
                        if (commonDataTable != null)
                            ledgerEstimate.CommonDataName = commonDataTable.Description;
                        _dbContext.LedgerEstimates.Add(ledgerEstimate);
                    }
                }

                _dbContext.SaveChanges();
                _dbContext.DetachAll();

                return ledgerEstimate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BudgetRent CreateOrUpdate(BudgetRent budgetRent)
        {
            try
            {
                if (budgetRent.Id != 0)
                {
                    var budgetRentUpd = _dbContext.BudgetRents
                        .Where(b => b.Id.Equals(budgetRent.Id))
                        .FirstOrDefault();

                    budgetRentUpd.AmountBudget = budgetRent.AmountBudget;

                    _dbContext.BudgetRents.Update(budgetRent);
                }
                else
                {
                    var budgetRentUpd = _dbContext.BudgetRents
                        .Where(b => b.Year.Equals(budgetRent.Year)
                        && b.Month.Equals(budgetRent.Month)
                        && b.CategoryId.Equals(budgetRent.CategoryId)
                        && b.CommonDataId.Equals(budgetRent.CommonDataId)
                        && b.ZoneId.Equals(budgetRent.ZoneId))
                        .FirstOrDefault();
                    if(budgetRentUpd is null)
                    {
                        var commonDataTable = _reportContext.CommonDataTable
                        .Where(c => c.CategoryId.Equals(budgetRent.CategoryId)
                        && c.Id.Equals(budgetRent.CommonDataId))
                        .FirstOrDefault();                        
                        if (commonDataTable != null)
                            budgetRent.CommonDataName = commonDataTable.Description;

                        if(!string.IsNullOrEmpty(budgetRent.ZoneId))
                        {
                            var zone = _reportContext.CommonDataTable
                                .Where(c => c.CategoryId.Equals("ZoneParent")
                                    && c.Id.Equals(budgetRent.ZoneId))
                                .FirstOrDefault();

                            if (!(zone is null))
                                budgetRent.ZoneName = zone.Description;
                        }
                        

                        _dbContext.BudgetRents.Add(budgetRent);
                    }
                }

                _dbContext.SaveChanges();
                _dbContext.DetachAll();

                return budgetRent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool RemoveBudgetRent(BudgetRent budgetRent)
        {
            var budgetRentRem = _dbContext.BudgetRents
                        .Where(b => b.Id.Equals(budgetRent.Id))
                        .FirstOrDefault();

            if (budgetRentRem is null)
            {
                return false;
            }
            else
            {
                _dbContext.BudgetRents.Remove(budgetRentRem);
                _dbContext.SaveChanges();
                _dbContext.DetachAll();
                return true;
            }
        }

        public bool RemoveLedgerEstimate(LedgerEstimate ledgerEstimate)
        {
            var ledgerEstimateRem = _dbContext.LedgerEstimates
                        .Where(b => b.Id.Equals(ledgerEstimate.Id))
                        .FirstOrDefault();

            if (ledgerEstimateRem is null)
            {
                return false;
            }
            else
            {
                _dbContext.LedgerEstimates.Remove(ledgerEstimateRem);
                _dbContext.SaveChanges();
                _dbContext.DetachAll();
                return true;
            }
        }

        public CommonDataTable CreateOrUpdate(CommonDataTable commonDataTable)
        {
            try
            {                
                if (!string.IsNullOrEmpty(commonDataTable.Id))
                {
                    var commonDataTableUpd = _reportContext.CommonDataTable
                        .Where(b => b.Id.Equals(commonDataTable.Id))
                        .FirstOrDefault();

                    commonDataTableUpd.CommonDataTitle = commonDataTable.CommonDataTitle;

                    _reportContext.CommonDataTable.Update(commonDataTableUpd);
                }

                _reportContext.SaveChanges();
                _reportContext.DetachAll();

                return commonDataTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LastUpdateModule GetLastUpdateModule(string module)
        {
            var lastUpdateModule = _dbContext.SimexLastUpdateModuleLog
                .Where(s => s.Module == module)
                .OrderByDescending(l => l.LastUpdateModule_At)
                .FirstOrDefault();

            return lastUpdateModule;
        }

        
        public int UpdateCommercialData()
        {
            try
            {
                
                var lastUpdateModel = _dbContext.SimexLastUpdateModuleLog.Where(l => l.Module.Equals("Commercial")).FirstOrDefault();

                _reportContext.Database.SetCommandTimeout(1800);
                var response = _reportContext.Database.ExecuteSqlCommand("dbo.sp_UpdateCommercialData");

                if (!(lastUpdateModel is null))
                {
                    lastUpdateModel.LastUpdateModule_At = DateTime.Now;
                    _dbContext.Update(lastUpdateModel);
                    
                }
                else
                {
                    _dbContext.SimexLastUpdateModuleLog.Add(new LastUpdateModule()
                    {
                        Module = "Commercial",
                        LastUpdateModule_At = DateTime.Now
                    });
                }

                _dbContext.SaveChanges();

                _dbContext.DetachAll();
                _reportContext.DetachAll();

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

