using System.Configuration;

namespace VelocityMail.Configuration
{
    /// <summary>
    /// Application-wide configuration of the mail service.
    /// </summary>
    public class VelocityMailSection : ConfigurationSection
    {
        /// <summary>
        /// Static method to get the settings from App|Web.config. Will be null if not found.
        /// </summary>
        public static VelocityMailSection Settings
        {
            get { return ConfigurationManager.GetSection("velocityMail") as VelocityMailSection; }
        }

        /// <summary>
        /// Service operation mode. <see cref="MailServiceMode"/> for details.
        /// </summary>
        [ConfigurationProperty("mode", DefaultValue = MailServiceMode.Enabled)]
        public MailServiceMode MailServiceMode
        {
            get { return (MailServiceMode)base["mode"]; }
            set { base["mode"] = value; }
        }

        /// <summary>
        /// Whether or not to re-write e-mail addresses according to the
        /// <see cref="MailAddressRewriteRuleCollection"/>
        /// </summary>
        [ConfigurationProperty("rewriteAddresses", DefaultValue = false)]
        public bool RewriteAddresses
        {
            get { return (bool)base["rewriteAddresses"]; }
            set { base["rewriteAddresses"] = value; }
        }

        /// <summary>
        /// List of comma-separated e-mail addresses which will be Bcc'd on all e-mails sent
        /// from this service.
        /// </summary>
        [ConfigurationProperty("globalBcc", IsRequired = false)]
        public string GlobalBcc
        {
            get { return (string)base["globalBcc"]; }
            set { base["globalBcc"] = value; }
        }

        /// <summary>
        /// Name of the namespace in the corresponding assembly which holds the e-mail templates as
        /// resources. If null, TemplatesPath must be set. If both are set, TemplatesPath is
        /// searched for matching templates before TemplatesAssembly.
        /// </summary>
        [ConfigurationProperty("templatesAssembly", IsRequired = false)]
        public string TemplatesAssembly
        {
            get { return (string)base["templatesAssembly"]; }
            set { base["templatesAssembly"] = value; }
        }

        /// <summary>
        /// Path to the e-mail templates on the file system. If null, TemplatesPath must be set.
        /// If both are set, TemplatesPath is searched for matching templates before TemplatesAssembly.
        /// </summary>
        [ConfigurationProperty("templatesPath", IsRequired = false)]
        public string TemplatesPath
        {
            get { return (string)base["templatesPath"]; }
            set { base["templatesPath"] = value; }
        }

        /// <summary>
        /// Default From address. If a From address is not specified in the <see cref="VelocityMailMessage"/>
        /// itself, this is used instead.
        /// </summary>
        [ConfigurationProperty("defaultFrom", DefaultValue = "test@test.com", IsRequired = false)]
        [RegexStringValidator(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")]
        public string DefaultFrom
        {
            get { return (string)base["defaultFrom"]; }
            set { base["defaultFrom"] = value; }
        }

        /// <summary>
        /// Collection of global variables that are made available to all templates automatically
        /// </summary>
        [ConfigurationProperty("globalVars", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(GlobalVarCollection), AddItemName = "add")]
        public GlobalVarCollection GlobalVariables
        {
            get { return base["globalVars"] as GlobalVarCollection; }
        }

        /// <summary>
        /// Set of rules for re-writing e-mail addresses when |RewriteAddresses| is set to
        /// true.
        /// </summary>
        [ConfigurationProperty("rewriteRules", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MailAddressRewriteRuleCollection), AddItemName = "addrule")]
        public MailAddressRewriteRuleCollection MailAddressRewriteRules
        {
            get { return base["rewriteRules"] as MailAddressRewriteRuleCollection; }
        }
    }
}