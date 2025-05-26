using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.PL
{
    public class OrderPicture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long OrderId { get; set; }

        public OrderType OrderType { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }        
    }
}
