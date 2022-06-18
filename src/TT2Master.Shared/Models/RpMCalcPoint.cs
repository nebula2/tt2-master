using System;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// A point in relics per minute calculation
    /// </summary>
    public class RpMCalcPoint
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public string ID { get; set; } = "";

        /// <summary>
        /// Id of run
        /// </summary>
        public int RunId { get; set; }

        /// <summary>
        /// ID of point
        /// </summary>
        public int PointId { get; set; }

        /// <summary>
        /// Passed minutes in run
        /// </summary>
        public int MinsPassed { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Current stage of player
        /// </summary>
        public double CurrentStage { get; set; } = 0;

        /// <summary>
        /// Relics
        /// </summary>
        public double Relics { get; set; } = 0;
    }
}
