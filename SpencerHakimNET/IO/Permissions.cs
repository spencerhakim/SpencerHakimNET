using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SpencerHakim.Extensions;

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
            //kind of a bruteforce methodology, but it works
            return has(path,
                file=>{
                    new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite).SafeDispose();
                },
                dir=>{
                    string tempFile = Path.Combine(dir, Guid.NewGuid().ToString());
                    System.IO.File.Create(tempFile).SafeDispose();
                    System.IO.File.Delete(tempFile);
                }
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

            Uri uri = null;
            if( !Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out uri) || uri.Scheme != "file" )
                throw new ArgumentException("Malformed or invalid path", "path");

            string originalPath = path;

            try
            {
                //loop through path/parent paths
                do
                {
                    try
                    {
                        //try file attrs first, then try a file/dir specific test
                        FileAttributes attr = System.IO.File.GetAttributes(path);

                        //pick the appropriate test
                        Action<string> test;
                        if( (attr & FileAttributes.Directory) == FileAttributes.Directory )
                            test = dirTest;
                        else
                            test = fileTest;

                        test(path); //throws if the test fails
                        Directory.CreateDirectory(originalPath); //make sure we create any missing directories we moved up from

                        return true;
                    }
                    catch(Exception ex)
                    {
                        if( ex is FileNotFoundException || ex is DirectoryNotFoundException )
                            path = Directory.GetParent(path).FullName;
                        else
                            return false; //unauthorized access or some other problem
                    }
                }
                while( path != null );
            }
            catch{}

            return false;
        }
    }
}
