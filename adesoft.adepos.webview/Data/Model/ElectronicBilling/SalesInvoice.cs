using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adesoft.adepos.webview.Data.Model.ElectronicBilling
{
    public class SalesInvoice
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        public DateTime Date { get; set; }

        [StringLength(20)]
        public string InvoiceNum { get; set; }

        [StringLength(20)]
        public string CustomerNum { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        [StringLength(20)]
        public string WorkNo { get; set; }

        [StringLength(100)]
        public string WorkName { get; set; }

        [StringLength(36)]
        public string ZoneId { get; set; }

        [StringLength(100)]
        public string ZoneName { get; set; }

        [StringLength(36)]
        public string ZoneParentId { get; set; }

        [StringLength(100)]
        public string ZoneParentName { get; set; }

        [StringLength(60)]
        public string PO { get; set; }

        [StringLength(36)]
        public string AdminId { get; set; }

        [StringLength(60)]
        public string AdminName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Rent { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AdditionalCharges { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ProductCharges { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal NetAmountValue { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalInvoiceAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalBalanceAmount { get; set; }

        [StringLength(20)]
        public string EBNumber { get; set; }

        public SalesInvoiceStatus Status { get; set; }

        [StringLength(1)]
        public string RecordStatus { get; set; }

        [StringLength(8)]
        public string Period { get; set; }

        public DateTime ConfirmedDate { get; set; }

        public DateTime CancelledDate { get; set; }

        public string Note { get; set; }

        public string BillingNote { get; set; }

        public DateTime LastTrackingDate { get; set; }

        public bool RequiredActa { get; set; }

        public DateTime EBDate { get; set; }

        public ICollection<SalesInvoiceNote> SalesInvoiceNotes { get; set; }
    }

    public enum SalesInvoiceStatus
    {
        None,
        Confirmed,
        Cancelled
    }
}
