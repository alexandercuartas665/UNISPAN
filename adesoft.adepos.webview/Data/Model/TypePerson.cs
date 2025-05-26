using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class TypePerson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TypePersonId { get; set; }

        public string Name { get; set; }
    }
}
