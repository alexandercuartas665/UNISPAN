using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOComprasReport : ICloneable
    {
        public DTOComprasReport()
        {

        }

        public string TipoDoc { get; set; }
        public DateTime FechaDcto { get; set; }

        public string FechaDctoLbl { get; set; }
        public string DocumentoOC { get; set; }

        public string ParaUsar { get; set; }

        public string IdLocal { get; set; }

        public string IdUnidad { get; set; }

        public decimal CantEntrada { get; set; }

        public decimal TotalVenta { get; set; }

        public string EstadoNom { get; set; }

        public string GuidFilter { get; set; }
        public string IdItem { get; set; }

        public string NombreItem { get; set; }

        public string NombreProveedor { get; set; }

        public long NumMes { get; set; }

        public string NombreMes { get; set; }

        public long Ano { get; set; }

        public decimal CantMes { get; set; }

        public decimal CostMes { get; set; }

        public decimal valorunit { get; set; }

        public string TerceroId { get; set; }
        #region Filtros
        public List<long> Items { get; set; }

        public List<long> Meses { get; set; }

        public List<long> Anos { get; set; }

        public string AuxField { get; set; }

        public decimal VarCant { get; set; }

        public decimal VarTotal { get; set; }

        public decimal VarTotal2 { get; set; }

        public string DetalleDoc { get; set; }

        public bool visibleyear { get; set; }

        public string IdCo { get; set; }

        public string Sede { get; set; }

        public List<DTOInventary> Inventarys { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

    }
}
