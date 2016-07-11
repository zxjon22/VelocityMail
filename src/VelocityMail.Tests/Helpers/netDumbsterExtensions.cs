using System;
using System.Text;
using netDumbster.smtp;

namespace netDumbster.smtp
{
    /// <summary>
    /// Useful extensions to netDumbster.
    /// </summary>
    public static class netDumbsterExtensions
    {
        /// <summary>
        /// Decodes the message box. This assumes the body is utf-8 text
        /// encoded using Base64.
        /// </summary>
        /// <param name="msgPart">Extension point</param>
        /// <returns>Decoded message body</returns>
        public static string GetDecodedBody(this SmtpMessagePart msgPart)
        {
            var bodyBytes = Convert.FromBase64String(msgPart.BodyData);
            var body = Encoding.UTF8.GetString(bodyBytes);

            return body;
        }
    }
}
