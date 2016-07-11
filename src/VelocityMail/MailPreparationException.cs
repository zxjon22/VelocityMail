using System;

namespace VelocityMail
{
    /// <summary>
    /// Exception thrown when there is a problem parsing a VelocityMailMessage template
    /// </summary>
    public class MailPreparationException : ApplicationException
    {
        /// <summary>
        /// Creates a new <see cref="MailPreparationException"/>
        /// </summary>
        public MailPreparationException()
            : base()
        {

        }

        /// <summary>
        /// Creates a new <see cref="MailPreparationException"/> using the specified
        /// exception message
        /// </summary>
        /// <param name="message">Exception message</param>
        public MailPreparationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="MailPreparationException"/> using the specified
        /// exception message and inner exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public MailPreparationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
