
namespace VelocityMail.Configuration
{
    /// <summary>
    /// Service operation
    /// </summary>
    public enum MailServiceMode
    {
        /// <summary>
        /// Sends e-mail normally
        /// </summary>
        Enabled = 0,

        /// <summary>
        /// Re-writes any e-mail addresses and then sends the e-mail normally
        /// </summary>
        EnabledWithAddressRewrite,

        /// <summary>
        /// Only logs e-mail as sent - it does not actually send any e-mail.
        /// </summary>
        Disabled,
    }
}
