using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class UserApp
    {
        [Key]
        public long UserAppId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public long RoleAppId { get; set; }
        public RoleApp RoleApp { get; set; }
        public string PassworNotCry { get; set; }

       
        public long CompanyId { get; set; }

        [StringLength(36)]
        public string AdminId { get; set; }


        [NotMapped]

        public string CompaniaDB { get; set; }

        [NotMapped]
        public string Sede { get; set; }

        [NotMapped]
        public ConnectionDB ConnectionDB { get; set; }


        [StringLength(36)]
        public string ZoneProductId { get; set; }

        public ZoneProduct ZoneProduct { get; set; }

        //public List<ActionApp> ActionApps { get; set; }

    }
}
