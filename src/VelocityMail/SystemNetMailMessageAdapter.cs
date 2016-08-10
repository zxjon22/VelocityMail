using System;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using NVelocity;
using NVelocity.App;
using NVelocity.Exception;
using VelocityMail.Logging;

namespace VelocityMail
{
    /// <summary>
    /// Creates a System.Net.Mail.MailMessage which can be sent from the SmtpClient from
    /// a VelocityMailMessage.
    /// </summary>
    static class SystemNetMailMessageAdapter
    {
        /// <summary>
        /// Logging
        /// </summary>
        static readonly ILog log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Parses the <see cref="VelocityMailMessage"/> and returns a <see cref="MailMessage"/>
        /// for sending.
        /// </summary>
        /// <param name="msg">Extension point</param>
        /// <param name="engine">Velocity instance used to parse the template</param>
        /// <returns>Final MailMessage for sending</returns>
        internal static MailMessage GetMailMessage(this VelocityMailMessage msg, VelocityEngine engine)
        {
            var ctx = new VelocityContext();

            foreach(var entry in msg.ContextData)
            {
                ctx.Put(entry.Key, entry.Value);
            }

            var txtBody = ParseLocalisedTemplate(engine, ctx, msg.TemplateName + "-txt");
            var htmlBody = ParseLocalisedTemplate(engine, ctx, msg.TemplateName + "-html");
            var subject = ParseString(engine, ctx, msg.Subject);

            if (string.IsNullOrWhiteSpace(txtBody) && string.IsNullOrWhiteSpace(htmlBody))
            {
                throw new MailPreparationException(String.Format(
                    "Could not find any e-mail templates for {0}-(-txt.vm|-html.vm)",
                    msg.TemplateName));
            }

            if (msg.To.Count == 0 && msg.Cc.Count == 0 && msg.Bcc.Count == 0)
            {
                throw new MailPreparationException("No recipients were specified");
            }

            // Message is valid. Create a System.Net.Mail.MailMessage for sending
            var mailMessage = new MailMessage();

            try
            {
                mailMessage.From = msg.From;

                foreach (MailAddress addr in msg.To)
                {
                    mailMessage.To.Add(addr);
                }

                foreach (MailAddress addr in msg.Cc)
                {
                    mailMessage.CC.Add(addr);
                }

                foreach (MailAddress addr in msg.Bcc)
                {
                    mailMessage.Bcc.Add(addr);
                }

                mailMessage.Subject = subject;
                mailMessage.SubjectEncoding = msg.Encoding;

                if (!string.IsNullOrEmpty(txtBody))
                {
                    mailMessage.Body = txtBody;
                    mailMessage.BodyEncoding = msg.Encoding;
                }

                if (!string.IsNullOrEmpty(htmlBody))
                {
                    if (string.IsNullOrEmpty(txtBody))
                    {
                        mailMessage.Body = htmlBody;
                        mailMessage.BodyEncoding = msg.HtmlBodyEncoding;
                        mailMessage.IsBodyHtml = true;
                    }

                    else
                    {
                        var av = AlternateView.
                            CreateAlternateViewFromString(htmlBody, msg.HtmlBodyEncoding, "text/html");

                        mailMessage.AlternateViews.Add(av);
                    }
                }

                foreach (var att in msg.Attachments)
                {
                    mailMessage.Attachments.Add(att);
                }
            }

            catch (Exception ex)
            {
                // Otherwise the attachments in the VelocityMailMessage will
                // get disposed as they're in both AttachmentCollections.
                mailMessage.Attachments.Clear();
                mailMessage.Dispose();
                throw ex;
            }

            return mailMessage;
        }

        /// <summary>
        /// Parses the specified template using NVelocity.
        /// </summary>
        /// <param name="ctx">VelocityContext used to populate the template</param>
        /// <param name="templateName">Name of the velocity template to use</param>
        /// <returns>The parsed template or null if no suitable template could be found</returns>
        static string ParseLocalisedTemplate(VelocityEngine engine, VelocityContext ctx, string templateName)
        {
            var ci = CultureInfo.CurrentUICulture;
            var langCode = ci.TwoLetterISOLanguageName;

            // Try to find a version of the template for the current UI culture
            var output = ParseTemplate(engine, ctx, string.Format("{0}-{1}.vm", templateName, langCode));

            // If not, fallback to the default.
            if (output == null)
            {
                output = ParseTemplate(engine, ctx, String.Format("{0}.vm", templateName));
            }

            return output;
        }

        static string ParseTemplate(VelocityEngine engine, VelocityContext ctx, string templateName)
        {
            try
            {
                var template = engine.GetTemplate(templateName);
                var writer = new StringWriter();
                template.Merge(ctx, writer);
                return writer.ToString();
            }

            // NOTE: If the template is not found, a log entry is made and
            // NULL is returned to the caller
            catch (ResourceNotFoundException)
            {
                if (log.IsDebugEnabled() && !templateName.EndsWith("en.vm"))
                {
                    log.DebugFormat("Could not find e-mail template: '{0}'", templateName);
                }
            }

            catch (ParseErrorException pee)
            {
                throw new MailPreparationException(pee.Message, pee);
            }

            catch (Exception ex)
            {
                throw new MailPreparationException(ex.Message, ex);
            }

            return null;
        }

        static string ParseString(VelocityEngine engine, VelocityContext ctx, string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return template;
            }

            using (var writer = new StringWriter())
            {
                if (!engine.Evaluate(ctx, writer, typeof(VelocityMailMessage).Name, template))
                {
                    throw new MailPreparationException(
                        "NVelocity failed to merge the VelocityMailMessage template. " +
                        "See log for details");
                }

                return writer.ToString();
            }
        }
    }
}
