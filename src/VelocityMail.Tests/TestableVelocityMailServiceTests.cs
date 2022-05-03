using FluentAssertions;
using System.Linq;
using VelocityMail.Service;
using Xunit;

namespace VelocityMail.Tests
{
    /// <summary>
    /// Tests for the <see cref="TestableVelocityMailService"/>.
    /// </summary>
    public class TestableVelocityMailServiceTests
    {
        public TestableVelocityMailServiceTests()
        {
        }

        /// <summary>
        /// Test capturing sent messages.
        /// </summary>
        [Fact]
        public void Send()
        {
            var service = new TestableVelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "VelocityMail.Tests.Assets.Embedded, VelocityMail.Tests",
            });

            using (var msg = service.CreateMailMessage("SimpleEmbedded", "testfrom@test.com", "testto@test.com"))
            {
                msg.Cc.Add("testcc@test.com");
                msg.ReplyTo.Add("reply@test.com");
                service.Send(msg);
            }

            service.SentMessages.Should().HaveCount(1);
            service.RawMessages.Should().HaveCount(1);

            service.SentMessages.First().CC[0].Should().Be("testcc@test.com");
            service.RawMessages.First().Cc[0].Should().Be("testcc@test.com");
        }

        /// <summary>
        /// Test clearing captured messages.
        /// </summary>
        [Fact]
        public void Clear()
        {
            var service = new TestableVelocityMailService(new VelocityMailOptions
            {
                TemplatesAssembly = "VelocityMail.Tests.Assets.Embedded, VelocityMail.Tests",
            });

            using (var msg = service.CreateMailMessage("SimpleEmbedded", "testfrom@test.com", "testto@test.com"))
            {
                msg.Cc.Add("testcc@test.com");
                msg.ReplyTo.Add("reply@test.com");
                service.Send(msg);
            }

            service.SentMessages.Should().HaveCount(1);
            service.RawMessages.Should().HaveCount(1);

            service.ClearMessages();

            service.SentMessages.Should().HaveCount(0);
            service.RawMessages.Should().HaveCount(0);
        }
    }
}
