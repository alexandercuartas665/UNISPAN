using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models
{
    public class DTORptMovEquipo
    {
        public DTORptMovEquipo()
        {

        }

        public string CodigoItem { get; set; }

        public string DescripcionItem { get; set; }
        public decimal TotalDespacho { get; set; }

        public decimal TotalDevolucion { get; set; }

        public decimal Neto { get; set; }

        public decimal TotalCierre { get; set; }

        public decimal TotalPeso { get; set; }

        public decimal Cant { get; set; }
        public decimal CantDespacho { get; set; }

        public decimal CantDevolucion { get; set; }
        public decimal CantCierre { get; set; }
        public string TipoDocument { get; set; }

        public DateTime DateMov { get; set; }
        public int Mes { get; set; }

        public int Ano { get; set; }

        public DTOMonth ObjMes { get; set; }


        public string EtiquetaMesAno { get; set; }



        public decimal TonEnAlquiler { get; set; }

        public decimal CantInvenTon { get; set; }
        

        public decimal IngAlquiler { get; set; }


        public decimal VrTon { get; set; }


        public string TitleFilterReport { get; set; }
    }
}