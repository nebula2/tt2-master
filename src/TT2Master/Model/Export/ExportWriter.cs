using System.IO;
using Xamarin.Forms;

namespace TT2Master
{
    /// <summary>
    /// Logger. Mainly to log startup
    /// </summary>
    public static class ExportWriter
    {
        /// <summary>
        /// Name of log
        /// </summary>
        public static string LogName => "artsp.json";
        /// <summary>
        /// name of small log
        /// </summary>
        public static string LogNameSmall => "artspLvl.json";

        /// <summary>
        /// Deletes the logfile
        /// </summary>
        public static void DeleteFile(bool small)
        {
            //get path and dir
            string path = DependencyService.Get<IDirectory>().GetDownloadPathName(small ? LogNameSmall :LogName);
            string dir = DependencyService.Get<IDirectory>().GetDownloadPath();

            //Check path and return if non existant
            if (!Directory.Exists(dir))
            {
                return;
            }

            //delete if existant
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Writes Artifact Optimization log
        /// </summary>
        /// <param name="text"></param>
        public static string WriteFile(string text, bool small)
        {
            //get path and dir
            string path = DependencyService.Get<IDirectory>().GetDownloadPathName(small ? LogNameSmall : LogName);
            string dir = DependencyService.Get<IDirectory>().GetDownloadPath();

            //Check path and create if non existant
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //write
            using (var sw = File.AppendText(path))
            {
                sw.WriteLine(text);
            }

            return path;
        }
    }
}