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
#if NET452
            // NOTE: netDumbster 2.x already decodes the body data where as
            //       the old version which supports net452 doesn't.
            var bodyBytes = Convert.FromBase64String(msgPart.BodyData);
            var body = Encoding.UTF8.GetString(bodyBytes);
#else
            var body = msgPart.BodyData;
#endif
            return body;
        }
    }
}
