using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace adesoft.adepos.webview.Data.Model
{
    public class AlertXOrder : BaseEntity
    {
        public AlertXOrder()
        {

        }
        [Key]
        public long AlertXOrderId { get; set; }
        public long TransactionId { get; set; }
        public string Description { get; set; }
        public bool IsAttend { get; set; }

        public DateTime DateRecord { get; set; }


        [NotMapped]
        public string DescriptionShow
        {
            get
            {
                if (Description != null && Description.Length > 60)
                {
                    return Description.Substring(0, 57) + "...";
                }
                else
                {
                    return Description;
                }
            }
        }

    }
}
