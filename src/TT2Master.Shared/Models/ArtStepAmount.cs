namespace TT2Master.Shared.Models
{
    /// <summary>
    /// Step Amount for Upgrade
    /// </summary>
    public class ArtStepAmount
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Is the value in percent/ relative?
        /// </summary>
        public bool IsInPercent { get; set; }
    }
}
