namespace VelocityMail
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
        /// Same as 'Enabled' but all subjects are prefixed with 'Test: '
        /// </summary>
        Test,

        /// <summary>
        /// Only logs e-mail as sent - it does not actually send any e-mail.
        /// </summary>
        Disabled,
    }
}
