using System;
using System.Configuration;
using System.Net.Mail;
using System.Text.RegularExpressions;
using NVelocity.App;
using VelocityMail.Configuration;
using VelocityMail.Logging;

namespace VelocityMail.Service
{
    /// <summary>
    /// Service object to encapsulate the creation and sending of VelocityMailMessages.
    /// </summary>
    public class VelocityMailService
    {
        /// <summary>
        /// Creates a new <see cref="VelocityMailService"/> using the configuration section in
        /// (Web|App).config
        /// </summary>
        public VelocityMailService()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new instance of a <see cref="VelocityMailService"/> using the settings
        /// in the given VelocityMailServiceConfigurationSection
        /// </summary>
        /// <param name="settings">Service settings. If null, the service attempts to retrieve them
        /// from (Web|App).Config</param>
        public VelocityMailService(VelocityMailSection settings)
        {
            if (settings == null)
            {
                settings = VelocityMailSection.Settings;

                if (settings == null)
                {
                    throw new ConfigurationErrorsException("velocityMail section was not found in (Web|App).config");
                }
            }

            // FIXME: 3 scenarios: assembly/paths/both

            // Create an engine automatically based on the settings in the configuration
            // file.
            var assemblyFragments = settings.TemplatesAssembly.Split(',');

            if (assemblyFragments.Length != 2)
            {
                throw new ConfigurationErrorsException("templatesAssembly must have two fragments (namespace, assembly name)");
            }

            if (string.IsNullOrWhiteSpace(settings.TemplatesPath))
            {
                // Just assembly resource templates
                this.Engine = VelocityEngineFactory.Create(assemblyFragments[0].Trim(), assemblyFragments[1].Trim());
            }
            else
            {
                // Both path based and assembly based. File system path is checked first so the file system
                // can override embedded templates in assemblies.
                this.Engine = VelocityEngineFactory.Create(
                    assemblyFragments[0].Trim(),
                    assemblyFragments[1].Trim(),
                    settings.TemplatesPath);
            }

            this.Settings = settings;
            this.SmtpClientFactory = () => new SmtpClient();
        }

        /// <summary>
        /// Logging
        /// </summary>
        private static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// VelocityEngine instance used by this service
        /// </summary>
        public VelocityEngine Engine { get; set; }

        /// <summary>
        /// Service settings
        /// </summary>
        public VelocityMailSection Settings { get; set; }

        /// <summary>
        /// Factory method to create an SmtpClient when sending an e-mail. By default, simply
        /// news an SmtpClient with the configuration from (Web|App).config
        /// </summary>
        public Func<SmtpClient> SmtpClientFactory { get; set; }

        /// <summary>
        /// Creates a new <see cref="VelocityMailMessage"/> and installs some application-wide
        /// context (variable) data for use by the templates.
        /// </summary>
        /// <param name="templateName">Velocity template to use (without the
        /// (-txt.vm|-html).vm extensions)</param>
        /// <returns>A new <see cref="VelocityMailMessage"/></returns>
        public VelocityMailMessage CreateMailMessage(string templateName)
        {
            var msg = new VelocityMailMessage(templateName);

            if (!string.IsNullOrWhiteSpace(this.Settings.DefaultFrom))
            {
                msg.From = new MailAddress(this.Settings.DefaultFrom);
            }
            
            this.InstallGlobalContextData(msg);
            return msg;
        }

        /// <summary>
        /// Creates a new <see cref="VelocityMailMessage"/> and installs some application-wide
        /// context data for use by the templates.
        /// </summary>
        /// <param name="templateName">Velocity template to use (without the
        /// (-txt.vm|-html).vm extensions)</param>
        /// <param name="from">The sender's e-mail address</param>
        /// <param name="to">The recipient's e-mail address</param>
        /// <returns>A new <see cref="VelocityMailMessage"/></returns>
        public VelocityMailMessage CreateMailMessage(string templateName, string from, string to)
        {
            var msg = new VelocityMailMessage(templateName, from, to);

            this.InstallGlobalContextData(msg);
            return msg;
        }

