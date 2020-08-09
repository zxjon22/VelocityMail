#if NET452
using System.Configuration;
using VelocityMail.Service;

namespace VelocityMail.Configuration
{
    /// <summary>
    /// Set of rules for re-writing e-mail addresses when |RewriteAddresses| in
    /// <see cref="VelocityMailOptions"/> is set to true.
    /// </summary>
    public class MailAddressRewriteRuleCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="MailAddressRewriteRuleElement"/> for the collection.
        /// </summary>
        /// <returns>Newly created <see cref="MailAddressRewriteRuleElement"/>.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new MailAddressRewriteRuleElement();
        }

        /// <summary>
        /// Gets the element key for the <see cref="MailAddressRewriteRuleElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="MailAddressRewriteRuleElement"/> to get the key for.</param>
        /// <returns>The element key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MailAddressRewriteRuleElement)element).Pattern;
        }
    }
}
#endif // NET452
