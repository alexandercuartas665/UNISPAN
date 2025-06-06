﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using adesoft.adepos.webview.Data.Model.ElectronicBilling;
using DocumentFormat.OpenXml.Spreadsheet;

namespace adesoft.adepos.webview.Data.DTO.ElectronicBilling
{
    public class DTOSalesInvoice
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string DateFilter { get; set; }

        public string InvoiceNum { get; set; }

        public string CustomerNum { get; set; }

        public string CustomerName { get; set; }

        public string WorkNo { get; set; }

        public string WorkName { get; set; }

        public string ZoneId { get; set; }

        public string ZoneName { get; set; }

        public string ZoneParentId { get; set; }

        public string ZoneParentName { get; set; }

        public string PO { get; set; }

        public string AdminId { get; set; }

        public string AdminName { get; set; }

        public decimal Rent { get; set; }

        public decimal AdditionalCharges { get; set; }

        public decimal ProductCharges { get; set; }

        public decimal NetAmountValue { get; set; }

        public decimal TotalInvoiceAmount { get; set; }

        public decimal TotalBalanceAmount { get; set; }

        public string EBNumber { get; set; }

        public SalesInvoiceStatus Status { get; set; }

        public bool Confirmed { get; set; }

        public DateTime ConfirmedDate { get; set; }

        public DateTime CancelledDate { get; set; }

        public string Note { get; set; }

        public string BillingNote { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public ICollection<DTOSalesInvoiceNote> SalesInvoiceNotes { get; set; }

        public decimal TotalOutstandingBalance { get; set; }

        public decimal TotalPendingAmountBalance { get; set; }

        public decimal TotalInvoiceAmountBalance { get; set; }

        public int QtyPending { get; set; }

        public int QtyInvoiced { get; set; }

        public int QtyCancelled { get; set; }

        public bool RequiredActa { get; set; }

        public DateTime EBDate { get; set; }

        public DateTime LastTrackingDate { get; set; }

        public bool Partial { get; set; }

        public bool Cancelled { get; set; }

        public string Period { get; set; }
    }
}
