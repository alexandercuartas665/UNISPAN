using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO.Simex
{

    public class DTOFilterSales
    {
        public DTOFilterSales()
        {
            try
            {
                COs = new List<string>();
                Sedes = new List<string>();
                Proveedores = new List<string>();
                multipleValuesMonth = new List<long>();
                multipleValuesYear = new List<long>();
                Inventaries = new List<DTOInventary>();
                Oportunidades = new List<OportunidadesCRM>();

            }
            catch { }
        }

        public string Item { get; set; }
        public int TypeMovementId { get; set; }
        public DateTime? DateInit { get; set; }

        public DateTime? DateEnd { get; set; }
        public string GuidFilter { get; set; }

        public long TypeReportId { get; set; }
        public bool AddDynamicField { get; set; }
        public IEnumerable<string> COs { get; set; }

        public IEnumerable<string> Sedes { get; set; }
        public IEnumerable<string> Proveedores { get; set; }
        public IEnumerable<long> multipleValuesYear { get; set; }
        public IEnumerable<string> GroupBy { get; set; }

        public IEnumerable<long> multipleValuesMonth { get; set; }
        public string Moneda { get; set; }
        public long yearfilter { get; set; }
        public bool AddVariaciones { get; set; }
        public List<Rendimiento> Rendimientos { get; set; }
        public bool Group1Active { get; set; }

        public bool Group2Active { get; set; }


        public List<DTOInventary> Inventaries { get; set; }

        public List<OportunidadesCRM> Oportunidades { get; set; }

    }

    public class DtoBiableCO
    {
        public string CO { get; set; }

        public string Code { get; set; }
    }

    public class DTOSimexSalesReportDetail
    {
        public DTOSimexSalesReportDetail()
        {
            Proveedores = new List<DTOTercero>();

            Sedes = new List<DtoBiableSede>();

            DtoBiableSede sede1 = new DtoBiableSede();
            sede1.CodeSede = "001"; 
            sede1.Sede = "PALMIRA";
            Sedes.Add(sede1);
            DtoBiableSede sede2 = new DtoBiableSede();
            sede2.CodeSede = "002"; sede2.Sede = "BOGOTA";
            Sedes.Add(sede2);


            COs = new List<DtoBiableCO>();

            DtoBiableCO co1 = new DtoBiableCO();
            co1.CO = "PRINCIPAL"; co1.Code = "00101";
            COs.Add(co1);
            DtoBiableCO co2 = new DtoBiableCO();
            co2.CO = "ACTIVO FIJOS"; co2.Code = "00199";
            COs.Add(co2);
            DtoBiableCO co3 = new DtoBiableCO();
            co3.CO = "STOCK DE PRODUCCION"; co3.Code = "00198";
            COs.Add(co3);

            DtoBiableCO co4 = new DtoBiableCO();
            co4.CO = "OTRAS"; co4.Code = "-1";
            COs.Add(co4);

            ListYears = new List<DTOYear>();

            ListMonths = new List<DTOMonth>();
            //ListMonths.AddRange(GetMonths());

            ListMonedas = new List<string>();
            ListMonedas.Add("PESOS");
            ListMonedas.Add("DOLAR");
        }



        public List<DtoBiableCO> COs { get; set; }
        public List<DTOTercero> Proveedores { get; set; }
        public List<DtoBiableSede> Sedes { get; set; }
        public List<DTOMonth> ListMonths { get; set; }
        public List<DTOYear> ListYears { get; set; }

        public List<string> ListMonedas { get; set; }
    }
}
