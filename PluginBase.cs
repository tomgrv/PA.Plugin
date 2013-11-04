using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using PA.Plugin;
using System.ComponentModel.Composition;


namespace PA.Plugin
{
    [PartNotDiscoverable]
    public partial class PluginBase : Component, IPlugin
    {
        #region Helpers

        public static T GetAttribute<T>(Type PluginType)
        {
            T[] attributes = PluginBase.GetAttributes<T>(PluginType);
            return attributes.Length > 0 ? attributes[0] : Activator.CreateInstance<T>();
        }

        public static T[] GetAttributes<T>(Type PluginType)
        {
            return Array.ConvertAll<object, T>(PluginType.GetCustomAttributes(typeof(T), true), (Converter<object, T>)(attribute => (T)attribute));
        }

        [Obsolete]
        public static bool SetObjectMember(object o, string name, params object[] value)
        {
            PropertyInfo p = o.GetType().GetProperty(name);

            if (p is PropertyInfo && p.CanWrite && value.Length > 0)
            {
                if (p.PropertyType.IsAssignableFrom(value[0].GetType()))
                {
                    p.SetValue(o, value[0], null);
                    return true;
                }

                if (p.PropertyType.IsEnum)
                {
                    p.SetValue(o, Enum.Parse(p.PropertyType, value[0].ToString()), null);
                    return true;
                }

                ConstructorInfo ci = p.PropertyType.GetConstructor(Type.GetTypeArray(value));
                if (ci is ConstructorInfo)
                {
                    p.SetValue(o, ci.Invoke(value), null);
                    return true;
                }

                foreach (MemberInfo mb in p.PropertyType.GetDefaultMembers())
                {
                    if (mb.MemberType == MemberTypes.Property)
                    {
                        return SetObjectMember(p.GetValue(o, null), mb.Name, value);
                    }

                    if (mb.MemberType == MemberTypes.Method)
                    {
                        object obj = CallObjectMember(p.GetValue(o, null), mb.Name, value);

                        if (obj is object)
                        {
                            p.SetValue(o, obj, null);
                        }

                        return obj is object;
                    }
                }

                return CallObjectMember(o, "Parse", value) != null;
            }

            return false;
        }

         [Obsolete]
        public static object CallObjectMember(object o, string name, params object[] value)
        {
            MethodInfo mi = o.GetType().GetMethod(name, Type.GetTypeArray(value));
            if (mi is MethodInfo)
            {
                if (mi.IsStatic)
                {
                    return mi.Invoke(null, value);
                }
                else
                {
                    mi.Invoke(o, value);
                }
            }

            return null;
        }


        #endregion

        #region ICloneable Membres

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

    }
}

