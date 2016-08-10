using VelocityMail.Service;

namespace VelocityMail.Configuration
{
    /// <summary>
    /// Helper class for converting a <see cref="VelocityMailSection"/> instance to a
    /// <see cref="VelocityMailOptions"/> instance
    /// </summary>
    static class VelocityMailOptionsMapper
    {
        /// <summary>
        /// Maps the <see cref="VelocityMailSection"/> to the equivalent
        /// <see cref="VelocityMailOptions"/>.
        /// </summary>
        /// <param name="section">VelocityMailSection to map</param>
        /// <returns>The equivalent VelocityMailOptions</returns>
        internal static VelocityMailOptions ToMailOptions(this VelocityMailSection section)
        {
            var options = new VelocityMailOptions
            {
                MailServiceMode = section.MailServiceMode,
                RewriteAddresses = section.RewriteAddresses,
                GlobalBcc = section.GlobalBcc,
                TemplatesAssembly = section.TemplatesAssembly,
                TemplatesPath = section.TemplatesPath,
                DefaultFrom = section.DefaultFrom
            };

            foreach(MailAddressRewriteRuleElement rule in section.MailAddressRewriteRules)
            {
                options.MailAddressRewriteRules
                    .Add(new MailAddressRewriteRule(rule.Pattern, rule.Replacement, rule.MatchMode));
            }

            foreach(GlobalVarElement global in section.GlobalVariables)
            {
                options.GlobalVariables
                    .Add(new GlobalVar(global.Name, global.Value));
            }

            return options;
        }
    }
}
