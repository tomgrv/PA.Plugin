using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace PA.Plugin.Extensions
{
    public static class PluginExtensions
    {
        public static string GetDescription<T>(this T p) where T : IPlugin
        {
            return p.GetAttribute<T,PluginDescriptionAttribute>().Description;
        }

        public static string GetCategory<T>(this T p) where T : IPlugin
        {
            return p.GetAttribute<T, PluginCategoryAttribute>().Category;
        }

        public static Image GetImage<T>(this T p) where T : IPlugin
        {
            Type t = p.GetType();

            foreach (string ressource in t.Assembly.GetManifestResourceNames())
            {
                if (ressource.Contains(t.Name) || ressource.Contains(t.Namespace))
                {
                    Stream stream = t.Assembly.GetManifestResourceStream(ressource);

                    try
                    {
                        return Bitmap.FromStream(stream);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        public static R GetAttribute<T,R>(this T p) 
            where T : IPlugin 
            where R : Attribute
        {
            return p.GetAttributes<T, R>().FirstOrDefault() ?? System.Activator.CreateInstance<R>();
        }

        public static R[] GetAttributes<T, R>(this T p)
            where T : IPlugin
            where R : Attribute
        {
            object[] Attributes = p.GetType().GetCustomAttributes(typeof(R), true);
            return Array.ConvertAll<object, R>(Attributes, a => (R)a).ToArray();
        }
    }
}
