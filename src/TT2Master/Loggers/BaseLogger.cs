using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace TT2Master.Loggers
{
    public static class BaseLogger
    {
        /// <summary>
        /// Deletes the logfile
        /// </summary>
        public static void DeleteLogFile(string logName)
        {
            string path = DependencyService.Get<IDirectory>().GetDocumentName(logName);
            string dir = DependencyService.Get<IDirectory>().GetDocumentPath();

            //Check path and return if non existant
            if (!Directory.Exists(dir))
            {
                return;
            }

            try
            {
                //delete if existant
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (System.Exception)
            { }
        }


        public static void WriteToLogFile(string logName, string text)
        {
            //get path and dir
            string path = DependencyService.Get<IDirectory>().GetDocumentName(logName);
            string dir = DependencyService.Get<IDirectory>().GetDocumentPath();

            try
            {
                //Check path and create if non existant
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                //Write
                using (var sw = File.AppendText(path))
                {
                    sw.WriteLine(text);
                }
            }
            catch (System.Exception)
            { }
        }
    }
}
