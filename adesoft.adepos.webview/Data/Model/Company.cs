using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace adesoft.adepos.webview.Data.Model
{
    public class Company : BaseEntity
    {
        [Key]
        public long CompanyId { get; set; }

        public string Name { get; set; }

        public string Nit { get; set; }


        public string Email { get; set; }


        public string Phone1 { get; set; }


        public string Adress { get; set; }

        public long Code { get; set; }

        public string Note { get; set; }

        public bool IsAccountAdmin { get; set; }


        public bool IsEnabled { get; set; }

        public long MenuId { get; set; }

        public string TypeAPP { get; set; }

       

        [Column(TypeName = "decimal(18,2)")]
        public decimal ComisionCredit { get; set; }

        [NotMapped]
        public string Usuario { get; set; }

        [NotMapped]
        public string Password { get; set; }
        [NotMapped]

        public UserApp userApp { get; set; }





    }
}
