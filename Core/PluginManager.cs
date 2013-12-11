using PA.Configuration;
using PA.Plugin.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace PA.Plugin
{
    public static class PluginManager
    {
        public static Image GetImage(Type  t) 
        {
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

        public static T[] GetAttributes<T>(Type t)
            where T : Attribute            
        {
            return t.GetCustomAttributes(typeof(T), true).OfType<T>().ToArray();
        }

        public static T GetAttribute<T>(Type t)
            where T : Attribute
        {
            return PluginManager.GetAttributes<T>(t).FirstOrDefault() ?? System.Activator.CreateInstance<T>();
        }

        public static CompositionContainer GetContainer(Action<CompositionConfigurator> configure)
        {
            CompositionConfigurator cc = new CompositionConfigurator();

            configure(cc);

            return cc.GetContainer();
        }

        public static void Compose<T>(this T z, Action<CompositionConfigurator> configure)
           where T : class
        {
            PluginManager.GetContainer(configure).ComposeParts(z);
        }

        public static void Compose<T>(this Queue<T> z, Action<CompositionConfigurator> configure)
          where T : class
        {
            PluginManager.GetContainer(configure).ComposeParts(z);

        }

        public static void ComposeParts<T, R>(this T z, IEnumerable<R> parts)
            where T : CompositionContainer
        {
            foreach (R part in parts.Distinct())
            {
                z.ComposeParts(part);
            }
        }

        [Obsolete]
        public static IEnumerable<Type> GetExportedType<T>(this T z)
         where T : CompositionContainer
        {
            return GetExportedTypes<T>(z);
        }

        public static IEnumerable<Type> GetExportedTypes<T>(this T z)
         where T : CompositionContainer
        {
            return z.Catalog.Parts
                .Select(p =>
                    ComposablePartExportType<T>(p))
                .Where(t =>
                    t is Type);
        }



        private static Type ComposablePartExportType<T>(ComposablePartDefinition part)
        {

            if (part.ExportDefinitions.Any(def =>
                def.Metadata.ContainsKey("ExportTypeIdentity") && def.Metadata["ExportTypeIdentity"].Equals(typeof(T).FullName)))
            {
                return ReflectionModelServices.GetPartType(part).Value;
            }

            return null;
        }
    }
}
