using adesoft.adepos.webview.Data.Model.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.DTO
{
    public class DTOViewScheduleDispRet
    {
        public DTOViewScheduleDispRet(DateTime fromDate, DateTime toDate, int filterById = 1)
        {
            this.Modules = new List<Module>()
            {
                new Module
                {
                    Id = 1,
                    Description = "Despachos"
                },
                new Module
                {
                    Id = 2,
                    Description = "Devoluciones"
                }
            };
            this.FilterBy = new List<Module>()
            {
                new Module
                {
                    Id = 1,
                    Description = "Ninguno"
                },
                new Module
                {
                    Id = 2,
                    Description = "Conforme"
                },
                new Module
                {
                    Id = 3,
                    Description = "No Conforme"
                }
            };
            this.ToDate = toDate;
            this.FromDate = fromDate;
            this.FilterById = filterById;
            this.ReponsableTransIds = new List<int>();
        }

        public DTOViewScheduleDispRet()
        {
            this.Modules = new List<Module>()
            {
                new Module
                {
                    Id = 1,
                    Description = "Despachos"
                },
                new Module
                {
                    Id = 2,
                    Description = "Devoluciones"
                }
            };
            this.FilterBy = new List<Module>()
            {
                new Module
                {
                    Id = 1,
                    Description = "Ninguno"
                },
                new Module
                {
                    Id = 2,
                    Description = "Conforme"
                },
                new Module
                {
                    Id = 3,
                    Description = "No Conforme"
                }
            };
            var nowDate = DateTime.Now;
            int dayOfWeek = (int)nowDate.DayOfWeek;
            //this.ToDate = nowDate.AddDays(6 - dayOfWeek);
            //this.FromDate = this.ToDate.AddDays(-6);
            this.FromDate = new DateTime(nowDate.Year, nowDate.Month, 1);
            this.ToDate = this.FromDate.AddMonths(1).AddDays(-1);
            this.FilterById = 1;
            this.ReponsableTransIds = new List<int>();
            this.OrderStatus = 0;            
        }

        public List<Module> Modules { get; set; }

        public int ModuleId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public List<Module> FilterBy { get; set; }

        public int FilterById { get; set; }

        public IEnumerable<int> ReponsableTransIds { get; set; }

        public int OrderStatus { get; set; }
    }

    public class Module {
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
