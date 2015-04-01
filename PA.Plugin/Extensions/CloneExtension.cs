using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using System.Diagnostics;

namespace PA.Clone.Extensions
{
    public static class CloneExtensions
    {
        public static U DeepClone<U>(this U source) where U : class, ICloneable
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (U)formatter.Deserialize(stream);
            }
        }

        public static IEnumerable<T> Clone<T>(this IEnumerable<T> list) where T : ICloneable
        {
            foreach (T item in list)
            {
                yield return (T)item.Clone();
            }
        }

        public static void CopyPropertiesTo<T>(this T copyFrom, T copyTo, bool copyParentProperties)
        {
            PropertyInfo[] props;

            if (copyParentProperties)
            {
                props = typeof(T).GetProperties();
            }
            else
            {
                props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo pi = copyFrom.GetType().GetProperty(props[i].Name);

                if (pi.CanRead && pi.CanWrite && pi.GetIndexParameters().Length == 0)
                {
                    object value = pi.GetValue(copyFrom, null);

                    if (value is ICloneable)
                    {
                        value = (value as ICloneable).Clone();
                    }

                    copyTo.GetType().GetProperty(props[i].Name).SetValue(copyTo, value, null);
                }
                else
                {
                  //  Debug.WriteLine(copyFrom + ":  Cannot copy property '" + pi.Name + "' to "+ copyTo );
                }
            }
        }
    }
}
