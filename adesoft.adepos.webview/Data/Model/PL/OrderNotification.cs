using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adesoft.adepos.webview.Data.Model.PL
{
    [Table("OrderNotifications")]
    public class OrderNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long OrderId { get; set; }

        public OrderType OrderType { get; set; }

        public DateTime NotificationDate { get; set; }

        public string NotifiedBy { get; set; }

        // Propiedad de navegación para la relación con la tabla Orders
        [ForeignKey("OrderId, OrderType")]
        public virtual Order Order { get; set; }
    }
}