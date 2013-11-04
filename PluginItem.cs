using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace PA.Plugin
{
    public class PluginItem
    {
        public string Description { get; private set; }
        public string Category { get; private set; }
        public Image Image { get; private set; }
        public IPlugin Plugin { get; private set; }

        public PluginItem(IPlugin p)
        {
            this.Plugin = p;
            this.Description = GetAttribute<PluginDescriptionAttribute>().Description;
            this.Category = GetAttribute<PluginCategoryAttribute>().Category;
            this.Image = GetImageFromType(this.Plugin.GetType());
        }

        public override string ToString()
        {
            return this.Description;
        }

        public static PluginItem FromPlugin(IPlugin p)
        {
            return new PluginItem(p);
        }

        public static IEnumerable<PluginItem> FromPluginList(IEnumerable<IPlugin> list)
        {
            foreach (IPlugin p in list)
            {
                yield return new PluginItem(p);
            }
        }

        public static Image GetImageFromType(Type PluginType)
        {
            foreach (string ressource in PluginType.Assembly.GetManifestResourceNames())
            {
                if (ressource.Contains(PluginType.Name) || ressource.Contains(PluginType.Namespace))
                {
                    Stream stream = PluginType.Assembly.GetManifestResourceStream(ressource);

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
       
        public T GetAttribute<T>() where T : Attribute
        {
            return GetAttributes<T>().FirstOrDefault() ?? System.Activator.CreateInstance<T>();
        }

        public T[] GetAttributes<T>() where T : Attribute
        {
            object[] Attributes = this.Plugin.GetType().GetCustomAttributes(typeof(T), true);
            return Array.ConvertAll<object, T>(Attributes, a => (T) a).ToArray();
        }
    }
}
