#if NET452
using System.Configuration;

namespace VelocityMail.Configuration
{
    /// <summary>
    /// Set of rules for re-writing e-mail addresses when |RewriteAddresses| in
    /// <see cref="VelocityMailOptions"/> is set to true.
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
#endif // NET452
