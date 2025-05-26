using BlazorInputFile;

namespace adesoft.adepos.webview.Data.DTO.ElectronicBilling
{
    public class DTOConfirmOPInvoiced
    {
        public DTOOPInvoiced OPInvoiced { get; set; }

        public string Note { get; set; }

        public string AttachFileName { get; set; }

        public string UserName { get; set; }

        public IFileListEntry[] Files { get; set; }
    }
}
