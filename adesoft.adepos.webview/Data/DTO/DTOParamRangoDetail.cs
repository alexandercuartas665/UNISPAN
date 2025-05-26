using adesoft.adepos.webview.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOParamRangoDetail : ICloneable
    {

        public DTOParamRangoDetail()
        {

        }

        public decimal RangoInit { get; set; }

        public decimal RangoEnd { get; set; }

        public decimal Bono { get; set; }

        public decimal MetaMinimaUnd { get; set; }

        public decimal MetaMinimaTon { get; set; }

        public string Activida { get; set; }

        public string CategoriaActividad { get; set; }
        public long CategoriaActividadId { get; set; }
        public long ActividadId { get; set; }
        [JsonIgnore]
        public string UndMedida
        {
            get
            {
                if (CategoriaActividadId == 3)
                {
                    return "M2";
                }
                else if (CategoriaActividadId == 4)
                {
                    return "KG";
                }
                else
                {
                    return "";
                }
            }

        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