        /// <summary>
        /// Sends the specified message to a SMTP server for delivery
        /// </summary>
        /// <param name="msg">A VelocityMailMessage to send</param>
        /// <exception cref="SmtpException">
        /// The connection to the SMTP server failed.
        /// -or-
        /// Authentication failed.
        /// -or-
        /// The operation timed out.
        /// </exception>
        /// <exception cref="SmtpFailedRecipientsException">The message could
        /// not be delivered to one or more recipients in the To, Cc and/or Bcc
        /// lists.</exception>
        public virtual void Send(VelocityMailMessage msg)
        {
            // Don't actually send any mail if the service has been disabled
            if (this.Settings.MailServiceMode == MailServiceMode.Disabled)
            {
                if (msg.To.Count > 0)
                {
                    log.InfoFormat("Message entitled '{0}' to '{1}' not sent as MailService is disabled.",
                        msg.To[0], msg.Subject);
                }

                return;
            }

            using (MailMessage mmsg = msg.GetMailMessage(this.Engine))
            {
                try
                {
                    // Re-write the destination addresses according to the rewrite
                    // rules, if enabled.
                    if (this.Settings.RewriteAddresses)
                    {
                        ApplyRewriteRules(mmsg.To);
                        ApplyRewriteRules(mmsg.CC);
                        ApplyRewriteRules(mmsg.Bcc);
                    }

                    if (this.Settings.MailServiceMode == MailServiceMode.Test)
                    {
                        mmsg.Subject = "[TEST]: " + mmsg.Subject;
                    }

                    // Add the global BCC list to the message.
                    if (!String.IsNullOrWhiteSpace(this.Settings.GlobalBcc))
                    {
                        mmsg.Bcc.Add(this.Settings.GlobalBcc);
                    }

                    using (var sender = this.SmtpClientFactory())
                    {
                        sender.Send(mmsg);
                    }
                }

                catch (Exception ex)
                {
                    log.ErrorFormat("Unable to send e-mail entitled '{0}' to '{1}",
                        mmsg.Subject, mmsg.To[0].ToString());

                    throw ex;
                }

                finally
                {
                    // Avoid Disposing the attachments as they're also in the
                    // VelocityMailMessage AttachmentCollection. Probably not
                    // a big deal since it's unlikely to be sent twice.
                    msg.Attachments.Clear();
                }

            }
        }

        #region Implementation
        
        /// <summary>
        /// Installs some application-wide context data into the <see cref="VelocityMailMessage"/>
        /// from the configuration section.
        /// <para>In particular, ${wwwroot} can be used to reference the root URL of the
        /// website (as specified in the configuration section).</para>
        /// </summary>
        /// <param name="msg">A VelocityMailMessage to install the variables data into</param>
        /// <remarks>Override to add additional application-specific global context
        /// data.</remarks>
        protected virtual void InstallGlobalContextData(VelocityMailMessage msg)
        {
            foreach (GlobalVarElement gvar in this.Settings.GlobalVariables)
            {
                msg.Put(gvar.Name, gvar.Value);
            }
        }

        /// <summary>
        /// Applies the mail address re-write rules from the configuration file.
        /// </summary>
        /// <param name="addresses">Collection of addresses to apply the rules to</param>
        protected void ApplyRewriteRules(MailAddressCollection addresses)
        {
            for (var idx = 0; idx < addresses.Count; idx++)
            {
                var address = addresses[idx];

                foreach (MailAddressRewriteRuleElement rule in this.Settings.MailAddressRewriteRules)
                {
                    var match = Regex.IsMatch(address.Address, rule.Pattern, RegexOptions.IgnoreCase);

                    if (rule.MatchMode == MatchMode.DoesNotMatch)
                    {
                        match = !match;
                    }

                    if (match)
                    {
                        log.InfoFormat("Address '{0}' matched '{1}'. Rewritten as '{2}'.",
                            address.Address, rule.Pattern, rule.Replacement);

                        addresses[idx] = new MailAddress(rule.Replacement, address.DisplayName);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
