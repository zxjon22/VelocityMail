using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Mail;

namespace VelocityMail.Service
{
    /// <summary>
    /// A testable thread-safe implementation of <see cref="VelocityMailService"/> that captures e-mails
    /// to <see cref="SentMessages"/> and <see cref="RawMessages"/> rather than actually sending them.
    /// </summary>
    public class TestableVelocityMailService : VelocityMailService
    {
        /// <inheritdoc/>
        public TestableVelocityMailService(VelocityMailOptions options) : base(options)
        {
        }

        private readonly ConcurrentQueue<MailMessage> _sentMessages = new ConcurrentQueue<MailMessage>();
        private readonly ConcurrentQueue<VelocityMailMessage> _rawMessages = new ConcurrentQueue<VelocityMailMessage>();

        /// <summary>
        /// Gets the collection of sent <see cref="MailMessage"/> messages.
        /// </summary>
        public MailMessage[] SentMessages { get => _sentMessages.ToArray(); }

        /// <summary>
        /// Gets the collection of sent <see cref="VelocityMailMessage"/> messages before
        /// processing and converting to a <see cref="MailMessage"/>.
        /// </summary>
        public VelocityMailMessage[] RawMessages { get => _rawMessages.ToArray(); }

        /// <summary>
        /// Clears (deletes) all sent messages.
        /// </summary>
        public void ClearMessages()
        {
            // NOTE: .NetStandard 2.0 does not contain `Clear()`. That first appears in 2.1.
            while (_rawMessages.TryDequeue(out _)) { };
            while (_sentMessages.TryDequeue(out _)) { };
        }

        /// <inheritdoc/>
        public override void Send(VelocityMailMessage msg)
        {
            var mailMessage = msg.GetMailMessage(Engine, true);
            _sentMessages.Enqueue(mailMessage);
            _rawMessages.Enqueue(msg);
        }
    }
}
