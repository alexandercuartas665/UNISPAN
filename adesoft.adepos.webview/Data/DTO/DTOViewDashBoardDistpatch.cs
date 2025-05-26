using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using adesoft.adepos.webview.Util;
namespace adesoft.adepos.webview.Data.DTO
{
   
    public class DTOViewDashBoardDistpatch : ICloneable
    {
        public DTOViewDashBoardDistpatch()
        {
            DTOColumnsDistpatch = new List<DTOColumnsDistpatch>();
            CopyAllCardDistpatchs = new List<DTOCardDistpatch>();
            Warehouses = new List<Warehouse>();
        }

        public List<Warehouse> Warehouses;
        public long WarehouseId { get; set; }
        public List<DTOColumnsDistpatch> DTOColumnsDistpatch { get; set; }


        public List<DTOCardDistpatch> CopyAllCardDistpatchs { get; set; }

        public object Clone()
        {
            // Obtenemos una copia superficial de la clase
            DTOViewDashBoardDistpatch nuevo = (DTOViewDashBoardDistpatch)this.MemberwiseClone();
            // Clonación manual de campos
            //nuevo.Warehouses = this.Warehouses.Clone();
            nuevo.DTOColumnsDistpatch = this.DTOColumnsDistpatch.Clone();
            nuevo.CopyAllCardDistpatchs = this.CopyAllCardDistpatchs.Clone();
            return nuevo;
        }
    }

    public class DTOColumnsDistpatch
    {
        public DTOColumnsDistpatch()
        {
            DTOCardDistpatchs = new List<DTOCardDistpatch>();
        }

        public string ColorCode { get; set; }
        public DateTime DateDistpatch { get; set; }
        public List<DTOCardDistpatch> DTOCardDistpatchs { get; set; }
        public string DateDistpatchLabel
        {
            get
            {
                if (DateDistpatch != DateTime.MinValue)
                {
                    return DateDistpatch.ToString("ddd dd, MMM", CultureInfo.GetCultureInfo("Es-co"));
                }
                else
                {
                    return "";
                }

            }
        }
    }

    public class DTOCardDistpatch
    {
        public DTOCardDistpatch() { }

        public DateTime DateDistpatch { get; set; }
        public long TurnOrder { get; set; }

        public string OrdenNum { get; set; }

        public string CardColor
        {
            get
            {
                if (TransactionDistpatch != null)
                {
                    if (TransactionDistpatch.StateTransactionGenericId == 11)
                    {
                        return "#ff0808";
                    }
                    else
                    {// 12 lista para despacho
                        return "#00a727";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        public TransactionGeneric TransactionDistpatch { get; set; }
    }
}
