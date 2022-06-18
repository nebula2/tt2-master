using System;
using System.IO;
using Xamarin.Forms;

namespace TT2Master.Droid
{
    /// <summary>
    /// Logger. Mainly to log startup
    /// </summary>
    public static class WidgetLogger
    {
        //private static readonly string _documentsPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDocuments, "TT2Master");
        //public static string GetDocumentName(string filename) => Path.Combine(DirectoryPath, filename);
        //public static string DirectoryPath => _documentsPath;
        //public static string GetDocumentPath() => DirectoryPath;

        public static string LogName => "tt2widgetlogger.txt";

        /// <summary>
        /// Should a logfile be written?
        /// </summary>
        public static bool WriteLog { get; set; } = false;

        /// <summary>
        /// Deletes the logfile
        /// </summary>
        public static void DeleteLogFile()
        {
            try
            {
                //get path and dir
                var h = new DirectoryHelper();
                string path = h.GetDocumentName(LogName);
                string dir = h.GetDocumentPath();

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
            catch (Exception)
            { }
        }

        /// <summary>
        /// Writes Artifact Optimization log
        /// </summary>
        /// <param name="text"></param>
        public static void WriteToLogFile(string text)
        {
            if (WriteLog)
            {
                try
                {
                    //get path and dir
                    var h = new DirectoryHelper();
                    string path = h.GetDocumentName(LogName);
                    string dir = h.GetDocumentPath();

                    //Check path and create if non existant
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    //write
                    using var sw = File.AppendText(path);
                    sw.WriteLine($"{DateTime.Now} - {text}");
                }
                catch (Exception)
                { }
            }
        }
    }
}