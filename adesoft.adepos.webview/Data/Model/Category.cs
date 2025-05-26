using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Data.Model
{
    public class Category : BaseEntity
    {
        public Category()
        {

        }
        [Key]
        public long CategoryId { get; set; }

        public string Name { get; set; }

        public bool State { get; set; }

        /// <summary>
        /// Tipo de categoria la 1 es para items en general y la 2 es para la medicion si es  KG o M2
        /// </summary>
        public long TypeCategoryId { get; set; }

        public string NickName { get; set; }
    }
}
