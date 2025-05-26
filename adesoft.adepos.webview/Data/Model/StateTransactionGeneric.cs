using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class StateTransactionGeneric : BaseEntity
    {
        public StateTransactionGeneric()
        {

        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long StateTransactionGenericId { get; set; }

        public string Name { get; set; }

        public long OrderNum { get; set; }


        public long TypeTransactionId { get; set; }
    }
}

