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

        /// <summary>
        /// Clamps a comparable value to within min and max, inclusive
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">Value to clamp</param>
        /// <param name="min">The minimum allowable value</param>
        /// <param name="max">The maximum allowable value</param>
        /// <returns>A clamped value</returns>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if( val.CompareTo(min) < 0 )
                return min;

            else if( val.CompareTo(max) > 0 )
                return max;

            else
                return val;
        }
    }
}
