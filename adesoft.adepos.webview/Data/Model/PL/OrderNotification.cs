using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adesoft.adepos.webview.Data.Model.PL
{
    [Table("OrderNotifications")] //Especifica el nombre exacto de la tabla en la BD
    public class OrderNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long OrderId { get; set; }

        public OrderType OrderType { get; set; }

        public DateTime NotificationDate { get; set; }

        public string NotifiedBy { get; set; } //Campo para el usuario

        //Propiedad de navegación para que EF sepa cómo relacionarse con la orden padre
        [ForeignKey("OrderId, OrderType")]
        public virtual Order Order { get; set; }
    }
}