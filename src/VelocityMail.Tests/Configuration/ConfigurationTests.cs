using System;
using System.Configuration;
using System.IO;
using FluentAssertions;
using netDumbster.smtp;
using VelocityMail.Configuration;
using VelocityMail.Service;
using Xunit;

namespace VelocityMail.Tests.Configuration
{
    public class ConfigurationTests : AbstractMailTest
    {
        #region Constants

        /// <summary>
        /// Expected body contents for the GlobalVars-txt.vm template
        /// </summary>
        private const string GLOBAL_VARS_TEMPLATE_BODY = "siteUrl=http://localhost:1234/, author=Jonathan";

        /// <summary>
        /// Body of the SimpleEmbedded-txt.vm template
        /// </summary>
        private const string SIMPLE_EMBEDDED_BODY = "Simple embedded template";
        #endregion

        /// <summary>
        /// Tests global vars from the configuration section are passed
        /// through correctly.
        /// </summary>
        [Fact]
        public void GlobalVars()
        {
            var settings = this.GetTestConfig(@"Configuration\GlobalVars.config");
            var svc = new VelocityMailService(settings);
            svc.SmtpClientFactory = () => this.CreateSmtpClient();

            using (var msg = svc.CreateMailMessage("GlobalVars", "testfrom@test.com", "testto@test.com"))
            {
                svc.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(1);
            this.smtpServer.ReceivedEmail[0].MessageParts[0].GetDecodedBody().Should().Be(GLOBAL_VARS_TEMPLATE_BODY);
        }

        /// <summary>
        /// Test mode should prepend '[TEST]: ' to all subjects, but still send the e-mails
        /// </summary>
        [Fact]
        public void TestMode()
        {
            var settings = this.GetTestConfig(@"Configuration\TestMode.config");
            var svc = new VelocityMailService(settings);
            var subject = "TestMode test";
            svc.SmtpClientFactory = () => this.CreateSmtpClient();

            using (var msg = svc.CreateMailMessage("SimpleEmbedded", "testfrom@test.com", "testto@test.com"))
            {
                msg.Subject = subject;
                svc.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(1);
            this.smtpServer.ReceivedEmail[0].MessageParts[0].GetDecodedBody().Should().Be(SIMPLE_EMBEDDED_BODY);
            this.smtpServer.ReceivedEmail[0].Headers["Subject"].Should().Be("[TEST]: " + subject);
        }

        /// <summary>
        /// Disabled mode should not send any e-mails.
        /// </summary>
        [Fact]
        public void DisabledMode()
        {
            var settings = this.GetTestConfig(@"Configuration\DisabledMode.config");
            var svc = new VelocityMailService(settings);
            svc.SmtpClientFactory = () => this.CreateSmtpClient();

            using (var msg = svc.CreateMailMessage("SimpleEmbedded", "testfrom@test.com", "testto@test.com"))
            {
                svc.Send(msg);
            }

            this.smtpServer.ReceivedEmailCount.Should().Be(0);
        }

        /// <summary>
        /// Loads the specified configuration file off disk.
        /// </summary>
        /// <param name="fileName">Name of the configuration file to load</param>
        /// <returns>The VelocityMailServiceConfigurationSection in the config file.</returns>
        private VelocityMailSection GetTestConfig(string fileName)
        {
            var fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = this.GetAssetPath(fileName)
            };

            var config = ConfigurationManager
                .OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            var settings = (VelocityMailSection)config.GetSection("velocityMail");
            return settings;
        }
    }
}
