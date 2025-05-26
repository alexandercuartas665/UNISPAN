using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adesoft.adeposx.report.Models
{
    public class DTOOportunidadesCRM
    {
        public DTOOportunidadesCRM()
        {

        }
        public long OportunidadID { get; set; }
        public string OPRT_NUMERO { get; set; }
        public string FECHA_APERTURA { get; set; }
        public DateTime FECHA_APERTURA_ { get; set; }

        public string CONSECUTIVO { get; set; }

        public string TIPO_NEGOCIO { get; set; }

        public string TIPO_OPRT { get; set; }

        public string NIT { get; set; }

        public string CLIENTE { get; set; }

        public string OBRA { get; set; }

        public string COMERCIAL { get; set; }

        public decimal? VR_VENTA { get; set; }

        public decimal? VR_RENTA_MENSUAL { get; set; }

        public decimal? DURACION { get; set; }

        public decimal? TONELADA { get; set; }

        public string APROBACION { get; set; }

        public string NUM_OP { get; set; }

        public string CONTACTO_CLIENTE { get; set; }

        public DateTime? FECHA_SOLICI_DOC { get; set; }

        public DateTime? FECHA_RECEP_DOC { get; set; }

        public string OBSERVATION { get; set; }

        public DateTime? FECHA_ENVIO { get; set; }

        public string HISTORIAL_OBRA { get; set; }

        public string NUM_CUENTA { get; set; }

        public DateTime? FECHA_RECEP { get; set; }

        public DateTime? FECHA_ANTICIPO { get; set; }

        public string COD_ETAPA { get; set; }


        public string ETAPA { get; set; }

        public DateTime? FECHA_DESPACHO { get; set; }


        [JsonIgnore]
        public decimal PorcentajeOpor { get; set; }



        public void PorcentajeOportSet()
        {

            switch (COD_ETAPA)
            {
                case "E1":
                    PorcentajeOpor = 0;
                    break;
                case "E2":
                    PorcentajeOpor = 10;
                    break;
                case "E3":
                    PorcentajeOpor = 20;
                    break;
                case "E4":
                    PorcentajeOpor = 30;
                    break;
                case "E5":
                    PorcentajeOpor = 40;
                    break;
                case "E6":
                    PorcentajeOpor = 50;
                    break;
                case "E7":
                    PorcentajeOpor = 60;
                    break;
                case "E8":
                    PorcentajeOpor = 70;
                    break;
                case "E9":
                    PorcentajeOpor = 80;
                    break;
                case "F10":
                    PorcentajeOpor = 90;
                    break;
                case "F11":
                    PorcentajeOpor = 100;
                    break;
                default:
                    PorcentajeOpor = 0;
                    break;
            }

        }

    }
}