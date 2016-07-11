using System.Configuration;

namespace VelocityMail.Configuration
{
    /// <summary>
    /// Collection of re-write address rules applied when running in test mode
    /// (MailServiceMode is set to EnabledWithAddressRewrite).
    /// </summary>
    public class MailAddressRewriteRuleCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MailAddressRewriteRuleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MailAddressRewriteRuleElement)element).Pattern;
        }
    }
}
