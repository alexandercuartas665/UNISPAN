using System.ComponentModel.DataAnnotations;

namespace adesoft.adepos.webview.Data.Model.VIews
{
    public class SViewCustomer
    {
        [Key]
        public string CustomerAccount { get; set; }

        public string CustomerName { get; set; }
    }
}
