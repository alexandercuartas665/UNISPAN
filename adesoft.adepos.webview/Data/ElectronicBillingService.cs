using adesoft.adepos.webview.Data.DTO;
using adesoft.adepos.webview.Data.DTO.ElectronicBilling;
using adesoft.adepos.webview.Data.Interfaces;
using adesoft.adepos.webview.Data.Model;
using adesoft.adepos.webview.Data.Model.ElectronicBilling;
using adesoft.adepos.webview.Data.Model.PL;
using adesoft.adepos.webview.Pages.ElectronicBilling;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Radzen;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using Parameter = adesoft.adepos.webview.Data.Model.Parameter;
using Tuple = System.Tuple;

namespace adesoft.adepos.webview.Data
{
    public class ElectronicBillingService: IElectronicBillingService
    {
        private readonly AdeposDBContext _dbContext;
        private readonly AdeposReportsContext _dbReportsContext;
        private readonly ISqlManagerService _sqlManagerService;
        private readonly IConfiguration _configuration;

        public static List<DTOElectronicBillingFilter> reportFilters = new List<DTOElectronicBillingFilter>();

        public ElectronicBillingService(AdeposDBContext dbContext, AdeposReportsContext dbReportsContext, ISqlManagerService sqlManagerService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _dbReportsContext = dbReportsContext;
            _sqlManagerService = sqlManagerService;
            _configuration = configuration;
        }

        public void AddReportFilter(DTOElectronicBillingFilter filter)
        {
            reportFilters.Add(filter);
        }

