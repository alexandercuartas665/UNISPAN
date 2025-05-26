using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOLedgerBalanceFilter
    {
        public DTOLedgerBalanceFilter()
        {
            this.Months = new List<CommonData>()
            {
                new CommonData()
                {
                    Id = 1,
                    Description = "Enero"
                },
                new CommonData()
                {
                    Id = 2,
                    Description = "Febrero"
                },
                new CommonData()
                {
                    Id = 3,
                    Description = "Marzo"
                },
                new CommonData()
                {
                    Id = 4,
                    Description = "Abril"
                },
                new CommonData()
                {
                    Id = 5,
                    Description = "Mayo"
                },
                new CommonData()
                {
                    Id = 6,
                    Description = "Junio"
                },
                new CommonData()
                {
                    Id = 7,
                    Description = "Julio"
                },
                new CommonData()
                {
                    Id = 8,
                    Description = "Agosto"
                },
                new CommonData()
                {
                    Id = 9,
                    Description = "Septiembre"
                },
                new CommonData()
                {
                    Id = 10,
                    Description = "Octubre"
                },
                new CommonData()
                {
                    Id = 11,
                    Description = "Noviembre"
                },
                new CommonData()
                {
                    Id = 12,
                    Description = "Diciembre"
                }
            };

            this.Top = new List<CommonData>()
            {
                new CommonData()
                {
                    Id = 0,
                    Description = "Todos"
                },
                new CommonData()
                {
                    Id = 5,
                    Description = "5"
                },
                new CommonData()
                {
                    Id = 10,
                    Description = "10"
                },
                new CommonData()
                {
                    Id = 15,
                    Description = "15"
                },
                new CommonData()
                {
                    Id = 20,
                    Description = "20"
                }
            };

            this.TopSelected = 0;
        }

        public string FilterId { get; set; }

        public int Year { get; set; }

        public List<CommonData> Months { get; set; }

        public IEnumerable<int> MonthsSelected { get; set; }

        public IEnumerable<string> SectorsSelected { get; set; }

        public IEnumerable<string> ZonesSelected { get; set; }

        public IEnumerable<string> AdministratorsSelected { get; set; }

        public IEnumerable<string> GroupBy { get; set; }
        
        public int TopSelected { get; set; }

        public IEnumerable<CommonData> Top { get; set; }
    }

    public class CommonData
    {
        public int Id { get; set; }

        public string IdStr { get; set; }

        public string Description { get; set; }
    }
}
