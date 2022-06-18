using System.IO;
using Xamarin.Forms;

namespace TT2Master.Loggers
{
    /// <summary>
    /// Logger. Mainly to log startup
    /// </summary>
    public static class RpMLogger
    {
        public static string LogName => "RpMlog.txt";

        public static void DeleteLogFile() => BaseLogger.DeleteLogFile(LogName);
        public static void WriteToLogFile(string text) => BaseLogger.WriteToLogFile(LogName, text);
    }
}