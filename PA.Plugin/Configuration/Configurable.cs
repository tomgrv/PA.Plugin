using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Toolbox.Configuration
{
    public class Configurable: IConfigurable
    {
        public string Contract { get; private set; }

        private IConfigurationSource source;

        public Configurable(string contract, IConfigurationSource source)
        {
            this.Contract = contract;
            this.source = source;
        }

        public string Get()
        {
            return this.source.GetSetting(this.Contract);
        }

        public void Set(object value)
        {
            this.source.SetSetting(this.Contract, value.ToString());
        }

        #region To

        internal static object ConvertTo(Type type, string value)
        {
            
            if (type.IsArray)
            {
                return ConvertTo(type, value.Substring(1), value[0]);
            }
            else
            {
                if (type == typeof(TimeSpan))
                {
                    return TimeSpan.Parse(value);
                }
                else
                {
                    try
                    {
                        return Convert.ChangeType(value, type);
                    }
                    catch(Exception e)
                    {
                        throw e;
                    }

                }
            }
        }

        internal static object ConvertTo(Type type, string value, char separator)
        {
            if (!type.IsArray)
            {
                throw new ArgumentException("Not an array");
            }

            var obj = value.Split(separator).Select(s => ConvertTo(type.GetElementType(), s)).ToArray();
            var arr = Array.CreateInstance(type.GetElementType(), obj.Length);
            Array.Copy(obj, arr, obj.Length);
            return arr;
        }

        #endregion
   
    }

    public class Configurable<T> : Configurable , IConfigurable<T>
    {
        public T Value { get; set; }

        public Configurable(string contract, IConfigurationSource source)
            : base(contract, source)
        {
            this.Value = (T)ConvertTo(typeof(T), this.Get());
        }

        public Configurable(string contract, IConfigurationSource source, T initialValue)
            : base(contract, source)
        {
            this.Value = initialValue;
        }

        public void Save()
        {
            this.Set(this.Value);
        }

        public void Save(T value)
        {
            this.Value = value;
            this.Set(this.Value);
        }
    }
}
