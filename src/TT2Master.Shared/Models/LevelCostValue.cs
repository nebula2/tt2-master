namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Object describing the relationship between level, cost and value
    /// </summary>
    public class LevelCostValue
    {
        /// <summary>
        /// Level
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// Cost
        /// </summary>
        public double Cost { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public double Value { get; set; }
    }
}
