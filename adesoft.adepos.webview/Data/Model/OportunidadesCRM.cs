using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class OportunidadesCRM : BaseEntity
    {
        public OportunidadesCRM()
        {

        }
        [Key]
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
        [Column(TypeName = "decimal(18,2)")]
        public decimal? VR_VENTA { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? VR_RENTA_MENSUAL { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DURACION { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TONELADA { get; set; }

        public string APROBACION { get; set; }

        //******************//

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


        public bool IsEdited { get; set; }

        public DateTime? FECHA_DESPACHO { get; set; }


        public string FECHA_ULT_MOD { get; set; }


        /// <summary>
        /// Fecha ulktima modificacion en CRM
        /// </summary>
        public DateTime FECHA_ULT_MOD_ { get; set; }


        [NotMapped]
        public string FECHA_DESPACHO_LBL
        {
            get
            {
                return FECHA_DESPACHO == null ? "" : FECHA_DESPACHO.Value.ToString("yyyy-MM-dd");
            }
        }

        [NotMapped]
        public string FECHA_RECEP_LBL
        {
            get
            {
                return FECHA_RECEP == null ? "" : FECHA_RECEP.Value.ToString("yyyy-MM-dd");
            }
        }

        [NotMapped]
        public DateTime? FilterDateInit { get; set; }

        [NotMapped]
        public DateTime? FilterDateEnd { get; set; }

        [NotMapped]
        public int FilterTipoEtapaId { get; set; }

        [NotMapped]
        public decimal PorcentajeOport
        {
            get
            {
                switch (COD_ETAPA)
                {
                    case "E1":
                        return 0;
                    case "E2":
                        return 10;
                    case "E3":
                        return 20;
                    case "E4":
                        return 30;
                    case "E5":
                        return 40;
                    case "E6":
                        return 50;
                    case "E7":
                        return 60;
                    case "E8":
                        return 70;
                    case "E9":
                        return 80;
                    case "F10":
                        return 90;
                    case "F11":
                        return 100;
                    default:
                        return 0;
                }
            }
        }
    }
}
