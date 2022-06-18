using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master
{
    /// <summary>
    /// Model for .csv-Export of Clan Members
    /// Deliminator is a comma
    /// </summary>
    public class ExportMemberContribution
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
        /// player id
        /// </summary>
        public string PlayerCode { get; set; } = "";
        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// MS
        /// </summary>
        public double MaxStage { get; set; } = 0;
        /// <summary>
        /// Damage
        /// </summary>
        public double Contribution { get; set; } = 0;
        /// <summary>
        /// Total Clan quests
        /// </summary>
        public int TotalClanQuests { get; set; } = 1;
        /// <summary>
        /// This clan quest
        /// </summary>
        public int ThisClanQuests { get; set; } = 1;

        /// <summary>
        /// Ctor
        /// </summary>
        public ExportMemberContribution()
        {

        }

        /// <summary>
        /// Returns csv header line
        /// </summary>
        /// <returns></returns>
        public static string GetHeaderLine() => $"Boss{_del}ID{_del}Name{_del}MS{_del}Dmg{_del}CQ{_del}T-CQ\n";

        /// <summary>
        /// Converts properties into a string variable to directly write into a csv-row
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{BossLevel}{_del}{PlayerCode}{_del}{(string.IsNullOrEmpty(Name) ? "" : Name.Replace(_del, ""))}{_del}{MaxStage}{_del}{Contribution}{_del}{ThisClanQuests}{_del}{ThisClanQuests}\n";
    }
}