        public DTOElectronicBillingFilter GetReportFilter(string reportFilterId)
        {
            try
            {
                DTOElectronicBillingFilter filter = reportFilters.Where(x => x.GuidFilter == reportFilterId).FirstOrDefault();
                return filter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DTOSalesInvoiceNote> CreateOrUpdateAsync(DTOSalesInvoiceNote dtoSalesInvoiceNote)
        {
            try
            {
                var salesInvoiceNote = new SalesInvoiceNote();
                if(!string.IsNullOrEmpty(dtoSalesInvoiceNote.Id))
                {
                    salesInvoiceNote = _dbContext.SalesInvoiceNotes
                        .Where(sin => sin.Id.Equals(dtoSalesInvoiceNote.Id))
                        .FirstOrDefault();

                    salesInvoiceNote.Note = dtoSalesInvoiceNote.Note;

                    _dbContext.SalesInvoiceNotes.Update(salesInvoiceNote);
                }
                else
                {
                    var salesInvoicesRepo = this._configuration.GetValue<string>("ElectronicBilling:RepoFiles");
                    var attachFileName = "";
                    if (!(dtoSalesInvoiceNote.Files is null) && (dtoSalesInvoiceNote.Files.Length != 0))
                    {
                        var file = dtoSalesInvoiceNote.Files[0];

                        attachFileName = string.Format("{0}/{1}", salesInvoicesRepo, file.Name);
                        using (var ms = new MemoryStream())
                        {
                            var reader = new System.IO.StreamReader(file.Data);
                            await reader.BaseStream.CopyToAsync(ms);

                            var fileBytes = ms.ToArray();
                            //string s = Convert.ToBase64String(fileBytes);

                            using (var imageFile = new FileStream(attachFileName, FileMode.Create))
                            {
                                imageFile.Write(fileBytes, 0, fileBytes.Length);
                                imageFile.Flush();


                            }
                        }
                    }

                    salesInvoiceNote = new SalesInvoiceNote()
                    {
                        CreatedAt = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        Note = dtoSalesInvoiceNote.Note,
                        SalesInvoiceId = dtoSalesInvoiceNote.SalesInvoiceId,
                        AttachFileName = attachFileName,
                        UserName = dtoSalesInvoiceNote.UserName,
                        NoteType = dtoSalesInvoiceNote.NoteType
                    };

                    _dbContext.SalesInvoiceNotes.Add(salesInvoiceNote);

                    var salesInvoice = _dbContext.SalesInvoices
                        .Where(si => si.Id.Equals(dtoSalesInvoiceNote.SalesInvoiceId))
                        .FirstOrDefault();                    

                    if (!(salesInvoice is null))
                    {
                        if (dtoSalesInvoiceNote.NoteType.Equals(NoteType.None))
                        {
                            salesInvoice.LastTrackingDate = salesInvoiceNote.CreatedAt;
                            salesInvoice.Note = dtoSalesInvoiceNote.Note;
                        }
                        else
                            salesInvoice.BillingNote = dtoSalesInvoiceNote.Note;

                        _dbContext.SalesInvoices.Update(salesInvoice);
                    }
                }

                _dbContext.SaveChanges();
                _dbContext.DetachAll();

                return this.GetSalesInvoiceNote(salesInvoiceNote.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOSalesInvoice CreateOrUpdate(DTOSalesInvoice dtoSalesInvoice)
        {
            try
            {
                var salesInvoice = new SalesInvoice();
                if (!string.IsNullOrEmpty(dtoSalesInvoice.Id))
                {
                    salesInvoice = _dbContext.SalesInvoices
                        .Where(sin => sin.Id.Equals(dtoSalesInvoice.Id))
                        .FirstOrDefault();

                    salesInvoice.TotalInvoiceAmount = dtoSalesInvoice.TotalInvoiceAmount;
                    salesInvoice.TotalBalanceAmount = dtoSalesInvoice.TotalBalanceAmount;
                    salesInvoice.RequiredActa = dtoSalesInvoice.RequiredActa;

                    _dbContext.SalesInvoices.Update(salesInvoice);
                }
                else
                {
                    salesInvoice = new SalesInvoice()
                    {
                        Id = Guid.NewGuid().ToString(),
                        AdminId = dtoSalesInvoice.AdminId,
                        AdminName = dtoSalesInvoice.AdminName,
                        CustomerName = dtoSalesInvoice.CustomerName,
                        CustomerNum = dtoSalesInvoice.CustomerNum,
                        Date = dtoSalesInvoice.Date,
                        EBNumber = dtoSalesInvoice.EBNumber,
                        EBDate = dtoSalesInvoice.EBDate,
                        InvoiceNum = dtoSalesInvoice.InvoiceNum,
                        NetAmountValue = dtoSalesInvoice.NetAmountValue,
                        PO = dtoSalesInvoice.PO,
                        Status = dtoSalesInvoice.Status,
                        TotalInvoiceAmount = dtoSalesInvoice.TotalInvoiceAmount,
                        WorkName = dtoSalesInvoice.WorkName,
                        WorkNo = dtoSalesInvoice.WorkNo,
                        RequiredActa = dtoSalesInvoice.RequiredActa
                    };

                    _dbContext.SalesInvoices.Add(salesInvoice);
                }

                _dbContext.SaveChanges();
                _dbContext.DetachAll();

                return this.GetSalesInvoice(salesInvoice.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOSalesInvoice GetSalesInvoice(string salesInvoiceId)
        {
            try
            {
                var salesInvoiceNotes = new List<DTOSalesInvoiceNote>(); 
                var salesInvoice = _dbContext.SalesInvoices
                    .Where(si => si.Id == salesInvoiceId)
                    .Select(si => new DTOSalesInvoice()
                    {
                        Id = si.Id,
                        AdminId = si.AdminId,
                        CustomerName = si.CustomerName,
                        CustomerNum = si.CustomerNum,
                        Date = si.Date,
                        InvoiceNum = si.InvoiceNum,
                        PO = si.PO,
                        WorkName = si.WorkName,
                        WorkNo = si.WorkNo,
                        SalesInvoiceNotes = salesInvoiceNotes,
                        EBNumber = si.EBNumber,
                        EBDate = si.EBDate,
                        AdminName = si.AdminName,
                        Status = si.Status,
                        Year = si.Date.Year,
                        Month = si.Date.Month,
                        Rent = si.Rent,
                        AdditionalCharges = si.AdditionalCharges,
                        ProductCharges = si.ProductCharges,
                        NetAmountValue = si.NetAmountValue,
                        TotalInvoiceAmount = si.TotalInvoiceAmount,
                        TotalBalanceAmount = si.TotalBalanceAmount,
                        ConfirmedDate = si.ConfirmedDate,
                        RequiredActa = si.RequiredActa,
                        ZoneId = si.ZoneId,
                        ZoneName = si.ZoneName,
                        ZoneParentId = si.ZoneParentId,
                        ZoneParentName = si.ZoneParentName,
                        CancelledDate = si.CancelledDate
                        
                    }).FirstOrDefault();

                salesInvoiceNotes = _dbContext.SalesInvoiceNotes
                    .Where(sin => sin.SalesInvoiceId == salesInvoiceId)
                    .Select(sin => new DTOSalesInvoiceNote()
                    {
                        CreatedAt = sin.CreatedAt,
                        Id = sin.Id,
                        Note = sin.Note,
                        SalesInvoiceId = sin.SalesInvoiceId,
                        UserName = sin.UserName,
                        AttachFileName = sin.AttachFileName,
                        NoteType = sin.NoteType
                    })
                    .OrderByDescending(sin => sin.CreatedAt)
                    .ToList();

                salesInvoice.SalesInvoiceNotes = salesInvoiceNotes;

                return salesInvoice;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            if (cell.CellValue is null)
                return "";

            var type = cell.CellValue.GetType();

            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }

            return value;
        }

        public void UploadFiles(Parameter parameter, string templateName)
        {           
            DataTable dataTable = null;

            Stream stream = new MemoryStream(parameter.FileBuffer);
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))
            {
                var sheet = document.WorkbookPart.Workbook.Sheets.Cast<Sheet>().Where(s => s.Name.Equals("Interface")).FirstOrDefault();
                if(!(sheet is null))
                {
                    //Get the Worksheet instance.
                    Worksheet worksheet = (document.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;

                    //Fetch all the rows present in the Worksheet.
                    IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();
                                       
                    switch (templateName)
                    {
                        case "EBProvision":
                            {
                                var tableName = "[dbo].[$SalesInvoices]";

                                var result = this.UploadFileSalesInvoice(document, rows);
                                var periodDate = (DateTime) result.GetType().GetProperty("PeriodDate").GetValue(result);
                                dataTable = (DataTable) result.GetType().GetProperty("DataTable").GetValue(result);

                                _sqlManagerService.DeleteRecords(tableName);
                                _sqlManagerService.BulkData(dataTable, tableName, "DefaultConnection");

                                _sqlManagerService.ExecuteNoQuery($"EXEC [dbo].[spUpdateProvision] @BillingDate = '{periodDate.ToString("yyyy-MM-dd")}'");

                                break;
                            }

                        default:
                            break;
                    }
                }
            }
        }

        public object UploadFileSalesInvoice(SpreadsheetDocument document, IEnumerable<Row> rows)
        {
            System.Data.DataTable dataTable = new DataTable(typeof(SalesInvoice).Name);
            List<PropertyInfo> columns = new List<PropertyInfo>();
            DateTime periodDate = DateTime.MinValue;
            int index = 2;

            foreach (var row in rows)
            {
                if (row.RowIndex.Value >= index)
                {
                    DataRow dataRow = dataTable.NewRow();
                    int columnIndex = 0;
                    bool createRow = false;                    

                    foreach (Cell cell in row.Descendants<Cell>())
                    {
                        int increment = 1;
                        int rowIndex = int.Parse(row.RowIndex.InnerText);
                        try
                        {                                                       
                            var value = this.GetValue(document, cell);

                            if (rowIndex.Equals(index))
                            {
                                if (columnIndex.Equals(0))
                                {
                                    dataTable.Columns.Add("Id", typeof(string));
                                    dataTable.Columns.Add("Period", typeof(string));
                                }

                                //EventLog evento;
                                //if (!EventLog.SourceExists("appUnispanLog"))
                                //    EventLog.CreateEventSource("appUnispanLog", "appUnispanLog");
                                //evento = new EventLog("appUnispanLog");
                                //evento.Source = "appUnispanLog";

                                //evento.WriteEntry($"|{value}|", EventLogEntryType.Error);


                                var propertyInfo = EBTemplate.GetPropertyInfo("EBProvision", value);
                                if (!(propertyInfo is null))
                                {
                                    var propertyType = propertyInfo.PropertyType;

                                    if (propertyInfo.Name.Equals("AdminName"))
                                    {
                                        dataTable.Columns.Add("AdminId", propertyType);
                                        dataTable.Columns.Add(propertyInfo.Name, propertyType);
                                    }
                                    else
                                    {
                                        if (propertyInfo.PropertyType.Equals(typeof(System.Decimal)))
                                            dataTable.Columns.Add(propertyInfo.Name, typeof(string));
                                        else
                                            dataTable.Columns.Add(propertyInfo.Name, propertyType);
                                    }

                                    
                                    columns.Add(propertyInfo);
                                }
                                else
                                    increment = 0;
                            }
                            else
                            {
                                if (columnIndex.Equals(0))
                                    dataRow["Id"] = Guid.NewGuid().ToString();

                                if (columnIndex < columns.Count())
                                {
                                    var propertyInfo = columns[columnIndex];
                                    if (propertyInfo.PropertyType.Equals(typeof(DateTime)))
                                    {
                                        periodDate = DateTime.FromOADate(int.Parse(value));
                                        dataRow[propertyInfo.Name] = periodDate;
                                        dataRow["Period"] = $"{periodDate.Year}-{periodDate.Month.ToString("D2")}";
                                    }
                                    else
                                    {
                                        if (propertyInfo.Name.Equals("AdminName"))
                                        {
                                            dataRow["AdminId"] = "";
                                            dataRow[propertyInfo.Name] = value;
                                        }
                                        else if (propertyInfo.Name.Equals("RequiredActa"))
                                        {
                                            dataRow[propertyInfo.Name] = value.ToLower().Equals("si");
                                        }
                                        else
                                        {
                                            dataRow[propertyInfo.Name] = value;
                                        }
                                    }

                                    createRow = true;
                                }
                            }

                            columnIndex += increment;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"{ex.Message} - Line({rowIndex}:{columnIndex})");
                        }
                    }

                    if (createRow)
                        dataTable.Rows.Add(dataRow);
                }
            }

            return new { PeriodDate = periodDate, DataTable = dataTable };
        }

        public List<int> GetYears()
        {
            try
            {
                List<int> years = new List<int>();

                var periods = _dbContext.SalesInvoices
                    .GroupBy(si => si.Date.Year)
                    .Select(si => si.Key)
                    .ToList();

                periods.ForEach( p => { years.Add(p); } );

                return years;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<object> GetMonths()
        {
            try
            {
                List<object> months = new List<object>();

                for (int i = 1; i <= 12; i++)
                {
                    string month = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(i);
                    months.Add(new { Id = i, Name = $"{char.ToUpper(month[0])}{month.Substring(1)}" });
                }

                return months;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetPeriods(int pendingType)
        {
            try
            {
                var periods = new List<string>();

                switch (pendingType)
                {
                    case 1:
                        periods = _dbContext.SalesInvoices
                            .GroupBy(si => si.Period)
                            .Select(si => si.Key)
                        .ToList();
                        break;

                    case 2:
                        periods = _dbContext.ClosingsInvoiced
                            .GroupBy(si => si.Period)
                            .Select(si => si.Key)
                        .ToList();
                        break;

                    case 3:
                        periods = _dbContext.OPsInvoiced
                            .GroupBy(si => si.Period)
                            .Select(si => si.Key)
                        .ToList();
                        break;

                    default:
                        break;
                }                

                var result = new List<string> { "Todos" };

                result.AddRange(periods.OrderByDescending(p => p).ToList());

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DateTime GetLastPeriod(int pendingType)
        {
            try
            {
                DateTime lastPeriod = DateTime.Now;

                switch (pendingType)
                {
                    case 1:
                        lastPeriod = _dbContext.SalesInvoices
                            .Select(si => si.Date)
                            .OrderByDescending(si => si.Date)
                            .FirstOrDefault();
                        break;

                    case 2:
                        lastPeriod = _dbContext.ClosingsInvoiced
                            .Select(si => si.Date)
                            .OrderByDescending(si => si.Date)
                            .FirstOrDefault();
                        break;

                    case 3:
                        lastPeriod = _dbContext.OPsInvoiced
                            .Select(si => si.Date)
                            .OrderByDescending(si => si.Date)
                            .FirstOrDefault();
                        break;

                    default:
                        break;
                }

                return lastPeriod;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DTOSalesInvoice> GetSalesInvoices(DTOElectronicBillingFilter filter)
        {
            try
            {
                var salesInvoices = new List<DTOSalesInvoice>();

                if (filter.CalcOutsBalance)
                {
                    salesInvoices = _dbContext.SalesInvoices
                    .Where(si => (si.Date < filter.FromDate)
                    && si.Status.Equals(SalesInvoiceStatus.None)
                    && ((filter.POs == null) || filter.POs.ToList().Contains(si.PO))
                    && (string.IsNullOrEmpty(filter.AdminId) || si.AdminId.Equals(filter.AdminId))
                    && (si.EBDate.Year == 1))
                    //&& (si.AdminName.Equals("SAAVEDRA CARDOZO MIGUEL ANGEL")))
                    .Select(si => new DTOSalesInvoice()
                    {
                        Id = si.Id,
                        AdminId = si.AdminId,
                        CustomerName = si.CustomerName,
                        CustomerNum = si.CustomerNum,
                        Date = si.Date,
                        DateFilter = si.Date.ToString("dd/MM/yyyy"),
                        InvoiceNum = si.InvoiceNum,
                        PO = si.PO,
                        WorkName = si.WorkName,
                        WorkNo = si.WorkNo,
                        EBDate = si.EBDate,
                        EBNumber = si.EBNumber,
                        Status = si.Status,
                        Year = si.Date.Year,
                        Month = si.Date.Month,
                        Rent = si.Rent,
                        AdditionalCharges = si.AdditionalCharges,
                        ProductCharges = si.ProductCharges,
                        NetAmountValue = si.NetAmountValue,
                        TotalInvoiceAmount = si.TotalInvoiceAmount,
                        TotalBalanceAmount = si.TotalBalanceAmount,
                        AdminName = si.AdminName,
                        Note = si.Note,
                        BillingNote = si.BillingNote,
                        RequiredActa = si.RequiredActa,
                        ZoneId = si.ZoneId,
                        ZoneName = si.ZoneName,
                        ZoneParentId = si.ZoneParentId,
                        ZoneParentName = si.ZoneParentName,
                        LastTrackingDate = si.LastTrackingDate,
                        Period = si.Period
                    }).ToList();

                    var partials = _dbContext.SalesInvoices
                    .Where(si => (si.Date < filter.FromDate)
                    && si.Status.Equals(SalesInvoiceStatus.None)
                    && ((filter.POs == null) || filter.POs.ToList().Contains(si.PO))
                    && (string.IsNullOrEmpty(filter.AdminId) || si.AdminId.Equals(filter.AdminId))
                    && (si.EBDate.Year > 1))
                    //&& (si.AdminName.Equals("SAAVEDRA CARDOZO MIGUEL ANGEL")))
                    .Select(si => new DTOSalesInvoice()
                    {
                        Id = si.Id,
                        AdminId = si.AdminId,
                        CustomerName = si.CustomerName,
                        CustomerNum = si.CustomerNum,
                        Date = si.Date,
                        DateFilter = si.Date.ToString("dd/MM/yyyy"),
                        InvoiceNum = si.InvoiceNum,
                        PO = si.PO,
                        WorkName = si.WorkName,
                        WorkNo = si.WorkNo,
                        EBDate = si.EBDate,
                        EBNumber = si.EBNumber,
                        Status = si.Status,
                        Year = si.Date.Year,
                        Month = si.Date.Month,
                        Rent = si.Rent,
                        AdditionalCharges = si.AdditionalCharges,
                        ProductCharges = si.ProductCharges,
                        NetAmountValue = 0,
                        TotalInvoiceAmount = si.TotalBalanceAmount,
                        TotalBalanceAmount = 0,
                        AdminName = si.AdminName,
                        Note = si.Note,
                        BillingNote = si.BillingNote,
                        RequiredActa = si.RequiredActa,
                        ZoneId = si.ZoneId,
                        ZoneName = si.ZoneName,
                        ZoneParentId = si.ZoneParentId,
                        ZoneParentName = si.ZoneParentName,
                        LastTrackingDate = si.LastTrackingDate,
                        Period = si.Period
                    }).ToList();

                    salesInvoices.AddRange(partials);
                }
                else
                {
                    switch (((SalesInvoiceStatus)filter.Status))
                    {
                        case SalesInvoiceStatus.None:
                            {
                                var pendings = _dbContext.SalesInvoices
                                    .Where(si => (string.IsNullOrEmpty(filter.Period) || filter.Period.Equals("Todos") || si.Period.Equals(filter.Period))
                                        && si.Status.Equals(SalesInvoiceStatus.None)
                                        && ((filter.POs == null) || filter.POs.ToList().Contains(si.PO))
                                        && (string.IsNullOrEmpty(filter.AdminId) || si.AdminId.Equals(filter.AdminId))
                                        && (si.EBDate.Year == 1))
                                        //&& (si.AdminName.Equals("PAEZ TAPIA YONAIRO")))
                                    .Select(si => new DTOSalesInvoice()
                                    {
                                        Id = si.Id,
                                        AdminId = si.AdminId,
                                        CustomerName = si.CustomerName,
                                        CustomerNum = si.CustomerNum,
                                        Date = si.Date,
                                        DateFilter = si.Date.ToString("dd/MM/yyyy"),
                                        InvoiceNum = si.InvoiceNum,
                                        PO = si.PO,
                                        WorkName = si.WorkName,
                                        WorkNo = si.WorkNo,
                                        EBDate = si.EBDate,
                                        EBNumber = si.EBNumber,
                                        Status = si.Status,
                                        Confirmed = si.Status.Equals(SalesInvoiceStatus.Confirmed),
                                        Year = si.Date.Year,
                                        Month = si.Date.Month,
                                        Rent = si.Rent,
                                        AdditionalCharges = si.AdditionalCharges,
                                        ProductCharges = si.ProductCharges,
                                        NetAmountValue = si.NetAmountValue,
                                        TotalInvoiceAmount = si.TotalInvoiceAmount,
                                        TotalBalanceAmount = si.TotalBalanceAmount,
                                        AdminName = si.AdminName,
                                        Note = si.Note,
                                        BillingNote = si.BillingNote,
                                        RequiredActa = si.RequiredActa,
                                        ZoneId = si.ZoneId,
                                        ZoneName = si.ZoneName,
                                        ZoneParentId = si.ZoneParentId,
                                        ZoneParentName = si.ZoneParentName,
                                        LastTrackingDate = si.LastTrackingDate,
                                        QtyPending = 1,
                                        Period = si.Period
                                    })
                                    .ToList();

                                salesInvoices.AddRange(pendings);

                                var partials = _dbContext.SalesInvoices
                                    .Where(si => (string.IsNullOrEmpty(filter.Period) || filter.Period.Equals("Todos") || si.Period.Equals(filter.Period))
                                        && si.Status.Equals(SalesInvoiceStatus.None)
                                        && ((filter.POs == null) || filter.POs.ToList().Contains(si.PO))
                                        && (string.IsNullOrEmpty(filter.AdminId) || si.AdminId.Equals(filter.AdminId))
                                        && (si.EBDate.Year > 1))
                                        //&& (si.AdminName.Equals("PAEZ TAPIA YONAIRO")))
                                    .Select(si => new DTOSalesInvoice()
                                    {
                                        Id = si.Id,
                                        AdminId = si.AdminId,
                                        CustomerName = si.CustomerName,
                                        CustomerNum = si.CustomerNum,
                                        Date = si.Date,
                                        DateFilter = si.Date.ToString("dd/MM/yyyy"),
                                        InvoiceNum = si.InvoiceNum,
                                        PO = si.PO,
                                        WorkName = si.WorkName,
                                        WorkNo = si.WorkNo,
                                        EBDate = si.EBDate,
                                        EBNumber = si.EBNumber,
                                        Status = si.Status,
                                        Confirmed = si.Status.Equals(SalesInvoiceStatus.Confirmed),
                                        Year = si.Date.Year,
                                        Month = si.Date.Month,
                                        Rent = si.Rent,
                                        AdditionalCharges = si.AdditionalCharges,
                                        ProductCharges = si.ProductCharges,
                                        NetAmountValue = si.NetAmountValue,
                                        TotalInvoiceAmount = si.TotalInvoiceAmount,
                                        TotalBalanceAmount = si.TotalBalanceAmount,
                                        AdminName = si.AdminName,
                                        Note = si.Note,
                                        BillingNote = si.BillingNote,
                                        RequiredActa = si.RequiredActa,
                                        ZoneId = si.ZoneId,
                                        ZoneName = si.ZoneName,
                                        ZoneParentId = si.ZoneParentId,
                                        ZoneParentName = si.ZoneParentName,
                                        LastTrackingDate = si.LastTrackingDate,
                                        Partial = true,
                                        QtyPending = 1,
                                        Period = si.Period
                                    })
                                    .ToList();

                                salesInvoices.AddRange(partials);

                                break;
                            }

                        case SalesInvoiceStatus.Confirmed:
                            {
                                var confirmeds = _dbContext.SalesInvoices
                                    .Where(si => (string.IsNullOrEmpty(filter.Period) || filter.Period.Equals("Todos") || si.Period.Equals(filter.Period))
                                    && ((filter.POs == null) || filter.POs.ToList().Contains(si.PO))
                                    && (string.IsNullOrEmpty(filter.AdminId) || si.AdminId.Equals(filter.AdminId))
                                    && (si.EBDate.Year > 1))
                                    .Select(si => new DTOSalesInvoice()
                                    {
                                        Id = si.Id,
                                        AdminId = si.AdminId,
                                        CustomerName = si.CustomerName,
                                        CustomerNum = si.CustomerNum,
                                        Date = si.Date,
                                        DateFilter = si.Date.ToString("dd/MM/yyyy"),
                                        InvoiceNum = si.InvoiceNum,
                                        PO = si.PO,
                                        WorkName = si.WorkName,
                                        WorkNo = si.WorkNo,
                                        EBDate = si.EBDate,
                                        EBNumber = si.EBNumber,
                                        Status = si.Status,
                                        Confirmed = si.Status.Equals(SalesInvoiceStatus.Confirmed),
                                        Year = si.Date.Year,
                                        Month = si.Date.Month,
                                        Rent = si.Rent,
                                        AdditionalCharges = si.AdditionalCharges,
                                        ProductCharges = si.ProductCharges,
                                        NetAmountValue = si.NetAmountValue,
                                        TotalInvoiceAmount = si.TotalInvoiceAmount,
                                        TotalBalanceAmount = si.TotalBalanceAmount,
                                        AdminName = si.AdminName,
                                        Note = si.Note,
                                        BillingNote = si.BillingNote,
                                        RequiredActa = si.RequiredActa,
                                        ZoneId = si.ZoneId,
                                        ZoneName = si.ZoneName,
                                        ZoneParentId = si.ZoneParentId,
                                        ZoneParentName = si.ZoneParentName,
                                        LastTrackingDate = si.LastTrackingDate,
                                        QtyInvoiced = 1,
                                        Period = si.Period
                                    })
                                    .ToList();

                                salesInvoices.AddRange(confirmeds);

                                break;
                            }

                        case SalesInvoiceStatus.Cancelled:
                            {
                                var cancelleds = _dbContext.SalesInvoices
                                    .Where(si => (string.IsNullOrEmpty(filter.Period) || filter.Period.Equals("Todos") || si.Period.Equals(filter.Period))
                                    && ((filter.POs == null) || filter.POs.ToList().Contains(si.PO))
                                    && (string.IsNullOrEmpty(filter.AdminId) || si.AdminId.Equals(filter.AdminId))
                                    && (si.Status.Equals(SalesInvoiceStatus.Cancelled)))
                                    .Select(si => new DTOSalesInvoice()
                                    {
                                        Id = si.Id,
                                        AdminId = si.AdminId,
                                        CustomerName = si.CustomerName,
                                        CustomerNum = si.CustomerNum,
                                        Date = si.Date,
                                        DateFilter = si.Date.ToString("dd/MM/yyyy"),
                                        InvoiceNum = si.InvoiceNum,
                                        PO = si.PO,
                                        WorkName = si.WorkName,
                                        WorkNo = si.WorkNo,
                                        EBDate = si.EBDate,
                                        EBNumber = si.EBNumber,
                                        Status = si.Status,
                                        Confirmed = si.Status.Equals(SalesInvoiceStatus.Confirmed),
                                        Year = si.Date.Year,
                                        Month = si.Date.Month,
                                        Rent = si.Rent,
                                        AdditionalCharges = si.AdditionalCharges,
                                        ProductCharges = si.ProductCharges,
                                        NetAmountValue = si.NetAmountValue,
                                        TotalInvoiceAmount = si.TotalInvoiceAmount,
                                        TotalBalanceAmount = si.TotalBalanceAmount,
                                        AdminName = si.AdminName,
                                        Note = si.Note,
                                        BillingNote = si.BillingNote,
                                        RequiredActa = si.RequiredActa,
                                        ZoneId = si.ZoneId,
                                        ZoneName = si.ZoneName,
                                        ZoneParentId = si.ZoneParentId,
                                        ZoneParentName = si.ZoneParentName,
                                        LastTrackingDate = si.LastTrackingDate,
                                        QtyCancelled = 1,
                                        Period = si.Period
                                    })
                                    .ToList();

                                salesInvoices.AddRange(cancelleds);

                                break;
                            }

                        default:
                            break;
                    }                                                          
                }

                return salesInvoices.OrderBy(s => s.Date).ToList();
            }
            catch (Exception)
            {
                throw;
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

        public async Task<DTOSalesInvoice> ConfirmAsync(DTOConfirmSalesInvoice confirmSalesInvoice)
        {
            var ebNumber = _dbReportsContext.vwEBNumbers
                                .Where(n => n.CustomerNum.Equals(confirmSalesInvoice.SalesInvoice.CustomerNum)
                                && n.InvoiceNum.Equals(confirmSalesInvoice.SalesInvoice.InvoiceNum))
                                .FirstOrDefault();

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    confirmSalesInvoice.SalesInvoice = this.CreateOrUpdate(confirmSalesInvoice.SalesInvoice);

                    var salesInvoice = new DTOSalesInvoice();
                    var salesInvoiceUpd = _dbContext.SalesInvoices
                        .Where(si => si.Id.Equals(confirmSalesInvoice.SalesInvoice.Id))
                        .FirstOrDefault();

                    if (!(salesInvoiceUpd is null))
                    {
                        var electronicBillingRepo = this._configuration.GetValue<string>("ElectronicBilling:RepoFiles");
                        var attachFileName = "";
                        if (!(confirmSalesInvoice.Files is null) && (confirmSalesInvoice.Files.Length != 0))
                        {
                            var file = confirmSalesInvoice.Files[0];
                            var ext = Path.GetExtension(file.Name);

                            attachFileName = string.Format("{0}/{1}{2}", electronicBillingRepo, Guid.NewGuid().ToString(), ext);
                            using (var ms = new MemoryStream())
                            {
                                var reader = new System.IO.StreamReader(file.Data);
                                await reader.BaseStream.CopyToAsync(ms);

                                var fileBytes = ms.ToArray();
                                //string s = Convert.ToBase64String(fileBytes);

                                using (var imageFile = new FileStream(attachFileName, FileMode.Create))
                                {
                                    imageFile.Write(fileBytes, 0, fileBytes.Length);
                                    imageFile.Flush();
                                }
                            }
                        }

                        var note = new SalesInvoiceNote()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Note = confirmSalesInvoice.Note,
                            AttachFileName = attachFileName,
                            CreatedAt = DateTime.Now,
                            UserName = confirmSalesInvoice.UserName,
                            SalesInvoiceId = confirmSalesInvoice.SalesInvoice.Id,
                            NoteType = NoteType.Billing
                        };

                        _dbContext.SalesInvoiceNotes.Add(note);

                        if(salesInvoiceUpd.TotalBalanceAmount <= 0)
                        {
                            salesInvoiceUpd.Status = SalesInvoiceStatus.Confirmed;
                            salesInvoiceUpd.ConfirmedDate = DateTime.Now;
                        }

                        salesInvoiceUpd.BillingNote = confirmSalesInvoice.Note;                        

                        if (!(ebNumber is null) && !string.IsNullOrEmpty(ebNumber.EBNumber.Trim()))
                        {
                            salesInvoiceUpd.EBNumber = ebNumber.EBNumber.Trim();
                            salesInvoiceUpd.EBDate = ebNumber.EBDate;
                        }
                        else
                        {
                            salesInvoiceUpd.EBDate = DateTime.Now;
                        }

                        _dbContext.SalesInvoices.Update(salesInvoiceUpd);

                        _dbContext.SaveChanges();
                        _dbContext.DetachAll();

                        salesInvoice = this.GetSalesInvoice(salesInvoiceUpd.Id);
                    }

                    transaction.Commit();

                    return salesInvoice;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<DTOSalesInvoice> CancelAsync(DTOConfirmSalesInvoice confirmSalesInvoice)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    confirmSalesInvoice.SalesInvoice = this.CreateOrUpdate(confirmSalesInvoice.SalesInvoice);

                    var salesInvoice = new DTOSalesInvoice();
                    var salesInvoiceUpd = _dbContext.SalesInvoices
                        .Where(si => si.Id.Equals(confirmSalesInvoice.SalesInvoice.Id))
                        .FirstOrDefault();

                    if (!(salesInvoiceUpd is null))
                    {
                        var electronicBillingRepo = this._configuration.GetValue<string>("ElectronicBilling:RepoFiles");
                        var attachFileName = "";
                        if (!(confirmSalesInvoice.Files is null) && (confirmSalesInvoice.Files.Length != 0))
                        {
                            var file = confirmSalesInvoice.Files[0];
                            var ext = Path.GetExtension(file.Name);

                            attachFileName = string.Format("{0}/{1}{2}", electronicBillingRepo, Guid.NewGuid().ToString(), ext);
                            using (var ms = new MemoryStream())
                            {
                                var reader = new System.IO.StreamReader(file.Data);
                                await reader.BaseStream.CopyToAsync(ms);

                                var fileBytes = ms.ToArray();
                                //string s = Convert.ToBase64String(fileBytes);

                                using (var imageFile = new FileStream(attachFileName, FileMode.Create))
                                {
                                    imageFile.Write(fileBytes, 0, fileBytes.Length);
                                    imageFile.Flush();
                                }
                            }
                        }

                        var note = new SalesInvoiceNote()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Note = confirmSalesInvoice.Note,
                            AttachFileName = attachFileName,
                            CreatedAt = DateTime.Now,
                            UserName = confirmSalesInvoice.UserName,
                            SalesInvoiceId = confirmSalesInvoice.SalesInvoice.Id,
                            NoteType = NoteType.Billing
                        };

                        _dbContext.SalesInvoiceNotes.Add(note);

                        salesInvoiceUpd.Status = SalesInvoiceStatus.Cancelled;
                        salesInvoiceUpd.CancelledDate = DateTime.Now;
                        salesInvoiceUpd.BillingNote = confirmSalesInvoice.Note;
                        salesInvoiceUpd.TotalBalanceAmount = salesInvoiceUpd.TotalBalanceAmount.Equals(0) ?
                            salesInvoiceUpd.TotalInvoiceAmount : salesInvoiceUpd.TotalBalanceAmount;
                        salesInvoiceUpd.TotalInvoiceAmount = salesInvoiceUpd.TotalBalanceAmount.Equals(salesInvoiceUpd.TotalInvoiceAmount) ? 0 : salesInvoiceUpd.TotalInvoiceAmount;

                        _dbContext.SalesInvoices.Update(salesInvoiceUpd);

                        _dbContext.SaveChanges();
                        _dbContext.DetachAll();

                        salesInvoice = this.GetSalesInvoice(salesInvoiceUpd.Id);
                    }

                    transaction.Commit();

                    return salesInvoice;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public DTOSalesInvoiceNote GetSalesInvoiceNote(string  salesInvoiceNoteId)
        {
            try
            {
                var salesInvoiceNote = _dbContext.SalesInvoiceNotes
                    .Where(sin => sin.Id.Equals(salesInvoiceNoteId))
                    .FirstOrDefault();

                return new DTOSalesInvoiceNote()
                {
                    Id = salesInvoiceNote.Id,
                    AttachFileName = salesInvoiceNote.AttachFileName,
                    CreatedAt = salesInvoiceNote.CreatedAt,
                    Note = salesInvoiceNote.Note,
                    SalesInvoiceId = salesInvoiceNote.SalesInvoiceId,
                    UserName = salesInvoiceNote.UserName
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DTOPendingInvoiceReport> GetPendingInvoiceReports(string guidfilter)
        {
            try
            {
                string reportName = "PENDIENTES DE FACTURACION - ";

                DTOElectronicBillingFilter reportFilter = this.GetReportFilter(guidfilter);
                List<DTOPendingInvoiceReport> pendingInvoices = new List<DTOPendingInvoiceReport>();

                switch (reportFilter.Option)
                {
                    case 1:
                        {
                            List<DTOSalesInvoice> salesInvoices = new List<DTOSalesInvoice> ();
                            if(reportFilter.ShowBalance)
                            {
                                var month = reportFilter.MonthsSelected.OrderBy(m => m).FirstOrDefault();
                                var fromDate = new DateTime(reportFilter.Year, month, 1);
                                reportFilter.FromDate = fromDate;
                                reportFilter.CalcOutsBalance = true;
                                var outstanding = this.GetSalesInvoices(reportFilter);
                                outstanding.ForEach(p =>
                                {                                    
                                    p.TotalOutstandingBalance = p.TotalInvoiceAmount;
                                    p.TotalInvoiceAmount = 0;
                                    p.NetAmountValue = 0;
                                    p.TotalBalanceAmount = 0;
                                });

                                salesInvoices.AddRange(outstanding);
                            }

                            reportFilter.CalcOutsBalance = false;

                            reportName += "ARRIENDOS";
                            string periodTitle = "";

                            foreach (var month in reportFilter.MonthsSelected.OrderBy(m => m).ToList())
                            {
                                var period = new DateTime(reportFilter.Year, month, 1).ToString("yyyy-MM");
                                periodTitle += string.IsNullOrEmpty(periodTitle) ? period : $" | {period}";

                                reportFilter.Status = 0;
                                reportFilter.Period = period;
                                var pendings = this.GetSalesInvoices(reportFilter);
                                // Pending net amount
                                pendings.Where(p => !p.Partial).ToList().ForEach(p => 
                                {
                                    p.TotalPendingAmountBalance = p.NetAmountValue;
                                    p.TotalBalanceAmount = 0;
                                });
                                // Partial net amount
                                pendings.Where(p => p.Partial).ToList().ForEach(p =>
                                {
                                    p.TotalPendingAmountBalance = p.TotalBalanceAmount;
                                    p.TotalBalanceAmount = 0;
                                });

                                reportFilter.Status = 1;
                                var confirms = this.GetSalesInvoices(reportFilter);
                                confirms.ForEach(p =>
                                {
                                    p.TotalInvoiceAmountBalance = p.TotalInvoiceAmount;                                      
                                    p.TotalBalanceAmount = 0;
                                    if(p.Status.Equals(SalesInvoiceStatus.Cancelled))
                                    {
                                        p.NetAmountValue = 0;
                                    }
                                    else if (p.Status.Equals(SalesInvoiceStatus.None))
                                    {
                                        p.NetAmountValue = 0;
                                    }
                                });

                                reportFilter.Status = 2;
                                var cancelleds = this.GetSalesInvoices(reportFilter);
                                //cancelleds.ForEach(p =>
                                //{                                    
                                //    //p.TotalInvoiceAmountBalance = p.TotalInvoiceAmount;
                                //    //p.QtyInvoiced = 1;
                                //    //if (p.TotalInvoiceAmount.Equals(0))
                                //    //    p.QtyCancelled = 1;

                                //    //if (p.TotalBalanceAmount < 0)
                                //    //    p.TotalBalanceAmount = 0;
                                //    //else
                                //    //{
                                //    //    if (!p.Status.Equals(SalesInvoiceStatus.Cancelled))
                                //    //        p.TotalBalanceAmount = 0;
                                //    //}

                                //});

                                salesInvoices.AddRange(pendings);
                                salesInvoices.AddRange(confirms);
                                salesInvoices.AddRange(cancelleds);
                            }

                            var lambda = this.GroupByExpression<DTOSalesInvoice>(reportFilter.GroupBy.ToArray());

                            pendingInvoices = salesInvoices.AsQueryable()
                                .GroupBy(lambda.Compile())
                                .Select(b => this.Select(reportFilter.GroupBy, b))
                                .ToList();            
                            
                            if(pendingInvoices.Count != 0)
                            {
                                foreach (var pendingInvoice in pendingInvoices)
                                {
                                    pendingInvoice.ReportName = reportName;
                                    pendingInvoice.PeriodTitle = $"TOTAL {periodTitle}";
                                    pendingInvoice.ShowBalance = reportFilter.ShowBalance ? 1 : 0;

                                    pendingInvoice.PercPending = pendingInvoice.TotalNetAmount.Equals(0) ? 0 : ((pendingInvoice.TotalPendingAmountBalance) / pendingInvoice.TotalNetAmount);
                                    pendingInvoice.PercInvoiced = pendingInvoice.TotalInvoiceAmount.Equals(0) ? 0 : ((pendingInvoice.TotalInvoiceAmountBalance) / pendingInvoice.TotalNetAmount);

                                    pendingInvoice.ActualBalanceOutstanding = pendingInvoice.PrevBalanceOutstanding + pendingInvoice.TotalPendingAmountBalance;
                                }
                            }
                            else
                            {
                                pendingInvoices.Add(new DTOPendingInvoiceReport 
                                { 
                                    ReportName = reportName, 
                                    PeriodTitle = $"TOTAL {periodTitle}",
                                    ShowBalance = reportFilter.ShowBalance ? 1 : 0,
                                    GroupBy1Value = "NO_DATA"
                                });
                            }
                            

                            break;
                        }

                    case 2:
                        {
                            List<DTOClosingInvoiced> closingsInvoiced = new List<DTOClosingInvoiced>();
                            if (reportFilter.ShowBalance)
                            {
                                var month = reportFilter.MonthsSelected.OrderBy(m => m).FirstOrDefault();
                                var fromDate = new DateTime(reportFilter.Year, month, 1);
                                reportFilter.FromDate = fromDate;
                                reportFilter.CalcOutsBalance = true;
                                var outstanding = this.GetClosingsInvoiced(reportFilter);
                                outstanding.ForEach(p =>
                                {
                                    p.TotalOutstandingBalance = p.TotalInvoiceAmount;
                                    p.TotalInvoiceAmount = 0;
                                    p.NetAmountValue = 0;
                                });

                                closingsInvoiced.AddRange(outstanding);
                            }

                            reportFilter.CalcOutsBalance = false;

                            reportName += "CIERRES";
                            string periodTitle = "";

                            foreach (var month in reportFilter.MonthsSelected.OrderBy(m => m).ToList())
                            {
                                var period = new DateTime(reportFilter.Year, month, 1).ToString("yyyy-MM");
                                periodTitle += string.IsNullOrEmpty(periodTitle) ? period : $" | {period}";

                                reportFilter.Status = 0;
                                reportFilter.Period = period;
                                var pendings = this.GetClosingsInvoiced(reportFilter);
                                pendings.ForEach(p =>
                                {
                                    p.TotalPendingAmountBalance = p.TotalInvoiceAmount;
                                    p.QtyPending = 1;
                                });

                                reportFilter.Status = 1;
                                var confirms = this.GetClosingsInvoiced(reportFilter);
                                confirms.ForEach(p =>
                                {
                                    p.TotalInvoiceAmountBalance = p.TotalInvoiceAmount;
                                    p.QtyInvoiced = 1;
                                });

                                closingsInvoiced.AddRange(pendings);
                                closingsInvoiced.AddRange(confirms);
                            }

                            var lambda = this.GroupByExpression<DTOClosingInvoiced>(reportFilter.GroupBy.ToArray());

                            pendingInvoices = closingsInvoiced.AsQueryable()
                                .GroupBy(lambda.Compile())
                                .Select(b => this.Select(reportFilter.GroupBy, b))
                                .ToList();                            

                            if (pendingInvoices.Count != 0)
                            {
                                foreach (var pendingInvoice in pendingInvoices)
                                {
                                    pendingInvoice.ReportName = reportName;
                                    pendingInvoice.PeriodTitle = $"TOTAL {periodTitle}";
                                    pendingInvoice.ShowBalance = reportFilter.ShowBalance ? 1 : 0;

                                    pendingInvoice.PercPending = pendingInvoice.TotalNetAmount.Equals(0) ? 0 : ((pendingInvoice.TotalPendingAmountBalance) / pendingInvoice.TotalNetAmount);
                                    pendingInvoice.PercInvoiced = pendingInvoice.TotalNetAmount.Equals(0) ? 0 : ((pendingInvoice.TotalInvoiceAmountBalance) / pendingInvoice.TotalNetAmount);

                                    pendingInvoice.ActualBalanceOutstanding = pendingInvoice.PrevBalanceOutstanding + pendingInvoice.TotalPendingAmountBalance;
                                }
                            }
                            else
                            {
                                pendingInvoices.Add(new DTOPendingInvoiceReport
                                {
                                    ReportName = reportName,
                                    PeriodTitle = $"TOTAL {periodTitle}",
                                    ShowBalance = reportFilter.ShowBalance ? 1 : 0,
                                    GroupBy1Value = "NO_DATA"
                                });
                            }

                            break;
                        }

                    case 3:
                        {
                            List<DTOOPInvoiced> opsInvoiced = new List<DTOOPInvoiced>();
                            if (reportFilter.ShowBalance)
                            {
                                var month = reportFilter.MonthsSelected.OrderBy(m => m).FirstOrDefault();
                                var fromDate = new DateTime(reportFilter.Year, month, 1);
                                reportFilter.FromDate = fromDate;
                                reportFilter.CalcOutsBalance = true;
                                var outstanding = this.GetOPsInvoiced(reportFilter);
                                outstanding.ForEach(p =>
                                {
                                    p.TotalOutstandingBalance = p.TotalInvoiceAmount;
                                    p.TotalInvoiceAmount = 0;
                                    p.NetAmountValue = 0;
                                });

                                opsInvoiced.AddRange(outstanding);
                            }

                            reportFilter.CalcOutsBalance = false;

                            reportName += "ORDENES DE PRODUCCION";
                            string periodTitle = "";

                            foreach (var month in reportFilter.MonthsSelected.OrderBy(m => m).ToList())
                            {
                                var period = new DateTime(reportFilter.Year, month, 1).ToString("yyyy-MM");
                                periodTitle += string.IsNullOrEmpty(periodTitle) ? period : $" | {period}";

                                reportFilter.Status = 0;
                                reportFilter.Period = period;
                                var pendings = this.GetOPsInvoiced(reportFilter);
                                pendings.ForEach(p =>
                                {
                                    p.TotalPendingAmountBalance = p.TotalInvoiceAmount;
                                    p.QtyPending = 1;
                                });

                                reportFilter.Status = 1;
                                var confirms = this.GetOPsInvoiced(reportFilter);
                                confirms.ForEach(p =>
                                {
                                    p.TotalInvoiceAmountBalance = p.TotalInvoiceAmount;
                                    p.QtyInvoiced = 1;
                                });

                                opsInvoiced.AddRange(pendings);
                                opsInvoiced.AddRange(confirms);
                            }

                            var lambda = this.GroupByExpression<DTOOPInvoiced>(reportFilter.GroupBy.ToArray());

                            pendingInvoices = opsInvoiced.AsQueryable()
                                .GroupBy(lambda.Compile())
                                .Select(b => this.Select(reportFilter.GroupBy, b))
                                .ToList();

                            if (pendingInvoices.Count != 0)
                            {
                                foreach (var pendingInvoice in pendingInvoices)
                                {
                                    pendingInvoice.ReportName = reportName;
                                    pendingInvoice.PeriodTitle = $"TOTAL {periodTitle}";
                                    pendingInvoice.ShowBalance = reportFilter.ShowBalance ? 1 : 0;

                                    pendingInvoice.PercPending = pendingInvoice.TotalNetAmount.Equals(0) ? 0 : ((pendingInvoice.TotalPendingAmountBalance) / pendingInvoice.TotalNetAmount);
                                    pendingInvoice.PercInvoiced = pendingInvoice.TotalNetAmount.Equals(0) ? 0 : ((pendingInvoice.TotalInvoiceAmountBalance) / pendingInvoice.TotalNetAmount);

                                    pendingInvoice.ActualBalanceOutstanding = pendingInvoice.PrevBalanceOutstanding + pendingInvoice.TotalPendingAmountBalance;
                                }
                            }
                            else
                            {
                                pendingInvoices.Add(new DTOPendingInvoiceReport
                                {
                                    ReportName = reportName,
                                    PeriodTitle = $"TOTAL {periodTitle}",
                                    ShowBalance = reportFilter.ShowBalance ? 1 : 0,
                                    GroupBy1Value = "NO_DATA"
                                });
                            }

                            break;
                        }

                    default:
                        break;
                }

                return pendingInvoices;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool Confirm(List<DTOSalesInvoice> salesInvoicesSelected)
        {
            try
            {
                bool confirm = false;
                foreach (var salesInvoiceSelected in salesInvoicesSelected)
                {
                    var salesInvoiceUpd = _dbContext.SalesInvoices
                    .Where(si => si.Id.Equals(salesInvoiceSelected.Id))
                    .FirstOrDefault();

                    if (!(salesInvoiceUpd is null))
                    {
                        var ebNumber = _dbReportsContext.vwEBNumbers
                            .Where(n => n.CustomerNum.Equals(salesInvoiceUpd.CustomerNum)
                            && n.InvoiceNum.Equals(salesInvoiceUpd.InvoiceNum))
                            .FirstOrDefault();

                        if(!(ebNumber is null) && !string.IsNullOrEmpty(ebNumber.EBNumber.Trim()))
                        {
                            salesInvoiceUpd.EBNumber = ebNumber.EBNumber.Trim();
                            salesInvoiceUpd.EBDate = ebNumber.EBDate;
                            salesInvoiceUpd.Status = SalesInvoiceStatus.Confirmed;
                            salesInvoiceUpd.ConfirmedDate = DateTime.Now;

                            _dbContext.SalesInvoices.Update(salesInvoiceUpd);
                            confirm = true;
                        }                        
                    }
                }

                if (confirm)
                {
                    _dbContext.SaveChanges();
                    _dbContext.DetachAll();
                }
                return confirm;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DTOClosingInvoiced> GetClosingsInvoiced(DTOElectronicBillingFilter filter)
        {
            try
            {
                var closingsInvoiced = new List<DTOClosingInvoiced>();

                if (filter.CalcOutsBalance)
                {
                    closingsInvoiced = _dbContext.ClosingsInvoiced
                    .Where(si => (si.Date < filter.FromDate)
                    && si.Confirmed.Equals(false)
                    && (string.IsNullOrEmpty(filter.AdminId) || si.AdminId.Equals(filter.AdminId)))
                    .Select(ci => new DTOClosingInvoiced()
                    {
                        Id = ci.Id,
                        AdminName = ci.AdminName,
                        Confirmed = ci.Confirmed,
                        ConfirmedDate = ci.ConfirmedDate,
                        CustomerName = ci.CustomerName,
                        Date = ci.Date,
                        Note = ci.Note,
                        NetAmountValue = ci.TotalInvoiceAmount,
                        TotalInvoiceAmount = ci.TotalInvoiceAmount,
                        WorkName = ci.WorkName,
                        Period = ci.Period,
                        EBNumber = ci.EBNumber,
                        AdminId = ci.AdminId,
                        CustomerNum = ci.CustomerNum,
                        OthersWorkNo = ci.OthersWorkNo,
                        WorkNo = ci.WorkNo,
                    }).ToList();
                }
                else
                {
                    closingsInvoiced = _dbContext.ClosingsInvoiced
                    .Where(ci => (string.IsNullOrEmpty(filter.Period) || filter.Period.Equals("Todos") || ci.Period.Equals(filter.Period))
                    && ci.Confirmed.Equals(filter.Status.Equals(1))
                    && (string.IsNullOrEmpty(filter.AdminId) || ci.AdminId.Equals(filter.AdminId)))
                    .Select(ci => new DTOClosingInvoiced()
                    {
                        Id = ci.Id,
                        AdminName = ci.AdminName,
                        Confirmed = ci.Confirmed,
                        ConfirmedDate = ci.ConfirmedDate,
                        CustomerName = ci.CustomerName,
                        Date = ci.Date,
                        Note = ci.Note,
                        NetAmountValue = ci.TotalInvoiceAmount,
                        TotalInvoiceAmount = ci.TotalInvoiceAmount,
                        WorkName = ci.WorkName,
                        Period = ci.Period,
                        EBNumber = ci.EBNumber,
                        AdminId = ci.AdminId,
                        CustomerNum = ci.CustomerNum,
                        OthersWorkNo = ci.OthersWorkNo,
                        WorkNo = ci.WorkNo,
                    }).ToList();

                    
                }

                return closingsInvoiced;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private DTOPendingInvoiceReport Select(IEnumerable<string> groupBy, IGrouping<object, DTOSalesInvoice> s)
        {
            DTOPendingInvoiceReport pendingInvoiceReport = new DTOPendingInvoiceReport();

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
                        key = s.Max(s => s.ZoneParentId?.ToString());
                        value = s.Max(s => s.ZoneParentName?.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_ZONE" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN ZONA" : value;
                        label = "ZONA";
                        break;

                    case "WorkNo":
                        key = s.Max(s => s.WorkNo.ToString());
                        value = s.Max(s => s.WorkName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_WORK" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN OBRA" : value;
                        label = "OBRA";
                        break;

                    case "CustomerNum":
                        key = s.Max(s => s.CustomerNum.ToString());
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
                        pendingInvoiceReport.GroupBy1Key = key.Trim();
                        pendingInvoiceReport.GroupBy1Value = value.Trim();
                        pendingInvoiceReport.GroupBy1Label = label.Trim();
                        break;

                    case 2:
                        pendingInvoiceReport.GroupBy2Key = key.Trim();
                        pendingInvoiceReport.GroupBy2Value = value.Trim();
                        pendingInvoiceReport.GroupBy2Label = label.Trim();
                        break;

                    case 3:
                        pendingInvoiceReport.GroupBy3Key = key.Trim();
                        pendingInvoiceReport.GroupBy3Value = value.Trim();
                        pendingInvoiceReport.GroupBy3Label = label.Trim();
                        break;

                    case 4:
                        pendingInvoiceReport.GroupBy4Key = key.Trim();
                        pendingInvoiceReport.GroupBy4Value = value.Trim();
                        pendingInvoiceReport.GroupBy4Label = label.Trim();
                        break;

                    default:
                        break;
                }

                column++;
            }

            pendingInvoiceReport.PrevBalanceOutstanding = s.Sum(s => s.TotalOutstandingBalance);
            pendingInvoiceReport.TotalNetAmount = s.Sum(s => s.NetAmountValue);
            pendingInvoiceReport.TotalInvoiceAmount = s.Sum(s => s.TotalInvoiceAmount);
            pendingInvoiceReport.TotalInvoiceAmountBalance = s.Sum(s => s.TotalInvoiceAmountBalance);
            pendingInvoiceReport.TotalPendingAmountBalance = s.Sum(s => s.TotalPendingAmountBalance);
            pendingInvoiceReport.TotalAmountCancelled = s.Sum(s => s.TotalBalanceAmount);

            pendingInvoiceReport.QtyPending = s.Sum(s => s.QtyPending);
            pendingInvoiceReport.QtyInvoiced = s.Sum(s => s.QtyInvoiced);
            pendingInvoiceReport.QtyProvision = pendingInvoiceReport.QtyPending + pendingInvoiceReport.QtyInvoiced;
            pendingInvoiceReport.QtyInvoiced -= s.Sum(s => s.QtyCancelled);

            return pendingInvoiceReport;
        }

        private DTOSalesInvoiceTracking Select(IGrouping<object, DTOSalesInvoiceTracking> s)
        {
            DTOSalesInvoiceTracking pendingInvoiceReport = new DTOSalesInvoiceTracking();

            pendingInvoiceReport.GroupBy1Key = s.Max(s => s.GroupBy1Key);
            pendingInvoiceReport.GroupBy1Value = s.Max(s => s.GroupBy1Value);
            pendingInvoiceReport.GroupBy1Label = s.Max(s => s.GroupBy1Label);
            pendingInvoiceReport.GroupBy2Key = s.Max(s => s.GroupBy2Key);
            pendingInvoiceReport.GroupBy2Value = s.Max(s => s.GroupBy2Value);
            pendingInvoiceReport.GroupBy2Label = s.Max(s => s.GroupBy2Label);
            pendingInvoiceReport.GroupBy3Key = s.Max(s => s.GroupBy3Key);
            pendingInvoiceReport.GroupBy3Value = s.Max(s => s.GroupBy3Value);
            pendingInvoiceReport.GroupBy3Label = s.Max(s => s.GroupBy3Label);
            pendingInvoiceReport.GroupBy4Key = s.Max(s => s.GroupBy4Key);
            pendingInvoiceReport.GroupBy4Value = s.Max(s => s.GroupBy4Value);
            pendingInvoiceReport.GroupBy4Label = s.Max(s => s.GroupBy4Label);
            pendingInvoiceReport.GroupBy5Key = s.Max(s => s.GroupBy5Key);
            pendingInvoiceReport.GroupBy5Value = s.Max(s => s.GroupBy5Value);
            pendingInvoiceReport.GroupBy5Label = s.Max(s => s.GroupBy5Label);
            pendingInvoiceReport.GroupBy6Key = s.Max(s => s.GroupBy6Key);
            pendingInvoiceReport.GroupBy6Value = s.Max(s => s.GroupBy6Value);
            pendingInvoiceReport.GroupBy6Label = s.Max(s => s.GroupBy6Label);
            pendingInvoiceReport.Period = s.Max(s => s.Period);
            pendingInvoiceReport.ProvisionPeriod = s.Max(s => s.ProvisionPeriod);
            pendingInvoiceReport.InvoiceAmount1Label = s.Max(s => s.InvoiceAmount1Label);
            pendingInvoiceReport.InvoiceAmount2Label = s.Max(s => s.InvoiceAmount2Label);
            pendingInvoiceReport.InvoiceAmount3Label = s.Max(s => s.InvoiceAmount3Label);
            pendingInvoiceReport.InvoiceAmount4Label = s.Max(s => s.InvoiceAmount4Label);
            pendingInvoiceReport.InvoiceAmount5Label = s.Max(s => s.InvoiceAmount5Label);
            pendingInvoiceReport.InvoiceAmount6Label = s.Max(s => s.InvoiceAmount6Label);
            pendingInvoiceReport.InvoiceAmount7Label = s.Max(s => s.InvoiceAmount7Label);
            pendingInvoiceReport.InvoiceAmount8Label = s.Max(s => s.InvoiceAmount8Label);
            pendingInvoiceReport.InvoiceAmount9Label = s.Max(s => s.InvoiceAmount9Label);
            pendingInvoiceReport.InvoiceAmount10Label = s.Max(s => s.InvoiceAmount10Label);
            pendingInvoiceReport.InvoiceAmount11Label = s.Max(s => s.InvoiceAmount11Label);
            pendingInvoiceReport.InvoiceAmount12Label = s.Max(s => s.InvoiceAmount12Label);
            pendingInvoiceReport.InvoiceAmount13Label = s.Max(s => s.InvoiceAmount13Label);
            pendingInvoiceReport.InvoiceAmount14Label = s.Max(s => s.InvoiceAmount14Label);
            pendingInvoiceReport.InvoiceAmount15Label = s.Max(s => s.InvoiceAmount15Label);
            pendingInvoiceReport.InvoiceAmount16Label = s.Max(s => s.InvoiceAmount16Label);
            pendingInvoiceReport.InvoiceAmount17Label = s.Max(s => s.InvoiceAmount17Label);
            pendingInvoiceReport.InvoiceAmount18Label = s.Max(s => s.InvoiceAmount18Label);
            pendingInvoiceReport.InvoiceAmount19Label = s.Max(s => s.InvoiceAmount19Label);
            pendingInvoiceReport.InvoiceAmount20Label = s.Max(s => s.InvoiceAmount20Label);
            pendingInvoiceReport.InvoiceAmount21Label = s.Max(s => s.InvoiceAmount21Label);
            pendingInvoiceReport.InvoiceAmount22Label = s.Max(s => s.InvoiceAmount22Label);
            pendingInvoiceReport.InvoiceAmount23Label = s.Max(s => s.InvoiceAmount23Label);
            pendingInvoiceReport.InvoiceAmount24Label = s.Max(s => s.InvoiceAmount24Label);

            pendingInvoiceReport.TotalNetAmount = s.Sum(s => s.TotalNetAmount);
            pendingInvoiceReport.TotalInvoiceAmount = s.Sum(s => s.TotalInvoiceAmount);

            pendingInvoiceReport.InvoiceAmount1 = s.Sum(s => s.InvoiceAmount1);
            pendingInvoiceReport.InvoiceAmount2 = s.Sum(s => s.InvoiceAmount2);
            pendingInvoiceReport.InvoiceAmount3 = s.Sum(s => s.InvoiceAmount3);
            pendingInvoiceReport.InvoiceAmount4 = s.Sum(s => s.InvoiceAmount4);
            pendingInvoiceReport.InvoiceAmount5 = s.Sum(s => s.InvoiceAmount5);
            pendingInvoiceReport.InvoiceAmount6 = s.Sum(s => s.InvoiceAmount6);
            pendingInvoiceReport.InvoiceAmount7 = s.Sum(s => s.InvoiceAmount7);
            pendingInvoiceReport.InvoiceAmount8 = s.Sum(s => s.InvoiceAmount8);
            pendingInvoiceReport.InvoiceAmount9 = s.Sum(s => s.InvoiceAmount9); 
            pendingInvoiceReport.InvoiceAmount10 = s.Sum(s => s.InvoiceAmount10);
            pendingInvoiceReport.InvoiceAmount11 = s.Sum(s => s.InvoiceAmount11);
            pendingInvoiceReport.InvoiceAmount12 = s.Sum(s => s.InvoiceAmount12);
            pendingInvoiceReport.InvoiceAmount13 = s.Sum(s => s.InvoiceAmount13);
            pendingInvoiceReport.InvoiceAmount14 = s.Sum(s => s.InvoiceAmount14);
            pendingInvoiceReport.InvoiceAmount15 = s.Sum(s => s.InvoiceAmount15);
            pendingInvoiceReport.InvoiceAmount16 = s.Sum(s => s.InvoiceAmount16);
            pendingInvoiceReport.InvoiceAmount17 = s.Sum(s => s.InvoiceAmount17);
            pendingInvoiceReport.InvoiceAmount18 = s.Sum(s => s.InvoiceAmount18);
            pendingInvoiceReport.InvoiceAmount19 = s.Sum(s => s.InvoiceAmount19);
            pendingInvoiceReport.InvoiceAmount20 = s.Sum(s => s.InvoiceAmount20);
            pendingInvoiceReport.InvoiceAmount21 = s.Sum(s => s.InvoiceAmount21);
            pendingInvoiceReport.InvoiceAmount22 = s.Sum(s => s.InvoiceAmount22);
            pendingInvoiceReport.InvoiceAmount23 = s.Sum(s => s.InvoiceAmount23);
            pendingInvoiceReport.InvoiceAmount24 = s.Sum(s => s.InvoiceAmount24);

            pendingInvoiceReport.SumTotalInvoiceAmount = s.Sum(s => s.SumTotalInvoiceAmount);
            pendingInvoiceReport.BalanceTotalAmount = s.Sum(s => s.BalanceTotalAmount);

            return pendingInvoiceReport;
        }

        private DTOSalesInvoiceTracking TrackingSelect(IEnumerable<string> groupBy, IGrouping<object, DTOSalesInvoice> s)
        {
            DTOSalesInvoiceTracking pendingInvoiceReport = new DTOSalesInvoiceTracking();

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
                        key = s.Max(s => s.ZoneParentId?.ToString());
                        value = s.Max(s => s.ZoneParentName?.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_ZONE" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN ZONA" : value;
                        label = "ZONA";
                        break;

                    case "WorkNo":
                        key = s.Max(s => s.WorkNo.ToString());
                        value = s.Max(s => s.WorkName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_WORK" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN OBRA" : value;
                        label = "OBRA";
                        break;

                    case "CustomerNum":
                        key = s.Max(s => s.CustomerNum.ToString());
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
                        pendingInvoiceReport.GroupBy1Key = key.Trim();
                        pendingInvoiceReport.GroupBy1Value = value.Trim();
                        pendingInvoiceReport.GroupBy1Label = label.Trim();
                        break;

                    case 2:
                        pendingInvoiceReport.GroupBy2Key = key.Trim();
                        pendingInvoiceReport.GroupBy2Value = value.Trim();
                        pendingInvoiceReport.GroupBy2Label = label.Trim();
                        break;

                    case 3:
                        pendingInvoiceReport.GroupBy3Key = key.Trim();
                        pendingInvoiceReport.GroupBy3Value = value.Trim();
                        pendingInvoiceReport.GroupBy3Label = label.Trim();
                        break;

                    case 4:
                        pendingInvoiceReport.GroupBy4Key = key.Trim();
                        pendingInvoiceReport.GroupBy4Value = value.Trim();
                        pendingInvoiceReport.GroupBy4Label = label.Trim();
                        break;

                    default:
                        break;
                }

                column++;

            }

            pendingInvoiceReport.EBDate = s.Max(s => s.EBDate);
            pendingInvoiceReport.Period = pendingInvoiceReport.EBDate.ToString("yyyy-MM");
            pendingInvoiceReport.TotalNetAmount = s.Sum(s => s.NetAmountValue);
            pendingInvoiceReport.TotalInvoiceAmount = s.Sum(s => s.TotalInvoiceAmount);
            pendingInvoiceReport.BalanceTotalAmount = s.Sum(s => s.TotalPendingAmountBalance);

            return pendingInvoiceReport;
        }

        private DTOPendingInvoiceReport Select(IEnumerable<string> groupBy, IGrouping<object, DTOClosingInvoiced> s)
        {
            DTOPendingInvoiceReport pendingInvoiceReport = new DTOPendingInvoiceReport();

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

                    /*case "ZoneParentId":
                        key = s.Max(s => s.ZoneParentId.ToString());
                        value = s.Max(s => s.ZoneParentName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_ZONE" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN ZONA" : value;
                        label = "ZONA";
                        break;*/

                    case "WorkNo":
                        key = s.Max(s => s.WorkNo.ToString());
                        value = s.Max(s => s.WorkName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_WORK" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN OBRA" : value;
                        label = "OBRA";
                        break;

                    case "CustomerNum":
                        key = s.Max(s => s.CustomerNum.ToString());
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
                        pendingInvoiceReport.GroupBy1Key = key.Trim();
                        pendingInvoiceReport.GroupBy1Value = value.Trim();
                        pendingInvoiceReport.GroupBy1Label = label.Trim();
                        break;

                    case 2:
                        pendingInvoiceReport.GroupBy2Key = key.Trim();
                        pendingInvoiceReport.GroupBy2Value = value.Trim();
                        pendingInvoiceReport.GroupBy2Label = label.Trim();
                        break;

                    case 3:
                        pendingInvoiceReport.GroupBy3Key = key.Trim();
                        pendingInvoiceReport.GroupBy3Value = value.Trim();
                        pendingInvoiceReport.GroupBy3Label = label.Trim();
                        break;

                    case 4:
                        pendingInvoiceReport.GroupBy4Key = key.Trim();
                        pendingInvoiceReport.GroupBy4Value = value.Trim();
                        pendingInvoiceReport.GroupBy4Label = label.Trim();
                        break;

                    default:
                        break;
                }

                column++;
            }

            pendingInvoiceReport.PrevBalanceOutstanding = s.Sum(s => s.TotalOutstandingBalance);
            pendingInvoiceReport.TotalNetAmount = s.Sum(s => s.NetAmountValue);
            pendingInvoiceReport.TotalInvoiceAmount = s.Sum(s => s.TotalInvoiceAmount);
            pendingInvoiceReport.TotalInvoiceAmountBalance = s.Sum(s => s.TotalInvoiceAmountBalance);
            pendingInvoiceReport.TotalPendingAmountBalance = s.Sum(s => s.TotalPendingAmountBalance);

            pendingInvoiceReport.QtyPending = s.Sum(s => s.QtyPending);
            pendingInvoiceReport.QtyInvoiced = s.Sum(s => s.QtyInvoiced);
            pendingInvoiceReport.QtyProvision = pendingInvoiceReport.QtyPending + pendingInvoiceReport.QtyInvoiced;

            return pendingInvoiceReport;
        }

        private DTOPendingInvoiceReport Select(IEnumerable<string> groupBy, IGrouping<object, DTOOPInvoiced> s)
        {
            DTOPendingInvoiceReport pendingInvoiceReport = new DTOPendingInvoiceReport();

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

                    /*case "ZoneParentId":
                        key = s.Max(s => s.ZoneParentId.ToString());
                        value = s.Max(s => s.ZoneParentName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_ZONE" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN ZONA" : value;
                        label = "ZONA";
                        break;*/

                    case "WorkNo":
                        key = s.Max(s => s.WorkNo.ToString());
                        value = s.Max(s => s.WorkName.ToString());
                        key = string.IsNullOrEmpty(key) ? "NO_WORK" : key;
                        value = string.IsNullOrEmpty(value) ? "SIN OBRA" : value;
                        label = "OBRA";
                        break;

                    case "CustomerNum":
                        key = s.Max(s => s.CustomerNum.ToString());
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
                        pendingInvoiceReport.GroupBy1Key = key.Trim();
                        pendingInvoiceReport.GroupBy1Value = value.Trim();
                        pendingInvoiceReport.GroupBy1Label = label.Trim();
                        break;

                    case 2:
                        pendingInvoiceReport.GroupBy2Key = key.Trim();
                        pendingInvoiceReport.GroupBy2Value = value.Trim();
                        pendingInvoiceReport.GroupBy2Label = label.Trim();
                        break;

                    case 3:
                        pendingInvoiceReport.GroupBy3Key = key.Trim();
                        pendingInvoiceReport.GroupBy3Value = value.Trim();
                        pendingInvoiceReport.GroupBy3Label = label.Trim();
                        break;

                    case 4:
                        pendingInvoiceReport.GroupBy4Key = key.Trim();
                        pendingInvoiceReport.GroupBy4Value = value.Trim();
                        pendingInvoiceReport.GroupBy4Label = label.Trim();
                        break;

                    default:
                        break;
                }

                column++;
            }

            pendingInvoiceReport.PrevBalanceOutstanding = s.Sum(s => s.TotalOutstandingBalance);
            pendingInvoiceReport.TotalNetAmount = s.Sum(s => s.NetAmountValue);
            pendingInvoiceReport.TotalInvoiceAmount = s.Sum(s => s.TotalInvoiceAmount);
            pendingInvoiceReport.TotalInvoiceAmountBalance = s.Sum(s => s.TotalInvoiceAmountBalance);
            pendingInvoiceReport.TotalPendingAmountBalance = s.Sum(s => s.TotalPendingAmountBalance);

            pendingInvoiceReport.QtyPending = s.Sum(s => s.QtyPending);
            pendingInvoiceReport.QtyInvoiced = s.Sum(s => s.QtyInvoiced);
            pendingInvoiceReport.QtyProvision = pendingInvoiceReport.QtyPending + pendingInvoiceReport.QtyInvoiced;

            return pendingInvoiceReport;
        }

        public DTOClosingInvoiced GetClosingInvoiced(string closingInvoicedId)
        {
            try
            {
                var closingInvoicedNotes = new List<DTOClosingInvoicedNote>();
                var closingInvoiced = _dbContext.ClosingsInvoiced
                    .Where(ci => ci.Id == closingInvoicedId)
                    .Select(ci => new DTOClosingInvoiced()
                    {
                        Id = ci.Id,
                        AdminName = ci.AdminName,
                        Confirmed = ci.Confirmed,
                        ConfirmedDate = ci.ConfirmedDate,
                        CustomerName = ci.CustomerName,
                        Date = ci.Date,
                        Note = ci.Note,
                        TotalInvoiceAmount = ci.TotalInvoiceAmount,
                        WorkName = ci.WorkName,                        
                        Period = ci.Period,
                        EBNumber = ci.EBNumber,
                        EBDate = ci.EBDate,
                        WorkNo = ci.WorkNo,
                        OthersWorkNo = ci.OthersWorkNo,
                        AdminId = ci.AdminId,
                        CustomerNum = ci.CustomerNum,
                    }).FirstOrDefault();

                closingInvoicedNotes = _dbContext.ClosingInvoicedNotes
                    .Where(cin => cin.ClosingInvoicedId == closingInvoicedId)
                    .Select(cin => new DTOClosingInvoicedNote()
                    {
                        CreatedAt = cin.CreatedAt,
                        Id = cin.Id,
                        Note = cin.Note,
                        ClosingInvoicedId = cin.ClosingInvoicedId,
                        UserName = cin.UserName,
                        AttachFileName = cin.AttachFileName
                    })
                    .OrderByDescending(cin => cin.CreatedAt)
                    .ToList();

                closingInvoiced.ClosingInvoicedNotes = closingInvoicedNotes;

                return closingInvoiced;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DTOOPInvoiced> GetOPsInvoiced(DTOElectronicBillingFilter filter)
        {
            try
            {
                var opsInvoiced = new List<DTOOPInvoiced>();

                if (filter.CalcOutsBalance)
                {
                    opsInvoiced = _dbContext.OPsInvoiced
                    .Where(opi => (opi.Date < filter.FromDate)
                    && opi.Confirmed.Equals(false)
                    && (string.IsNullOrEmpty(filter.AdminId) || opi.AdminId.Equals(filter.AdminId)))
                    .Select(opi => new DTOOPInvoiced()
                    {
                        Id = opi.Id,
                        AdminName = opi.AdminName,
                        Confirmed = opi.Confirmed,
                        ConfirmedDate = opi.ConfirmedDate,
                        CustomerNum = opi.CustomerNum,
                        CustomerName = opi.CustomerName,
                        Date = opi.Date,
                        Note = opi.Note,
                        TotalInvoiceAmount = opi.TotalInvoiceAmount,
                        NetAmountValue = opi.TotalInvoiceAmount,
                        WorkNo = opi.WorkNo,
                        WorkName = opi.WorkName,
                        Period = opi.Period,
                        EBNumber = opi.EBNumber,
                        OPNum = opi.OPNum,
                        AdminId = opi.AdminId,
                    }).ToList();
                }
                else
                {
                    opsInvoiced = _dbContext.OPsInvoiced
                    .Where(opi => (string.IsNullOrEmpty(filter.Period) || filter.Period.Equals("Todos") || opi.Period.Equals(filter.Period))
                    && opi.Confirmed.Equals(filter.Status.Equals(1))
                    && (string.IsNullOrEmpty(filter.AdminId) || opi.AdminId.Equals(filter.AdminId)))
                    .Select(opi => new DTOOPInvoiced()
                    {
                        Id = opi.Id,
                        AdminName = opi.AdminName,
                        Confirmed = opi.Confirmed,
                        ConfirmedDate = opi.ConfirmedDate,
                        CustomerNum = opi.CustomerNum,
                        CustomerName = opi.CustomerName,
                        Date = opi.Date,
                        Note = opi.Note,
                        NetAmountValue = opi.TotalInvoiceAmount,
                        TotalInvoiceAmount = opi.TotalInvoiceAmount,
                        WorkNo = opi.WorkNo,
                        WorkName = opi.WorkName,
                        Period = opi.Period,
                        EBNumber = opi.EBNumber,
                        OPNum = opi.OPNum,
                        AdminId = opi.AdminId,
                    }).ToList();
                }

                return opsInvoiced;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOOPInvoiced GetOPInvoiced(string opInvoicedId)
        {
            try
            {
                var opInvoicedNotes = new List<DTOOPInvoicedNote>();
                var opInvoiced = _dbContext.OPsInvoiced
                    .Where(opi => opi.Id == opInvoicedId)
                    .Select(opi => new DTOOPInvoiced()
                    {
                        Id = opi.Id,
                        AdminName = opi.AdminName,
                        Confirmed = opi.Confirmed,
                        ConfirmedDate = opi.ConfirmedDate,
                        CustomerNum = opi.CustomerNum,
                        CustomerName = opi.CustomerName,
                        Date = opi.Date,
                        Note = opi.Note,
                        TotalInvoiceAmount = opi.TotalInvoiceAmount,
                        WorkName = opi.WorkName,
                        Period = opi.Period,
                        EBNumber = opi.EBNumber,
                        EBDate = opi.EBDate,
                        OPNum = opi.OPNum,
                        AdminId = opi.AdminId,
                        WorkNo = opi.WorkNo,
                    }).FirstOrDefault();

                opInvoicedNotes = _dbContext.OPInvoicedNotes
                    .Where(cin => cin.OPInvoicedId == opInvoicedId)
                    .Select(cin => new DTOOPInvoicedNote()
                    {
                        CreatedAt = cin.CreatedAt,
                        Id = cin.Id,
                        Note = cin.Note,
                        OPInvoicedId = cin.OPInvoicedId,
                        UserName = cin.UserName,
                        AttachFileName = cin.AttachFileName
                    })
                    .OrderByDescending(cin => cin.CreatedAt)
                    .ToList();

                opInvoiced.OPInvoicedNotes = opInvoicedNotes;

                return opInvoiced;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Confirm(List<DTOOPInvoiced> opsInvoicedSelected)
        {
            try
            {
                bool confirm = false;
                foreach (var opInvoiced in opsInvoicedSelected)
                {
                    var opInvoiceUpd = _dbContext.OPsInvoiced
                    .Where(oi => oi.Id.Equals(opInvoiced.Id))
                    .FirstOrDefault();

                    if (!(opInvoiceUpd is null))
                    {
                        opInvoiceUpd.Confirmed = true;
                        opInvoiceUpd.ConfirmedDate = DateTime.Now;

                        _dbContext.OPsInvoiced.Update(opInvoiceUpd);

                        confirm = true;
                    }
                }

                if (confirm)
                {
                    _dbContext.SaveChanges();
                    _dbContext.DetachAll();
                }
                return confirm;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Confirm(List<DTOClosingInvoiced> closingsInvoicedSelected)
        {
            try
            {
                bool confirm = false;
                foreach (var closingInvoicedSelected in closingsInvoicedSelected)
                {
                    var closingInvoicedUpd = _dbContext.ClosingsInvoiced
                    .Where(ci => ci.Id.Equals(closingInvoicedSelected.Id))
                    .FirstOrDefault();

                    if (!(closingInvoicedUpd is null))
                    {
                        closingInvoicedUpd.Confirmed = true;
                        closingInvoicedUpd.ConfirmedDate = DateTime.Now;

                        _dbContext.ClosingsInvoiced.Update(closingInvoicedUpd);

                        confirm = true;
                    }
                }

                if (confirm)
                {
                    _dbContext.SaveChanges();
                    _dbContext.DetachAll();
                }
                return confirm;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CommonData> GetCustomers()
        {
            try
            {
                List<CommonData> customers = new List<CommonData>();

                customers = _dbReportsContext.vwCustomers                    
                    .Select(bt => new CommonData () { IdStr = bt.CustomerAccount.Trim(), Description = bt.CustomerName })
                    .ToList();

                return customers;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CommonData> GetWorks(string customerAccount)
        {
            try
            {
                List<CommonData> works = new List<CommonData>();

                works = _dbReportsContext.BalanceTransRent
                    .Where(bt => bt.CustomerAccount.Trim().Equals(customerAccount)) 
                    .GroupBy(bt => new { bt.WorkId, bt.WorkName })
                    .Select(bt => new CommonData() { IdStr = bt.Key.WorkId.Trim().Replace("OB", ""), Description = bt.Key.WorkName })
                    .ToList();

                return works;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CommonData> GetAdministrators(string customerAccount, string workId)
        {
            try
            {
                List<CommonData> administrators = new List<CommonData>();

                administrators = _dbReportsContext.BalanceTransRent
                    .Where(bt => (string.IsNullOrEmpty(customerAccount) || bt.CustomerAccount.Trim().Equals(customerAccount)) 
                        && (string.IsNullOrEmpty(workId) || bt.WorkId.Equals($"OB{workId}")) 
                        && !string.IsNullOrEmpty(bt.AdminId))
                    .GroupBy(bt => new { bt.AdminId, bt.AdminName})
                    .Select(bt => new CommonData() { IdStr = bt.Key.AdminId.Trim().Replace("OB", ""), Description = bt.Key.AdminName })
                    .ToList();

                administrators.ForEach(w =>
                {
                    var adminName = _dbReportsContext.CommonDataTable
                        .Where(c => c.Id.Equals(w.IdStr))
                        .Select(c => c.CommonDataTitle)
                        .FirstOrDefault();

                    if(!string.IsNullOrEmpty(adminName))
                        w.Description = adminName;
                });

                return administrators;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CommonData GetZoneParent(string customerAccount, string workId)
        {
            try
            {
                var zoneParent = _dbReportsContext.BalanceTransRent
                    .Where(bt => bt.CustomerAccount.Trim().Equals(customerAccount) && bt.WorkId.Equals($"OB{workId}")
                        && !string.IsNullOrEmpty(bt.ZoneParentId))
                    .OrderByDescending(bt => bt.Id)
                    .GroupBy(bt => new { bt.ZoneParentId, bt.ZoneParentName })
                    .Take(1)
                    .Select(bt => new CommonData() { IdStr = bt.Key.ZoneParentId, Description = bt.Key.ZoneParentName })
                    .FirstOrDefault();

                return zoneParent;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOOPInvoiced CreateOrUpdate(DTOOPInvoiced dtoOPInvoiced)
        {
            try
            {
                var opInvoice = new OPInvoiced();
                if (!string.IsNullOrEmpty(dtoOPInvoiced.Id))
                {
                    var opInvoiceUpd = _dbContext.OPsInvoiced
                        .Where(op => op.Id.Equals(dtoOPInvoiced.Id))
                        .FirstOrDefault();

                    if (opInvoiceUpd is null)
                        throw new Exception("La OP no existe");

                    var customer = this.GetCustomers().Where(c => c.IdStr.Equals(dtoOPInvoiced.CustomerNum)).FirstOrDefault();
                    var work = this.GetWorks(customer.IdStr).Where(w => w.IdStr.Equals(dtoOPInvoiced.WorkNo)).FirstOrDefault();
                    var administrator = this.GetAdministrators(customer.IdStr, work.IdStr).Where(a => a.IdStr.Equals(dtoOPInvoiced.AdminId)).FirstOrDefault();

                    opInvoiceUpd.AdminId = dtoOPInvoiced.AdminId;
                    opInvoiceUpd.AdminName = administrator.Description;
                    opInvoiceUpd.CustomerName = customer.Description;
                    opInvoiceUpd.CustomerNum = dtoOPInvoiced.CustomerNum;
                    opInvoiceUpd.Date = dtoOPInvoiced.Date;
                    opInvoiceUpd.Period = dtoOPInvoiced.Date.ToString("yyyy-MM");
                    opInvoiceUpd.EBNumber = dtoOPInvoiced.EBNumber;
                    opInvoiceUpd.EBDate = dtoOPInvoiced.EBDate;
                    opInvoiceUpd.OPNum = dtoOPInvoiced.OPNum;
                    opInvoiceUpd.Confirmed = dtoOPInvoiced.Confirmed;
                    opInvoiceUpd.TotalInvoiceAmount = dtoOPInvoiced.TotalInvoiceAmount;
                    opInvoiceUpd.WorkName = work.Description;
                    opInvoiceUpd.WorkNo = dtoOPInvoiced.WorkNo;

                    _dbContext.OPsInvoiced.Update(opInvoiceUpd);
                }
                else
                {
                    var customer = this.GetCustomers().Where(c => c.IdStr.Equals(dtoOPInvoiced.CustomerNum)).FirstOrDefault();
                    var work = this.GetWorks(customer.IdStr).Where(w => w.IdStr.Equals(dtoOPInvoiced.WorkNo)).FirstOrDefault();
                    var administrator = this.GetAdministrators(customer.IdStr, work.IdStr).Where(a => a.IdStr.Equals(dtoOPInvoiced.AdminId)).FirstOrDefault();
                    opInvoice = new OPInvoiced()
                    {
                        Id = Guid.NewGuid().ToString(),
                        AdminId = dtoOPInvoiced.AdminId,
                        AdminName = administrator.Description,
                        CustomerName = customer.Description,
                        CustomerNum = dtoOPInvoiced.CustomerNum,
                        Date = dtoOPInvoiced.Date,
                        Period = dtoOPInvoiced.Date.ToString("yyyy-MM"),
                        EBNumber = dtoOPInvoiced.EBNumber,
                        EBDate = dtoOPInvoiced.EBDate,
                        OPNum = dtoOPInvoiced.OPNum,
                        Confirmed = dtoOPInvoiced.Confirmed,
                        TotalInvoiceAmount = dtoOPInvoiced.TotalInvoiceAmount,
                        WorkName = work.Description,
                        WorkNo = dtoOPInvoiced.WorkNo
                    };

                    _dbContext.OPsInvoiced.Add(opInvoice);
                }

                _dbContext.SaveChanges();
                _dbContext.DetachAll();

                return new DTOOPInvoiced()
                {
                    Id = opInvoice.Id,
                    AdminId = opInvoice.AdminId,
                    AdminName = opInvoice.AdminName,
                    CustomerName = opInvoice.CustomerName,
                    CustomerNum = opInvoice.CustomerNum,
                    Date = opInvoice.Date,
                    EBNumber = opInvoice.EBNumber,
                    OPNum = opInvoice.OPNum,
                    Confirmed = opInvoice.Confirmed,
                    TotalInvoiceAmount = opInvoice.TotalInvoiceAmount,
                    WorkName = opInvoice.WorkName,
                    WorkNo = opInvoice.WorkNo,
                    EBDate = opInvoice.EBDate
                };            
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DTOOPInvoiced> ConfirmAsync(DTOConfirmOPInvoiced confirmOPInvoiced)
        {
            try
            {
                var opInvoiced = new DTOOPInvoiced();
                var opInvoicedUpd = _dbContext.OPsInvoiced
                    .Where(si => si.Id.Equals(confirmOPInvoiced.OPInvoiced.Id))
                    .FirstOrDefault();

                if (!(opInvoicedUpd is null))
                {
                    var electronicBillingRepo = this._configuration.GetValue<string>("ElectronicBilling:RepoFiles");
                    var attachFileName = "";
                    if (!(confirmOPInvoiced.Files is null) && (confirmOPInvoiced.Files.Length != 0))
                    {
                        var file = confirmOPInvoiced.Files[0];
                        var ext = Path.GetExtension(file.Name);

                        attachFileName = string.Format("{0}/{1}{2}", electronicBillingRepo, Guid.NewGuid().ToString(), ext);
                        using (var ms = new MemoryStream())
                        {
                            var reader = new System.IO.StreamReader(file.Data);
                            await reader.BaseStream.CopyToAsync(ms);

                            var fileBytes = ms.ToArray();
                            //string s = Convert.ToBase64String(fileBytes);

                            using (var imageFile = new FileStream(attachFileName, FileMode.Create))
                            {
                                imageFile.Write(fileBytes, 0, fileBytes.Length);
                                imageFile.Flush();
                            }
                        }
                    }

                    var note = new OPInvoicedNote()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Note = confirmOPInvoiced.Note,
                        AttachFileName = attachFileName,
                        CreatedAt = DateTime.Now,
                        UserName = confirmOPInvoiced.UserName,
                        OPInvoicedId = confirmOPInvoiced.OPInvoiced.Id,
                    };

                    _dbContext.OPInvoicedNotes.Add(note);

                    opInvoicedUpd.Confirmed = true;
                    opInvoicedUpd.ConfirmedDate = DateTime.Now;
                    opInvoicedUpd.Note = confirmOPInvoiced.Note;

                    _dbContext.OPsInvoiced.Update(opInvoicedUpd);

                    _dbContext.SaveChanges();
                    _dbContext.DetachAll();

                    opInvoiced = this.GetOPInvoiced(opInvoicedUpd.Id);
                }

                return opInvoiced;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DTOOPInvoicedNote> CreateOrUpdateAsync(DTOOPInvoicedNote dtoOPInvoicedNote)
        {
            try
            {
                var opInvoicedNote = new OPInvoicedNote();
                if (!string.IsNullOrEmpty(opInvoicedNote.Id))
                {
                    opInvoicedNote = _dbContext.OPInvoicedNotes
                        .Where(opi => opi.Id.Equals(opInvoicedNote.Id))
                        .FirstOrDefault();

                    opInvoicedNote.Note = opInvoicedNote.Note;

                    _dbContext.OPInvoicedNotes.Update(opInvoicedNote);
                }
                else
                {
                    var electronicBillingRepo = this._configuration.GetValue<string>("ElectronicBilling:RepoFiles");
                    var attachFileName = "";
                    if (!(dtoOPInvoicedNote.Files is null) && (dtoOPInvoicedNote.Files.Length != 0))
                    {
                        var file = dtoOPInvoicedNote.Files[0];
                        var ext = Path.GetExtension(file.Name);

                        attachFileName = string.Format("{0}/{1}{2}", electronicBillingRepo, Guid.NewGuid().ToString(), ext);
                        using (var ms = new MemoryStream())
                        {
                            var reader = new System.IO.StreamReader(file.Data);
                            await reader.BaseStream.CopyToAsync(ms);

                            var fileBytes = ms.ToArray();
                            //string s = Convert.ToBase64String(fileBytes);

                            using (var imageFile = new FileStream(attachFileName, FileMode.Create))
                            {
                                imageFile.Write(fileBytes, 0, fileBytes.Length);
                                imageFile.Flush();


                            }
                        }
                    }

                    opInvoicedNote = new OPInvoicedNote()
                    {
                        CreatedAt = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        Note = dtoOPInvoicedNote.Note,
                        OPInvoicedId = dtoOPInvoicedNote.OPInvoicedId,
                        AttachFileName = attachFileName,
                        UserName = dtoOPInvoicedNote.UserName
                    };

                    _dbContext.OPInvoicedNotes.Add(opInvoicedNote);

                    var opInvoiced = _dbContext.OPsInvoiced
                        .Where(opi => opi.Id.Equals(dtoOPInvoicedNote.OPInvoicedId))
                        .FirstOrDefault();

                    if (!(opInvoiced is null))
                    {
                        opInvoiced.Note = dtoOPInvoicedNote.Note;

                        _dbContext.OPsInvoiced.Update(opInvoiced);
                    }
                }

                _dbContext.SaveChanges();
                _dbContext.DetachAll();

                return this.GetOPInvoicedNote(opInvoicedNote.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DTOClosingInvoicedNote> CreateOrUpdateAsync(DTOClosingInvoicedNote dtoClosingInvoicedNote)
        {
            try
            {
                var closingInvoicedNote = new ClosingInvoicedNote();
                if (!string.IsNullOrEmpty(dtoClosingInvoicedNote.Id))
                {
                    closingInvoicedNote = _dbContext.ClosingInvoicedNotes
                        .Where(opi => opi.Id.Equals(dtoClosingInvoicedNote.Id))
                        .FirstOrDefault();

                    closingInvoicedNote.Note = dtoClosingInvoicedNote.Note;

                    _dbContext.ClosingInvoicedNotes.Update(closingInvoicedNote);
                }
                else
                {
                    var electronicBillingRepo = this._configuration.GetValue<string>("ElectronicBilling:RepoFiles");
                    var attachFileName = "";
                    if (!(dtoClosingInvoicedNote.Files is null) && (dtoClosingInvoicedNote.Files.Length != 0))
                    {
                        var file = dtoClosingInvoicedNote.Files[0];
                        var ext = Path.GetExtension(file.Name);

                        attachFileName = string.Format("{0}/{1}{2}", electronicBillingRepo, Guid.NewGuid().ToString(), ext);
                        using (var ms = new MemoryStream())
                        {
                            var reader = new System.IO.StreamReader(file.Data);
                            await reader.BaseStream.CopyToAsync(ms);

                            var fileBytes = ms.ToArray();
                            //string s = Convert.ToBase64String(fileBytes);

                            using (var imageFile = new FileStream(attachFileName, FileMode.Create))
                            {
                                imageFile.Write(fileBytes, 0, fileBytes.Length);
                                imageFile.Flush();


                            }
                        }
                    }

                    closingInvoicedNote = new ClosingInvoicedNote()
                    {
                        CreatedAt = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        Note = dtoClosingInvoicedNote.Note,
                        ClosingInvoicedId = dtoClosingInvoicedNote.ClosingInvoicedId,
                        AttachFileName = attachFileName,
                        UserName = dtoClosingInvoicedNote.UserName
                    };

                    _dbContext.ClosingInvoicedNotes.Add(closingInvoicedNote);

                    var closingInvoiced = _dbContext.ClosingsInvoiced
                        .Where(cin => cin.Id.Equals(dtoClosingInvoicedNote.ClosingInvoicedId))
                        .FirstOrDefault();

                    if (!(closingInvoiced is null))
                    {
                        closingInvoiced.Note = dtoClosingInvoicedNote.Note;

                        _dbContext.ClosingsInvoiced.Update(closingInvoiced);
                    }
                }

                _dbContext.SaveChanges();
                _dbContext.DetachAll();

                return this.GetClosingInvoicedNote(closingInvoicedNote.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOOPInvoicedNote GetOPInvoicedNote(string opInvoicedNoteId)
        {
            try
            {
                var opInvoicedNote = _dbContext.OPInvoicedNotes
                    .Where(opi => opi.Id.Equals(opInvoicedNoteId))
                    .FirstOrDefault();

                return new DTOOPInvoicedNote()
                {
                    Id = opInvoicedNote.Id,
                    AttachFileName = opInvoicedNote.AttachFileName,
                    CreatedAt = opInvoicedNote.CreatedAt,
                    Note = opInvoicedNote.Note,
                    OPInvoicedId = opInvoicedNote.OPInvoicedId,
                    UserName = opInvoicedNote.UserName
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOClosingInvoicedNote GetClosingInvoicedNote(string closingInvoicedNoteId)
        {
            try
            {
                var closingInvoicedNote = _dbContext.ClosingInvoicedNotes
                    .Where(ci => ci.Id.Equals(closingInvoicedNoteId))
                    .FirstOrDefault();

                return new DTOClosingInvoicedNote()
                {
                    Id = closingInvoicedNote.Id,
                    AttachFileName = closingInvoicedNote.AttachFileName,
                    CreatedAt = closingInvoicedNote.CreatedAt,
                    Note = closingInvoicedNote.Note,
                    ClosingInvoicedId = closingInvoicedNote.ClosingInvoicedId,
                    UserName = closingInvoicedNote.UserName
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DTOClosingInvoiced CreateOrUpdate(DTOClosingInvoiced dtoClosingInvoiced)
        {
            try
            {
                var closingInvoiced = new ClosingInvoiced();
                if (!string.IsNullOrEmpty(dtoClosingInvoiced.Id))
                {
                    var closingInvoiceUpd = _dbContext.ClosingsInvoiced
                        .Where(ci => ci.Id.Equals(dtoClosingInvoiced.Id))
                        .FirstOrDefault();

                    if (closingInvoiceUpd is null)
                        throw new Exception("El cierre no existe");

                    var customer = this.GetCustomers().Where(c => c.IdStr.Equals(dtoClosingInvoiced.CustomerNum)).FirstOrDefault();
                    var work = this.GetWorks(customer.IdStr).Where(w => w.IdStr.Equals(dtoClosingInvoiced.WorkNo)).FirstOrDefault();
                    var administrator = this.GetAdministrators(customer.IdStr, work.IdStr).Where(a => a.IdStr.Equals(dtoClosingInvoiced.AdminId)).FirstOrDefault();

                    closingInvoiceUpd.AdminId = dtoClosingInvoiced.AdminId;
                    closingInvoiceUpd.AdminName = administrator.Description;
                    closingInvoiceUpd.CustomerName = customer.Description;
                    closingInvoiceUpd.CustomerNum = dtoClosingInvoiced.CustomerNum;
                    closingInvoiceUpd.Date = dtoClosingInvoiced.Date;
                    closingInvoiceUpd.Period = dtoClosingInvoiced.Date.ToString("yyyy-MM");
                    closingInvoiceUpd.EBNumber = dtoClosingInvoiced.EBNumber;
                    closingInvoiceUpd.EBDate = dtoClosingInvoiced.EBDate;
                    closingInvoiceUpd.Confirmed = dtoClosingInvoiced.Confirmed;
                    closingInvoiceUpd.TotalInvoiceAmount = dtoClosingInvoiced.TotalInvoiceAmount;
                    closingInvoiceUpd.WorkName = work.Description;
                    closingInvoiceUpd.WorkNo = dtoClosingInvoiced.WorkNo;
                    closingInvoiceUpd.OthersWorkNo = dtoClosingInvoiced.OthersWorkNo;
                    closingInvoiceUpd.ConfirmedDate = dtoClosingInvoiced.ConfirmedDate;
                    closingInvoiceUpd.Note = dtoClosingInvoiced.Note;

                    _dbContext.ClosingsInvoiced.Update(closingInvoiceUpd);
                }
                else
                {
                    var customer = this.GetCustomers().Where(c => c.IdStr.Equals(dtoClosingInvoiced.CustomerNum)).FirstOrDefault();
                    var work = this.GetWorks(customer.IdStr).Where(w => w.IdStr.Equals(dtoClosingInvoiced.WorkNo)).FirstOrDefault();
                    var administrator = this.GetAdministrators(customer.IdStr, work.IdStr).Where(a => a.IdStr.Equals(dtoClosingInvoiced.AdminId)).FirstOrDefault();
                    closingInvoiced = new ClosingInvoiced()
                    {
                        Id = Guid.NewGuid().ToString(),
                        AdminId = dtoClosingInvoiced.AdminId,
                        AdminName = administrator.Description,
                        CustomerName = customer.Description,
                        CustomerNum = dtoClosingInvoiced.CustomerNum,
                        Date = dtoClosingInvoiced.Date,
                        Period = dtoClosingInvoiced.Date.ToString("yyyy-MM"),
                        EBDate = dtoClosingInvoiced.EBDate,
                        EBNumber = dtoClosingInvoiced.EBNumber,
                        Confirmed = dtoClosingInvoiced.Confirmed,
                        TotalInvoiceAmount = dtoClosingInvoiced.TotalInvoiceAmount,
                        WorkName = work.Description,
                        WorkNo = dtoClosingInvoiced.WorkNo,
                        OthersWorkNo = dtoClosingInvoiced.OthersWorkNo,
                        ConfirmedDate = dtoClosingInvoiced.ConfirmedDate,
                        Note = dtoClosingInvoiced.Note,
                    };

                    _dbContext.ClosingsInvoiced.Add(closingInvoiced);
                }

                _dbContext.SaveChanges();
                _dbContext.DetachAll();

                return new DTOClosingInvoiced()
                {
                    Id = closingInvoiced.Id,
                    AdminId = closingInvoiced.AdminId,
                    AdminName = closingInvoiced.AdminName,
                    CustomerName = closingInvoiced.CustomerName,
                    CustomerNum = closingInvoiced.CustomerNum,
                    Date = closingInvoiced.Date,
                    EBDate = closingInvoiced.EBDate,
                    EBNumber = closingInvoiced.EBNumber,
                    Confirmed = closingInvoiced.Confirmed,
                    TotalInvoiceAmount = closingInvoiced.TotalInvoiceAmount,
                    WorkName = closingInvoiced.WorkName,
                    WorkNo = closingInvoiced.WorkNo,
                    OthersWorkNo = closingInvoiced.OthersWorkNo,
                    ConfirmedDate = closingInvoiced.ConfirmedDate,
                    Note = closingInvoiced.Note,
                    Period = closingInvoiced.Period,
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DTOClosingInvoiced> ConfirmAsync(DTOConfirmClosingInvoiced confirmClosingInvoiced)
        {
            try
            {
                var closingInvoiced = new DTOClosingInvoiced();
                var closingInvoicedUpd = _dbContext.ClosingsInvoiced
                    .Where(si => si.Id.Equals(confirmClosingInvoiced.ClosingInvoiced.Id))
                    .FirstOrDefault();

                if (!(closingInvoicedUpd is null))
                {
                    var electronicBillingRepo = this._configuration.GetValue<string>("ElectronicBilling:RepoFiles");
                    var attachFileName = "";
                    if (!(confirmClosingInvoiced.Files is null) && (confirmClosingInvoiced.Files.Length != 0))
                    {
                        var file = confirmClosingInvoiced.Files[0];
                        var ext = Path.GetExtension(file.Name);

                        attachFileName = string.Format("{0}/{1}{2}", electronicBillingRepo, Guid.NewGuid().ToString(), ext);
                        using (var ms = new MemoryStream())
                        {
                            var reader = new System.IO.StreamReader(file.Data);
                            await reader.BaseStream.CopyToAsync(ms);

                            var fileBytes = ms.ToArray();
                            //string s = Convert.ToBase64String(fileBytes);

                            using (var imageFile = new FileStream(attachFileName, FileMode.Create))
                            {
                                imageFile.Write(fileBytes, 0, fileBytes.Length);
                                imageFile.Flush();
                            }
                        }
                    }

                    var note = new ClosingInvoicedNote()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Note = confirmClosingInvoiced.Note,
                        AttachFileName = attachFileName,
                        CreatedAt = DateTime.Now,
                        UserName = confirmClosingInvoiced.UserName,
                        ClosingInvoicedId = confirmClosingInvoiced.ClosingInvoiced.Id,
                    };

                    _dbContext.ClosingInvoicedNotes.Add(note);

                    closingInvoicedUpd.Confirmed = true;
                    closingInvoicedUpd.ConfirmedDate = DateTime.Now;
                    closingInvoicedUpd.Note = confirmClosingInvoiced.Note;

                    _dbContext.ClosingsInvoiced.Update(closingInvoicedUpd);

                    _dbContext.SaveChanges();
                    _dbContext.DetachAll();

                    closingInvoiced = this.GetClosingInvoiced(closingInvoicedUpd.Id);
                }

                return closingInvoiced;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<DTOSalesInvoice> GetSalesInvoices(string guidfilter)
        {
            try
            {
                DTOElectronicBillingFilter reportFilter = this.GetReportFilter(guidfilter);
                List<DTOSalesInvoice> salesInvoices = this.GetSalesInvoices(reportFilter);

                var iqueryable = this.ApplyQuery(salesInvoices.AsQueryable(), reportFilter.GridQuery);

                return iqueryable.ToDynamicList<DTOSalesInvoice>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IQueryable ApplyQuery<T>(IQueryable<T> items, Radzen.Query query) where T : class
        {
            if (!(query is null))
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p);
                    }
                }
                
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    items = items.Where(query.Filter);
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }
            }

            return items;
        }

        public List<DTOOPInvoiced> GetOPsInvoiced(string guidfilter)
        {
            try
            {
                DTOElectronicBillingFilter reportFilter = this.GetReportFilter(guidfilter);
                List<DTOOPInvoiced> opsInvoiced = this.GetOPsInvoiced(reportFilter);

                var iqueryable = this.ApplyQuery(opsInvoiced.AsQueryable(), reportFilter.GridQuery);

                return iqueryable.ToDynamicList<DTOOPInvoiced>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<DTOClosingInvoiced> GetClosingsInvoiced(string guidfilter)
        {
            try
            {
                DTOElectronicBillingFilter reportFilter = this.GetReportFilter(guidfilter);
                List<DTOClosingInvoiced> closingsInvoiced = this.GetClosingsInvoiced(reportFilter);

                var iqueryable = this.ApplyQuery(closingsInvoiced.AsQueryable(), reportFilter.GridQuery);

                return iqueryable.ToDynamicList<DTOClosingInvoiced>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<DTOSalesInvoiceTracking> GetSalesInvoiceTrackings(string guidfilter)
        {
            try
            {
                List<DTOSalesInvoiceTracking> salesInvoiceTrackings = new List<DTOSalesInvoiceTracking>();
                DTOElectronicBillingFilter reportFilter = this.GetReportFilter(guidfilter);

                DateTime provisionDate = new DateTime(reportFilter.Year, reportFilter.MonthSelected, 1);
                var provisionPeriod = provisionDate.ToString("yyyy-MM");
                var salesInvoices = _dbContext.SalesInvoices
                    .Where(s => s.Period.Equals(provisionPeriod))
                    //&& s.AdminName.Equals("PAEZ TAPIA YONAIRO"))
                    .Select(si => new DTOSalesInvoice()
                    {
                        Id = si.Id,
                        AdminId = si.AdminId,
                        CustomerName = si.CustomerName,
                        CustomerNum = si.CustomerNum,
                        Date = si.Date,
                        DateFilter = si.Date.ToString("dd/MM/yyyy"),
                        InvoiceNum = si.InvoiceNum,
                        PO = si.PO,
                        WorkName = si.WorkName,
                        WorkNo = si.WorkNo,
                        EBDate = si.EBDate,
                        EBNumber = si.EBNumber,
                        Status = si.Status,
                        Year = si.Date.Year,
                        Month = si.Date.Month,
                        Rent = si.Rent,
                        AdditionalCharges = si.AdditionalCharges,
                        ProductCharges = si.ProductCharges,
                        NetAmountValue = si.NetAmountValue,
                        TotalInvoiceAmount = si.TotalInvoiceAmount,
                        TotalBalanceAmount = si.TotalBalanceAmount,
                        AdminName = si.AdminName,
                        Note = si.Note,
                        BillingNote = si.BillingNote,
                        RequiredActa = si.RequiredActa,
                        ZoneId = si.ZoneId,
                        ZoneName = si.ZoneName,
                        ZoneParentId = si.ZoneParentId,
                        ZoneParentName = si.ZoneParentName,
                        LastTrackingDate = si.LastTrackingDate,
                        Period = si.Period
                    })
                    .ToList();

                reportFilter.CalcOutsBalance = false;
                reportFilter.Status = 0;
                reportFilter.Period = provisionPeriod; 
                var pendings = this.GetSalesInvoices(reportFilter);
                foreach (var pending in pendings)
                {
                    if (pending.EBDate.Year > 1)
                        pending.TotalPendingAmountBalance = pending.TotalBalanceAmount;
                    else
                        pending.TotalPendingAmountBalance = pending.NetAmountValue;

                    pending.NetAmountValue = 0;
                    pending.TotalInvoiceAmount = 0;
                    pending.TotalBalanceAmount = 0;
                }

                salesInvoices.AddRange(pendings);

                var groupBy = reportFilter.GroupBy.ToList();
                groupBy.Add("EBDate");

                var lambda = this.GroupByExpression<DTOSalesInvoice>(groupBy.ToArray());

                var preSalesInvoiceTrackings = salesInvoices.AsQueryable()
                    .GroupBy(lambda.Compile())
                    .Select(b => this.TrackingSelect(reportFilter.GroupBy, b))
                    .ToList();

                DateTime toDate = provisionDate.AddMonths(1).AddDays(-1);

                var preInvoiced = salesInvoices.AsQueryable()
                    .Where(si => !si.EBDate.ToString("yyyy").Equals("0001") && (si.EBDate <= toDate) && (si.Period.Equals(provisionPeriod)))
                    .GroupBy(lambda.Compile())
                    .Select(b => this.TrackingSelect(reportFilter.GroupBy, b))
                    .ToList();                               

                int toy = DateTime.Now.Year;
                int tom = DateTime.Now.Month;                

                foreach ( var s in preSalesInvoiceTrackings)
                {
                    s.ProvisionPeriod = provisionPeriod;

                    int y = reportFilter.Year;
                    int m = reportFilter.MonthSelected;
                    if (preInvoiced.Count().Equals(0))
                        m += 1;

                    if(m.Equals(13))
                    {
                        y++;
                        m = 1;
                    }
                    int col = 1;                    

                    decimal sumTotalInvoiceAmount = 0;
                    //decimal sumBalanceTotalAmount = 0;

                    while (y <= toy)
                    {
                        if(y.Equals(toy))
                        {
                            for (_ = m;  m <= tom; m++)
                            {
                                var ebDate = new DateTime(y, m, 1).ToString("yyyy-MM");
                                s.GetType().GetProperty($"InvoiceAmount{col}Label").SetValue(s, ebDate);
                                if (ebDate.Equals(s.Period) && !s.Period.Equals("0001-01"))
                                {
                                    //var totalInvoiceAmunt = s.TotalInvoiceAmount;
                                    s.GetType().GetProperty($"InvoiceAmount{col}").SetValue(s, s.TotalInvoiceAmount);
                                    sumTotalInvoiceAmount += s.TotalInvoiceAmount;                                  
                                }

                                //sumBalanceTotalAmount += s.BalanceTotalAmount;

                                col++;
                            }
                        }
                        else if(y != toy)
                        {

                            for (_ = m; m <= 12; m++)
                            {
                                var ebDate = new DateTime(y, m, 1).ToString("yyyy-MM");
                                s.GetType().GetProperty($"InvoiceAmount{col}Label").SetValue(s, ebDate);
                                if (ebDate.Equals(s.Period) && !s.Period.Equals("0001-01"))
                                {
                                    //var totalInvoiceAmunt = s.TotalInvoiceAmount;
                                    s.GetType().GetProperty($"InvoiceAmount{col}").SetValue(s, s.TotalInvoiceAmount);
                                    sumTotalInvoiceAmount += s.TotalInvoiceAmount;
                                }

                                //sumBalanceTotalAmount += s.BalanceTotalAmount;

                                col++;
                            }
                        }                        

                        if (m.Equals(13))
                            m = 1;

                        y++;
                    }

                    s.SumTotalInvoiceAmount = sumTotalInvoiceAmount;
                    //s.BalanceTotalAmount = s.BalanceTotalAmount;
                }

                if (preSalesInvoiceTrackings.Count != 0)
                {
                    groupBy = new List<string>
                    {
                        "GroupBy1Key",
                        "GroupBy2Key",
                        "GroupBy3Key",
                        "GroupBy4Key",
                        "GroupBy5Key",
                        "GroupBy6Key"
                    };

                    var lambdaTracking = this.GroupByExpression<DTOSalesInvoiceTracking>(groupBy.ToArray());

                    salesInvoiceTrackings = preSalesInvoiceTrackings.AsQueryable()
                        .GroupBy(lambdaTracking.Compile())
                        .Select(t => this.Select(t))
                        .ToList();
                }
                else
                {
                    var salesInvoiceTracking = new DTOSalesInvoiceTracking();
                    int column = 1;
                    foreach (var gb in reportFilter.GroupBy.ToList())
                    {
                        string label = "";
                        switch(gb)
                        {
                            case "AdminId":
                                label = "ADMINSTRADOR";
                                break;

                            case "ZoneParentId":
                                label = "ZONA";
                                break;

                            case "WorkNo":
                                label = "OBRA";
                                break;

                            case "CustomerNum":
                                label = "CLIENTE";
                                break;
                        }

                        salesInvoiceTracking.GetType().GetProperty($"GroupBy{column}Label").SetValue(salesInvoiceTracking, label);

                        column++;
                    }

                    int y = reportFilter.Year;
                    int m = reportFilter.MonthSelected + 1;
                    int col = 1;

                    while (y <= toy)
                    {
                        if (y.Equals(toy))
                        {
                            for (_ = m; m <= tom; m++)
                            {
                                var ebDate = new DateTime(y, m, 1).ToString("yyyy-MM");
                                salesInvoiceTracking.GetType().GetProperty($"InvoiceAmount{col}Label").SetValue(salesInvoiceTracking, ebDate);

                                col++;
                            }
                        }
                        else if (y != toy)
                        {

                            for (_ = m; m <= 12; m++)
                            {
                                var ebDate = new DateTime(y, m, 1).ToString("yyyy-MM");
                                salesInvoiceTracking.GetType().GetProperty($"InvoiceAmount{col}Label").SetValue(salesInvoiceTracking, ebDate);

                                col++;
                            }
                        }

                        if (m.Equals(13))
                            m = 1;

                        y++;
                    }

                    salesInvoiceTracking.ProvisionPeriod = provisionPeriod;

                    salesInvoiceTrackings.Add(salesInvoiceTracking);
                }

                return salesInvoiceTrackings;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<CommonData> GetPOs()
        {
            try
            {
                var pos = _dbContext.vwPOs
                    .Select(po => new CommonData()
                    {
                        IdStr = po.PO,
                        Description = po.PO
                    })
                    .ToList();

                return pos;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DTOSalesInvoice> EnableSalesInvoiceAsync(DTOConfirmSalesInvoice confirmSalesInvoice, bool complete = false)
        {
            try
            {
                var salesInvoiceUpd = _dbContext.SalesInvoices
                    .Where(si => si.Id.Equals(confirmSalesInvoice.SalesInvoice.Id))
                    .FirstOrDefault();

                if(salesInvoiceUpd != null)
                {
                    salesInvoiceUpd.EBNumber = "";
                    salesInvoiceUpd.EBDate = DateTime.MinValue;
                    if(complete)
                    {
                        salesInvoiceUpd.CancelledDate = DateTime.MinValue;
                        salesInvoiceUpd.ConfirmedDate = DateTime.MinValue;
                        salesInvoiceUpd.Status = SalesInvoiceStatus.None;
                        salesInvoiceUpd.BillingNote = confirmSalesInvoice.Note;

                        var electronicBillingRepo = this._configuration.GetValue<string>("ElectronicBilling:RepoFiles");
                        var attachFileName = "";
                        if (!(confirmSalesInvoice.Files is null) && (confirmSalesInvoice.Files.Length != 0))
                        {
                            var file = confirmSalesInvoice.Files[0];
                            var ext = Path.GetExtension(file.Name);

                            attachFileName = string.Format("{0}/{1}{2}", electronicBillingRepo, Guid.NewGuid().ToString(), ext);
                            using (var ms = new MemoryStream())
                            {
                                var reader = new System.IO.StreamReader(file.Data);
                                await reader.BaseStream.CopyToAsync(ms);

                                var fileBytes = ms.ToArray();
                                //string s = Convert.ToBase64String(fileBytes);

                                using (var imageFile = new FileStream(attachFileName, FileMode.Create))
                                {
                                    imageFile.Write(fileBytes, 0, fileBytes.Length);
                                    imageFile.Flush();
                                }
                            }
                        }

                        var note = new SalesInvoiceNote()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Note = confirmSalesInvoice.Note,
                            AttachFileName = attachFileName,
                            CreatedAt = DateTime.Now,
                            UserName = confirmSalesInvoice.UserName,
                            SalesInvoiceId = confirmSalesInvoice.SalesInvoice.Id,
                            NoteType = NoteType.Billing
                        };

                        _dbContext.SalesInvoiceNotes.Add(note);
                    }

                    _dbContext.SalesInvoices.Update(salesInvoiceUpd);

                    _dbContext.SaveChanges();
                    _dbContext.DetachAll();

                    confirmSalesInvoice.SalesInvoice = this.GetSalesInvoice(confirmSalesInvoice.SalesInvoice.Id);
                }

                return confirmSalesInvoice.SalesInvoice;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
