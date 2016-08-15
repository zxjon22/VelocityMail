using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using netDumbster.smtp;
using VelocityMail.Service;
using Xunit;

namespace VelocityMail.Tests
{
    public class SaveToTests : AbstractMailTest
    {
        public SaveToTests()
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            this.mailDropDir = Path.Combine(path, "MailDrop");
            Directory.CreateDirectory(mailDropDir);

            // Delete any old files.
            foreach (var file in Directory.GetFiles(mailDropDir))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Directory to save a copy of the e-mails to.
        /// </summary>
        private readonly string mailDropDir;

        /// <summary>
        /// Test saving a copy of the e-mail to disk when sending.
        /// </summary>
        [Fact]
        public void SaveTo()
        {
            var svc = new VelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "VelocityMail.Tests.Assets.Embedded, VelocityMail.Tests",
                SaveTo = this.mailDropDir,
                SmtpClientFactory = () => this.CreateSmtpClient()
            });

            using (var msg = svc.CreateMailMessage("SimpleEmbedded", "testfrom@test.com", "testto@test.com"))
            {
                svc.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(1);
            Directory.GetFiles(this.mailDropDir).Count().Should().Be(1);
        }
    }
}
