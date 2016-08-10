namespace VelocityMail
{
    /// <summary>
    /// Match mode for a rewrite rule.
    /// </summary>
    public enum MatchMode
    {
        /// <summary>
        /// If the address matches the RegEx
        /// </summary>
        Matches = 0,

        /// <summary>
        /// If the address does not match the regex
        /// </summary>
        DoesNotMatch
    }
}
