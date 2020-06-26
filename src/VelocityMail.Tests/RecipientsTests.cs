using FluentAssertions;
using VelocityMail.Service;
using Xunit;

namespace VelocityMail.Tests
{
    public class RecipientsTests : AbstractMailTest
    {
        /// <summary>
        /// Tests sending an e-mail using a simple text-only template embedded
        /// in an assembly.
        /// </summary>
        [Fact]
        public void RecipientsPopulated()
        {
            var service = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "VelocityMail.Tests.Assets.Embedded, VelocityMail.Tests",
                SmtpClientFactory = () => this.CreateSmtpClient()
            });

            using (var msg = service.CreateMailMessage("SimpleEmbedded", "testfrom@test.com", "testto@test.com"))
            {
                msg.Cc.Add("testcc@test.com");
                msg.ReplyTo.Add("reply@test.com");
                service.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(1);
            this.smtpServer.ReceivedEmail[0].FromAddress.Address.Should().Be("testfrom@test.com");
            this.smtpServer.ReceivedEmail[0].Headers["To"].Should().Be("testto@test.com");
            this.smtpServer.ReceivedEmail[0].Headers["Cc"].Should().Be("testcc@test.com");
            this.smtpServer.ReceivedEmail[0].Headers["Reply-To"].Should().Be("reply@test.com");
        }
    }
}
