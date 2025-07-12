using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adesoft.adepos.webview.Data.Model.PL
{
    [Table("Obras")] 
    public class Obras
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        public int ClienteId { get; set; }
        public int? CiudadId { get; set; }
        public int? ComercialId { get; set; }

        public string Correos { get; set; }

        public bool Activo { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        // Propiedad de navegación para la relación con LogisticMasterData
        [ForeignKey("ClienteId")]
        public virtual LogisticMasterData Cliente { get; set; }

        [ForeignKey("CiudadId")]
        public virtual LogisticMasterData Ciudad { get; set; }

        [ForeignKey("ComercialId")]
        public virtual LogisticMasterData Comercial { get; set; }
    }
}