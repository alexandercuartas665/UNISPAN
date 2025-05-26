using System.ComponentModel.DataAnnotations;
using System;

namespace adesoft.adepos.webview.Data.Model.ElectronicBilling
{
    public class ClosingInvoicedNote
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Note { get; set; }

        [StringLength(60)]
        public string UserName { get; set; }

        [StringLength(36)]
        public string ClosingInvoicedId { get; set; }

        public string AttachFileName { get; set; }

        public ClosingInvoiced ClosingInvoiced { get; set; }
    }
}
