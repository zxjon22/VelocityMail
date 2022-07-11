using FluentAssertions;
using netDumbster.smtp;
using System;
using System.Linq;
using VelocityMail.Service;
using Xunit;

namespace VelocityMail.Tests
{
    public class AttachmentTests : AbstractMailTest
    {
        /// <summary>
        /// Tests sending an e-mail attachment whose source is a readable stream.
        /// </summary>
        [Fact]
        public void StreamAttachment_ShouldSucceed()
        {
            // Arrange
            var service = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "VelocityMail.Tests.Assets.Embedded, VelocityMail.Tests",
                SmtpClientFactory = () => this.CreateSmtpClient()
            });

            // Act
            var resource = this.GetType().Assembly
                .GetManifestResourceStream("VelocityMail.Tests.Assets.Attachments.foo.txt");

            using (var msg = service.CreateMailMessage("SimpleEmbedded", "testfrom@test.com", "testto@test.com"))
            {
                msg.AttachFile(resource, null, "foo.txt");
                service.Send(msg);
            }

            // Assert
            this.smtpServer.ReceivedEmailCount.Should().Be(1);

            var att = this.smtpServer.ReceivedEmail[0].MessageParts.Last();
            att.HeaderData.Should().Be("application/octet-stream; name=foo.txt");
            att.GetDecodedBody().Should().Be("This is an attachment.");

            resource.Invoking(r => r.ReadByte())
                .Should().Throw<ObjectDisposedException>(because: "VelocityMail will have disposed the stream");
        }
    }
}
