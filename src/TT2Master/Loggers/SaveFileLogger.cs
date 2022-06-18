using System.IO;
using Xamarin.Forms;

namespace TT2Master.Loggers
{
    /// <summary>
    /// Logger. Mainly to log startup
    /// </summary>
    public static class SaveFileLogger
    {
        public static string LogName => "tt2saveFile.txt";
        public static bool WriteLog { get; set; } = false;

        public static void DeleteLogFile() => BaseLogger.DeleteLogFile(LogName);
        public static void WriteToLogFile(string text)
        {
            if (WriteLog) BaseLogger.WriteToLogFile(LogName, text);
        }
    }
}