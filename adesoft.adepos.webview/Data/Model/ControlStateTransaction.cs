using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class ControlStateTransaction : BaseEntity
    {
        [Key]
        public long ControlStateTransactionId { get; set; }

        public long StateTransactionId { get; set; }

        public long StateTransactionShowId { get; set; }

        public long OrderNum { get; set; }
    }
}
