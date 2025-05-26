using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adeposx.report.Models
{
    public class DTOInventary  
    {
        public DTOInventary() { }

        public long ItemId { get; set; }

        public string Barcode { get; set; }
        public string ItemName { get; set; }

       
        public decimal Cost { get; set; }

    
        public decimal PriceUnd { get; set; }
        
        public decimal CantInv { get; set; }

        public long Warehouseid { get; set; }

        public string WarehouseName { get; set; }


        public decimal CantReservada { get; set; }

     
        public decimal Saldo { get; set; }

        public long TransOption { get; set; }

        public decimal CantFabricacion { get; set; }


        public decimal CantPendienteFabricacion { get; set; }

        public decimal Area { get; set; }

        public decimal Weight { get; set; }

        public string Syncode { get; set; }

        public string ZoneProduct { get; set; }
    }
}
