using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace adesoft.adeposx.report.Models
{
    public class BaseEntity
    {
      
        public long TransOption { get; set; }


     
        public bool TransactionIsOk { get; set; }


       
        public string MessageResponse { get; set; }

     
        public string MessageType { get; set; }
 
        public T GetClone<T>()
        {
            T clone = (T)Activator.CreateInstance(typeof(T));
            foreach (PropertyInfo p in clone.GetType().GetProperties())
            {
                p.SetValue(clone, p.GetValue(this));
            }
            return clone;
        }
    }
}
