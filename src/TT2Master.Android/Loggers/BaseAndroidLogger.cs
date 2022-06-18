using System.IO;


namespace TT2Master.Droid.Loggers
{
    public static class BaseAndroidLogger
    {
        /// <summary>
        /// Deletes the logfile
        /// </summary>
        public static void DeleteLogFile(string logName)
        {
            var h = new DirectoryHelper();
            string path = h.GetDocumentName(logName);
            string dir = h.GetDocumentPath();

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

        /// <summary>
        /// Writes text to given log
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="text"></param>
        public static void WriteToLogFile(string logName, string text)
        {
            //get path and dir
            var h = new DirectoryHelper();
            string path = h.GetDocumentName(logName);
            string dir = h.GetDocumentPath();

            try
            {
                //Check path and create if non existant
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                //Write
                using var sw = File.AppendText(path);
                sw.WriteLine(text);
            }
            catch (System.Exception)
            { }
        }
    }
}