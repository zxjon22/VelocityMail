using FluentAssertions;
using netDumbster.smtp;
using VelocityMail.Service;
using Xunit;

namespace VelocityMail.Tests
{
    public class FormattingTests : AbstractMailTest
    {
        #region Constants

        /// <summary>
        /// Body of the HtmlEncoding-html.vm template
        /// </summary>
        private const string HTML_ENCODING_BODY_CONTENT =
            "<html><head></head><body>The names, $names, which contain an '&amp;' should " +
            "be HTML-encoded.</body></html>";

        /// <summary>
        /// Body of the NoHtmlEncoding-txt.vm template
        /// </summary>
        private const string NO_HTML_ENCODING_BODY_CONTENT =
            "The names, Bill & Ben, which contain an '&' should *not* be HTML-encoded in this text alternate view.";

        #endregion

        /// <summary>
        /// Tests that substitution text in HTML templates is correctly HTML-encoded.
        /// </summary>
        [Fact]
        public void HtmlEncodingInHtmlTemplate()
        {
            var service = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "VelocityMail.Tests.Assets.Embedded, VelocityMail.Tests",
                SmtpClientFactory = () => this.CreateSmtpClient()
            });

            using (var msg = service.CreateMailMessage("HtmlEncoding", "testfrom@test.com", "testto@test.com"))
            {
                msg.Put("names", "Bill & Ben");
                service.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(1);
            this.smtpServer.ReceivedEmail[0].MessageParts[0].GetDecodedBody().Should().Be(HTML_ENCODING_BODY_CONTENT);
        }

        /// <summary>
        /// Tests that substitution text in HTML templates is *not* HTML-encoded for plain text
        /// templates.
        /// </summary>
        [Fact]
        public void NoHtmlEncodingInHtmlTemplate()
        {
            var service = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "VelocityMail.Tests.Assets.Embedded, VelocityMail.Tests",
                SmtpClientFactory = () => this.CreateSmtpClient()
            });

            using (var msg = service.CreateMailMessage("NoHtmlEncoding", "testfrom@test.com", "testto@test.com"))
            {
                msg.Put("names", "Bill & Ben");
                service.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(1);
            this.smtpServer.ReceivedEmail[0].MessageParts[0].GetDecodedBody().Should().Be(NO_HTML_ENCODING_BODY_CONTENT);
        }
    }
}
