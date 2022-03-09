using System.Collections.Generic;
using System.Net.Mail;

namespace VelocityMail.Service
{
    /// <summary>
    /// A testable implementation of <see cref="VelocityMailService"/> that captures e-mails
    /// to <see cref="SentMessages"/> rather than actually sending them.
    /// </summary>
    public class TestableVelocityMailService : VelocityMailService
    {
        /// <inheritdoc/>
        public TestableVelocityMailService(VelocityMailOptions options) : base(options)
        {
        }

        private readonly List<MailMessage> _sentMessages = new List<MailMessage>();

        /// <summary>
        /// Gets the collection of sent <see cref="VelocityMailMessage"/> messages.
        /// </summary>
        public IReadOnlyCollection<MailMessage> SentMessages { get => _sentMessages; }

        /// <summary>
        /// Clears (deletes) all sent messages.
        /// </summary>
        public void ClearMessages()
        {
            _sentMessages.Clear();
        }

        /// <inheritdoc/>
        public override void Send(VelocityMailMessage msg)
        {
            var mailMessage = msg.GetMailMessage(Engine, true);
            _sentMessages.Add(mailMessage);
        }
    }
}
