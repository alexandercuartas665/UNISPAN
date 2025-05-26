using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class ActionApp : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ActionAppId { get; set; }

        public long? IdFather { get; set; }

        public string NameAction { get; set; }

        public string Label { get; set; }

        public string Icon { get; set; }

        public int OrderNum { get; set; }
        public string Type { get; set; }

        public bool IsActive { get; set; }

        [NotMapped]
        public bool? HavePermission { get; set; }


        public long MenuId { get; set; }


        public static List<ActionApp> UtilSetPermission(ref List<ActionApp> listactions)
        {
            foreach (ActionApp a in listactions.Where(x => x.Type == "Option").ToList())
            {
                AuxSetPermission(a, ref listactions);
            }
            return listactions;
        }

        private static void AuxSetPermission(ActionApp act, ref List<ActionApp> listactions)
        {
            if (act.Type != "Option")
            {
                var r = listactions.Where(x => x.IdFather == act.ActionAppId);
                if (r.Where(x => x.HavePermission == true).Count() == r.Count())
                {
                    act.HavePermission = true;
                }
                else if (r.Where(x => x.HavePermission == null).Count() > 0 || r.Where(x => x.HavePermission == true).Count() > 0)
                {
                    act.HavePermission = null;
                }
                else
                {
                    act.HavePermission = false;

                }
            }
            if (act.Type != "Multiple")
            {
                var r = listactions.Where(x => x.ActionAppId == act.IdFather);
                foreach (ActionApp aux in r.ToList())
                {
                    AuxSetPermission(aux, ref listactions);
                }
            }

        }
    }
}
