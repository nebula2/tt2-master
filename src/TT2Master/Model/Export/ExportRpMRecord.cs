using System;
using System.Collections.Generic;
using System.Text;

namespace TT2Master
{
    /// <summary>
    /// Model for .csv-Export of Clan Members
    /// Deliminator is a comma
    /// </summary>
    public class ExportRpMRecord
    {
        /// <summary>
        /// delimiter for csv
        /// </summary>
        private static readonly string _del = LocalSettingsORM.CsvDelimiter;

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id { get; set; } = "1";
        /// <summary>
        /// Start time
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;
        /// <summary>
        /// end time
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.Now;
        /// <summary>
        /// averate stage increase
        /// </summary>
        public double AvgStageIncrease { get; set; } = 1;

        /// <summary>
        /// Ctor
        /// </summary>
        public ExportRpMRecord()
        {

        }

        /// <summary>
        /// Converts properties into a string variable to directly write into a csv-row
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"ID{_del}{Id}\nStartTime{_del}{StartTime}\nEnd{_del}{EndTime}\nDuration{_del}{AvgStageIncrease}\n\n";
    }
}