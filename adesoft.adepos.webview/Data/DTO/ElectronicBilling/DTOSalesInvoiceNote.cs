using adesoft.adepos.webview.Data.Model.ElectronicBilling;
using System.ComponentModel.DataAnnotations;
using System;
using BlazorInputFile;

namespace adesoft.adepos.webview.Data.DTO.ElectronicBilling
{
    public class DTOSalesInvoiceNote
    {
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Note { get; set; }

        public string UserName { get; set; }

        public string SalesInvoiceId { get; set; }

        public string AttachFileName { get; set; }

        public IFileListEntry[] Files { get; set; }

        public NoteType NoteType { get; set; }
    }
}
