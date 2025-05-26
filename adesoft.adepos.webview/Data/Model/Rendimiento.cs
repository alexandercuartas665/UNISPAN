using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace adesoft.adepos.webview.Data.Model
{
    public class Rendimiento : BaseEntity
    {
        public Rendimiento()
        {
            ReportNovedades = new List<NominaNovedad>();
        }


        public long TerceroId { get; set; }


        public DateTime DateActivity { get; set; }


        public long TypeActivityId { get; set; }


        public long TypeActivityGroupId { get; set; }

        public decimal GroupMinimumGoalTon { get; set; }
        /// <summary>
        /// Medicion del dia cantidad
        /// </summary>

        public decimal Cant { get; set; }

        public decimal TotalBonificacion { get; set; }

        public decimal TotalBonificacionNotRepeat { get; set; }

        public long IdDay { get; set; }

        public long MonthId { get; set; }

        public long YearId { get; set; }
        /// <summary>
        /// Total de medicion de todo el mes
        /// </summary>

        public decimal TotalMedicion { get; set; }

        public decimal TotalMedicionNotRepeat { get; set; }
        [JsonIgnore]
        public TypeActivity TypeActivity { get; set; }
        /// <summary>
        /// Categoria de la activida Enconfrados o Accesorios
        /// </summary>

        public string CategoryActivityName { get; set; }
        [JsonIgnore]
        public Tercero Tercero { get; set; }


        public string UndMedida { get; set; }

        public decimal MinimumGoalTon { get; set; }
        public decimal MinimumGoal { get; set; }
        public int AcumOperario { get; set; }
        public decimal CantTon { get; set; }

        public decimal DiasAusentismo { get; set; }
        /// <summary>
        /// Es el porcentaje de bonificacion que se va pagar que se calcula en base a los dias de ausentismo
        /// </summary>

        public decimal PorcentajeBonificacion { get; set; }
        /// <summary>
        /// Total Bonificacion *  PorcentajeBonificacion = ValorAPagar
        /// </summary>

        public decimal ValorAPagar { get; set; }

        public decimal ValorAPagarNotRepeat { get; set; }
        public string TerceroName { get; set; }

        public long CodeTercero { get; set; }

        public string TypeActivityName { get; set; }

        public bool ReportaNovedad { get; set; }

        public string ItemBarcode { get; set; }

        public string ItemName { get; set; }

        public long GroupTerceroId { get; set; }

        public long GroupTypeActivityId { get; set; }
        public List<NominaNovedad> ReportNovedades { get; set; }
    }
}
