using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class TypeAccountingAccount
    {
        [Key]
        public long TypeAccountingAccountId { get; set; }
        public string Name { get; set; }
        public long CodCuenta { get; set; }
        
    }
}
