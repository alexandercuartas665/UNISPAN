using System.Collections.Generic;

namespace adesoft.adepos.webview.Data.DTO.PL
{
    public class DTOObras
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public int ClienteId { get; set; }
        public string Correos { get; set; }
        public bool Activo { get; set; }
    }
}