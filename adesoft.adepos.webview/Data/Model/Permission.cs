using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class Permission
    {


        [Key, Column(Order = 0)]
        public long ActionAppId { get; set; }
        [Key, Column(Order = 1)]
        public long RoleAppId { get; set; }
        

        
    }
}
