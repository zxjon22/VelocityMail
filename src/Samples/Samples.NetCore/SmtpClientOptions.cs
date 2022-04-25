using System.Net;
using System.Net.Mail;

namespace Samples.NetCore
{
    /// <summary>
    /// The <see cref="SmtpClient"/> in .Net Core exists for compatibility only and to ease porting of applications
    /// over from the full framework. There is no built-in way to configure the SMTP client using appsettings.json.
    /// This is a small configuration class to workaround this.
    /// </summary>
    public class SmtpClientOptions
    {
        /// <summary>
        /// Host or IP to use for SMTP transactions.
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// Port to use for SMTP transactions.
        /// </summary>
        public int Port { get; set; } = 25;

        /// <summary>
        /// Optional username to authenticate with.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Password to authenticate with, if <see cref="UserName"/> has been specified.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Specifies whether or not the client uses SSL to encrypt the connection.
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Specifies whether or not CredentialCache.DefaultCredentials are sent with requests.
        /// </summary>
        public bool UseDefaultCredentials { get; set; } = true;

        /// <summary>
        /// Factory method to create a <see cref="SmtpClient"/> usin the configuration in this
        /// <see cref="SmtpSettings"/> instance.
        /// </summary>
        /// <returns>Configured <see cref="SmtpClient"/></returns>
        public virtual SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient(Host, Port);
            client.UseDefaultCredentials = UseDefaultCredentials;
            client.EnableSsl = EnableSsl;

            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrEmpty(Password))
            {
                client.Credentials = new NetworkCredential(UserName, Password);
            }

            return client;
        }
    }
}
