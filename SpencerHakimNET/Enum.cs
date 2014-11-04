namespace SpencerHakim
{
    /// <summary>
    /// Helpful Enum methods
    /// </summary>
    public static class Enum
    {
        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <returns>An object of type T with the specified value</returns>
        public static T Parse<T>(string value)
        {
            return (T)System.Enum.Parse(typeof(T), value);
        }
    }
}
