using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adesoft.adepos.webview.Data.Model.ElectronicBilling
{
    public class ClosingInvoiced
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        public DateTime Date { get; set; }

        [StringLength(20)]
        public string CustomerNum { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        [StringLength(20)]
        public string WorkNo { get; set; }

        [StringLength(100)]
        public string WorkName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalInvoiceAmount { get; set; }

        [StringLength(36)]
        public string AdminId { get; set; }

        [StringLength(60)]
        public string AdminName { get; set; }

        public DateTime ConfirmedDate { get; set; }

        public string Note { get; set; }

        public bool Confirmed { get; set; }

        [StringLength(8)]
        public string Period { get; set; }

        [StringLength(20)]
        public string EBNumber { get; set; }

        public DateTime EBDate { get; set; }

        [StringLength(100)]
        public string OthersWorkNo { get; set; }

        [StringLength(36)]
        public string ZoneId { get; set; }

        [StringLength(100)]
        public string ZoneName { get; set; }

        [StringLength(36)]
        public string ZoneParentId { get; set; }

        [StringLength(100)]
        public string ZoneParentName { get; set; }

        public ICollection<ClosingInvoicedNote> ClosingInvoicedNotes { get; set; }
    }
}
