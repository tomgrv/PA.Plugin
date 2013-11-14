using PA.Configuration;
using PA.Plugin.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PA.Plugin
{
    public static class PluginManager
    {

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
            PluginManager.GetContainer(configure).ComposeParts(z); ;
        }

        public static void ComposeParts<T, R>(this T z, Queue<R> parts)
            where T : CompositionContainer
        {
            while (parts.Count > 0)
            {
                z.ComposeParts(parts.Dequeue());
            }
        }

        public static IEnumerable<Type> GetExportedType<T>(this T z)
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
