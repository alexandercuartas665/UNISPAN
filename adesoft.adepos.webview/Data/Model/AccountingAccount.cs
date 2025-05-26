using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    /// <summary>
    /// Cuenta contable: Me sirve para el tema de las cuentas contables. Bancos , cajas , activos , patrimonio
    /// </summary>
    public class AccountingAccount : BaseEntity
    {
        public AccountingAccount()
        {

        }
        [Key]
        public long AccountingAccountId { get; set; }
        public string Name { get; set; }
        public long TypeAccountingAccountId { get; set; }

        public TypeAccountingAccount TypeAccountingAccount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }


        public long AccountOwnerId { get; set; }

        //    public Account MyProperty { get; set; }


    }
}
