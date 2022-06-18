using System;

namespace TT2Master
{
    /// <summary>
    /// Export clan message
    /// </summary>
    public class ExportClanMessage
    {
        /// <summary>
        /// csv delimiter
        /// </summary>
        private static readonly string _del = LocalSettingsORM.CsvDelimiter;

        /// <summary>
        /// ID of the message
        /// </summary>
        public int MessageID { get; set; }

        /// <summary>
        /// Type of clan message
        /// </summary>
        public string ClanMessageType { get; set; }

        /// <summary>
        /// Message that has been sent in text
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// From whom was this sent
        /// </summary>
        public string PlayerIdFrom { get; set; }

        /// <summary>
        /// Name of the Member
        /// </summary>
        public string MemberName { get; set; }

        /// <summary>
        /// When was this sent
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Ctor
        /// </summary>
        public ExportClanMessage()
        {

        }

        /// <summary>
        /// Returns the csv header line
        /// </summary>
        /// <returns></returns>
        public static string GetHeaderLine() => $"ID{_del}Type{_del}Message{_del}From{_del}FromID{_del}Time\n";

        /// <summary>
        /// Converts properties into a string variable to directly write into a csv-row
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{MessageID}{_del}{ClanMessageType}{_del}{Message.Replace(_del, "")}{_del}{MemberName.Replace(_del, "")}{_del}{PlayerIdFrom}{_del}{TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")}\n";
    }
}
