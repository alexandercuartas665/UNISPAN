using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class TypeTercero : BaseEntity
    {
        public TypeTercero()
        {

        }
        [Key]
        public long TypeTerceroId { get; set; }

        public string Name { get; set; }


        public string CampoPrueba { get; set; }
    }
}
