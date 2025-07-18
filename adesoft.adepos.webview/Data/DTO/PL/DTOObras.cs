using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOObras
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; }
        public string Correos { get; set; }
        public bool Activo { get; set; }
        public int? CiudadId { get; set; }
        public int? ComercialId { get; set; }
        public string NombreCiudad { get; set; }

        public string NombreComercial { get; set; }
    }
}