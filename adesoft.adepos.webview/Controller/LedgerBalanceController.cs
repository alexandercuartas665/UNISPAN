using adesoft.adepos.webview.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerBalanceController : ControllerBase
    {
        private readonly ILedgerBalanceService _balanceService;

        public LedgerBalanceController(ILedgerBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpGet("GetSummaryRents")]
        public IActionResult GetSummaryRents(string guidfilter)
        {
            try
            {
                var summaryRentReportList = _balanceService.GetSummaryRents(guidfilter);
                return Ok(summaryRentReportList); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
}
