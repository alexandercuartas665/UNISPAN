using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{

    public class DTOFiltersCompras
    {
        public DTOFiltersCompras()
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

    public class DTOYear
    {

        public long IdYear { get; set; }

        public string Name { get; set; }

    }

    public class DtoBiableSede
    {
        public string Sede { get; set; }

        public string CodeSede { get; set; }
    }

    public class DtoBiableCO
    {
        public string CO { get; set; }

        public string Code { get; set; }
    }

    public class DTOMonth
    {
        public long IdMonth { get; set; }

        public string Name { get; set; }

        public string NameAbrev { get; set; }

    }

    public class DTOViewRptCompra
    {
        public DTOViewRptCompra()
        {
            Proveedores = new List<DTOTercero>();

            Sedes = new List<DtoBiableSede>();

            DtoBiableSede sede1 = new DtoBiableSede();
            sede1.CodeSede = "001"; sede1.Sede = "PALMIRA";
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
            //DTOYear year = new DTOYear();
            //year.IdYear = 2020;
            //year.Name = "2020";
            //ListYears.Add(year);
            //DTOYear year2 = new DTOYear();
            //year2.IdYear = 2021;
            //year2.Name = "2021";
            //ListYears.Add(year2);

            ListMonths = new List<DTOMonth>();
            ListMonths.AddRange(GetMonths());

            ListMonedas = new List<string>();
            ListMonedas.Add("PESOS");
            ListMonedas.Add("DOLAR");
        }

        public static List<DTOMonth> GetMonths()
        {

            List<DTOMonth> List = new List<DTOMonth>();
            DTOMonth month = new DTOMonth();
            month.IdMonth = 1;
            month.Name = "ENERO"; month.NameAbrev = "ENE";
            List.Add(month);
            DTOMonth month2 = new DTOMonth();
            month2.IdMonth = 2;
            month2.Name = "FEBRERO"; month2.NameAbrev = "FEB";
            List.Add(month2);
            DTOMonth month3 = new DTOMonth();
            month3.IdMonth = 3;
            month3.Name = "MARZO"; month3.NameAbrev = "MAR";
            List.Add(month3);
            DTOMonth month4 = new DTOMonth();
            month4.IdMonth = 4;
            month4.Name = "ABRIL"; month4.NameAbrev = "ABR";
            List.Add(month4);
            DTOMonth month5 = new DTOMonth();
            month5.IdMonth = 5;
            month5.Name = "MAYO"; month5.NameAbrev = "MAY";
            List.Add(month5);
            DTOMonth month6 = new DTOMonth();
            month6.IdMonth = 6;
            month6.Name = "JUNIO"; month6.NameAbrev = "JUN";
            List.Add(month6);
            DTOMonth month7 = new DTOMonth();
            month7.IdMonth = 7;
            month7.Name = "JULIO"; month7.NameAbrev = "JUL";
            List.Add(month7);
            DTOMonth month8 = new DTOMonth();
            month8.IdMonth = 8;
            month8.Name = "AGOSTO"; month8.NameAbrev = "AGT";
            List.Add(month8);
            DTOMonth month9 = new DTOMonth();
            month9.IdMonth = 9;
            month9.Name = "SEPTIEMBRE"; month9.NameAbrev = "SEP";
            List.Add(month9);
            DTOMonth month10 = new DTOMonth();
            month10.IdMonth = 10;
            month10.Name = "OCTUBRE"; month10.NameAbrev = "OCT";
            List.Add(month10);
            DTOMonth month11 = new DTOMonth();
            month11.IdMonth = 11;
            month11.Name = "NOVIEMBRE"; month11.NameAbrev = "NOV";
            List.Add(month11);
            DTOMonth month12 = new DTOMonth();
            month12.IdMonth = 12;
            month12.Name = "DICIEMBRE"; month12.NameAbrev = "DIC";
            List.Add(month12);

            return List;
        }



        public List<DtoBiableCO> COs { get; set; }
        public List<DTOTercero> Proveedores { get; set; }
        public List<DtoBiableSede> Sedes { get; set; }
        public List<DTOMonth> ListMonths { get; set; }
        public List<DTOYear> ListYears { get; set; }

        public List<string> ListMonedas { get; set; }
    }
}
