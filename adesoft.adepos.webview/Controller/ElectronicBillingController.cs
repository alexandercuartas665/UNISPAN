using adesoft.adepos.Extensions;
using adesoft.adepos.webview.Data;
using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.DTO.ElectronicBilling;
using adesoft.adepos.webview.Data.Interfaces;
using adesoft.adepos.webview.Data.Model;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace adesoft.adepos.webview.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectronicBillingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IElectronicBillingService electronicBillingService;
        private AdeposDBContext _dbcontext;
        ConnectionDB connectionDB;

        public ElectronicBillingController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IElectronicBillingService electronicBillingService)
        {
            this.connectionDB = httpContextAccessor.HttpContext.Session.Get<ConnectionDB>("ConnectionDB");
            this._configuration = configuration;
            this.electronicBillingService = electronicBillingService;
            if (connectionDB == null)
                connectionDB = SecurityService.GetConnectionDefault();
            _dbcontext = new AdeposDBContext(connectionDB.Connection);
        }

        [HttpGet("download/{option}/{noteId}")]
        public IActionResult Download(int option, string noteId)
        {
            var attachFileName = "";
            switch(option)
            {
                case 1:
                    var salesInvoiceNote = _dbcontext.SalesInvoiceNotes
                        .Where(sin => sin.Id.Equals(noteId))
                        .FirstOrDefault();                    

                    if (salesInvoiceNote is null)
                        return NotFound();

                    attachFileName = salesInvoiceNote.AttachFileName;

                    break;

                case 2:
                    var closingsInvoicedNote = _dbcontext.ClosingInvoicedNotes
                        .Where(cin => cin.Id.Equals(noteId))
                        .FirstOrDefault();

                    if (closingsInvoicedNote is null)
                        return NotFound();

                    attachFileName = closingsInvoicedNote.AttachFileName;

                    break;

                case 3:
                    var opInvoicedNote = _dbcontext.OPInvoicedNotes
                        .Where(opin => opin.Id.Equals(noteId))
                        .FirstOrDefault();

                    if (opInvoicedNote is null)
                        return NotFound();

                    attachFileName = opInvoicedNote.AttachFileName;

                    break;
            }

            MemoryStream memory = new MemoryStream();
            using (var stream = new FileStream(attachFileName, FileMode.Open))
            {
                stream.CopyTo(memory);

                memory.Position = 0;
                return File(memory, "application/octet-stream", System.IO.Path.GetFileName(attachFileName)); // returns a FileStreamResult
            }
        }

        [HttpGet("GetPendingInvoices")]
        public IActionResult GetPendingInvoices([FromQuery] string guidfilter)
        {
            try
            {
                var pendingInvoices = this.electronicBillingService.GetPendingInvoiceReports(guidfilter);
                return Ok(pendingInvoices);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetSalesInvoices")]
        public IActionResult GetSalesInvoices([FromQuery] string guidfilter)
        {
            try
            {
                var salesInvoices = this.electronicBillingService.GetSalesInvoices(guidfilter);
                return Ok(salesInvoices);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetOPsInvoiced")]
        public IActionResult GetOPsInvoiced([FromQuery] string guidfilter)
        {
            try
            {
                var opsInvoiced = this.electronicBillingService.GetOPsInvoiced(guidfilter);
                return Ok(opsInvoiced);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetClosingsInvoiced")]
        public IActionResult GetClosingsInvoiced([FromQuery] string guidfilter)
        {
            try
            {
                var closingsInvoiced = this.electronicBillingService.GetClosingsInvoiced(guidfilter);
                return Ok(closingsInvoiced);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetSalesInvoiceTrackings")]
        public IActionResult GetSalesInvoiceTrackings([FromQuery] string guidfilter)
        {
            try
            {
                var salesInvoiceTrackings = this.electronicBillingService.GetSalesInvoiceTrackings(guidfilter);
                return Ok(salesInvoiceTrackings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
