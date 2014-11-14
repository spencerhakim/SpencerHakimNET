using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpencerHakim.Extensions
{
    /// <summary>
    /// Methods for easy Stream usage
    /// </summary>
    public static class StreamMethods
    {
        /// <summary>
        /// Nicer looking Deserialize method that handles the type casting automatically
        /// </summary>
        /// <typeparam name="T">Type to deserialize to</typeparam>
        /// <param name="bf">BinaryFormatter to deserialize with</param>
        /// <param name="serializationStream">Stream to deserialize from</param>
        /// <param name="obj">Object to deserialize stream into</param>
        /// <remarks>An out parameter is used to automatically determine the T type</remarks>
        public static void Deserialize<T>(this BinaryFormatter bf, Stream serializationStream, out T obj)
        {
            if( bf == null )
                throw new ArgumentNullException("bf");

            if( serializationStream == null )
                throw new ArgumentNullException("serializationStream");

            obj = (T)bf.Deserialize(serializationStream);
        }

        /// <summary>
        /// Gets data from a Uri resource and returns it as the specified type, transforming it to that type using the Func callback
        /// </summary>
        /// <typeparam name="T">Type to return data as</typeparam>
        /// <param name="uri">Location of the data resource</param>
        /// <param name="func">Transforms data resource stream into the specified type</param>
        /// <returns>Data from the Uri resource, in the specified type</returns>
        public static T Get<T>(this Uri uri, Func<Stream, T> func)
        {
            if( func == null )
                throw new ArgumentNullException("func");

            using( var response = WebRequest.Create(uri).GetResponse() )
                using( var stream = response.GetResponseStream() )
                    return func(stream);
        }

        /// <summary>
        /// Gets data from a Uri resource and returns it as a MemoryStream
        /// </summary>
        /// <param name="uri">Location of the data resource</param>
        /// <returns>Data from the Uri resource, in a MemoryStream</returns>
        public static MemoryStream Get(this Uri uri)
        {
            return Get(uri, (stream) => {
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms;
            });
        }
    }
}
