using System;
using System.IO;
using FluentAssertions;
using netDumbster.smtp;
using VelocityMail.Service;
using Xunit;

namespace VelocityMail.Tests
{
    public class TemplateLocationTests : AbstractMailTest
    {
        #region Constants
        
        /// <summary>
        /// Body of the SimpleEmbedded-txt.vm template
        /// </summary>
        private const string SIMPLE_EMBEDDED_BODY = "Simple embedded template";

        /// <summary>
        /// Body of the SimpleOnDisk-txt.vm template
        /// </summary>
        private const string SIMPLE_ON_DISK_TEMPLATE = "Simple on-disk template";

        /// <summary>
        /// Body of the SimpleOnDesk-txt.vm template on disk (not the embedded version)
        /// </summary>
        private const string SEARCH_PRIORITY_DISK_TEMPLATE = "This is the on-disk template";

        #endregion

        /// <summary>
        /// Tests sending an e-mail using a simple text-only template embedded
        /// in an assembly.
        /// </summary>
        [Fact]
        public void TextTemplateEmbeddedInAssembly()
        {
            var service = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "VelocityMail.Tests.Assets.Embedded, VelocityMail.Tests",
                SmtpClientFactory = () => this.CreateSmtpClient()
            });

            using (var msg = service.CreateMailMessage("SimpleEmbedded", "testfrom@test.com", "testto@test.com"))
            {
                service.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(1);
            this.smtpServer.ReceivedEmail[0].MessageParts[0].GetDecodedBody().Should().Be(SIMPLE_EMBEDDED_BODY);
        }

        /// <summary>
        /// Tests sending an e-mail using a simple text-only template on
        /// the file system.
        /// </summary>
        [Fact]
        public void TextTemplateOnDisk()
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            var service = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesPath = Path.Combine(path, @"Assets\OnDisk"),
                SmtpClientFactory = () => this.CreateSmtpClient()
            });

            using (var msg = service.CreateMailMessage("SimpleOnDisk", "testfrom@test.com", "testto@test.com"))
            {
                service.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(1);
            this.smtpServer.ReceivedEmail[0].MessageParts[0].GetDecodedBody().Should().Be(SIMPLE_ON_DISK_TEMPLATE);
        }

        /// <summary>
        /// Tests the search priority is correct - on disk templates should always be preferred
        /// over templates embedded in an assembly, if both are configured.
        /// </summary>
        [Fact]
        public void EnsureCorrectSearchPriority()
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            var service = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "VelocityMail.Tests.Assets.Embedded, VelocityMail.Tests",
                TemplatesPath = Path.Combine(path, @"Assets\OnDisk"),
                SmtpClientFactory = () => this.CreateSmtpClient()
            });

            using (var msg = service.CreateMailMessage("SearchPriority", "testfrom@test.com", "testto@test.com"))
            {
                service.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(1);
            this.smtpServer.ReceivedEmail[0].MessageParts[0].GetDecodedBody().Should().Be(SEARCH_PRIORITY_DISK_TEMPLATE);
        }
    }
}
