using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOOportunidadesCRM
    {

        /*E1         0% Contacto Inicial                             
E2         10%   Cotizacion Carta Kit                      
E3         20%   Aprobacion Carta Kit                      
E4         30%   Modulacion                                
E5         40%  Pdte Aprobación Cotizacion                
E6         50%   Validación Cotizacion y Planos Firmados  
E7         60%   Estudio de Credito                        
E8         70%   Contrato y Pagare                         
E9         80%   Liberacion y Logistica                    
F10        90%   Validacion de la Entrega                  
F11        Ganada 100%                                     
G12        40%  Total                                      
G13        90%  Total*/

        public void CargarCantEtapas(List<OportunidadesCRM> lists)
        {
            CantE1 = lists.Where(x => x.COD_ETAPA == "E1").Count();
            CantE2 = lists.Where(x => x.COD_ETAPA == "E2").Count();
            CantE3 = lists.Where(x => x.COD_ETAPA == "E3").Count();
            CantE4 = lists.Where(x => x.COD_ETAPA == "E4").Count();
            CantE5 = lists.Where(x => x.COD_ETAPA == "E5").Count();
            CantE6 = lists.Where(x => x.COD_ETAPA == "E6").Count();
            CantE7 = lists.Where(x => x.COD_ETAPA == "E7").Count();
            CantE8 = lists.Where(x => x.COD_ETAPA == "E8").Count();
            CantE9 = lists.Where(x => x.COD_ETAPA == "E9").Count();
            CantF10 = lists.Where(x => x.COD_ETAPA == "F10").Count();
            CantF11 = lists.Where(x => x.COD_ETAPA == "F11").Count();
            CantG12 = lists.Where(x => x.COD_ETAPA == "G12").Count();
            CantG13 = lists.Where(x => x.COD_ETAPA == "G13").Count();
        }

        public decimal CantE1 { get; set; }

        public decimal CantE2 { get; set; }

        public decimal CantE3 { get; set; }

        public decimal CantE4 { get; set; }
        public decimal CantE5 { get; set; }

        public decimal CantE6 { get; set; }
        public decimal CantE7 { get; set; }

        public decimal CantE8 { get; set; }
        public decimal CantE9 { get; set; }

        public decimal CantF10 { get; set; }
        public decimal CantF11 { get; set; }

        public decimal CantG12 { get; set; }
        public decimal CantG13 { get; set; }

    }
}
