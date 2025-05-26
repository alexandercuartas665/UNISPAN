using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Extensions
{
    public static class TextExtensions
    {

        public static T ToUpper<T>(this T value) where T : class
        {
            var t = value.GetType();
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(c => c.PropertyType == typeof(string));
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanWrite)
                {
                    var newValue = (string)propertyInfo.GetValue(value);
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        newValue = newValue.ToUpper();
                    }
                    propertyInfo.SetValue(value, newValue);
                }
            }

            return value;
        }
    }
}
