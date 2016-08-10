using System;
using System.Collections.Generic;
using System.Net.Mail;
using VelocityMail.Configuration;

namespace VelocityMail.Service
{
    /// <summary>
    /// Configuration options
    /// </summary>
    public class VelocityMailOptions
    {
        public VelocityMailOptions()
        {
            this.GlobalVariables = new List<GlobalVar>();
            this.MailAddressRewriteRules = new List<MailAddressRewriteRule>();
        }

        /// <summary>
        /// Service operation mode. <see cref="MailServiceMode"/> for details.
        /// </summary>
        public MailServiceMode MailServiceMode { get; set; }

        /// <summary>
        /// Whether or not to re-write e-mail addresses according to the
        /// rules in <see cref="MailAddressRewriteRules"/>.
        /// </summary>
        public bool RewriteAddresses { get; set; }

        /// <summary>
        /// List of comma-separated e-mail addresses which will be Bcc'd on all e-mails sent
        /// from the service.
        /// </summary>
        public string GlobalBcc { get; set; }

        /// <summary>
        /// Name of the namespace in the corresponding assembly which holds the e-mail templates as
        /// resources. If null, TemplatesPath must be set. If both are set, TemplatesPath is
        /// searched for matching templates before TemplatesAssembly.
        /// </summary>
        public string TemplatesAssembly { get; set; }

        /// <summary>
        /// Path to the e-mail templates on the file system. If null, TemplatesPath must be set.
        /// If both are set, TemplatesPath is searched for matching templates before TemplatesAssembly.
        /// </summary>
        public string TemplatesPath { get; set; }

        /// <summary>
        /// Default From address. If a From address is not specified in the <see cref="VelocityMailMessage"/>
        /// itself, this is used instead.
        /// </summary>
        public string DefaultFrom { get; set; }

        /// <summary>
        /// Collection of global variables that are made available to all templates automatically
        /// </summary>
        public List<GlobalVar> GlobalVariables { get; set; }

        /// <summary>
        /// Set of rules for re-writing e-mail addresses when |RewriteAddresses| is set to
        /// true.
        /// </summary>
        public List<MailAddressRewriteRule> MailAddressRewriteRules { get; set; }

        /// <summary>
        /// Factory method to create an SmtpClient when sending an e-mail. By default, simply
        /// creates a new SmtpClient with the configuration from (Web|App).config
        /// </summary>
        public Func<SmtpClient> SmtpClientFactory { get; set; }
    }
}
