using System;

namespace Toolbox.Plugin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PluginDataAttribute : Attribute
    {
        private string attrKey;

        public string Key
        {
            get { return attrKey; }
        }

        private string attrValue;

        public string Value
        {
            get { return attrValue; }
        }
	                

        public PluginDataAttribute(string key, string value)
        {
            this.attrKey = key;
            this.attrValue = value;
        }

    }
}
