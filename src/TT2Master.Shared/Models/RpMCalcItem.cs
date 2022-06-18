using System;
using System.Collections.Generic;

namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Relics per Minute calculation item
    /// </summary>
    public class RpMCalcItem
    {
        #region CalculationProps
        /// <summary>
        /// Crafting power level
        /// </summary>
        public int CraftingPower { get; set; }
        /// <summary>
        /// Book of Shadows level
        /// </summary>
        public double BoSLevel { get; set; }
        /// <summary>
        /// Amount of completed sets
        /// </summary>
        public int SetsCompleted { get; set; }
        #endregion

        #region RecordProps
        /// <summary>
        /// Identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Time the record started
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Time the record ended
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// True, if calculation has been done
        /// </summary>
        public bool IsCalculated { get; set; }

        /// <summary>
        /// Average stage increase
        /// </summary>
        public double AvgStageIncrease { get; set; }

        /// <summary>
        /// List of <see cref="RpMCalcPoint"/>
        /// </summary>
        public List<RpMCalcPoint> Points { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Default constructor
        /// </summary>
        public RpMCalcItem()
        {

        }
        #endregion
    }
}
