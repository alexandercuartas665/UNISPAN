using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model.PL
{
    public class OrderComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long OrderId { get; set; }

        public OrderType OrderType { get; set; }

        public DateTime CreatedDatetTime { get; set; }

        public string Comment { get; set; }
    }
}
