using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class TypeActivity
    {
        public TypeActivity()
        {

        }
        [Key]
        public long TypeActivityId { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 3	ENCOFRADOS ,4	ACCESORIOS
        /// </summary>
        public long CategoryId { get; set; }
       
        public bool IsActive { get; set; }

        [NotMapped]
        public Category Category { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }

    }
}
