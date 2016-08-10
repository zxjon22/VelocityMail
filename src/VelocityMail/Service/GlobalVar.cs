namespace VelocityMail.Service
{
    /// <summary>
    /// Global variables that are made available to all templates automatically
    /// </summary>
    public class GlobalVar
    {
        /// <summary>
        /// Creates a new <see cref="GlobalVar"/>
        /// </summary>
        /// <param name="name">Unique name (key) of the variable</param>
        /// <param name="value">Value of the variable</param>
        public GlobalVar(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Unique name (key) of the global variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of the global variable
        /// </summary>
        public object Value { get; set; }
    }
}
