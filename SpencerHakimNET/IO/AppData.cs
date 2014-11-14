using System;
using System.IO;

namespace SpencerHakim.IO
{
    using Environment = System.Environment;

    /// <summary>
    /// AppData type
    /// </summary>
    public enum AppDataKind
    {
        /// <summary>
        /// Global for all users
        /// </summary>
        System,

        /// <summary>
        /// Per user
        /// </summary>
        User
    }

    /// <summary>
    /// Handles AppData concerns
    /// </summary>
    public static class AppData
    {
        /// <summary>
        /// Gets the path for storing specific kinds of app data, with fallbacks in case more preferred locations aren't accessible
        /// </summary>
        /// <param name="kind">The kind of app data to get the path for</param>
        /// <param name="appendPath">Path to append, except when the result is the current directory</param>
        /// <returns>The path for storing the provided kind of app data</returns>
        public static string GetPath(AppDataKind kind, string appendPath)
        {
            string folder;
            string[] dirs;

            switch (kind)
            {
                case AppDataKind.System:
                    dirs = new[]
                    {
                        Environment.CurrentDirectory, // Program Files (because, in practice, this is where people look), or the VS solution dir
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) // %ProgramData%
                    };
                    break;

                case AppDataKind.User:
                    dirs = new[]
                    {
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), // %AppData%
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), // %LocalAppData%
                        Environment.GetFolderPath(Environment.SpecialFolder.Personal) // probably My Documents
                    };
                    break;

                default:
                    throw new ArgumentException("Unknown value", "kind");
            }

            foreach(var dir in dirs)
            {
                if( !Permissions.HasReadWrite(folder = Path.GetFullPath(dir + "\\")) )
                    continue;

                return folder == Path.GetFullPath(Environment.CurrentDirectory + "\\") ?
                    folder :
                    Path.Combine(folder, appendPath);
            }

            throw new UnauthorizedAccessException( String.Format("Can't access {0}-level AppData storage", kind) );
        }
    }
}
