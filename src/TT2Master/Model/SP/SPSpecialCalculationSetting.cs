namespace TT2Master.Model.SP
{
    /// <summary>
    /// Settings for special calculation (SP optimizer)
    /// </summary>
    public class SPSpecialCalculationSetting
    {
        /// <summary>
        /// Talent identifier
        /// </summary>
        public string TalentId { get; set; }

        /// <summary>
        /// Name of method that is used to calculate efficiency
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Is the value additive (true) or absolute (false)?
        /// </summary>
        public bool IsAdditive { get; set; }
    }
}