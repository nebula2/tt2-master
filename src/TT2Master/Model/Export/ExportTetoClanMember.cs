using System;

namespace TT2Master
{
    /// <summary>
    /// Handles teto clan member export
    /// </summary>
    public class ExportTetoClanMember
    {
        /// <summary>
        /// Csv delimiter
        /// </summary>
        private static readonly string _del = LocalSettingsORM.CsvDelimiter;

        /// <summary>
        /// Member name
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// Clan role
        /// </summary>
        public string ClanRole { get; set; } = "";
        /// <summary>
        /// Weekly clan quest count
        /// </summary>
        [Obsolete("Gone with 3.0")]
        public int WeeklyClanQuestCount { get; set; } = 0;

        /// <summary>
        /// Weekly ticket count
        /// </summary>
        public int WeeklyTicketCount { get; set; } = 0;

        /// <summary>
        /// Ctor
        /// </summary>
        public ExportTetoClanMember()
        {

        }

        /// <summary>
        /// Returns the header line for this export
        /// </summary>
        /// <returns></returns>
        public static string GetHeaderLine() => $"Name{_del}Role{_del}Weekly Morale Count\n";

        /// <summary>
        /// Converts properties into a string variable to directly write into a csv-row
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Name.Replace(_del, "")}{_del}{ClanRole}{_del}{WeeklyTicketCount}\n";
    }
}