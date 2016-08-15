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
            : this((VelocityMailSection)null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="VelocityMailService"/> with the specified options.
        /// </summary>
        /// <param name="options">Configuration options</param>
        public VelocityMailService(VelocityMailOptions options)
        {
            this.Init(options);
        }

        /// <summary>
        /// Creates a new instance of a <see cref="VelocityMailService"/> using the settings
        /// in the given <see cref="VelocityMailSection"/>
        /// </summary>
        /// <param name="settings">Service settings. If null, the service attempts to retrieve them
        /// from (Web|App).Config</param>
        /// <param name="smtpClientFactory">Factory method for create an SmtpClient used to send
        /// e-mails. If null, uses the default.</param>
        public VelocityMailService(VelocityMailSection settings, Func<SmtpClient>smtpClientFactory = null)
        {
            if (settings == null)
            {
                settings = VelocityMailSection.Settings;

                if (settings == null)
                {
                    throw new ConfigurationErrorsException("velocityMail section was not found in (Web|App).config");
                }
            }

            var options = settings.ToMailOptions();            
            options.SmtpClientFactory = smtpClientFactory;
            this.Init(options);
        }

        /// <summary>
        /// Logging
        /// </summary>
        static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// VelocityEngine instance used by this service
        /// </summary>
        VelocityEngine Engine { get; set; }

        /// <summary>
        /// Service options
        /// </summary>
        VelocityMailOptions Options { get; set; }

        /// <summary>
        /// Initialises the service with the specified options.
        /// </summary>
        /// <param name="options">Configuration options</param>
        void Init(VelocityMailOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            bool usesEmbedded = !string.IsNullOrWhiteSpace(options.TemplatesAssembly);
            bool usesFiles = !string.IsNullOrWhiteSpace(options.TemplatesPath);

            if (!usesEmbedded && !usesFiles)
            {
                throw new ArgumentException("At least one of TemplatesAssembly and TemplatesPath must be set");
            }

            string namespacePrefix = null;
            string assemblyName = null;

            if (usesEmbedded)
            {
                var assemblyFragments = options.TemplatesAssembly.Split(',');

                if (assemblyFragments.Length != 2)
                {
                    throw new ArgumentException("TemplatesAssembly must have two fragments (namespace, assembly name)");
                }

                namespacePrefix = assemblyFragments[0].Trim();
                assemblyName = assemblyFragments[1].Trim();
            }

            if (usesEmbedded && usesFiles)
            {
                // Both path based and assembly based. File system path is checked first so the file system
                // can override embedded templates in assemblies.
                this.Engine = VelocityEngineFactory.Create(
                    namespacePrefix,
                    assemblyName,
                    options.TemplatesPath);
            }

            else if (usesEmbedded)
            {
                // Just embedded templates in an assembly
                this.Engine = VelocityEngineFactory.Create(namespacePrefix, assemblyName);
            }

            else
            {
                // File system.
                this.Engine = VelocityEngineFactory.CreateUsingDirectory(options.TemplatesPath);
            }

            // Use the default SmtpClient if not specified by the user
            if (options.SmtpClientFactory == null)
            {
                options.SmtpClientFactory = () => new SmtpClient();
            }

            this.Options = options;
        }

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

            if (!string.IsNullOrWhiteSpace(this.Options.DefaultFrom))
            {
                msg.From = new MailAddress(this.Options.DefaultFrom);
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
            if (this.Options.MailServiceMode == MailServiceMode.Disabled)
            {
                if (msg.To.Count > 0)
                {
                    log.InfoFormat("Message entitled '{0}' to '{1}' not sent as VelocityMail is disabled.",
                        msg.To[0], msg.Subject);
                }

                return;
            }

            using (MailMessage mmsg = msg.GetMailMessage(this.Engine))
            {
                try
                {
                    // Add the global BCC list to the message.
                    if (!string.IsNullOrWhiteSpace(this.Options.GlobalBcc))
                    {
                        mmsg.Bcc.Add(this.Options.GlobalBcc);
                    }

                    // Re-write the destination addresses according to the rewrite
                    // rules, if enabled.
                    if (this.Options.RewriteAddresses)
                    {
                        ApplyRewriteRules(mmsg.To);
                        ApplyRewriteRules(mmsg.CC);
                        ApplyRewriteRules(mmsg.Bcc);
                    }

                    if (this.Options.MailServiceMode == MailServiceMode.Test)
                    {
                        mmsg.Subject = "[TEST]: " + mmsg.Subject;
                    }

                    using (var sender = this.Options.SmtpClientFactory())
                    {
                        sender.Send(mmsg);
                    }

                    if (!string.IsNullOrWhiteSpace(this.Options.SaveTo))
                    {
                        using(var sender = new SmtpClient())
                        {
                            sender.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                            sender.PickupDirectoryLocation = this.Options.SaveTo;
                            sender.Send(mmsg);
                        }
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
            foreach (var gvar in this.Options.GlobalVariables)
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

                foreach (var rule in this.Options.MailAddressRewriteRules)
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
