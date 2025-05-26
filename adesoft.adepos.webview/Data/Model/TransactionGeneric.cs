using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class TransactionGeneric : BaseEntity
    {
        public TransactionGeneric()
        {

            Details = new List<DetailTransactionGeneric>();

            DetailsRemove = new List<DetailTransactionGeneric>();
            AlertXOrders = new List<AlertXOrder>();
        }

        [Key]
        public long TransactionGenericId { get; set; }

        public long Consecutive { get; set; }

        public string ConsecutiveChar { get; set; }

        public DateTime DateInit { get; set; }

        public DateTime DateEnd { get; set; }

        public DateTime DatePayInit { get; set; }
        /// <summary>
        /// Proovedor , Cliente
        /// </summary>
        public long TerceroId { get; set; }

        public long VendedorId { get; set; }

        public long TypeTransactionId { get; set; }

        public TypeTransaction TypeTransaction { get; set; }

        public List<DetailTransactionGeneric> Details { get; set; }
        /// <summary>
        /// Total Compra 
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalBuy { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cash { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Change { get; set; }
        [Column(TypeName = "decimal(18,2)")]

        public decimal TotalTax { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDiscount { get; set; }

        /// <summary>
        /// Aqui puede aplicar mermas
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOtherDiscount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        public long WarehouseOriginId { get; set; }

        public long WarehouseDestinId { get; set; }

        public long StateTransactionGenericId { get; set; }

        public long AuxTerceroId { get; set; }
        public StateTransactionGeneric StateTransactionGeneric { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAnticipo { get; set; }

        /// <summary>
        /// Total a pagar ... aqui puede aplicar descuentos x anticipos
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPay { get; set; }

        public string Note { get; set; }

        public string DocumentExtern { get; set; }

        public string NameWork { get; set; }
        /// <summary>
        /// turnos de despacho .. de orden de trabajo y otros
        /// </summary>
        public long TurnId { get; set; }







        /// <summary>
        /// Para uso de APIs
        /// </summary>
        [NotMapped]
        public string SDateInit { get; set; }

        [NotMapped]
        public string SDateEnd { get; set; }
        [NotMapped]

        public Tercero AUxTercero { get; set; }
        [NotMapped]
        public Tercero AUxTercero2 { get; set; }



        [NotMapped]
        public List<DetailTransactionGeneric> DetailsRemove { get; set; }

        [NotMapped]
        public List<AlertXOrder> AlertXOrders { get; set; }



        [NotMapped]
        public DateTime? DateReportInit { get; set; }
        [NotMapped]
        public DateTime? DateReportEnd { get; set; }

        [NotMapped]
        public string ComercialNameTercerp
        {
            get
            {
                if (AUxTercero != null)
                    return (string.IsNullOrEmpty(AUxTercero.ComercialName) ? "" : AUxTercero.ComercialName);
                else
                    return "";
            }
        }


        [NotMapped]

        public string AuxTest { get; set; }

        [NotMapped]

        public Warehouse Warehouse { get; set; }

        [NotMapped]

        public static DateTime DateAplazados = new DateTime(2100, 1, 1);
        [NotMapped]
        public bool selectedToGenerateOrden { get; set; }

        [NotMapped]
        public string TextShowOrder
        {
            get
            {
                if (DocumentExtern != null && NameWork != null)
                    return "T-" + TurnId.ToString() + " - " + DocumentExtern + " - " + NameWork;
                else return "";
            }
        }

        [StringLength(100)]
        public string Works { get; set; }

        [StringLength(20)]
        public string CustomerAccount { get; set; }

        public int SalesPersonId { get; set; }

        public int ModuleId { get; set; }

        public int CityId { get; set; }

        public int ReponsableTransId { get; set; }

        public int VehicleTypeId { get; set; }

        public decimal Wight { get; set; }

        public bool Scheduled { get; set; }
    }
}
