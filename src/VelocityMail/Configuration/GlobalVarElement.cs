#if NET452
using System.Configuration;

namespace VelocityMail.Configuration
{
    /// <summary>
    /// Global variables are made available to all templates automatically
    /// </summary>
    public class GlobalVarElement : ConfigurationElement
    {
        /// <summary>
        /// Unique name (key) of the global variable
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Value of the global variable
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }
    }

    /// <summary>
    /// Collection of global variables that are made available to all contexts automatically
    /// </summary>
    public class GlobalVarCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new GlobalVarElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((GlobalVarElement)element).Name;
        }
    }
}
#endif // NET452
