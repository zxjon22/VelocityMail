using VelocityMail.Configuration;

namespace VelocityMail.Service
{
    /// <summary>
    /// Rule for re-writing e-mail address when |RewriteAddresses| is set to
    /// true. Any addresses that match the 'replace' element are replaced with
    /// the 'with' element.
    /// </summary>
    public class MailAddressRewriteRule
    {
        /// <summary>
        /// Creates a new <see cref="MailAddressRewriteRule"/>.
        /// </summary>
        /// <param name="pattern">RegEx to match each address against</param>
        /// <param name="replacement">Replacement pattern to re-write the address</param>
        /// <param name="matchMode">Whether to re-write the address if the regex matches
        /// or does not match</param>
        public MailAddressRewriteRule(string pattern, string replacement, MatchMode matchMode)
        {
            this.Pattern = pattern;
            this.Replacement = replacement;
            this.MatchMode = matchMode;
        }

        /// <summary>
        /// RegEx rule for matching.
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Address to replace with if the rule matches.
        /// </summary>
        public string Replacement { get; set; }

        /// <summary>
        /// How to handle matches. <see cref="MatchMode"/>
        /// </summary>
        public MatchMode MatchMode { get; set; }
    }
}
