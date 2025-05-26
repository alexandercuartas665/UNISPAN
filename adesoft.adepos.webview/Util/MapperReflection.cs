using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Util
{
    public class MapperReflection
    {


        //where X : BaseModelView
        public static List<X> ToModelView<T, X>(List<T> entities)
        {
            List<X> list = new List<X>();
            foreach (T model in entities)
            {
                X destin = (X)Activator.CreateInstance(typeof(X));
                Type typedest = destin.GetType();
                foreach (PropertyInfo infoorigen in model.GetType().GetProperties())
                {
                    PropertyInfo infodest = typedest.GetProperties().Where(x => x.Name == infoorigen.Name).FirstOrDefault();
                    if (infodest != null && infodest.GetType() == infoorigen.GetType())
                    {
                        object value = infoorigen.GetValue(model);
                        //if (infoorigen.Name == "Id")
                        //{
                        //    perm.IdEntity = (long)value;
                        //}

                        infodest.SetValue(destin, value);
                    }
                }
                try
                {
                    //Para setear el objeto transformado y tenerlo en el objeto model view
                    PropertyInfo destobjbase = typedest.GetProperties().Where(x => x.Name == "ObjTransf").FirstOrDefault();
                    if (destobjbase != null)
                    {
                        destobjbase.SetValue(destin, model);
                    }
                }
                catch { }
                list.Add(destin);
            }
            return list;
        }

        public static List<X> ToModelEntityList<T, X>(List<T> entities)
        {
            List<X> list = new List<X>();
            foreach (T model in entities)
            {
                X destin = (X)Activator.CreateInstance(typeof(X));
                Type typedest = destin.GetType();

                foreach (PropertyInfo infoorigen in model.GetType().GetProperties())
                {
                    PropertyInfo infodest = typedest.GetProperties().Where(x => x.Name == infoorigen.Name).FirstOrDefault();
                    if (infodest != null && infodest.PropertyType == infoorigen.PropertyType && infodest.CanWrite)
                    {
                        object value = infoorigen.GetValue(model);
                        infodest.SetValue(destin, value);
                    }
                    else
                    {

                    }
                }
                list.Add(destin);
            }
            return list;
        }

        public static X ToModelEntity<T, X>(T entities)
        {
            X destin = (X)Activator.CreateInstance(typeof(X));
            Type typedest = destin.GetType();

            foreach (PropertyInfo infoorigen in entities.GetType().GetProperties())
            {
                PropertyInfo infodest = typedest.GetProperties().Where(x => x.Name == infoorigen.Name).FirstOrDefault();
                //string pr = infodest.PropertyType.GetElementType();
                if (infodest != null && infodest.PropertyType == infoorigen.PropertyType && infodest.CanWrite)
                {
                    object value = infoorigen.GetValue(entities);
                    infodest.SetValue(destin, value);
                }
                else
                {

                }
            }
            return destin;
        }


        public static X MapperTo<T, X>(T origen, X destin_)
        {
            //   X destin = (X)Activator.CreateInstance(typeof(X));
            X destin = destin_;
            Type typedest = destin.GetType();

            foreach (PropertyInfo infoorigen in origen.GetType().GetProperties())
            {
                PropertyInfo infodest = typedest.GetProperties().Where(x => x.Name == infoorigen.Name).FirstOrDefault();
                if (infodest != null && infodest.PropertyType == infoorigen.PropertyType)
                {
                    object value = infoorigen.GetValue(origen);
                    infodest.SetValue(destin, value);
                }
            }
            return destin;
        }
    }
}
