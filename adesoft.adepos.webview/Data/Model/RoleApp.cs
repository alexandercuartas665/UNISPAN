using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class RoleApp
    {
        [Key]
        public long RoleAppId { get; set; }
        public string Name { get; set; }
        public ICollection<Permission> Permissions { get; set; }

   
        public long CompanyId { get; set; }
    }
}
