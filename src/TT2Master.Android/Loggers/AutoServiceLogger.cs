namespace TT2Master.Droid.Loggers
{
    /// <summary>
    /// Logger to log automatic ClanExport
    /// </summary>
    public static class AutoServiceLogger
    {
        public static string LogName => "AutoServiceLogger.txt";

        public static void DeleteLogFile() => BaseAndroidLogger.DeleteLogFile(LogName);

        public static void WriteToLogFile(string text) => BaseAndroidLogger.WriteToLogFile(LogName, text);
    }
}