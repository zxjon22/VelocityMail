using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace VelocityMail
{
    /// <summary>
    /// Templated mail message. This class is independant of the templating engine.
    /// The Velocity-specific parts are in <see cref="SystemNetMailMessageAdapter"/>
    /// </summary>
    public class VelocityMailMessage : IDisposable
    {
        #region Constructors
        
        /// <summary>
        /// Constructs a new <see cref="VelocityMailMessage"/> using the specified
        /// template.
        /// </summary>
        /// <param name="templateName">Name of the template to use (minus the {(-txt|-html).vm extension)</param>
        public VelocityMailMessage(string templateName)
            : this()
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentNullException("templateName", "templateName is required");
            }

            this.TemplateName = templateName;
        }

        /// <summary>
        /// Constructs a new <see cref="VelocityMailMessage"/> using the specified
        /// template.
        /// </summary>
        /// <param name="templateName">Name of the template to use (minus the {(-txt|-html).vm extension)</param>
        /// <param name="from">'From' e-mail address</param>
        /// <param name="to">Recipient e-mail address</param>
        public VelocityMailMessage(string templateName, string from, string to)
            : this(templateName)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentNullException("from", "from is required");
            }

            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentNullException("to", "to is required");
            }

            this.From = new MailAddress(from);
            this.To.Add(to);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected VelocityMailMessage()
        {
            this.To = new MailAddressCollection();
            this.Cc = new MailAddressCollection();
            this.Bcc = new MailAddressCollection();
            this.ReplyTo = new MailAddressCollection();
            this.ContextData = new Dictionary<string, object>();
            this.Attachments = new List<Attachment>();
            this.Encoding = System.Text.Encoding.GetEncoding("utf-8");
            this.HtmlBodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
        }

        #endregion

        #region Properties

        /// <summary>
        /// E-mail 'From' address
        /// </summary>
        public MailAddress From { get; set; }

        /// <summary>
        /// 'To' Recipients
        /// </summary>
        public MailAddressCollection To { get; private set; }

        /// <summary>
        /// 'Cc' Recipients
        /// </summary>
        public MailAddressCollection Cc { get; private set; }

        /// <summary>
        /// 'Bcc' Recipients
        /// </summary>
        public MailAddressCollection Bcc { get; private set; }

        /// <summary>
        /// Reply-to Recipients
        /// </summary>
        public MailAddressCollection ReplyTo { get; private set; }

        /// <summary>
        /// Subject of the e-mail. This text is processed in the same
        /// way as the template body.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// List of e-mail attachments.
        /// FIXME: Should be our own collection so we don't have dispose issues.
        /// </summary>
        public List<Attachment> Attachments { get; private set; }

        /// <summary>
        /// Text-encoding for the subject and any text body part of the mail message.
        /// Defaults to utf-8.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Text-encoding for any HTML body part of the mail message. Defaults to utf-8.
        /// </summary>
        public Encoding HtmlBodyEncoding { get; set; }

        /// <summary>
        /// Name of the template of this e-mail.
        /// </summary>
        public string TemplateName { get; private set; }

        /// <summary>
        /// Context data (variables) for use by the template engine.
        /// </summary>
        public IDictionary<string, object> ContextData { get; set; }

        #endregion

        #region Attachments

        /// <summary>
        /// Attach the specified file using the supplied media type to this mail message.
        /// </summary>
        /// <param name="fileName">Path to the file to attach</param>
        /// <param name="mediaType">Mime type of the file. If null, application/octet-stream is
        /// used.</param>
        /// <param name="attachmentName">Suggested file name for the attachment.</param>
        /// <exception cref="FileNotFoundException">If the file was not found</exception>
        public void AttachFile(string fileName, string mediaType, string attachmentName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Could not attach file", fileName);
            }

            if (null == mediaType)
            {
                mediaType = "application/octet-stream";
            }

            var att = new Attachment(fileName, mediaType);
            att.ContentDisposition.Inline = false;
            att.Name = Path.GetFileName(attachmentName);
            att.ContentDisposition.FileName = att.Name;
            Attachments.Add(att);
        }

        /// <summary>
        /// Attach the specified file using the supplied media type
        /// to this TemplatedMailMessage.
        /// </summary>
        /// <param name="fileName">Path to the file to attach</param>
        /// <param name="mediaType">Mime type of the file. If null,
        /// application/octet-stream is used.</param>
        public void AttachFile(string fileName, string mediaType)
        {
            this.AttachFile(fileName, mediaType, fileName);
        }

        /// <summary>
        /// Attach the specified file using the supplied media type
        /// to this TemplatedMailMessage.
        /// </summary>
        /// <param name="contentStream">A readable stream containing the attachment's content.</param>
        /// <param name="mediaType">Mime type of the file. If null, application/octet-stream is used.</param>
        /// <param name="attachmentName">Suggested file name for the attachment.</param>
        /// <remarks>
        /// The stream will be closed when the <see cref="VelocityMailMessage"/> is disposed.
        /// </remarks>
        public void AttachFile(Stream contentStream, string mediaType, string attachmentName)
        {
            _ = contentStream ?? throw new ArgumentNullException(nameof(contentStream));
            _ = attachmentName ?? throw new ArgumentNullException(nameof(attachmentName));
            mediaType ??= "application/octet-stream";

            var att = new Attachment(contentStream, attachmentName, mediaType);
            att.ContentDisposition.Inline = false;
            att.Name = Path.GetFileName(attachmentName);
            att.ContentDisposition.FileName = att.Name;
            Attachments.Add(att);
        }

        #endregion

        #region Context data (variables)
        
        /// <summary>
        /// Adds a variable to the context data under the specified key.
        /// </summary>
        /// <example>
        /// mailMessage.Put("name", name");
        /// ...
        /// ${name}
        /// </example>
        /// <param name="key">The key of this variable in the context data</param>
        /// <param name="value">Variable</param>
        public void Put(string key, object value)
        {
            this.ContextData[key] = value;
        }

        #endregion

        #region IDisposable implementation
        
        private bool attachmentsDisposed = false;

        /// <summary>
        /// Dispose of any attachments
        /// </summary>
        public void Dispose()
        {
            if (!attachmentsDisposed)
            {
                this.Attachments.ForEach(x => x.Dispose());
                this.attachmentsDisposed = true;
            }
        }

        #endregion
    }
}
