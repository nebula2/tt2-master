namespace TT2Master.Droid.Loggers
{
    /// <summary>
    /// Logger to log automatic ClanExport
    /// </summary>
    public static class ClanExportLogger
    {
        public static string LogName => "ClanExportLogger.txt";

        public static void DeleteLogFile() => BaseAndroidLogger.DeleteLogFile(LogName);

        public static void WriteToLogFile(string text) => BaseAndroidLogger.WriteToLogFile(LogName, text);
    }
}