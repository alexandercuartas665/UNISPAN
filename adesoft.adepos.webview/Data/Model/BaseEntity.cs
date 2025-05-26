using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
namespace adesoft.adepos.webview.Data.Model
{
    public class BaseEntity
    {
        [NotMapped]
        public long TransOption { get; set; }


        [NotMapped]
        public bool TransactionIsOk { get; set; }


        [NotMapped]
        public string MessageResponse { get; set; }

        [NotMapped]
        public string MessageType { get; set; }

        [NotMapped]
        public int pageIndex { get; set; }

        [NotMapped]
        public int pageSize { get; set; }
        public T GetClone<T>()
        {
            T clone = (T)Activator.CreateInstance(typeof(T));
            foreach (PropertyInfo p in clone.GetType().GetProperties())
            {
                if (p.SetMethod != null)
                {
                    p.SetValue(clone, p.GetValue(this));
                }
            }
            return clone;
        }

        /// <summary>
        /// no mapea los valores de las propiedades q sean de tipo clase .. esto para evitar q EF duplique al guardar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCloneWithNativePropertys<T>()
        {
            T clone = (T)Activator.CreateInstance(typeof(T));
            foreach (PropertyInfo p in clone.GetType().GetProperties())
            {
                if (p.SetMethod != null && !(p.PropertyType.BaseType != null && p.PropertyType.BaseType.Name == "BaseEntity"))//!p.GetType().IsClass no mapear clases
                {
                    p.SetValue(clone, p.GetValue(this));
                }
            }
            return clone;
        }


        public bool IsDiferent<T>(T objCompare)
        {
            bool IsDiferent = false;
            foreach (PropertyInfo p in objCompare.GetType().GetProperties())
            {
                if (p.SetMethod != null && !(p.PropertyType.BaseType != null && p.PropertyType.BaseType.Name == "BaseEntity"))//!p.GetType().IsClass no mapear clases
                {

                    if(!AreValuesEqual(p.GetValue((T)objCompare),p.GetValue(this)))
                    {
                        IsDiferent = true;
                    }
                }
            }
            return IsDiferent;
        }

        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;
            IComparable selfValueComparer;

            selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (!object.Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }
    }
}
