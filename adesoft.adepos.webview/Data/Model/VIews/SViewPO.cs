using System.ComponentModel.DataAnnotations;

namespace adesoft.adepos.webview.Data.Model.VIews
{
    public class SViewPO
    {
        [Key]
        [StringLength(100)]
        public string PO { get; set; }
    }
}
