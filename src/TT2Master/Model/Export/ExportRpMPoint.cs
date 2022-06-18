using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master
{
    /// <summary>
    /// Model for .csv-Export of Clan Members
    /// Deliminator is a comma
    /// </summary>
    public class ExportRpMPoint
    {
        /// <summary>
        /// csv delimiter
        /// </summary>
        private static readonly string _del = LocalSettingsORM.CsvDelimiter;

        /// <summary>
        /// identifier
        /// </summary>
        public string ID { get; set; } = "1";
        /// <summary>
        /// passed minutes
        /// </summary>
        public int MinsPassed { get; set; } = 1;
        /// <summary>
        /// timestamp
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
        /// <summary>
        /// current stage
        /// </summary>
        public double CurrentStage { get; set; } = 0;
        /// <summary>
        /// amount of relics
        /// </summary>
        public double Relics { get; set; } = 0;
        /// <summary>
        /// relics-per-minute
        /// </summary>
        public double RpMVal { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public ExportRpMPoint()
        {

        }

        /// <summary>
        /// returns the csv header line
        /// </summary>
        /// <returns></returns>
        public static string GetHeaderLine() => $"ID{_del}MinsPassed{_del}Timestamp{_del}CurrentStage{_del}Relics{_del}RpM\n";

        /// <summary>
        /// Converts properties into a string variable to directly write into a csv-row
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{ID}{_del}{MinsPassed}{_del}{Timestamp}{_del}{CurrentStage}{_del}{Relics}{_del}{RpMVal}\n";
    }
}