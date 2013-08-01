using System;

namespace SpencerHakim.Extensions
{
    /// <summary>
    /// Some random extension methods
    /// </summary>
    public static class GenericMethods
    {
        /// <summary>
        /// Safely get the hash value of a possibly null object
        /// </summary>
        /// <param name="obj">The object to get the hash value of</param>
        /// <returns>The hash value of the object, or 0 if it's null</returns>
        public static int SafeGetHashCode(this object obj)
        {
            if( obj == null )
                return 0;

            return obj.GetHashCode();
        }

        /// <summary>
        /// Safely disposes of IDisposables by first checking if they are null
        /// </summary>
        /// <param name="disposable">The object to safely dispose</param>
        public static void SafeDispose(this IDisposable disposable)
        {
            if( disposable != null )
                disposable.Dispose();
        }

        /// <summary>
        /// Casts objects to anonymous types
        /// </summary>
        /// <typeparam name="T">Type to cast to</typeparam>
        /// <param name="obj">The object to cast</param>
        /// <param name="objOfAnonType">An object of the anonymous type</param>
        /// <returns>The casted object</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="objOfAnonType")]
        public static T AnonymousCast<T>(this object obj, T objOfAnonType)
        {
            return (T)obj;
        }
    }
}
