#if NET452
using System.Configuration;

namespace VelocityMail.Configuration
{
    /// <summary>
    /// Rule for re-writing e-mail address when |RewriteAddresses| is set to
    /// true. Any addresses that match the 'replace' element are replaced with
    /// the 'with' element.
    /// </summary>
    public class MailAddressRewriteRuleElement : ConfigurationElement
    {
        /// <summary>
        /// RegEx rule for matching.
        /// </summary>
        [ConfigurationProperty("replace", IsRequired = true)]
        public string Pattern
        {
            get { return (string)base["replace"]; }
            set { base["replace"] = value; }
        }

        /// <summary>
        /// Address to replace with if the rule matches.
        /// </summary>
        [ConfigurationProperty("with", IsRequired = true)]
        public string Replacement
        {
            get { return (string)base["with"]; }
            set { base["with"] = value; }
        }

        /// <summary>
        /// How to handle matches. <see cref="MatchMode"/>
        /// </summary>
        [ConfigurationProperty("matchMode", DefaultValue = MatchMode.Matches)]
        public MatchMode MatchMode
        {
            get { return (MatchMode)base["matchMode"]; }
            set { base["matchMode"] = value; }
        }
    }
}
#endif // NET452
