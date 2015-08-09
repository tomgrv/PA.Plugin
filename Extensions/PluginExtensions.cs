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
        public static string GetDescription<T>(this T p) where T :  IPlugin
        {
            return PluginManager.GetAttribute<PluginDescriptionAttribute>(p.GetType()).Description ?? p.GetType().Name.ToString();
        }

        public static string GetCategory<T>(this T p) where T :  IPlugin
        {
            return PluginManager.GetAttribute<PluginCategoryAttribute>(p.GetType()).Category;
        }

        public static Image GetImage<T>(this T p) where T : IPlugin
        {
            return PluginManager.GetImage(p.GetType());
        }
    }
}
