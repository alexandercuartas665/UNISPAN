using System.ComponentModel.DataAnnotations;
using System;

namespace adesoft.adepos.webview.Data.Model.ElectronicBilling
{
    public class OPInvoicedNote
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Note { get; set; }

        [StringLength(60)]
        public string UserName { get; set; }

        [StringLength(36)]
        public string OPInvoicedId { get; set; }

        public string AttachFileName { get; set; }

        public OPInvoiced OPInvoiced { get; set; }
    }
}
