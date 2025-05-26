using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations;

namespace adesoft.adepos.webview.Data.Model.ElectronicBilling
{
    public class SalesInvoiceNote
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public string Note { get; set; }

        [StringLength(60)]
        public string UserName { get; set; }

        [StringLength(36)]
        public string SalesInvoiceId { get; set; }

        public string AttachFileName { get; set; }

        public NoteType NoteType { get; set; }

        public SalesInvoice SalesInvoice { get; set; }
    }

    public  enum NoteType
    {
        None,
        Billing 
    }
}
