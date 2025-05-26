using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOInventary : BaseEntity
    {
        public DTOInventary() { }

        public long ItemId { get; set; }

        public string Barcode { get; set; }
        public string ItemName { get; set; }

     
        public decimal Cost { get; set; }

   
        public decimal PriceUnd { get; set; }
    
        public decimal CantInv { get; set; }

        public long Warehouseid { get; set; }

        public Warehouse Warehouse { get; set; }

        public string WarehouseName { get; set; }


        public decimal CantReservada { get; set; }


        public decimal Saldo { get; set; }



        public decimal CantFabricacion { get; set; }


        public decimal CantPendienteFabricacion { get; set; }

        public decimal Area { get; set; }

        public decimal Weight { get; set; }
        /// <summary>
        /// Cantidad de inventario en toneladas
        /// </summary>
        public decimal CantInvTon { get; set; }
        /// <summary>
        /// Cantidad de inventario en bodega alquiler
        /// </summary>
        public decimal CantAlqu { get; set; }
        /// <summary>
        /// Cantidad de inventario en bodega alquiler toneladas
        /// </summary>
        public decimal CantAlquTon { get; set; }


        public bool ReceiveRequireInspection { get; set; }


        public bool HideZeroInvoiceItems { get; set; }


        public string Syncode { get; set; }

        public string ZoneProduct { get; set; }
    }
}
