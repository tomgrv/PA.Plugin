using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using PA.Configuration;
using System.Collections;

namespace PA.Plugin
{
   

    [DefaultMember("Init")]
    [Obsolete]
    public class PluginInstances<T> //: Collection<ExportFactory<T>>, IEnumerable<ExportFactory<T>> where T : IPlugin 
    {
        private List<T> instances;

        public T[] Values { get { return this.instances.ToArray(); } }

        public PluginInstances()
            : base()
        {
            this.instances = new List<T>();
        }

        public void Init(string ConfigString)
        {
            this.Init(new Uri(ConfigString));
        }

        public void Init(Uri ConfigUri)
        {
            //string type = ConfigUri.GetQueryEntry("Type", true);

            //foreach (ExportFactory<T> ef in this.Items)
            //{
            //    ExportLifetimeContext<T> elc = ef.CreateExport();

            //    string scheme = Plugin.GetAttribute<PluginAdressableAttribute>(elc.Value.GetType()).Scheme;

            //    //if (ConfigUri.Scheme.EndsWith(ef.Metadata.Scheme, StringComparison.InvariantCultureIgnoreCase) || ConfigUri.Scheme.EndsWith(elc.Value.GetType().FullName, StringComparison.InvariantCultureIgnoreCase) )
            //    if (ConfigUri.Scheme.EndsWith(scheme, StringComparison.InvariantCultureIgnoreCase) || ConfigUri.Scheme.EndsWith(elc.Value.GetType().FullName, StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        elc.Value.BeginInit();


            //        ///TODO A refaire
            //        PropertyInfo pu = elc.Value.GetType().GetProperty("Directory");
            //        if (pu is PropertyInfo && pu.CanWrite)
            //        {
            //            elc.Value.SetProperty(pu.Name, ConfigUri);
            //        }
            //        ///////////////////

            //        foreach (KeyValuePair<string, string> arg in ConfigUri.GetAsEnumeration())
            //        {
            //            PropertyInfo p = elc.Value.GetType().GetProperty(arg.Key);

            //            if (p is PropertyInfo && p.CanWrite)
            //            {
            //                elc.Value.SetProperty(p.Name, Uri.UnescapeDataString(arg.Value));
            //            }
            //        }

            //        elc.Value.EndInit();

            //        this.instances.Add(elc.Value);
            //    }
            //    else
            //    {
            //        elc.Dispose();
            //    }
            //}
        }
    }
}
