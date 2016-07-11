using System.Configuration;

namespace VelocityMail.Configuration
{
    /// <summary>
    /// Application-wide configuration of the mail service.
    /// </summary>
    public class VelocityMailServiceConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Static method to get the settings from App|Web.config. Will be null if not found.
        /// </summary>
        public static VelocityMailServiceConfigurationSection Settings
        {
            get { return ConfigurationManager.GetSection("velocityMail") as VelocityMailServiceConfigurationSection; }
        }

        /// <summary>
        /// Service operation mode. <see cref="MailServiceMode"/> for details.
        /// </summary>
        [ConfigurationProperty("mode", DefaultValue = MailServiceMode.Enabled)]
        public MailServiceMode MailServiceMode
        {
            get { return (MailServiceMode)base["mode"]; }
            set { base["Mode"] = value; }
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
        /// Name of the assembly which holds the e-mail templates as
        /// resources. If this field is missing, it is assumed |templatesPath|
        /// is a file system path. This and/or |templatesPath| must be set unless you are
        /// configuring the mail service manually via its constructor.
        /// </summary>
        [ConfigurationProperty("templatesAssembly", IsRequired = false)]
        public string TemplatesAssembly
        {
            get { return (string)base["templatesAssembly"]; }
            set { base["templatesAssembly"] = value; }
        }

        /// <summary>
        /// Path to the e-mail templates. This is either a file system path
        /// if the templates are files on disk, or an assembly namespace, e.g.
        /// MyAssembly.EmailTemplates. This and/or |templatesAssembly| must be set unless you are
        /// configuring the mail service manually via its constructor.
        /// </summary>
        [ConfigurationProperty("templatesPath", IsRequired = false)]
        public string TemplatesPath
        {
            get { return (string)base["templatesPath"]; }
            set { base["templatesPath"] = value; }
        }

        /// <summary>
        /// If false, 'TEST: ' is automatically added to the subject line of all e-mails sent
        /// from the service. If true, no changes are made.
        /// </summary>
        [ConfigurationProperty("runningOnLive", IsRequired = false, DefaultValue = true)]
        public bool RunningOnLive
        {
            get { return (bool)base["runningOnLive"]; }
            set { base["runningOnLive"] = value; }
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
        /// Collection of global variables that are made available to all contexts automatically
        /// </summary>
        [ConfigurationProperty("globalVars", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(GlobalVarCollection), AddItemName = "add")]
        public GlobalVarCollection GlobalVariables
        {
            get { return base["globalVars"] as GlobalVarCollection; }
        }

        /// <summary>
        /// Set of rules for re-writing e-mail address when MailServiceMode is set to
        /// EnabledWithAddressRewrite
        /// </summary>
        [ConfigurationProperty("rewriteRules", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(MailAddressRewriteRuleCollection), AddItemName = "addrule")]
        public MailAddressRewriteRuleCollection MailAddressRewriteRules
        {
            get { return base["rewriteRules"] as MailAddressRewriteRuleCollection; }
        }
    }
}