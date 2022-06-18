using System;

namespace TT2Master
{
    /// <summary>
    /// Model for .csv-Export of Clan Members
    /// Deliminator is a comma
    /// </summary>
    public class ExportDungeonContribution
    {
        /// <summary>
        /// csv delimiter
        /// </summary>
        private static readonly string _del = LocalSettingsORM.CsvDelimiter;

        /// <summary>
        /// Boss level
        /// </summary>
        public int BossLevel { get; set; } = 1;
        /// <summary>
        /// Clan goal
        /// </summary>
        public double ClanGoal { get; set; } = 1;
        /// <summary>
        /// Starting time
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;
        /// <summary>
        /// Duration the boss stays
        /// </summary>
        public string Duration { get; set; } = "";

        /// <summary>
        /// Ctor
        /// </summary>
        public ExportDungeonContribution()
        {

        }

        /// <summary>
        /// Converts properties into a string variable to directly write into a csv-row
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"BossLevel{_del}{BossLevel}\nHP{_del}{ClanGoal}\nStart{_del}{StartTime}\nDuration{_del}{Duration}\n\n";
    }
}