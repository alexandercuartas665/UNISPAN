using adesoft.adepos.webview.Data.Model.ElectronicBilling;
using System.ComponentModel.DataAnnotations;
using System;
using BlazorInputFile;

namespace adesoft.adepos.webview.Data.DTO.ElectronicBilling
{
    public class DTOClosingInvoicedNote
    {
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Note { get; set; }

        public string UserName { get; set; }

        public string ClosingInvoicedId { get; set; }

        public string AttachFileName { get; set; }

        public IFileListEntry[] Files { get; set; }
    }
}
