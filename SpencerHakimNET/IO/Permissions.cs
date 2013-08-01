using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpencerHakim.IO
{
    /// <summary>
    /// Read/write permission tests for files and directories
    /// </summary>
    public static class Permissions
    {
        /// <summary>
        /// Tests for read permission on the specified path. If the path does not exist, the nearest parent directory is tested.
        /// </summary>
        /// <param name="path">Path to a file or directory</param>
        /// <returns>True if the path (or, if the path does not exist, a parent path) can be read; otherwise false</returns>
        public static bool HasRead(string path)
        {
            return has(path,
                x=>{ using( var fs = File.OpenRead(x) ){} },
                x=>{ Directory.EnumerateFileSystemEntries(x); }
            );
        }

        /// <summary>
        /// Tests for write permission on the specified path. If the path does not exist, the nearest parent directory is tested.
        /// </summary>
        /// <param name="path">Path to a file or directory</param>
        /// <returns>True if the path (or, if the path does not exist, a parent path) can be written; otherwise false</returns>
        public static bool HasWrite(string path)
        {
            return has(path,
                x=>{ using( var fs = File.OpenWrite(x) ){} },
                x=>{ Directory.SetLastWriteTime(path, Directory.GetLastWriteTime(path)); } //might be kind of a shitty test, but it works?
            );
        }

        /// <summary>
        /// Tests for both read and write permissions on the specified path. If the path does not exist, the nearest parent directory is tested.
        /// </summary>
        /// <param name="path">Path to a file or directory</param>
        /// <returns>True if the path (or, if the path does not exist, a parent path) can be read and written; otherwise false</returns>
        public static bool HasReadWrite(string path)
        {
            return HasRead(path) && HasWrite(path);
        }

        private static bool has(string path, Action<string> fileTest, Action<string> dirTest)
        {
            if( path == null )
                throw new ArgumentNullException("path");

            if( !Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute) && (new Uri(path)).Scheme == "file" )
                throw new ArgumentException("Malformed or invalid path", "path");

            try
            {
                //loop through path/parent paths
                do
                {
                    try
                    {
                        //try file attrs first, then try a file/dir specific test
                        FileAttributes attr = File.GetAttributes(path);
                        if( (attr & FileAttributes.Directory) == FileAttributes.Directory )
                        {
                            dirTest(path);
                            return true;
                        }
                        else
                        {
                            fileTest(path);
                            return true;
                        }
                    }
                    catch(UnauthorizedAccessException)
                    {
                        return false;
                    }
                    catch
                    {
                        path = Directory.GetParent(path).FullName;
                    }
                }
                while( path != null );
            }
            catch{}

            return false;
        }
    }
}
