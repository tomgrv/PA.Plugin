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
            return PluginManager.GetImage(p.GetType());
        }

        public static R GetAttribute<T,R>(this T p) 
            where T : IPlugin 
            where R : Attribute
        {
            return PluginManager.GetAttribute<R>(typeof(T));
        }

        public static R[] GetAttributes<T, R>(this T p)
            where T : IPlugin
            where R : Attribute
        {            
            return PluginManager.GetAttributes<R>(typeof(T));
        }
    }
}
