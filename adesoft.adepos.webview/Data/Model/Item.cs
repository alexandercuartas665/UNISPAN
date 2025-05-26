using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class Item : BaseEntity
    {
        public Item()
        {
            ListItemKits = new List<ItemKit>();
        }

        [Key]
        public long ItemId { get; set; }

        public string Description { get; set; }

        public string Barcode { get; set; }

        public string Referencia { get; set; }

        public long CategoryId { get; set; }

        public Category Category { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioDef { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioBase { get; set; }

        public long UnitMeasurementId { get; set; }

        public UnitMeasurement UnitMeasurement { get; set; }

        public long TypeTaxId { get; set; }

        public TypeTax TypeTax { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceCost { get; set; }


        public bool HasIventory { get; set; }

        public bool IsCompuesto { get; set; }
        /// <summary>
        /// 3 ENCOFRADOS , 4 ACCESORIOS
        /// </summary>
        public long categoryMedicionId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Area { get; set; }

        public string Syncode { get; set; }

    
        /// <summary>
        /// Precio que se usa en un plano para traer el codigo  8.5 del inventario y para sincronizar el precio externo
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
      
        public decimal PrecioSyncode { get; set; }


        [NotMapped]
        public decimal Cant { get; set; }

        [NotMapped]
        public long NumOrder { get; set; }

        [NotMapped]

        public long Aux1 { get; set; }

        [NotMapped]

        public List<ItemKit> ListItemKits { get; set; }

        [StringLength(36)]
        public string? ZoneProductId { get; set; }

        public ZoneProduct? ZoneProduct { get; set; }


        [NotMapped]
        public string BarcodeAndName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    return Barcode + " " + Description;
                }
                else
                {
                    return Description;
                }
            }
        }

        [NotMapped]
        public Category CategoryMedicion { get; set; }

        [NotMapped]
        public string DescriptionShow
        {
            get
            {
                if (Description != null && Description.Length > 60)
                {
                    return Description.Substring(0, 57) + "...";
                }
                else
                {
                    return Description;
                }
            }
        }




   
    }
}
