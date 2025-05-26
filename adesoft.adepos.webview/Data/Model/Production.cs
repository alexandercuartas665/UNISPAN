using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class Production : BaseEntity
    {
        public Production()
        {
            DetailTerceros = new List<DetailProductionTercero>();
            DetailProductions = new List<DetailProduction>();
            DetailTercerosDetele = new List<DetailProductionTercero>();
            DetailProductionsDetele = new List<DetailProduction>();
        }
        [Key]
        public long ProductionId { get; set; }

        public long Consecutive { get; set; }

        public DateTime DateProduction { get; set; }
        [NotMapped]
        public string DateProductionLabel
        {
            get
            {
                return DateProduction.ToString("dd/MMMM/yyyy", CultureInfo.GetCultureInfo("ES-co"));
            }
        }

        public long TypeActivityId { get; set; }

        public long StateId { get; set; }

        public long UserAppId { get; set; }

        public string ConsecutiveMobileId { get; set; }

        public string Photo1Name { get; set; }

        public string Photo2Name { get; set; }

        public string Photo3Name { get; set; }

        [NotMapped]
        public string Photo1Base64 { get; set; }
        [NotMapped]
        public string Photo2Base64 { get; set; }
        [NotMapped]
        public string Photo3Base64 { get; set; }

        [NotMapped]
        public long CategoryMedicionId { get; set; }

        public List<DetailProductionTercero> DetailTerceros { get; set; }

        public List<DetailProduction> DetailProductions { get; set; }

        [NotMapped]
        public List<DetailProductionTercero> DetailTercerosDetele { get; set; }

        [NotMapped]
        public List<DetailProduction> DetailProductionsDetele { get; set; }

        [NotMapped]
        public TypeActivity TypeActivity { get; set; }

        [NotMapped]
        public string ReadTerceros
        {
            get
            {
                string varconcat = string.Empty;
                if (DetailTerceros.Count > 0)
                {
                    DetailTerceros.ForEach(x => {
                        varconcat += x.Tercero.FullNameCode + Environment.NewLine;
                    });
                    return varconcat;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

    }
}
